using UnityEngine;
using System.Collections;

public class item_instance {
    public Etilesprite tile;
    public bool ismob;

    public item_instance(Etilesprite _tile,bool _ismob=false)
    {
        tile = _tile;
        ismob = _ismob;
    }
}
