using UnityEngine;
using System.Collections;

public partial class Game : MonoBehaviour
{
    Color colour_snakespit = new Color(0.125f, 0.85f, 0.05f, 0.5f);
    Color colour_damage = new Color(0.85f, 0.05f, 0.125f, 0.5f);


    // bool trytomove(int deltax, int deltay) {
    bool trytomove(mob m,int rotdir, bool coasting = false)
    {
       // Debug.Log(m.archetype.name + " " + rotdir);
        if (coasting && !m.skates_currently) goto playercoastingbutnotaskater;//was &&m.isplayer

        if (coasting) rotdir = m.facing;

        int deltax = lil.rot_deltax[rotdir];
        int deltay = lil.rot_deltay[rotdir];
        int tentx = m.posx + deltax;
        int tenty = m.posy + deltay;

        if (tentx < 0 || tentx >= map.width || tenty < 0 || tenty >= map.height) return false;

        /*
         mob m=map.mobgrid[tentx,tenty];
        if (m != null) {
            //mob me up we're going smacking
            MobAttacksMob(player, m);
            TimeEngine = CradleOfTime.player_is_done;
            return true;
        }
    */
        //if (!map.passable[tentx, tenty]&&player.mob.speed>0) return false;

        if (!coasting)
            Speed.SpeedAndDirectionChange(m, rotdir);

        if (m.speed > 0)
        {
            if (map.passablecheck(tentx, tenty, m))
            {
               
                if (m.isplayer)
                {
                    m.posx = tentx; m.posy = tenty;
                    moveplayer();
                }
                else
                {
                    if (m.posx != tentx || m.posy != tenty)
                    {
                        map.itemgrid[tentx, tenty] = map.itemgrid[m.posx, m.posy];
                        map.itemgrid[m.posx, m.posy] = null;
                        m.posx = tentx; m.posy = tenty;
                    }
           
                }
            }
        }

        Etilesprite et = map.displaychar[m.posx, m.posy];
        if (et == Etilesprite.MAP_THIN_ICE && (m.speed == 0||(m.archetype.heavy&&!m.flies_currently)))
        {
            if(m.isplayer)log.Printline("The thin ice collapses!", Color.red);
            map.displaychar[m.posx, m.posy] = Etilesprite.MAP_WATER;
            map.passable[m.posx, m.posy] = false;
        }
        else if (!m.skates_currently || (et != Etilesprite.MAP_ICE && et != Etilesprite.MAP_THIN_ICE))
        {
            Speed.change(m, -6);
        }

        if (coasting)
            Speed.change(m, -1);
playercoastingbutnotaskater:
        if(m.isplayer)TimeEngine = CradleOfTime.player_is_done;
        return true;
    }


    void ProcessTurn()
    {


        if (TimeEngine == CradleOfTime.ready_to_process_turn)
        {

            // if (map.timegrid.beatgrid[0, 0])
            // {
            TimeEngine = CradleOfTime.waiting_for_player;
            return;
            // }

        }

        TimeEngine = CradleOfTime.processing_other_actors;

        //effects and mobs get to act
        //stop being scared of "foreach": think of all the other shitty garbage you are creating
        foreach (var f in map.moblist)
            MobGetsToAct(f);



        //check for mob hp so dead

        if (player.hp <= 0)
        {
            log.Printline("<snarky game over message>", Color.red);

            TimeEngine = CradleOfTime.dormant;
            gamestate = Egamestate.gameover;
            return;
        }
        player.turns++;


        TimeEngine = CradleOfTime.ready_to_process_turn;

    }
    void FloatingDamage(mob victim, mob attacker, int amount, string explanation = "", bool flashsupress = false)
    {
        Color c;

        if (amount == 0) c = Color.grey;
        else c = (amount < 0) ? Color.red : Color.green;

        FloatingTextItems.Add(new FloatingTextItem(explanation + " " + amount + " hp", victim.posx, victim.posy, c));        
        log.Printline(victim.archetype.name, Color.black);
        if (amount <= 0) log.Print(" takes ");
        else log.Print(" gains ");
        log.Print(amount + " from " +attacker.archetype.name, c);
        if (explanation.Length > 0) log.Print("[" + explanation + "]");

        //actually do the damage
        victim.hp += amount;
        c.a = 0.5f;
        map.gridflashcolour[victim.posx, victim.posy] = c;
        map.gridflashtime[victim.posx, victim.posy] = Time.time + 0.5f;
    }

    void MobAttacksMob(mob attacker, mob target)
    {
        int damage = attacker.speed - target.speed;
        if (damage < 1) damage = 1;
        FloatingDamage(target, attacker, -damage,attacker.archetype.weaponname);
    }
        void MobGetsToAct(mob e)
    {

        if (e.IsAdjacentTo(player.mob))
        {
            MobAttacksMob(e, player.mob);
            return;
        }

        map.passable[e.posx, e.posy] = true;//we need square the mob starts on to be passable, for pathfinding.
        //attempt to move 
        if (map.PathfindAStar(e.posx, e.posy, player.posx, player.posy, false))
        {
            int reldir = Speed.findrel(e.posx, e.posy, map.firststepx, map.firststepy);
            trytomove(e, reldir);
            //  MoveMob(e, map.firststepx, map.firststepy);
            return;
        }
        map.passable[e.posx, e.posy] = false;//we can't do this though- this hard sets the square to not passable. 
        //or can we? is mob's new position... are we setting initial mob pos to not passable?
    }

    //void MoveMob(mob e, int newx, int newy)
    // {
    //     map.displaychar[e.posx, e.posy] = Etilesprite.FLOOR;
    //     map.mobgrid[e.posx, e.posy] = null;
    //     map.passable[e.posx, e.posy] = true;
    //     e.posx = newx;
    //     e.posy = newy;//

    //map.passable[e.posx, e.posy] = false;

    //        int m = (int)map.displaychar[e.posx, e.posy];
    //      if (m > 30 && m < 44)
    //    {
    //      bool noeffect; int clicks;
    //    string s = oscs[1].ApplyPatch(m - 30, out noeffect, out clicks);
    //    if (noeffect) log.Printline("No effect", Color.red);
    //    else {
    //        log.Printline(s);
    //        playknob(clicks);
    //     }
    //  }

    // if ((int)map.displaychar[e.posx, e.posy] == 25)
    // {
    //     SS_overwroteexit();
    //     log.Printline("A glitch overwrote the exit.", Color.red);
    //     log.Printline("DELETE ALL GLITCHES!", Color.red);
    // }

    //  map.displaychar[e.posx, e.posy] = (Etilesprite)(13 + e.type);
    //  map.mobgrid[e.posx, e.posy] = e;
    //  return;
    // }

}
