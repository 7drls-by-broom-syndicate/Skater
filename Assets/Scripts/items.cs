using UnityEngine;
using System.Collections;

public class item_instance {
    public Etilesprite tile;
    public bool ismob;
    public mob mob;
    public item_instance(Etilesprite _tile,bool _ismob=false,mob m=null)
    {
        tile = _tile;
        ismob = _ismob;
        mob = m;
    }
}
