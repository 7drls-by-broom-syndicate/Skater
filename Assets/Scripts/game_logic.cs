﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Game : MonoBehaviour
{
    Color colour_snakespit = new Color(0.125f, 0.85f, 0.05f, 0.5f);
    Color colour_damage = new Color(0.85f, 0.05f, 0.125f, 0.5f);


    // bool trytomove(int deltax, int deltay) {

    void movemob(mob m, int tentx, int tenty)
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
    bool checkforitemactivation(mob m,int tentx,int tenty)
    {
        item_instance i = map.itemgrid[tentx, tenty];
        if (i == null) return false;
        if (i.tile == Etilesprite.ITEM_BARREL)
        {
            if (map.extradata[tentx, tenty] != null)
            {
                log.Printline("Inside the barrel was a something!", Color.blue);
                i.tile = Etilesprite.ITEM_WARP_BEADS;
                map.extradata[tentx, tenty] = null;
                return true;
            }
            else
            {
                log.Printline("Take that, you barrel bastard!", Color.gray);
                i.tile = Etilesprite.ITEM_BARREL_BROKEN;
                map.extradata[tentx, tenty] = null;
                return true;
            }
        }
        else if (i.tile == Etilesprite.ITEM_BARREL_BROKEN)
        {
            log.Printline(m.archetype.name + " plays cleanup!", Color.gray);
            map.itemgrid[tentx, tenty] = null;
            map.passable[tentx, tenty] = true;
            return true;
        }
        else if (i.tile == Etilesprite.ITEM_WARP_BEADS)
        {
            log.Printline(m.archetype.name + " collects the Warp Beads!", Color.magenta);
            map.itemgrid[tentx, tenty] = null;
            map.passable[tentx, tenty] = true;
            m.hasbeads = true;
            return true;
        }
        else if (i.tile == Etilesprite.ITEM_CAIRN_GREEN || i.tile == Etilesprite.ITEM_CAIRN_RED || i.tile == Etilesprite.ITEM_CAIRN_BLUE || i.tile == Etilesprite.ITEM_CAIRN_PURPLE)//DEBUGGING
        {
            log.Printline("The ", Color.gray);
            log.Print("red cairn ", Color.red);
            log.Print("has healing properties!");
            FloatingDamage(m, m, +5 + lil.randi(0, 10), "magic");
            i.tile = Etilesprite.ITEM_CAIRN_USED_RED;
            map.dostaticlights();
            return true;
        }
        return false;
    }

    bool trytomove(mob m, int rotdir, bool coasting = false)
    { if (coasting) rotdir = m.facing;
        int deltax = lil.rot_deltax[rotdir];
        int deltay = lil.rot_deltay[rotdir];
        int tentx = m.posx + deltax;
        int tenty = m.posy + deltay;

        if (tentx < 0 || tentx >= map.width || tenty < 0 || tenty >= map.height)
                {
                    Speed.change(m, -200);
                    return false;
                }
        bool didanactivation=false;
        if (m.speed == 0 && !coasting) didanactivation=checkforitemactivation(m, tentx, tenty);

        // Debug.Log(m.archetype.name + " " + rotdir);
        if (coasting && !m.skates_currently) goto playercoastingbutnotaskater;//was &&m.isplayer

       

      

      


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

        if (m.speed > 0 && !didanactivation)
        {
            if (map.passablecheck(tentx, tenty, m))
            {
                movemob(m, tentx, tenty);
            }
            else
            {//not passable this could be a mob, water or something like a tree, cairn or wall                            
                item_instance i = map.itemgrid[tentx, tenty]; //1.is there a mob there:
                if (i != null)
                {
                    if (i.ismob)//mob crashes into mob! this could need changing
                    {
                        //bumping into something attacks it. 
                        //might need to make this player only or mobs could attack twice?
                        MobAttacksMob(m, i.mob);
                        m.speed = 0;
                    }
                    else
                    {//item there but not a mob, so cairn, tree, ?
                     //take damage                        
                        if (m.speed > 0)
                        {
                            FloatingDamage(m, m, -(m.speed / 2), "crashed into " + Tilestuff.tilestring[(int)i.tile + 2]);
                            m.speed = 0;
                        }
                        else {

                        }
                    }
                }
                else
                {//no mob or item to crash into but maybe non-passable map tile or water
                    if (map.displaychar[tentx, tenty] == Etilesprite.MAP_WATER)
                    {

                        if (map.displaychar[m.posx, m.posy] == Etilesprite.MAP_WATER)
                            log.Printline(m.archetype.name + " wades on, foolishly!", Color.magenta);
                        else log.Printline(m.archetype.name + " skids into the water!", Color.magenta);
                        movemob(m, tentx, tenty);
                        m.speed = 0;

                    }
                    else if (map.displaychar[tentx, tenty] == Etilesprite.ITEM_WARP_GATE_ANIM_1 && m.hasbeads)
                    {//this means in theory, depending on how i coded this, a mob could get you to next level!
                        log.Printline("I'm stepping through the door", Color.green);
                        log.Printline("And I'm floating in a most peculiar way", Color.green);
                        log.Printline("And the stars look very different today", Color.green);
                        NextLevel();
                    }
                    else
                    {//non-passable tile that isn't water, so probably snow-covered rock
                        FloatingDamage(m, m, -(m.speed / 2), "crashed into " + Tilestuff.tilestring[(int)map.displaychar[tentx, tenty] + 2]);
                        m.speed = 0;
                    }
                }
            }//end of not passable
        }//end speed >0
        else m.speed = 0;



        //if on thin ice and no speed, or if moop, and not flying fall through
        Etilesprite et = map.displaychar[m.posx, m.posy];
        if (et == Etilesprite.MAP_THIN_ICE && (m.speed == 0 || (m.archetype.heavy) && !m.flies_currently))
        {
            if (m.isplayer) log.Printline("The thin ice collapses!", Color.red);
            map.displaychar[m.posx, m.posy] = Etilesprite.MAP_WATER;
            map.passable[m.posx, m.posy] = false;
            if (map.bloodgrid[m.posx, m.posy] != null)
            {
                log.Printline("The blood disperses", new Color(0.5f, 0, 0));
                map.bloodgrid[m.posx, m.posy] = null;
            }
            //if (!m.archetype.heavy) FloatingDamage(m, m, -lil.randi(1, 4), "cold");
        }

        //general if you are in water you get damaged line
        if (map.displaychar[m.posx, m.posy] == Etilesprite.MAP_WATER)
            if (!m.archetype.heavy) FloatingDamage(m, m, -lil.randi(1, 4), "cold");

        //if not on ice or thin ice, reduce speed such that it is gone in 2 turns
        if (!m.skates_currently || (et != Etilesprite.MAP_ICE && et != Etilesprite.MAP_THIN_ICE))
        {
            Speed.change(m, Speed.nonice);
        }
        //coasting reduces speed by 1
        if (coasting)
            Speed.change(m, Speed.coasting);
        playercoastingbutnotaskater:
        if (m.isplayer) TimeEngine = CradleOfTime.player_is_done;
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
        foreach (var f in map.moblist)
        {
            if (f.dead_currently == false && f.hp <= 0)
            {
                log.Printline("The " + f.archetype.name + " dies.", new Color(0.6f, 0, 0));
                player.score++;
                f.speed = 0;
                f.dead_currently = true;
                f.tile = f.archetype.tile_dead;
                if (map.itemgrid[f.posx, f.posy] == null) Debug.Log("error map thing in mob dies thing");
                else map.itemgrid[f.posx, f.posy].tile = f.tile;
            }
        }
        if (player.hp <= 0)
        {
            log.Printline("This life no longer grips you.", Color.magenta);
            log.Printline("Now you can into Sky Burrow.", Color.magenta);

            TimeEngine = CradleOfTime.dormant;
            gamestate = Egamestate.gameover;
            MyAudioSource.Stop();
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

        if (amount > 0 && victim.hp + amount > victim.archetype.hp) amount = victim.archetype.hp - victim.hp;

        FloatingTextItems.Add(new FloatingTextItem(explanation + " " + amount + " hp", victim.posx, victim.posy, c));

        if (attacker == player.mob && victim != player.mob)
        {
            log.Printline(attacker.archetype.name + " deals " + (-amount) + " to " + victim.archetype.name + " [" + explanation + "]", Color.green);
        }
        else {
            log.Printline(victim.archetype.name, c);
            if (amount <= 0) log.Print(" takes ", c);
            else log.Print(" gains ", c);
            if (victim == attacker) log.Print(amount + " ", c);
            else log.Print(amount + " from " + attacker.archetype.name, c);
            if (explanation.Length > 0) log.Print("[" + explanation + "]", c);
        }
        //actually do the damage
        victim.hp += amount;
        c.a = 0.5f;
        if (map.displaychar[victim.posx, victim.posy] != Etilesprite.MAP_WATER && amount != 0)
            if (map.bloodgrid[victim.posx, victim.posy] == null) map.bloodgrid[victim.posx, victim.posy] = lil.randi(0, 3) + ((amount < -3) ? 4 : 0);
        map.gridflashcolour[victim.posx, victim.posy] = c;
        map.gridflashtime[victim.posx, victim.posy] = Time.time + 0.5f;

    }
    void BresLineColour(int startx, int starty, int endx, int endy, bool includestart, bool includeend, Color c)
    {
        List<Cell> celllist = map.BresLine(startx, starty, endx, endy);
        for (int i = 0; i < celllist.Count; i++)
        {
            if (!includestart && startx == celllist[i].x && starty == celllist[i].y) continue;
            if (!includeend && endx == celllist[i].x && endy == celllist[i].y) continue;
            map.gridflashcolour[celllist[i].x, celllist[i].y] = c;
            map.gridflashtime[celllist[i].x, celllist[i].y] = Time.time + 0.5f;
        }
    }
    void MobAttacksMob(mob attacker, mob target)

    {
        int damage = attacker.speed - target.speed;
        if (damage < 1) damage = 1;
        FloatingDamage(target, attacker, -damage, attacker.archetype.weaponname);
    }

    void MobGetsToAct(mob e)
    {
        if (!e.noticedyou || e.dead_currently) return; //METAL MOOP SOLID
        if (e.IsAdjacentTo(player.mob))
        {
            MobAttacksMob(e, player.mob);
            e.speed = 0;//this is a hack. mobs should attack and coast. all combat is a hack at the moment though
            return;
        }

        map.passable[e.posx, e.posy] = true;//we need square the mob starts on to be passable, for pathfinding.
                                            //attempt to move 
        if (map.PathfindAStar(e.posx, e.posy, player.posx, player.posy, false))
        {
            int reldir = Speed.findrel(e.posx, e.posy, map.firststepx, map.firststepy);
            trytomove(e, reldir);
            map.passable[e.posx, e.posy] = false;
            return;
        }
        //map.passable[e.posx, e.posy] = false;//we can't do this though- this hard sets the square to not passable. 
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
