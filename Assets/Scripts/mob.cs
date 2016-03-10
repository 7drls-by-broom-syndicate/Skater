using UnityEngine;
using System.Collections;

public enum Emobtype { playerpango, playermarsby, playerpapa, playermoop,
    polarmoop, lancer, antipaladin, swinger, tefrog, kobbybomber, bat, mage, necro,
    wolf, kitten1, kitten2, kitten3, kitten4, skelsumd, golem, lich, fox }

public class mobarchetype
{
    public string name;
    public string weaponname;
    public bool skates;
    public bool flies;
    public bool hostile_toplayer;
    public bool hostile_toenemies;
    public bool undead;
    public Etilesprite tile;
    public Etilesprite tile_dead;
    public Etilesprite tile_undead;
    public int hp;

    public mobarchetype(string _name,string _wep,int _hp,
        bool _hostileplayer,bool _hostileenemy,
        bool _undead,bool _skates,bool _flies,
        Etilesprite _tile,Etilesprite _tile_dead,Etilesprite _tile_undead)
    {
        name = _name; weaponname = _wep; hp = _hp; skates = _skates;flies = _flies;
        tile = _tile; tile_dead = _tile_dead;tile_undead = _tile_undead;
    }
}

public class mob {

    static mobarchetype[] archetypes =
    {//                    name       wep       hp  hostp hoste unded   skates flies    tile,deadtile,undeadtile
        new mobarchetype("pango","sword",       40, false,true,false,   true,false,     Etilesprite.PLAYER_PANGO_PANGOLIN,Etilesprite.EMPTY,Etilesprite.EMPTY),
        new mobarchetype("marsby","assegai",    40, false,true,false,   true,false,     Etilesprite.PLAYER_REGINALD_MARSBY,Etilesprite.EMPTY,Etilesprite.EMPTY),
        new mobarchetype("papa greebo","chainhook",40, false,true,false,   true,false,  Etilesprite.PLAYER_PAPA_GREEBO,Etilesprite.EMPTY,Etilesprite.EMPTY),
        new mobarchetype("polarmoop","claws",   60, false,true,false,   true,false,     Etilesprite.PLAYER_POLARMOOP,Etilesprite.EMPTY,Etilesprite.EMPTY),

        new mobarchetype("polarmoop","claws",   15, true,false,false,   false,false,    Etilesprite.ENEMY_POLARMOOP,Etilesprite.ENEMY_POLARMOOP_CORPSE,Etilesprite.ENEMY_POLARMOOP_SKELETON) ,
        new mobarchetype("lancer","spear",      10, true,false,false,   true,false,     Etilesprite.ENEMY_SKATER_SPEAR,Etilesprite.ENEMY_SKATER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON) ,
        new mobarchetype("antipaladin","sword", 10, true,false,false,   true,false,     Etilesprite.ENEMY_SKATER_SWORDANDBOARD,Etilesprite.ENEMY_SKATER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON) ,
        new mobarchetype("swinger","chain",     10, true,false,false,   true,false,     Etilesprite.ENEMY_SKATER_SPEAR,Etilesprite.ENEMY_SKATER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON) ,
        new mobarchetype("tef-rog","dagger",    10, true,false,false,   true,false,     Etilesprite.ENEMY_SKATER_DAGGER,Etilesprite.ENEMY_SKATER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON),
        new mobarchetype("kobby bomber","knife",5,  true,false,false,   false,false,    Etilesprite.ENEMY_KOBBY_BOMBER,Etilesprite.ENEMY_KOBBY_BOMBER_CORPSE,Etilesprite.ENEMY_KOBBY_BOMBER_SKELETON),
        new mobarchetype("giant bat","fangs",   5,  true,false,false,   false,true,     Etilesprite.ENEMY_GIANTBAT,Etilesprite.ENEMY_GIANTBAT_CORPSE,Etilesprite.ENEMY_GIANTBAT_SKELETON),
        new mobarchetype("ice mage","spell",    10, true,false,false,   false,false,    Etilesprite.ENEMY_MAGE,Etilesprite.ENEMY_MAGE_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON),
        new mobarchetype("necromancer","spell", 10, true,false,false,   false,false,    Etilesprite.ENEMY_NECROMANCER,Etilesprite.ENEMY_NECROMANCER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON),

        new mobarchetype("wolf","bite",         5,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_WOLF,Etilesprite.PLAYER_COMPANION_WOLF_CORPSE,Etilesprite.PLAYER_COMPANION_WOLF_SKELETON),
        new mobarchetype("muhkitten","sharps",  1,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_MUHKITTENS_BLACK,Etilesprite.PLAYER_COMPANION_MUHKITTENS_BLACK_CORPSE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_SKELETON),
        new mobarchetype("muhkitten","sharps",  1,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_MUHKITTENS_BW,Etilesprite.PLAYER_COMPANION_MUHKITTENS_BW_CORPSE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_SKELETON),
        new mobarchetype("muhkitten","sharps",  1,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_MUHKITTENS_GINGER,Etilesprite.PLAYER_COMPANION_MUHKITTENS_GINGER_CORPSE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_SKELETON),
        new mobarchetype("muhkitten","sharps",  1,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_MUHKITTENS_BRITISHBLUE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_BRITISHBLUE_CORPSE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_SKELETON),

        new mobarchetype("summoned skel","raw bones",5,true,false,true,    false,false,Etilesprite.ENEMY_HUMAN_SKELETON,Etilesprite.EMPTY,Etilesprite.ENEMY_HUMAN_SKELETON),
        new mobarchetype("ice golem","icy prong",20,true,false,false,  false,false,    Etilesprite.ENEMY_ICE_GOLEM,Etilesprite.EMPTY,Etilesprite.EMPTY),

        new mobarchetype("lich","spell",        5,true,false,true,    false,true,     Etilesprite.ENEMY_LICH,Etilesprite.EMPTY,Etilesprite.EMPTY),

        new mobarchetype("hopped-up fox","bodypart",5,true,true,false,false,false,    Etilesprite.ENEMY_HOPPED_UP_FOX,Etilesprite.ENEMY_HOPPED_UP_FOX_CORPSE,Etilesprite.ENEMY_HOPPED_UP_FOX_SKELETON)
           
 };



    mobarchetype archetype;
    int hp;
    int posx, posy;//maybe not needed

    bool hostile_toplayer_currently;
    bool hostile_toenemies_currently;
    bool undead_currently;
    bool flies_currently;
    bool skates_currently;

    int facing;
    int speed;

	public mob(Emobtype typ)
    {
        mobarchetype at = archetypes[(int)typ];
        hp = at.hp;
        hostile_toenemies_currently = at.hostile_toenemies;
        hostile_toplayer_currently = at.hostile_toplayer;
        undead_currently = at.undead;
        flies_currently = at.flies;
        skates_currently = at.skates;

        facing = lil.randi(0, 7);
        speed = 0;
    }
}
