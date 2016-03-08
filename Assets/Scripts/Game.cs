//#define FLICKERING_LANTERN

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class FloatingTextItem {
    public byte[] message;
    public Color colour;
    public float fade=1.0f;
    
    public int rise=0;
    public int sqx, sqy;
    public int mwoffset;

    public FloatingTextItem(string m, int _sqx, int _sqy, Color c) {
        message =   System.Text.Encoding.ASCII.GetBytes(m);
        colour = c;
        sqx = _sqx;
        sqy = _sqy;
        mwoffset = (m.Length*6)/2;
    }

}

class Pangotimer{
    public float period=0;
    public float timer=0;
    public Pangotimer(float p){
        period=p;
    }
    public bool test(){
        return (Time.time>timer);
    }
    public void reset(){
        timer=Time.time+period;
    }
}


public partial class Game : MonoBehaviour {
    
    
    public Texture wednesdayfont;
    public Texture sprites;
    
    public Texture2D particle;

    float[] StoredNoise = new float[640];
    List<FloatingTextItem> FloatingTextItems = new List<FloatingTextItem>();

    const int zoomfactor = 2;           //1,2 or 3 for 640x360,1280x720 or 1920x1080
    const int zoomfactorx16 = zoomfactor * 16;
    const bool fullscreenp = true;

    MessageLog log;             //message log

    //vars for drawing sprites 
    const float xratio = 1f / 32f;
    const float yratio = 1f / 8f;
    float spriteratio;
    static Rect r = new Rect(0, 0, 6 * zoomfactor, 12 * zoomfactor);
    static Rect r2 = new Rect(0, 0, xratio, yratio);
    static Rect r3 = new Rect(0, 0, 16 * zoomfactor, 16 * zoomfactor);
    static Rect r3b = new Rect(0, 0, 8 * zoomfactor, 8 * zoomfactor);
    static Rect r4 = new Rect(0, 0, 0, 1f);
    static Rect r5 = new Rect(0, 0, 0, 1f);
    static Rect Rectparticle = new Rect(0,0,zoomfactor,zoomfactor);
    static Rect Rectparticle2 = new Rect(0, 0, zoomfactor, zoomfactor);

    static Rect r_minimap; //= new Rect(339 * zoomfactor, 0, 80 * 2 * zoomfactor, 50 * 2 * zoomfactor);
    static Rect r2_minimap = new Rect(0, 0, 1f, -1f);
    //

    //viewport dimensions
    static int VIEWPORT_WIDTH = 21;
    static int VIEWPORT_HEIGHT = 21;
    static int XHALF = 10;
    static int YHALF = 10;
    //

    public static int originx = 0, originy = 0;
    RLMap map;
    Player player = new Player();

    Pangotimer floating_text_timer = new Pangotimer(0.008f);

#if FLICKERING_LANTERN
    float LF_period=0.1f; //frequency of lantern flicker
    float LF_timer = 0.0f; //progression towards period
    float LF_amount=0.0f; //deviation from full light
#endif

   


    void OnGUI() {
     
       
        int curnoi = 0;

        for (int f = 0; f < 640 * zoomfactor; f += zoomfactor) {
            float ff = (float)(((float)f / (640.0f * (float)zoomfactor)) * 2.0f) - 1.0f;
            float gg = Time.time;
            //Rectparticle.x = f; 
            StoredNoise[curnoi] = SimplexNoise.Noise.Generate(ff * 128, gg);
            //Rectparticle.y = (int)(180+ ((4) * StoredNoise[curnoi]))*zoomfactor;
            curnoi++;
            //GUI.DrawTextureWithTexCoords(Rectparticle, particle, Rectparticle2);
        }

        Action<int, int, int> DrawSprite = (int x, int y, int s) => {
            r3.x = x * zoomfactorx16;
            r3.y = y * zoomfactorx16;
            r4.x = spriteratio * (s-1);//s-1 is new for 2016
            GUI.DrawTextureWithTexCoords(r3, sprites, r4);
        };
        Action<int, int, int> DrawSprite_Particle = (int x, int y, int s) => {
            r3b.x = x;
            r3b.y = y;

            r5.x = spriteratio * s;
            r5.y = 0.5f;
            r5.width = spriteratio/2;
            r5.height = 0.5f;
            GUI.DrawTextureWithTexCoords(r3b, sprites, r5);
        };

        originx = (player.posx < XHALF) ? 0 : player.posx - XHALF;
        originy = (player.posy < YHALF) ? 0 : player.posy - YHALF;

        if (player.posx > (map.width - (XHALF + 1))) originx = map.width - VIEWPORT_WIDTH;
        if (player.posy > (map.height - (YHALF + 1))) originy = map.height - VIEWPORT_HEIGHT;

        //draw message log
        int currentline = log.GetCurrentLine();
        for (int y = 0; y < 15; y++) {
            for (int x = 0; x < 50; x++) {
                byte c = log.screenmap[x, currentline];
                int xpos = c % 32;
                int ypos = 7- (c / 32);
                r.x = (339 + (x * 6)) * zoomfactor;
                r.y = (180 + (y * 12)) * zoomfactor;
                r2.x = xratio * xpos;
                r2.y = yratio * ypos;
                GUI.color = log.screenfg[x, currentline];
                GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);
            }
            currentline++; if (currentline == 15) currentline = 0;
        }
        //draw minimap

        GUI.color = Color.white;
        GUI.DrawTextureWithTexCoords(r_minimap, map.minimap, r2_minimap);
        //draw viewport of map
        //draw mob if there is one. item. noticed player icon if it has
        //draw locked padlock if appropriate
        //consider player stealth. 
        //draw user interface: lives, score , statuses, held items etc.

        int screenx = 0; int screeny = 0;
        for (int yy = originy; yy < originy + VIEWPORT_HEIGHT; yy++) {
            for (int xx = originx; xx < originx + VIEWPORT_WIDTH; xx++) {

                if (map.in_FOV.AtGet(xx, yy)) {

                   #if FLICKERING_LANTERN
                    if (Time.time > LF_timer) {
                        if (lil.randi(1, 100) > 50) {
                            LF_amount -= 0.1f; if (LF_amount < 0.0f) LF_amount = 0.0f;
                        } else {
                            LF_amount += 0.1f; if (LF_amount > 0.5f) LF_amount = 0.5f;
                        }
                        LF_timer = Time.time + LF_period;
                    }
                    Color c = map.dynamiclight[xx, yy];
                    c.r -= LF_amount; if (c.r < 0) c.r = 0;
                    c.g -= LF_amount; if (c.g < 0) c.g = 0;
                    c.b -= LF_amount; if (c.b < 0) c.b = 0;
                    GUI.color = lil.colouradd(c, map.staticlight.AtGet(xx, yy));
#else
    GUI.color = lil.colouradd(map.staticlight[xx, yy], map.dynamiclight[xx, yy]);
#endif              
                    
                    DrawSprite(screenx, screeny, (int)map.displaychar.AtGet(xx, yy));


                    if (player.posx == xx && player.posy == yy) {
                        GUI.color = Color.white;
                        DrawSprite(screenx, screeny, 2);
                    }

                    //smoke/cloud/gas
                    GUI.color = new Color(GUI.color.r, 255, GUI.color.b, 0.3f);
                    if (map.fogoffog[xx, yy]) { 
                    for (int f = 0; f < 8; f++) {
                        float tx = (StoredNoise[(screenx * 16) + f] + 1.0f) * 4.5f;
                        float ty = (StoredNoise[(screeny * 16) + f + 8] + 1.0f) * 4.5f;

                        DrawSprite_Particle((screenx* 16 * zoomfactor) + zoomfactor * (int)tx, (screeny* 16 * zoomfactor) + zoomfactor * (int)ty, 13);
                    }
                }




                } else {
                    if (!map.fogofwar.AtGet(xx, yy)) {
                        GUI.color = RLMap.memorylight;
                        DrawSprite(screenx, screeny, (int)map.playermemory.AtGet(xx, yy));
                    }
                }
                screenx++;
            }
            screeny++; screenx = 0;
        }
    
       //floating text
        for (int f = 0; f < FloatingTextItems.Count; f++) {
            FloatingTextItem fti = FloatingTextItems[f];
            int myx = (((fti.sqx - originx) * 16) - fti.mwoffset+8) * zoomfactor;
            int myy = (((fti.sqy - originy) * 16) - fti.rise+5) * zoomfactor;

            fti.colour.a = fti.fade;
            GUI.color = fti.colour;

            for (int x = 0; x < fti.message.Length; x++) {
                byte c = fti.message[x];
                int xpos = c % 32;
                int ypos = 7 - (c / 32);
                r.x = myx+(6*x*zoomfactor);
                r.y = myy;
                r2.x = xratio * xpos;
                r2.y = yratio * ypos;
              
                GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);
            }


        }

    }
   
    //0 -> -1, (640*zoomfactor) -> 1
    //map the whole thing to go from 0 to 1
    //x/(640*zoomfactor)

    void Start() {
        Screen.SetResolution(640 * zoomfactor, 360 * zoomfactor, fullscreenp);
        log = new MessageLog(50,15);
        spriteratio = 1f / (float)(sprites.width / 16);
        r4.width = spriteratio;
        particle = new Texture2D(1,1, TextureFormat.ARGB32, false, false);//zoomfactor by zoomfactor
        particle.filterMode = FilterMode.Point;
       // for (int f = 0; f < zoomfactor; f++)
       //     for (int g = 0; g < zoomfactor;g++ )
        //        particle.SetPixel(f,g, Color.white);
        particle.SetPixel(0,0, Color.white);          
        particle.Apply();


        log.Printline("Welcome to genericRL");

        lil.seednow();

        map = new RLMap(player, DungeonGenType.Skater2016);
        r_minimap = new Rect(336 * zoomfactor, 0, map.width * 2 * zoomfactor, map.height * 2 * zoomfactor);//was 339

        //take this map reveal cheat out 
        
        for (int y= 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Etilesprite et = map.displaychar.AtGet(x, y);
                map.minimap.SetPixel(x, y, (Color)map.minimapcolours[(int)et]);
            }
        }
        //end cheat 


        int freex, freey;
        map.FreeSpace(out freex, out freey);
        player.emerge(freex, freey);
        
        moveplayer();

        log.Printline("number of spaces free: " + map.emptyspaces.Count);
        log.Printline("player @ " + freex + " , " + freey);
        log.Printline("underneath is " + map.displaychar.AtGet(freex, freey));
        log.Printline("passable=" + (map.passable.AtGet(freex, freey) == true ? "true" : "false"));
        log.Printline("blocks sight=" + (map.blocks_sight.AtGet(freex, freey) == true ? "true" : "false"));
        log.Printline("staticlight @ player=" + map.staticlight.AtGet(freex, freey).r + " " + map.staticlight.AtGet(freex, freey).g + " " + map.staticlight.AtGet(freex, freey).b);
        log.Printline("dynamiclight @ player=" + map.dynamiclight.AtGet(freex, freey).r + " " + map.dynamiclight.AtGet(freex, freey).g + " " + map.dynamiclight.AtGet(freex, freey).b);

     
      
    }




    void moveplayer(){
	    map.do_fov_rec_shadowcast(player.posx, player.posy, 11);
	    map.dynamiclight.Fill(Color.black);
	    if (player.lantern)
		    map.do_fov_foradynamiclight(player.posx, player.posy, 11, Color.white);
  
	    if (lil.totalcolour(map.dynamiclight.AtGet(player.posx, player.posy))== 0f &&
		    lil.totalcolour(map.dynamiclight.AtGet(player.posx, player.posy)) == 0f)
		    player.stealthed = true;
	    else
		    player.stealthed = false;
    }
    
    float nextfire=0.0f;
    float firerate = 0.2f;
    float initialdelay = 0.5f;
    int currentcommand = -1;
    bool keydown = false;
    bool firstpress = false;
    bool mauswalking = false;

    void Update() {
        if (floating_text_timer.test()) {
            for (int f = FloatingTextItems.Count-1; f > -1; f--) {
                FloatingTextItem fti = FloatingTextItems[f];
                fti.fade -= 0.01f;
                fti.rise++;
                if (fti.fade <= 0) FloatingTextItems.Remove(fti);
           }

           floating_text_timer.reset();
        }
    
        if (Input.GetMouseButtonDown(0)) {
            int fx=(int)Input.mousePosition.x; int fy=(int)((360*zoomfactor)-1-Input.mousePosition.y);
            //log.Printline("x = " + fx.ToString() + " y = " + fy.ToString());
            int x = fx / zoomfactorx16;
            int y =fy / zoomfactorx16;
           
            if (x < VIEWPORT_WIDTH && y < VIEWPORT_HEIGHT) {
                x = originx + x; y = originy + y;
                if (map.passable[x, y] && !(x == player.posx && y == player.posy)) {
                    bool worked = map.PathfindAStar(player.posx, player.posy, x, y, true,true);
                    if (worked) {
                        mauswalking = true;
                        //log.Printline("And we're walking!");
                    } //else log.Printline("didn't work");
                } //else log.Printline("not passable or clicked player");
            } else {//outside viewport
                int mmx=((fx/zoomfactor)-336)/2;
                int mmy=((fy/zoomfactor)/2);
                //log.Printline(mmx.ToString() + " " + mmy.ToString() + " ");
                if (mmx >= 0 && mmx < map.width && mmy >= 0 && mmy < map.height) {
                    if (map.passable[mmx, mmy] && !(mmx == player.posx && mmy == player.posy)) {
                        bool worked = map.PathfindAStar(player.posx, player.posy, mmx, mmy, true, true);
                        if (worked) {
                            mauswalking = true;
                        }
                    }
                }
            }
        } else if (Input.GetMouseButtonDown(1)){
             FloatingTextItems.Add(new FloatingTextItem("-10 hp",player.posx,player.posy,Color.red));
           // FloatingTextItems.Add(new FloatingTextItem("In the valleh of the shadow of death", player.posx, player.posy, Color.red));
        }
        //todo: fix it so if you press a key or press the maus when mauswalking it breaks the auto walk

        if (mauswalking) {
            //log.Printline(map.lastpath.Count.ToString());
            if (map.lastpath.Count < 1) {
                mauswalking = false;
                currentcommand = -1;
            } else {
                Cell next = map.lastpath.Dequeue();
                if (next.x == player.posx && next.y < player.posy) { currentcommand = 0; nextfire = 0.0f; firstpress = false; } //up
                else if (next.x == player.posx && next.y > player.posy) { currentcommand = 1; nextfire = 0.0f; firstpress = false; }//down
                else if (next.x < player.posx && next.y == player.posy) { currentcommand = 2; nextfire = 0.0f; firstpress = false; }//left
                else if (next.x > player.posx && next.y == player.posy) { currentcommand = 3; nextfire = 0.0f; firstpress = false; }//right
                else if (next.x < player.posx && next.y < player.posy) { currentcommand = 6; nextfire = 0.0f; firstpress = false; }//upleft
                else if (next.x > player.posx && next.y < player.posy) { currentcommand = 7; nextfire = 0.0f; firstpress = false; }//upright
                else if (next.x < player.posx && next.y > player.posy) { currentcommand = 8; nextfire = 0.0f; firstpress = false; }//downleft
                else if (next.x > player.posx && next.y > player.posy) { currentcommand = 9; nextfire = 0.0f; firstpress = false; }//downright
            }
        } else {
            if (Input.GetButtonDown("up")) { currentcommand = 0; nextfire = 0.0f; firstpress = true; } 
            else if (Input.GetButtonDown("down")) { currentcommand = 1; nextfire = 0.0f; firstpress = true; } 
            else if (Input.GetButtonDown("left")) { currentcommand = 2; nextfire = 0.0f; firstpress = true; } 
            else if (Input.GetButtonDown("right")) { currentcommand = 3; nextfire = 0.0f; firstpress = true; }
            else if (Input.GetButtonDown("lantern")) { currentcommand = 4; nextfire = 0.0f; firstpress = true; } 
            else if (Input.GetButtonDown("wait")) { currentcommand = 5; nextfire = 0.0f; firstpress = true; } 
            else if (Input.GetButtonDown("upleft")) { currentcommand = 6; nextfire = 0.0f; firstpress = true; }
            else if (Input.GetButtonDown("upright")) { currentcommand = 7; nextfire = 0.0f; firstpress = true; } 
            else if (Input.GetButtonDown("downleft")) { currentcommand = 8; nextfire = 0.0f; firstpress = true; }
            else if (Input.GetButtonDown("downright")) { currentcommand = 9; nextfire = 0.0f; firstpress = true; }
        }

        if (currentcommand > -1 && Time.time > nextfire ) {
            switch (currentcommand) {
                case 0: trytomove(0, -1);
                    break;
                case 1: trytomove(0,1);
                    break;
                case 2: trytomove(-1, 0);
                    break;
                case 3: trytomove(1, 0);
                    break;
                case 4: { player.lantern = !player.lantern; moveplayer(); }
                    break;
                case 5: log.Printline("You wait. Time passes");
                    break;
                case 6: trytomove(-1, -1);
                    break;
                case 7: trytomove(1, -1);
                    break;
                case 8: trytomove(-1, 1);
                    break;
                case 9: trytomove(1, 1);
                    break;
            }
            nextfire = Time.time + firerate;
            if (firstpress) { nextfire += initialdelay; firstpress = false; }
        }

        if ( (Input.GetButtonUp("up")) ||
        (Input.GetButtonUp("down")) || 
        (Input.GetButtonUp("left")) ||
      (Input.GetButtonUp("right")) ||
         (Input.GetButtonUp("lantern")) ||
        (Input.GetButtonUp("wait")) ||
         (Input.GetButtonUp("upleft")) ||
        (Input.GetButtonUp("upright")) ||
         (Input.GetButtonUp("downleft")) ||
        (Input.GetButtonUp("downright")) ){ currentcommand = -1; } 
        
    }
}
