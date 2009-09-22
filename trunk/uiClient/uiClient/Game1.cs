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
using System.Threading;



namespace uiClient
{


    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //VARIABILE------------------
        ConnectionManager cm;
        Form1 fm1;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldKbStat;
        clientPacket pachet;
        Boolean mapReceived = false;        //boolean de verificare a hartii primite
        public Boolean readyToReceive = false;
        int myId = -1;
        bool writeWintext = false;
        bool gameWon = false;

        public bool gamePaused = false; // used for pause/resume
            //input variables
            MouseState prevState;
            KeyboardState kstat, oldkbstat;
            

            //int variables
            int inGame = 0, jg = 0, opt = 0, exg = 0;
            int paus = 0, resum, leav;
            int lfa = 0, rga = 0, poz = 240;
            int lfa1 = 0, rga1 = 0;
            int lfa2 = 0, rga2 = 0;
            float volume = 1.0f, volume1 = 1.0f;
            Vector2[] rez;
            Boolean pwapalive = false, pwzidalive = false;

            //font variables
            int experience = 0, armor = 10, attack = 3;
            SpriteFont font;
            SpriteFont font1;

            //Object Variables
            GameObject Body1;
            GameObject Turret1;
            GameObject Body2;
            GameObject Turret2;
            GameObject Body3;
            GameObject Turret3;
            GameObject[] CannonBall= new GameObject[20];


            //Texture Variables
            Texture2D[] Tile = new Texture2D[7];
            Texture2D[,] Tank = new Texture2D[2,4];
            Texture2D JoinGame;
            Texture2D Options;
            Texture2D JoinGameFly;
            Texture2D OptionsFly;
            Texture2D JoinGameOn;
            Texture2D OptionsOn;
            Texture2D ExitGame;
            Texture2D ExitGameFly;
            Texture2D ExitGameOn;
            Texture2D LeftArrow;
            Texture2D RightArrow;
            Texture2D LeftArrowFly;
            Texture2D RightArrowFly;
            Texture2D LeftArrowOn;
            Texture2D RightArrowOn;
            Texture2D Pause;
            Texture2D PauseFly;
            Texture2D PauseOn;
            Texture2D Leave;
            Texture2D LeaveFly;
            Texture2D LeaveOn;
            Texture2D Resume;
            Texture2D ResumeFly;
            Texture2D ResumeOn;
            Texture2D Logo1;
            Texture2D Logo2;
            Texture2D cball;

            //Map Variables
            Texture2D[,] texturi;
            Rectangle[,] tiles;
            Rectangle base1=Rectangle.Empty, base2=Rectangle.Empty;     //bazele respective
            int mapsiz;                 //dimensiunea hartii in patratele
            float scalefact;     //factorul de scalare

            //variabile de tancuri
            int numtanks = 0;           //numarul de tancuri desenate in mod curent
            GameObject[] tanks = new GameObject[20];  //dreptunghiurile pentru tancuri
            int[] tankIDs = new int[20];        //IDul fiecarui tanc
            int bulletno=0;
            Rectangle pwapa, pwzid;
            
            //Variabile sunet
            AudioEngine ae;
            WaveBank wb;
            SoundBank sb;
            


        //FUNCTII PERSONALE--------------------------------------------------------
        //functie care primeste un update periodic
            public void getUpdate(serverPacket pack)
            {
                //daca se primeste harta, se genereaza dreptunghiurile
                if (inGame == 0) generateRectangles();
                
                for (int i = 0; i < bulletno; i++)
                    CannonBall[i].alive = false;
                bulletno = 0;

                int newnumtanks = 0;
                //fm1.MessageFromGame("update: " + pack.xPlayer[0] + " " + pack.yPlayer[0]);
                for (int i = 0; i < 8; i++)
                    if (pack.playerExists[i] == true)
                    {
                        tanks[i] = new GameObject(Tank[0, 1]);   //pozitia tancurilor
                        tanks[i].turret = Tank[1, 1];
                        tanks[i].position.X = pack.xPlayer[i] * tiles[1, 1].Width; ;
                        tanks[i].position.Y = pack.yPlayer[i] * tiles[1,1].Width;
                        tanks[i].rotation = pack.direction[i]*90;
                        
                        newnumtanks++;
                    };
                for (int i = 0; i < pack.changesNo; i++)
                    if (pack.whatChanges[i] == 0)
                    {    //tratare gloante
                        CannonBall[bulletno] = new GameObject(cball);
                        CannonBall[bulletno].alive = true;
                        CannonBall[bulletno].position.X = pack.xChanges[i]* tiles[1,1].Width;
                        CannonBall[bulletno].position.Y = pack.yChanges[i] *tiles[1,1].Width;
                        bulletno++;
                    }
                    else if (pack.whatChanges[i] == 2)
                    {
                        float tx, ty;
                        tx = pack.xChanges[i] * tiles[1, 1].Width;
                        ty = pack.yChanges[i] * tiles[1, 1].Width;
                        for (int i2 = 0; i2 < bulletno; i2++)
                            if (CannonBall[i2].position.X == tx && CannonBall[i2].position.Y == ty)
                                CannonBall[i2].alive = false;
                    }
                    else if (pack.whatChanges[i] == 1)
                    {  //daramam zid
                        int x = (int)pack.xChanges[i];
                        int y = (int)pack.yChanges[i];
                        texturi[x, y] = null;
                    }
                    else if (pack.whatChanges[i] == 5)
                    {
                        //pup apa alive
                        Console.WriteLine("Primit apa la " + pack.xChanges[i] + " " + pack.yChanges[i]);
                        pwapa = new Rectangle((int)pack.xChanges[i] * tiles[1, 1].Width,
                            (int)pack.yChanges[i] * tiles[1, 1].Width, tiles[1, 1].Width, tiles[1, 1].Width);
                        pwapalive = true;
                    }
                    else if (pack.whatChanges[i] == 6)
                    {
                        //omoram powerup apa
                        pwapalive = false;
                    }
                    else if (pack.whatChanges[i] == 7)
                    {
                        //pup zid alive
                        Console.WriteLine("Primit zid la " + pack.xChanges[i] + " " + pack.yChanges[i]);
                        pwzid = new Rectangle((int)pack.xChanges[i] * tiles[1, 1].Width,
                            (int)pack.yChanges[i] * tiles[1, 1].Width, tiles[1, 1].Width, tiles[1, 1].Width);
                        pwzidalive = true;
                    }
                    else if (pack.whatChanges[i] == 8)
                    {
                        //omoram powerup apa
                        pwzidalive = false;
                    }
                    else if (pack.whatChanges[i] == 9)
                    {
                        writeWintext = true;
                        gameWon = true;
                    }
                    else if (pack.whatChanges[i] == 10)
                    {
                        writeWintext = true;
                        gameWon = false;
                    }




                //dam drumul la joc daca nu a fost dat drumu
                if (inGame == 0 && leav==0)
                {
                    inGame = 1;
                    fm1.MessageFromGame("got tank at: " + tanks[0].position.X + " " +
                        tanks[0].position.Y + " mapped from " + pack.xPlayer[0] + " " + pack.yPlayer[0]);
                    fm1.MessageFromGame("latimea unui patrat este " + Tile[1].Width);
                }

                //refacem numarul de tancuri
                numtanks = newnumtanks;
            }

            

        //functie care incarca texturile
        void LoadTextures()
        {
            JoinGame = Content.Load<Texture2D>("JoinGame");
            Options = Content.Load<Texture2D>("Options");
            JoinGameOn = Content.Load<Texture2D>("JoinGameOn");
            OptionsOn = Content.Load<Texture2D>("OptionsOn");
            JoinGameFly = Content.Load<Texture2D>("JoinGameFly");
            OptionsFly = Content.Load<Texture2D>("OptionsFly");
            ExitGame = Content.Load<Texture2D>("ExitGame");
            ExitGameFly = Content.Load<Texture2D>("ExitGameFly");
            ExitGameOn = Content.Load<Texture2D>("ExitGameOn");
            LeftArrow = Content.Load<Texture2D>("LeftArrow");
            RightArrow = Content.Load<Texture2D>("RightArrow");
            LeftArrowFly = Content.Load<Texture2D>("LeftArrowFly");
            RightArrowFly = Content.Load<Texture2D>("RightArrowFly");
            LeftArrowOn = Content.Load<Texture2D>("LeftArrowOn");
            RightArrowOn = Content.Load<Texture2D>("RightArrowOn");
            Pause = Content.Load<Texture2D>("Pause");
            PauseFly = Content.Load<Texture2D>("PauseFly");
            PauseOn = Content.Load<Texture2D>("PauseOn");
            Leave = Content.Load<Texture2D>("Leave");
            LeaveFly = Content.Load<Texture2D>("LeaveFly");
            LeaveOn = Content.Load<Texture2D>("LeaveOn");
            Resume = Content.Load<Texture2D>("Resume");
            ResumeFly = Content.Load<Texture2D>("ResumeFly");
            ResumeOn = Content.Load<Texture2D>("ResumeOn");
            cball=Content.Load<Texture2D>("cannonball");
            //fm1.MessageFromGame("incarcat texturi"+cball.ToString());

            Tile[1] = Content.Load<Texture2D>("Tile 1");
            Tile[2] = Content.Load<Texture2D>("Tile 2");
            Tile[3] = Content.Load<Texture2D>("Tile 3");
            Tile[4] = Content.Load<Texture2D>("Tile 4");
            Tile[5] = Content.Load<Texture2D>("Logo 1");
            Tile[6] = Content.Load<Texture2D>("Logo 2");

           // fm1.MessageFromGame("incarcat texturi2");
            Tank[0, 1] = Content.Load<Texture2D>("Tank 1 - Body");
            Tank[1, 1] = Content.Load<Texture2D>("Tank 1 - Turret");

            Tank[0, 2] = Content.Load<Texture2D>("Tank 2 - Body");
            Tank[1, 2] = Content.Load<Texture2D>("Tank 2 - Turret");

            Tank[0, 3] = Content.Load<Texture2D>("Tank 3 - Body");
            Tank[1, 3] = Content.Load<Texture2D>("Tank 3 - Turret");
           // fm1.MessageFromGame("incarcat texturi3");
            readyToReceive = true;

            font = Content.Load<SpriteFont>("Fonts\\GameFont");
            font1 = Content.Load<SpriteFont>("Fonts\\MenuFont");
            //fm1.MessageFromGame("incarcat texturi4");
            
        }

        //functie care adauga un tanc in vectoru de tancuri
        void addTank(int id, float x, float y, int skin, int dir)
        {
            int i = numtanks; numtanks++;

            tankIDs[i] = id;
            tanks[i] = new GameObject(Tank[0,skin]);
            tanks[i].position.X = (x * graphics.PreferredBackBufferHeight);
            tanks[i].position.Y = (y * graphics.PreferredBackBufferHeight);
            tanks[i].rotation = dir;
            tanks[i].sprite = Tank[0, skin];
            tanks[i].turret = Tank[1, skin];

            CannonBall[i]=new GameObject(cball);
            CannonBall[i].position=tanks[i].center;
            CannonBall[i].alive=false;
        }


            //generare dreptunghiuri in functie de coordonatele date
            //tanks[i]=new Rectangle();
            //tanks[i].X=(int)(x*graphics.PreferredBackBufferHeight);
            //tanks[i].Y=(int)(y*graphics.PreferredBackBufferHeight);
            //if (tankDirs[i] == 1 || tankDirs[i] == 3)
            //    {
            //        tanks[i].Width = (int)(Tank[0, skin].Height * scalefact);
            //        tanks[i].Height = (int)(Tank[0, skin].Width * scalefact);
            //    }
            //    else
            //    {
            //        tanks[i].Width = (int)(Tank[0, skin].Width * scalefact);
            //        tanks[i].Height = (int)(Tank[0, skin].Height * scalefact);
            //    }
            //}
        


        //functie care deseneaza tancurile
        void drawTanks()
        {
            Vector2 center = new Vector2(Tile[1].Width*scalefact / 2, Tile[1].Width*scalefact / 2);
            int i;
            for (i = 0; i < numtanks; i++)
            {
                spriteBatch.Draw(tanks[i].sprite, tanks[i].position+center, null, Color.White,
                        MathHelper.ToRadians(tanks[i].rotation), tanks[i].center, 
                        scalefact * Tile[1].Width / tanks[i].sprite.Width, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(tanks[i].turret, tanks[i].position+center, null, Color.White,
                    MathHelper.ToRadians(tanks[i].rotation), tanks[i].center, 
                    scalefact*Tile[1].Width/tanks[i].sprite.Width, SpriteEffects.None, 0.4f);

            }
                spriteBatch.DrawString(font, "Armor: " + armor.ToString() + "  Attack: " + attack.ToString() + "  XP: " + experience.ToString(),
                    new Vector2(0.7f * graphics.GraphicsDevice.Viewport.Width, 0.03f * graphics.GraphicsDevice.Viewport.Height), Color.LightGray);


                if (paus == 0)
                    spriteBatch.Draw(Pause, new Vector2(graphics.GraphicsDevice.Viewport.Width - Pause.Width, graphics.GraphicsDevice.Viewport.Height - Pause.Height), Color.White);
                else if (paus == 1)
                    spriteBatch.Draw(PauseFly, new Vector2(graphics.GraphicsDevice.Viewport.Width - Pause.Width, graphics.GraphicsDevice.Viewport.Height - Pause.Height), Color.White);
                else if (paus == 2)
                {
                    spriteBatch.Draw(PauseOn, new Vector2(graphics.GraphicsDevice.Viewport.Width - Pause.Width, graphics.GraphicsDevice.Viewport.Height - Pause.Height), Color.White);
                    inGame = 2;
                } 

            for(i=0; i<bulletno; i++)
                if (CannonBall[i].alive == true)
                    spriteBatch.Draw(CannonBall[i].sprite, CannonBall[i].position, null, Color.White,
                        0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);

        }



        //functie care primeste harta ca un string si o incarca
        public void getMap(serverPacket pack)
        {
            string st=pack.Load;
            char[] delim={'|','\n'};
            
            string[] elems=st.Split(delim);
            //fm1.MessageFromGame(elems[1]);
            while (readyToReceive == false) { }
            fm1.MessageFromGame("Hello again");
            parseMap(Int32.Parse(elems[0]),elems[1]);
            myId = pack.ID;
        }

        //functie care defineste harta ca o matrice de dreptunghiuri si texturi
        public void parseMap(int siz, string str)
        {
            int i, j, tileno;
            //Texture2D temptile=null;
            
            texturi = new Texture2D[siz, siz];
            tiles = new Rectangle[siz, siz];
            mapsiz = siz;
            char[] delim = { ' ','\n' };
           
            string[] parsedElements = str.Split(delim);

            //get each tile from the string
            for (i = 0; i < siz; i++)
                for (j = 0; j < siz; j++)
                {
                    tileno = Int32.Parse(parsedElements[i * siz + j]);
                    if (tileno == 0) texturi[i, j] = null;
                    else if (tileno == 1) texturi[i, j] = Tile[1];
                    else if (tileno == 2) texturi[i, j] = Tile[2];
                    else if (tileno == 3) texturi[i, j] = Tile[3];
                    else if (tileno == 4) texturi[i, j] = Tile[4];
                    else if (tileno == 5) texturi[i, j] = Tile[6];
                    else if (tileno == 6) texturi[i, j] = Tile[6];

                    
                }



            mapReceived = true;
        }

        //functie care genereaza dreptunghiurile hartii
        void generateRectangles()
        {
            int i, j;
            //generare dreptunghiuri pe harta in functie de inaltimea ecranului
            int rez = graphics.PreferredBackBufferHeight;
            int rectHeight = (int)rez / mapsiz;
            scalefact = (float)rez / (Tile[1].Height * mapsiz);
            //fm1.MessageFromGame(rez.ToString()+ " "+scalefact.ToString());

            //calculul coordonatelor dreptunghiurilor
            for (i = 0; i < mapsiz; i++)
                for (j = 0; j < mapsiz; j++){
                    tiles[i, j] = new Rectangle(j * rectHeight, i * rectHeight, rectHeight, rectHeight);
                    if (texturi[i,j]==Tile[6]){
                        if (base1==Rectangle.Empty) base1=tiles[i,j];
                        else base2=tiles[i,j];
                    }
                }
        }


        //functie care deseneaza harta
        void drawMap()
        {
            int i, j;
            for (i = 0; i < mapsiz; i++)
                for (j = 0; j < mapsiz; j++)
                    if (texturi[i, j] != null && texturi[i,j]!=Tile[3])
                        spriteBatch.Draw(texturi[i, j], tiles[i, j],null, Color.White,0,Vector2.Zero,SpriteEffects.None,1);
                    else if (texturi[i, j] != null && texturi[i,j]==Tile[3])
                        spriteBatch.Draw(texturi[i, j], tiles[i, j], null,Color.White,0,Vector2.Zero,
                            SpriteEffects.None,0);
            //desenam powerupuri daca sunt
            if (pwapalive == true)
                spriteBatch.Draw(Tile[5], pwapa,null, Color.Blue,0,Vector2.Zero,SpriteEffects.None,0);
            if (pwzidalive==true)
                spriteBatch.Draw(Tile[5], pwzid, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
           // if (pwzidalive) Console.WriteLine("Desenez zid la " + pwzid.X + " " + pwzid.Y);
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
             
            //tratare tasta "p" + "Pause"
            if (pressed.Contains(Keys.P) && oldkeys.Contains(Keys.P) == false)
            { inGame = 2; paus = 0; resum = 0; }
            else if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width - Pause.Width) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width) &&
                    Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height - Pause.Height) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height))
            {
                paus = 1;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                {
                    paus = 2;
                }
            }
            else paus = 0;
            //    pachet = new clientPacket(13, "");
            //if (pressed.Contains(Keys.Pause) && oldkeys.Contains(Keys.Pause) == false)
            //    pachet = new clientPacket(13, "");
            
            //tratare tasta "R" pentru Resume
            //if (pressed.Contains(Keys.R) && oldkeys.Contains(Keys.R) == false)
            //    inGame = 1;
                //pachet = new clientPacket(14, "");

            // nu permitem tranmisterea de miscari pe harta daca jocul e oprit
            //if (gamePaused && pachet.Type < 13) return;

            //trimitere pachet la server
            cm.SendMessage(pachet);
        }



        //FUNCTII DEFAULT----------------------------------------------------------
        public Game1(ConnectionManager cm, Form1 fm1)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.cm = cm;
            this.fm1 = fm1;
            fm1.MessageFromGame("Hello");
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            oldKbStat = Keyboard.GetState();
            pachet = new clientPacket();

            rez = new Vector2[4];
            rez[0] = new Vector2(); rez[0].X = 800; rez[0].Y = 600;
            rez[1] = new Vector2(); rez[1].X = 1024; rez[1].Y = 768;
            rez[2] = new Vector2(); rez[2].X = 1280; rez[2].Y = 1024;
            rez[3] = new Vector2(); rez[3].X = 1440; rez[3].Y = 900;

            graphics.PreferredBackBufferWidth = (int)rez[0].X;
            graphics.PreferredBackBufferHeight = (int)rez[0].Y;

           

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

            //incarcare Audio
            LoadAudio();

            //Incarcare texturi
            LoadTextures();

            addTank(0, 0.14f, 0.23f, 1, 90);

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

            if (inGame == 0) updateMenu();
            else if (inGame == 1)
            {

                //Trimitere la server tasta noua apasata
                kstat = Keyboard.GetState();
                Keys[] pressedKeys = kstat.GetPressedKeys();

                if (inGame == 1)
                    trateazaTaste(kstat, pressedKeys);

                //testTankMovement(kstat, oldkbstat);

            }
            else if (inGame == 2)
                updatePause();

            ae.Update();
            oldKbStat = kstat;
            prevState = Mouse.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkOliveGreen);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);

            //daca suntem in meniu
            if (inGame == 0)
                drawMenu();
            else if (inGame == 1)
            {
                //desenam harta
                if (mapReceived == true)
                    drawMap();

                drawTanks();
            }
            else if (inGame == 2)
                drawPause();


            if (writeWintext == true)
            {
                if (gameWon)
                {
                    spriteBatch.DrawString(font1, "Ai CASTIGAT :D bv bah",
                            new Vector2(graphics.PreferredBackBufferWidth / 2 - 50, graphics.PreferredBackBufferHeight / 2), Color.Gold);
                }
                else
                {
                    spriteBatch.DrawString(font1, "Ai PIERDUT :P LOOSER",
                         new Vector2(graphics.PreferredBackBufferWidth / 2 - 50, graphics.PreferredBackBufferHeight / 2), Color.Gold);
                }
                
            }
            

            spriteBatch.End();
            if (writeWintext == true)
            {
                writeWintext = false;

                int i = 2500000;
                while (i > 0)
                    i--;
                Thread.Sleep(4000);
                inGame = 0;
                cm.SendMessage(new clientPacket(1, ""));
            }
            base.Draw(gameTime);
        }

        //functie de modificat meniul
        void updateMenu()
        {
           if (sb.IsInUse == false)
                {
                    sb.PlayCue("Battlefield");
                    fm1.MessageFromGame("pun sunet");
                }
                if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width / 2 - JoinGame.Width / 2) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width / 2 + JoinGame.Width / 2) &&
                    Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height / 4) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height / 4 + JoinGame.Height))
                {
                    if (opt != 2)
                    {
                        jg = 1;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            jg = 2; opt = 0;
                        }
                    }
                }
                else
                {
                    jg = 0;
                }
                if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width / 2 - Options.Width / 2) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width / 2 + Options.Width / 2) &&
                    Mouse.GetState().Y > (2 * graphics.GraphicsDevice.Viewport.Height / 4) && Mouse.GetState().Y < (2 * graphics.GraphicsDevice.Viewport.Height / 4 + Options.Height))
                {
                    if (opt != 2)
                    {
                        opt = 1;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            opt = 2; jg = 0;
                        }
                    }
                    else
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            opt = 1;
                        }
                    }
                }
                else if (opt != 2)
                {
                    opt = 0;
                }
                if (opt == 2)
                {
                    if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width / 3 + LeftArrow.Width / 2) &&
                           Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height / 3) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height / 3 + LeftArrow.Height))
                    {
                        lfa = 1;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            lfa = 2;
                            poz--;
                        }
                    }
                    else
                    {
                        lfa = 0;
                    }
                    if (Mouse.GetState().X > (2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2) && Mouse.GetState().X < (2 * graphics.GraphicsDevice.Viewport.Width / 3 + LeftArrow.Width / 2) &&
                        Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height / 3) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height / 3 + LeftArrow.Height))
                    {
                        rga = 1;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            rga = 2;
                            poz++;
                        }
                    }
                    else
                    {
                        rga = 0;
                    }

                    if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width / 3 + LeftArrow.Width / 2) &&
                           Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height / 3 - 70) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height / 3 + LeftArrow.Height - 70))
                    {
                        lfa1 = 1;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            lfa1 = 2;
                            volume -= 0.1f;
                            ae.GetCategory("Music").SetVolume(volume);
                        }
                    }
                    else
                    {
                        lfa1 = 0;
                    }
                    if (Mouse.GetState().X > (2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2) && Mouse.GetState().X < (2 * graphics.GraphicsDevice.Viewport.Width / 3 + LeftArrow.Width / 2) &&
                        Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height / 3 - 70) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height / 3 + LeftArrow.Height - 70))
                    {
                        rga1 = 1;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            rga1 = 2;
                            volume += 0.1f;
                            ae.GetCategory("Music").SetVolume(volume);
                        }
                    }
                    else
                    {
                        rga1 = 0;
                    }

                    if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width / 3 + LeftArrow.Width / 2) &&
                           Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height / 3 - 140) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height / 3 + LeftArrow.Height - 140))
                    {
                        lfa2 = 1;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            lfa2 = 2;
                            volume1 -= 0.1f;
                            ae.GetCategory("Default").SetVolume(volume1);
                        }
                    }
                    else
                    {
                        lfa2 = 0;
                    }
                    if (Mouse.GetState().X > (2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2) && Mouse.GetState().X < (2 * graphics.GraphicsDevice.Viewport.Width / 3 + LeftArrow.Width / 2) &&
                        Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height / 3 - 140) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height / 3 + LeftArrow.Height - 140))
                    {
                        rga2 = 1;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            rga2 = 2;
                            volume1 += 0.1f;
                            ae.GetCategory("Default").SetVolume(volume1);
                        }
                    }
                    else
                    {
                        rga2 = 0;
                    }
                }

                if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width / 2 - Options.Width / 2) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width / 2 + Options.Width / 2) &&
                    Mouse.GetState().Y > (3 * graphics.GraphicsDevice.Viewport.Height / 4) && Mouse.GetState().Y < (3 * graphics.GraphicsDevice.Viewport.Height / 4 + Options.Height))
                {
                    exg = 1;
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                    {
                        exg = 2;
                    }
                }
                else
                {
                    exg = 0;
                }
            }



        //functie de afisat meniul
        void drawMenu()
        {
            if (opt == 0)
            {
                spriteBatch.Draw(Options, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Options.Width / 2, 2 * graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
            }
            else if (opt == 1)
            {
                spriteBatch.Draw(OptionsFly, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - OptionsFly.Width / 2, 2 * graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
            }
            else
            {
                spriteBatch.Draw(OptionsOn, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - OptionsFly.Width / 2, 2 * graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
                GraphicsDevice.Clear(Color.DarkOliveGreen);
                jg = -1; exg = -1;
                if (lfa == 0)
                    spriteBatch.Draw(LeftArrow, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                if (rga == 0)
                    spriteBatch.Draw(RightArrow, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                if (lfa == 1)
                    spriteBatch.Draw(LeftArrowFly, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                if (rga == 1)
                    spriteBatch.Draw(RightArrowFly, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                if (lfa == 2)
                {
                    spriteBatch.Draw(LeftArrowOn, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                    graphics.PreferredBackBufferWidth = (int)rez[poz % 4].X;
                    graphics.PreferredBackBufferHeight = (int)rez[poz % 4].Y;
                    graphics.ApplyChanges();

                }
                if (rga == 2)
                {
                    spriteBatch.Draw(RightArrowOn, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                    graphics.PreferredBackBufferWidth = (int)rez[poz % 4].X;
                    graphics.PreferredBackBufferHeight = (int)rez[poz % 4].Y;
                    graphics.ApplyChanges();
                }
                if (lfa1 == 0)
                    spriteBatch.Draw(LeftArrow, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 70), Color.White);
                if (rga1 == 0)
                    spriteBatch.Draw(RightArrow, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 70), Color.White);
                if (lfa1 == 1)
                    spriteBatch.Draw(LeftArrowFly, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 70), Color.White);
                if (rga1 == 1)
                    spriteBatch.Draw(RightArrowFly, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 70), Color.White);
                if (lfa1 == 2)
                    spriteBatch.Draw(LeftArrowOn, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 70), Color.White);
                if (rga1 == 2)
                    spriteBatch.Draw(RightArrowOn, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 70), Color.White);
                if (lfa2 == 0)
                    spriteBatch.Draw(LeftArrow, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 140), Color.White);
                if (rga2 == 0)
                    spriteBatch.Draw(RightArrow, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 140), Color.White);
                if (lfa2 == 1)
                    spriteBatch.Draw(LeftArrowFly, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 140), Color.White);
                if (rga2 == 1)
                    spriteBatch.Draw(RightArrowFly, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 140), Color.White);
                if (lfa2 == 2)
                    spriteBatch.Draw(LeftArrowOn, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 140), Color.White);
                if (rga2 == 2)
                    spriteBatch.Draw(RightArrowOn, new Vector2(2 * graphics.GraphicsDevice.Viewport.Width / 3 - LeftArrow.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3 - 140), Color.White);

                spriteBatch.DrawString(font1, "Resolution", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 70, graphics.GraphicsDevice.Viewport.Height / 3 + 20), Color.DarkRed);
                spriteBatch.DrawString(font1, "Menu Volume", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 97, graphics.GraphicsDevice.Viewport.Height / 3 - 50), Color.DarkRed);
                spriteBatch.DrawString(font1, "Game Volume", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 97, graphics.GraphicsDevice.Viewport.Height / 3 - 120), Color.DarkRed);
                spriteBatch.DrawString(font, rez[poz % 4].X.ToString() + "X" + rez[poz % 4].Y.ToString(), new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 45, graphics.GraphicsDevice.Viewport.Height / 3 + 50), Color.DarkRed);
            }
            if (jg == 0)
            {
                spriteBatch.Draw(JoinGame, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - JoinGame.Width / 2, graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
            }
            else if (jg == 1)
            {
                spriteBatch.Draw(JoinGameFly, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - JoinGameFly.Width / 2, graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
            }
            else if (jg == 2)
            {
                spriteBatch.Draw(JoinGameOn, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - JoinGameFly.Width / 2, graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
                //inGame = 1;
                sendJoinMessage();
            }
            if (exg == 0)
            {
                spriteBatch.Draw(ExitGame, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Options.Width / 2, 3 * graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
            }
            else if (exg == 1)
            {
                spriteBatch.Draw(ExitGameFly, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - OptionsFly.Width / 2, 3 * graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
            }
            else if (exg == 2)
            {
                spriteBatch.Draw(ExitGameOn, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - OptionsFly.Width / 2, 3 * graphics.GraphicsDevice.Viewport.Height / 4), Color.White);
                Exit();
            }
        }

        //functie incarcat audio
        void LoadAudio() {
            ae = new AudioEngine("Audio\\Audio.xgs");
            wb = new WaveBank(ae, "Audio\\Wave Bank.xwb");
            sb = new SoundBank(ae, "Audio\\Sound Bank.xsb");
        }

        //functie care testeaza miscarea tancului printr-un exemplu
        void testTankMovement(KeyboardState kstat, KeyboardState oldkbstate)
        {
            //testare
            if (kstat.IsKeyDown(Keys.Up) == true)
            {
                tanks[0].position.Y--;
                //Turret1.position.Y--;
                tanks[0].rotation = 90;
                //Turret1.rotation = 90;
            }
            else if (kstat.IsKeyDown(Keys.Down) == true)
            {
                tanks[0].position.Y++;
                //Turret1.position.Y++;
                tanks[0].rotation = 270;
                //Turret1.rotation = 270;
            }
            else if (kstat.IsKeyDown(Keys.Right) == true)
            {
                tanks[0].position.X++;
                //Turret1.position.X++;
                tanks[0].rotation = 180;
                //Turret1.rotation = 180;
            }
            else if (kstat.IsKeyDown(Keys.Left) == true)
            {
                tanks[0].position.X--;
                //Turret1.position.X--;
                tanks[0].rotation = 0;
                //Turret1.rotation = 0;
            }

            if (kstat.IsKeyDown(Keys.Space) == true && oldkbstat.IsKeyUp(Keys.Space) == true)
            {
                sb.PlayCue("Boom3");
                CannonBall[0].alive = true;
                CannonBall[0].position = tanks[0].position - CannonBall[0].center;
                CannonBall[0].velocity = new Vector2((float)Math.Cos(MathHelper.ToRadians(tanks[0].rotation)), (float)Math.Sin(MathHelper.ToRadians(tanks[0].rotation))) * 5.0f;
            }
            CannonBall[0].position -= CannonBall[0].velocity;
        }

        //functie care updateaza meniul de pauza
        void updatePause() {
            if (sb.IsInUse == false)
            {
                sb.PlayCue("Battlefield");
            }
            if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width / 2 - Leave.Width / 2) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width / 2 + Leave.Width / 2) &&
                Mouse.GetState().Y > (2 * graphics.GraphicsDevice.Viewport.Height / 3) && Mouse.GetState().Y < (2 * graphics.GraphicsDevice.Viewport.Height / 3 + Leave.Height))
            {
                leav = 1;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                {
                    leav = 2;
                }
            }
            else
            {
                leav = 0;
            }
            if (Mouse.GetState().X > (graphics.GraphicsDevice.Viewport.Width / 2 - Resume.Width / 2) && Mouse.GetState().X < (graphics.GraphicsDevice.Viewport.Width / 2 + Resume.Width / 2) &&
                Mouse.GetState().Y > (graphics.GraphicsDevice.Viewport.Height / 3) && Mouse.GetState().Y < (graphics.GraphicsDevice.Viewport.Height / 3 + Resume.Height))
            {
                resum = 1;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                {
                    resum = 2;
                }
            }
            else
            {
                resum = 0;
            }
        }

        //functie care deseneaza meniul de pauza
        void drawPause()
        {
            if (leav == 0)
                    spriteBatch.Draw(Leave, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Leave.Width / 2, 2 * graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                else if (leav == 1)
                    spriteBatch.Draw(LeaveFly, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Leave.Width / 2, 2 * graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                else
                {
                    spriteBatch.Draw(LeaveOn, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Leave.Width / 2, 2 * graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                    inGame = 0;
                    opt = 0;
                    jg = 0;
                    paus = 0; resum = 0; leav = 1;
                    cm.SendMessage(new clientPacket(1, ""));
                }
                if (resum == 0)
                    spriteBatch.Draw(Resume, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Resume.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                else if (resum == 1)
                    spriteBatch.Draw(ResumeFly, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Resume.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                else
                {
                    spriteBatch.Draw(ResumeOn, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Resume.Width / 2, graphics.GraphicsDevice.Viewport.Height / 3), Color.White);
                    inGame = 1;
                    paus = 0;
                    resum = 0;
                }
            }

        //functie care trimite mesajul de join
        void sendJoinMessage()
        {
            cm.SendMessage(new clientPacket(12,""));
        }
        
    }
}
