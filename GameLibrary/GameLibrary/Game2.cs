using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using PacketL;


namespace uiClient
{


    public class Game2 : Microsoft.Xna.Framework.Game
    {
        //VARIABILE------------------
        ConnectionManager cm;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldKbStat;
        clientPacket pachet;


            //Texture Variables
            Texture2D[] Tile = new Texture2D[5];
            Texture2D[,] Tank = new Texture2D[2,4];
            

            //Map Variables
            Texture2D[,] texturi;
            Rectangle[,] tiles;
            int mapsiz;                 //dimensiunea hartii in patratele
            float scalefact;     //factorul de scalare

            //variabile de tancuri
            int numtanks = 0;           //numarul de tancuri desenate in mod curent
            //GameObject[] tanks = new GameObject[10];



        //FUNCTII PERSONALE--------------------------------------------------------
        //functie care incarca texturile
        void LoadTextures()
        {
            Tile[1] = Content.Load<Texture2D>("Tile 1");
            Tile[2] = Content.Load<Texture2D>("Tile 2");
            Tile[3] = Content.Load<Texture2D>("Tile 3");
            Tile[4] = Content.Load<Texture2D>("Tile 4");

            Tank[0, 1] = Content.Load<Texture2D>("Tank 1 - Body");
            Tank[1, 1] = Content.Load<Texture2D>("Tank 1 - Turret");

            Tank[0, 2] = Content.Load<Texture2D>("Tank 2 - Body");
            Tank[1, 2] = Content.Load<Texture2D>("Tank 2 - Turret");

            Tank[0, 3] = Content.Load<Texture2D>("Tank 3 - Body");
            Tank[1, 3] = Content.Load<Texture2D>("Tank 3 - Turret");
        }



        //functie care primeste harta ca un string si o incarca
        public void getMap(string st) {
            char[] delim={'|'};
            string[] elems=st.Split(delim);

            parseMap(Int32.Parse(elems[0]),elems[1]);
        }

        //functie care defineste harta ca o matrice de dreptunghiuri si texturi
        public void parseMap(int siz, string str)
        {
            int i, j, tileno;
            Texture2D temptile;

            texturi = new Texture2D[siz, siz];
            tiles = new Rectangle[siz, siz];
            mapsiz = siz;
            char[] delim = { ' ' };
            string[] parsedElements = str.Split(delim);

            //get each tile from the string
            for (i = 0; i < siz; i++)
                for (j = 0; j < siz; j++)
                {
                    tileno = Int32.Parse(parsedElements[i * siz + j]);
                    if (tileno == 0) temptile = null;
                    else if (tileno == 1) temptile = Tile[1];
                    else if (tileno == 2) temptile = Tile[2];
                    else if (tileno == 3) temptile = Tile[3];
                    else if (tileno == 4) temptile = Tile[4];
                }

            //generare dreptunghiuri pe harta in functie de inaltimea ecranului
            int rez = graphics.GraphicsDevice.Viewport.Height;
            int rectHeight = (int) rez / mapsiz;
            scalefact = rez / (Tile[1].Height * mapsiz);

            //calculul coordonatelor dreptunghiurilor
            for(i=0; i<siz; i++)
                for (j = 0; j < siz; j++)
                    tiles[i,j]=new Rectangle(i*rectHeight, j*rectHeight,rectHeight,rectHeight);



        }


        //functie care deseneaza harta
        void drawMap()
        {
            int i, j;
            for (i = 0; i < mapsiz; i++)
                for (j = 0; j < mapsiz; j++)
                    if (texturi[i, j] != null)
                        spriteBatch.Draw(texturi[i, j], tiles[i, j], Color.White);
        }



        //cand primesti un mesaj de la server, updateaza mediul jocului cu informatiile respective
        public void updateStuff(String s)
        {
            String[] parts;
            char[] delim = { '|' };
            //luam stringul si parsam informatiile
            parts = s.Split(delim);


        }


        //functie care trateaza apasarile pe taste si le trimite la server
        void trateazaTaste(KeyboardState kstat, Keys[] pressed)
        {
            Keys[] oldkeys = oldKbStat.GetPressedKeys();
            //tratare tasta sus
            if (pressed.Contains(Keys.Up) && oldkeys.Contains(Keys.Up) == false)
                pachet = new clientPacket(2, "");
            if (pressed.Contains(Keys.Up) == false && oldkeys.Contains(Keys.Up))
                pachet = new clientPacket(3, "");

            //tratare tasta jos
            if (pressed.Contains(Keys.Down) && oldkeys.Contains(Keys.Down) == false)
                pachet = new clientPacket(4, "");
            if (pressed.Contains(Keys.Down) == false && oldkeys.Contains(Keys.Down))
                pachet = new clientPacket(5, "");

            //tratare tasta stanga
            if (pressed.Contains(Keys.Left) && oldkeys.Contains(Keys.Left) == false)
                pachet = new clientPacket(6, "");
            if (pressed.Contains(Keys.Left) == false && oldkeys.Contains(Keys.Left))
                pachet = new clientPacket(7, "");


            //tratare tasta dreapta
            if (pressed.Contains(Keys.Right) && oldkeys.Contains(Keys.Right) == false)
                pachet = new clientPacket(8, "");
            if (pressed.Contains(Keys.Right) == false && oldkeys.Contains(Keys.Right))
                pachet = new clientPacket(9, "");

            //tratare tasta space
            if (pressed.Contains(Keys.Space) && oldkeys.Contains(Keys.Space) == false)
                pachet = new clientPacket(10, "");
            if (pressed.Contains(Keys.Space) == false && oldkeys.Contains(Keys.Space))
                pachet = new clientPacket(11, "");


            //trimitere pachet la server
            cm.SendMessage(pachet);
        }



        //FUNCTII DEFAULT----------------------------------------------------------
        public Game2(ConnectionManager cm)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.cm = cm;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            oldKbStat = Keyboard.GetState();
            pachet = new clientPacket();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Incarcare texturi
            LoadTextures();
            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Trimitere la server tasta noua apasata
            KeyboardState kstat = Keyboard.GetState();
            Keys[] pressedKeys = kstat.GetPressedKeys();

            if (kstat != oldKbStat)
                trateazaTaste(kstat, pressedKeys);
            oldKbStat = kstat;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();


            //desenam harta
            drawMap();


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
