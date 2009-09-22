using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ui
{
    public class Player
    {
        public int id;
        public int nrFails = 0; // de cate ori serverul esuaza sa transmita la client 
                                // pana cand se inchide conexiunea
        public float x;
        public float y;
        public int score;
        public bool[] pressed;
        public int kills;
        public int team;
        public string userName;
        public string password;
        public bool outOfGame;
        public int life;
        /*rotation
         * 0 = LEFT
         * 90 = UP
         * 180 = RIGHT
         * 270 = DOWN
        */
        public int rotation;
        public GameObject bullet;      //glontul tras
        public bool pwuWater; // powerUp water
        public bool pwuSolid; // powerUp beton

        public Player(int id, int team, string userName, string password)
        {
            this.id = id;
            this.nrFails = 0;
            this.team = team;
            this.score = 0;
            this.pressed = new bool[5];
            this.kills = 0;
            this.userName = userName;
            this.password = password;
            this.outOfGame = false;
            this.x = 0;
            this.y = 0;
            bullet = new GameObject(5,5);
            bullet.alive = false;

            this.pwuSolid = false;
            this.pwuWater = false;
        }

       
        public Player(int id)
        {
            this.id = id;
            this.nrFails = 0;
            this.team = -1;
            this.score = 0;
            this.pressed = new bool[5];
            this.kills = 0;
            this.userName = "";
            this.password = "";
            this.outOfGame = true;
            this.x = 0;
            this.y = 0;

            bullet = new GameObject(5, 5);
            bullet.alive = false;
            

            this.pwuSolid = false;
            this.pwuWater = false;
        }
    }
}
