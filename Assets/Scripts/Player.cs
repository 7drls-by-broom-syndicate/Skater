using UnityEngine;
using System.Collections;

public class Player {
    public int posx, posy;
    public bool lantern;
    sbyte hp, mana;
    //item_instance* held = nullptr;
    int score;
    public bool stealthed = false;
    int charlevel = 0; int dunlevel = 0;

    //char stasis = 0;
    //bool devcheat = false;

    int turns = 0;

    public Player() {
        init();
    }

    void init(){
		posx = 0; posy = 0;
		lantern = true; stealthed = false;
		hp = 10; mana = 0; //stasis = 0;
        score = 0; charlevel = 0; dunlevel = 0;
        turns = 0;
	}

    //called when a new level has been generated and we want to put the player on it
    public void emerge(int x, int y) {//x and y are a free spot for player to appear on.
        posx = x; posy = y;
        dunlevel++;
    }

    /*void damage(int amount, bool willshieldhelp, string reason){
		if (held != nullptr 
			&& held->type->type == Eitemtype::ITEM_SHIELD){
			playsound("shield");
			messagelog("You block for half damage.");
			hp -= amount;
		}
		else
			hp -= amount*2;

		if (hp <= 0){
			//game over
			//stop playing heart
			stopsound();
			playsound("sadtrombone");

			messagelog("You were killed by " + reason+".",225,0,0);
			gameover = true;
			
		}
		else if (hp <= 2){
			playsoundloop("heart");
		}
	}*/

}

