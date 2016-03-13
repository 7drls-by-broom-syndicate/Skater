using UnityEngine;
using System.Collections;

public class item_instance {
    public Etilesprite tile;
    public bool ismob;
    public mob mob;
    public int bombcount;
    public int bombx, bomby; //sloppy- should use mob instead

    public item_instance(Etilesprite _tile,bool _ismob=false,mob m=null,int _bombcount=0)
    {
        tile = _tile;
        ismob = _ismob;
        mob = m;
        bombcount = _bombcount;
    }
}
