using System;
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
using System.IO;

namespace WindowsGame4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //variabile 
        Vector2 Rezolutie;
        //texturi
        Texture2D tile1, tile2, tile3, tile4;
        //dimensiunea tabelei in nr patratele
        int mapSize;
        //factor scalare
        float scaleFact;
        //dreptunghiuri pentru patratele
        Rectangle[,] mapa;
        Rectangle[] selectie;
        Rectangle clearScr, save,load;
        //textura pentru patratica respectiva
        int[,] texturi;
        //dimensiunea unei patratele finale
        int tileWith;
        //patratica selectata
        int selectedTile;
        //Font
        SpriteFont font;
        //daca se scrie numele fisierului de salvare sau nu
        bool SavingFile = false;
        bool LoadingFile = false;
        //fisier de salvare al stringului
        String SaveText = "Save Map";
        String LoadText = "Load Map";

        //stare a keyboardului
        KeyboardState oldKbState ;


        //FUNCTII PROPRII-------------------

        //functii de desenat dreptunghiuri
        void DrawRectangles()
        {

            //desenare dreptunghiuri
            Texture2D text = null;
            for (int i = 0; i < mapSize; i++)
                for (int j = 0; j < mapSize; j++)
                {
                    if (texturi[i, j] == 0) continue;
                    else if (texturi[i, j] == 1) text = tile1;
                    else if (texturi[i, j] == 2) text = tile2;
                    else if (texturi[i, j] == 3) text = tile3;
                    else if (texturi[i, j] == 4) text = tile4;
                    spriteBatch.Draw(text,
                        mapa[i, j], null, Color.White, 0.0f,
                        new Vector2(0, 0), SpriteEffects.None, 0);
                }

            //desenare texturi de selectare
            spriteBatch.Draw(tile1, selectie[1], Color.White);
            spriteBatch.Draw(tile2, selectie[2], Color.White);
            spriteBatch.Draw(tile3, selectie[3], Color.White);
            spriteBatch.Draw(tile4, selectie[4], Color.White);
        }

        //functie de desenat texte
        void DrawText()
        {
            spriteBatch.DrawString(font, "Clear screen",
                new Vector2(clearScr.Left, clearScr.Top), Color.Red);
            spriteBatch.DrawString(font, SaveText,
                new Vector2(save.Left, save.Top), Color.Green);
            spriteBatch.DrawString(font, LoadText,
                new Vector2(load.Left, load.Top), Color.SpringGreen);
            
        }




        //functie de curatat ecranul
        void ClearScreen()
        {
            for (int i = 0; i < mapSize; i++)
                for (int j = 0; j < mapSize; j++)
                    texturi[i, j] = 0;   
        }


        //functie de updatat textul la salvare fisier
        void updateSavetext(KeyboardState kstat)
        {
            Keys[] kpressed = kstat.GetPressedKeys();
            if ((kpressed[0] >= Keys.A && kpressed[0] <= Keys.Z))

                SaveText += kpressed[0].ToString().ToLower();
            if (kpressed[0] == Keys.OemPeriod)
                SaveText += '.';
            if (kpressed[0] == Keys.Back && SaveText.Length > 0)
                SaveText = SaveText.Remove(SaveText.Length - 1);
            if (kpressed[0] == Keys.Enter)
            {
                SavingFile = false;
                SaveToFile(SaveText);
            }

        }


        //functie de updatat textul la incarcare fisier
        void updateLoadtext(KeyboardState kstat)
        {
            Keys[] kpressed = kstat.GetPressedKeys();
            if ((kpressed[0] >= Keys.A && kpressed[0] <= Keys.Z))

                LoadText += kpressed[0].ToString().ToLower();
            if (kpressed[0] == Keys.OemPeriod)
                LoadText += '.';
            if (kpressed[0] == Keys.Back && LoadText.Length > 0)
                LoadText = LoadText.Remove(LoadText.Length - 1);
            if (kpressed[0] == Keys.Enter)
            {
                LoadingFile = false;
                LoadToFile(LoadText);
            }

        }




        //fucntie care salveaza harta intr-un fisier
        void SaveToFile(String fname)
        {
            //verificare string valid
            if (fname.IndexOf('.') != fname.LastIndexOf('.'))
                SaveText = "Nume Invalid";
            else
            {
                StreamWriter sr = new StreamWriter(fname);
                sr.WriteLine(mapSize);
                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                        sr.Write(texturi[i, j] + " ");
                    //sr.WriteLine();
                }
                sr.Close();
                
                SaveText = "Fisier Salvat";
            }
        }



        //fucntie care incarca harta dintr-un fisier
        void LoadToFile(String fname)
        {
            //verificare string valid
            if (fname.IndexOf('.') != fname.LastIndexOf('.'))
                LoadText = "Nume Invalid";
            else
            {
                StreamReader sr = new StreamReader(fname);
                String str = sr.ReadLine();
                char[] delim = { ' ', '\n' };
                String[] elem = str.Split(delim);
               // Rezolutie.X = Int32.Parse(elem[0]);
                //Rezolutie.Y = Int32.Parse(elem[1]);
                mapSize = Int32.Parse(elem[0]);
                LoadContent();

                str = sr.ReadLine();
                elem = str.Split(delim);
                for (int i = 0; i < mapSize; i++)
                    for (int j = 0; j < mapSize; j++)
                        texturi[i, j] = Int32.Parse(elem[i * mapSize + j]);

                sr.Close();
                LoadText = "Fisier Incarcat";

            }
        }



        //functie care creeaza dreptunghiuri 
        void CreateRectangles()
        {
            //setam dreptunghiurile pentru harta si selectie
            mapa = new Rectangle[mapSize, mapSize];
            texturi = new int[mapSize, mapSize];
            selectie = new Rectangle[5];
            selectie[0] = new Rectangle((int)Rezolutie.X - tileWith, 0, tileWith, tileWith);
            selectie[1] = new Rectangle((int)Rezolutie.X - tileWith, tileWith, tileWith, tileWith);
            selectie[2] = new Rectangle((int)Rezolutie.X - tileWith, tileWith * 2, tileWith, tileWith);
            selectie[3] = new Rectangle((int)Rezolutie.X - tileWith, tileWith * 3, tileWith, tileWith);
            selectie[4] = new Rectangle((int)Rezolutie.X - tileWith, tileWith * 4, tileWith, tileWith);
            clearScr = new Rectangle((int)Rezolutie.X - tileWith - 50, tileWith * 5, tileWith + 30, 30);
            save = new Rectangle((int)Rezolutie.X - tileWith - 50, (int)Rezolutie.Y-100, tileWith + 50, 30);
            load = new Rectangle((int)Rezolutie.X - tileWith - 50, (int)Rezolutie.Y - 70, tileWith + 50, 30);

            //setam pozitiile dreptunghiurilor
            for (int i = 0; i < mapSize; i++)
                for (int j = 0; j < mapSize; j++)
                    mapa[i, j] = new Rectangle(i * tileWith, j * tileWith, tileWith, tileWith);
        }

        //functie care trateaza clickurile de mouse
        void TrateazaClick(MouseState stat, Vector2 clickSquare)
        {
            if (selectie[0].Contains(stat.X, stat.Y)) selectedTile = 0;
            if (selectie[1].Contains(stat.X, stat.Y)) selectedTile = 1;
            if (selectie[2].Contains(stat.X, stat.Y)) selectedTile = 2;
            if (selectie[3].Contains(stat.X, stat.Y)) selectedTile = 3;
            if (selectie[4].Contains(stat.X, stat.Y)) selectedTile = 4;
            //verificam daca sa stergem ecranul
            if (clearScr.Contains(stat.X, stat.Y)) ClearScreen();
            //verificam daca vrem sa salvam
            if (save.Contains(stat.X, stat.Y))
            {
                SavingFile = true;
                SaveText = "";
            }

            if (load.Contains(stat.X, stat.Y))
            {
                LoadingFile= true;
                LoadText = "";
            }

            //generam patratica unde ar da click 
            clickSquare.X = stat.X / tileWith;
            clickSquare.Y = stat.Y / tileWith;

            //daca s-a dat click pe mapa, aplicam bucata respectiva
            if (clickSquare.X >= 0 && clickSquare.X < mapSize && clickSquare.Y >= 0
                && clickSquare.Y < mapSize)
                texturi[(int)clickSquare.X, (int)clickSquare.Y] = selectedTile;
        }

        //FUNCTII NECESARE----------------------
        public Game1(int W, int H,int siz)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Rezolutie.X = W;
            Rezolutie.Y = H;
            //setare rezolutie
            graphics.PreferredBackBufferWidth = W;
            graphics.PreferredBackBufferHeight = H;
            //permitere mouse
            this.IsMouseVisible = true;
            oldKbState = Keyboard.GetState();

            mapSize = siz;
        }



        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            selectedTile = 0;
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
            
            //incarcare font
            font = Content.Load<SpriteFont>("myFont");

            //incarcare texturi
            tile1 = Content.Load<Texture2D>("Tile 1");
            tile2 = Content.Load<Texture2D>("Tile 2");
            tile3 = Content.Load<Texture2D>("Tile 3");
            tile4 = Content.Load<Texture2D>("Tile 4");

            //calcul dimensiune dreptunghi in imaginea finala
            tileWith = (int)Rezolutie.Y / mapSize ;

            //calcul factor de scalare pentru imagini
            //Rezolutie verticala/nr patratele / dim text
            scaleFact = (Rezolutie.Y / tile1.Height) / mapSize;

            //creem dreptunghiuti
            CreateRectangles();
            
            //TEST!!!!!!!
            Random ra = new Random();
            for (int i = 0; i < mapSize; i++)
                for (int j = 0; j < mapSize; j++)
                    texturi[i, j] = ra.Next(5);
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
            MouseState stat = Mouse.GetState();
            KeyboardState kstat = Keyboard.GetState();
            Vector2 clickSquare = new Vector2();

            //vedem pe ce a facut click utilizatorul
            if (stat.LeftButton == ButtonState.Pressed)
            TrateazaClick(stat,clickSquare);
            //vedem daca s-a apasat vreo tasta si se citeste un nume
            if (SavingFile == true && kstat.GetPressedKeys().Count() > 0 && oldKbState != kstat)
                updateSavetext(kstat);

            if (LoadingFile== true && kstat.GetPressedKeys().Count() > 0 && oldKbState != kstat)
                updateLoadtext(kstat);
           
            oldKbState = kstat;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);
            
            //inceput desenare
            spriteBatch.Begin();

            //desenare dreptunghiuri
            DrawRectangles();
            
            //desenare texte
            DrawText();
            

            //final desenare
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

