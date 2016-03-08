using UnityEngine;
using System.Collections;

public partial class Game: MonoBehaviour {

    bool trytomove(int deltax, int deltay) {
        int tentx = player.posx + deltax;
        int tenty = player.posy + deltay;
        
        if (tentx < 0 || tentx >= map.width || tenty < 0 || tenty >= map.height) return false;

        if (!map.passable[tentx,tenty]) return false;

        player.posx = tentx; player.posy = tenty;
        moveplayer();
        return true;
    }
	
}
