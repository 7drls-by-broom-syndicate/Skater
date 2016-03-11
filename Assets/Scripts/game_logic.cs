using UnityEngine;
using System.Collections;

public partial class Game: MonoBehaviour {
    Color colour_snakespit = new Color(0.125f, 0.85f, 0.05f, 0.5f);
    Color colour_damage = new Color(0.85f, 0.05f, 0.125f, 0.5f);


    // bool trytomove(int deltax, int deltay) {
    bool trytomove(int rotdir,bool coasting=false) {
        if (coasting) rotdir = player.mob.facing;

        int deltax = lil.rot_deltax[rotdir];
        int deltay = lil.rot_deltay[rotdir];
        int tentx = player.posx + deltax;
        int tenty = player.posy + deltay;

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
            Speed.SpeedAndDirectionChange(player.mob, rotdir);

        if (player.mob.speed > 0) {
            if (map.passable[tentx, tenty])
            {
                player.posx = tentx; player.posy = tenty;
                moveplayer();
            }
        }

        Etilesprite et = map.displaychar[player.posx, player.posy];
        if (et == Etilesprite.MAP_THIN_ICE && player.mob.speed == 0)
        {
            log.Printline("The thin ice collapses!",Color.red);
            map.displaychar[player.posx, player.posy] = Etilesprite.MAP_WATER;
        }
        else if (et != Etilesprite.MAP_ICE && et!= Etilesprite.MAP_THIN_ICE)
        {
            Speed.change(player.mob, -6);
        }

        if (coasting)
            Speed.change(player.mob, -1);

            TimeEngine = CradleOfTime.player_is_done;        
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
      //code goes here

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

}
