using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using PacketL;
using System.Timers;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using uiClient;
//using System.Collections.Generic;
//using System.Linq;


namespace ui
{
    public partial class Form1 : Form
    {
        //VARS--------------------------------------------------
        //map variables
        int[,] tiles = new int[20, 20];
        Rectangle[,] tileRect = new Rectangle[20, 20];

        //given dimensions for known units
        public int tilewidth = 80;
        public int tankwidth = 128;
        int mapsize;

        //the two bases
        public bool Base1Up, Base2Up;
        public int Base1Arm, Base2Arm;
        public int Base1X, Base1Y, Base2X, Base2Y; //TODO get the values for these
        public int[] playersNoTeam;
        public int killScore;

        //rounde
        public bool roundOver;


        // powerUp timer
        int timerWater = 0;
        int timerSolid = 0;
        bool pwuWaterOn = false; // if pwu Water exits on the map
        bool pwuSolidOn = false; // if pwu Water exits on the map
        float xpwuWater = -1;
        float ypwuWater = -1;
        float xpwuSolid = -1;
        float ypwuSolid = -1;

        Random rand = new Random();

        //client array
        public Player[] players;
        public int noPlayers = 0;
        public int MAX_NO_PLAYERS = 8;

        //the unit for changing position
        public int dx, dy;

        //message received from client
        public clientPacket cp;

        //string that stores a map (size|square1 square2.....)
        string mapstring = "";

        private static System.Timers.Timer aTimer;

        //true until the first client joins the game
        public bool firstClient;


        public static int MAX_LAG = 3; // de cate ori are voie sa rateze transmiterea unui packet
        public static int PORT = 7033;
        public static int UP = 1;
        public static int DOWN = 3;
        public static int LEFT = 0;
        public static int RIGHT = 2;

        /* 
         *  Vars used by server
         */
        Socket pasiveSocket;
        public static int MaxClients = 10;
        List<Socket> newSocket = new List<Socket>(MaxClients);
        List<ConnectionManager> serverCM = new List<ConnectionManager>(MaxClients);

        // used for the process of accepting new clients
        public static ManualResetEvent allDone = new ManualResetEvent(false);
       

        public DatabaseManager dbmanager;

        //FUNCTII-------------------------------------------
        void initializePlayers(int i)
        {
            players[i] = new Player(i);
            players[i].outOfGame = true;
        }

        //definire metoda de message box
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWndle, String text, String caption, int buttons);
        
        public Form1()
        {
            // set up database
            dbmanager = new DatabaseManager();
            //System.Web.Security.Membership.CreateUser("as", "as");

            InitializeComponent();
            GetIpList();

            this.Text = "Game Server";
            textBox1.Text = PORT.ToString();
            comboBox2.SelectedIndex = 0;

            killScore = 1;

            playersNoTeam = new int[2];
            playersNoTeam[0] = playersNoTeam[1] = 0;

            Base1Up = true;
            Base2Up = true;
            Base1Arm = 50;
            Base2Arm = 50;

            roundOver = false;


            // initialize here - but noPlayers remains 0
            players = new Player[MAX_NO_PLAYERS];
            for (int i = 0; i < MAX_NO_PLAYERS; i++)
                initializePlayers(i);
              
            noPlayers = 0;
            dx = dy = 5;
            firstClient = true;

            // Create a timer with a 1 second interval.
            aTimer = new System.Timers.Timer(1000);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Set the Interval to 1/30 seconds (33 milliseconds).
            aTimer.Interval = 60;
            //aTimer.Enabled = true;   
        }

        Boolean checkIfBulletIntersectsThingies(Rectangle bullet, serverPacket pack, int id)
        {

            for (int i = 0; i < mapsize; i++)
                for (int j = 0; j < mapsize; j++)
                    //if we hit bricks, they fall down
                    if (tiles[i, j] == 1 && tileRect[i, j].Intersects(bullet) == true)
                    {
                        pack.changesNo++;
                        pack.whatChanges.Add(1);
                        pack.xChanges.Add(j);
                        pack.yChanges.Add(i);
                        players[id].bullet.alive = false;

                        tiles[i, j] = 0;

                        return true;
                    }
                    //if we hit solid
                    else if ((tiles[i, j] == 4) && tileRect[i, j].Intersects(bullet) == true)
                    {
                        //if we can break solid blocks
                        if (players[id].pwuSolid == true)
                        {
                            tiles[i, j] = 0;
                            pack.changesNo++;
                            pack.whatChanges.Add(1);
                            pack.xChanges.Add(j);
                            pack.yChanges.Add(i);
                        }
                        return true;
                    }
                    //if we hit a base
                    else if ((tiles[i, j] == 5 || tiles[i, j] == 6) && tileRect[i, j].Intersects(bullet) == true)
                    {
                        //if first base was hit
                        if (i == Base1X && j == Base1Y)
                        {
                            Base1Arm -= 10;
                            //base 1 was destroyed
                            if (Base1Arm <= 0)
                            {
                                pack.changesNo++;
                                pack.whatChanges.Add(3);
                                pack.xChanges.Add(j);
                                pack.yChanges.Add(i);
                                pack.changesNo++;
                                pack.whatChanges.Add(9); //round over message... team 2 has won
                                roundOver = true;
                            }
                        }
                        else
                        {
                            Base2Arm -= 10;
                            //base 2 was destroyed
                            if (Base2Arm <= 0)
                            {
                                pack.changesNo++;
                                pack.whatChanges.Add(4);
                                pack.xChanges.Add(j);
                                pack.yChanges.Add(i);
                                pack.changesNo++;
                                pack.whatChanges.Add(10); //round over message... team 1 has won
                                roundOver = true;
                            }
                        }
                        players[id].bullet.alive = false;
                        return true;
                    }
            for (int i = 0; i < MAX_NO_PLAYERS; i++)
                if ((players[i].outOfGame == false) && (i != id))
                {
                    //Rectangle temp = new Rectangle((int)players[i].x, (int)players[i].y, tilewidth, tilewidth);
                    Rectangle temp = new Rectangle((int)players[i].x * tilewidth, (int)players[i].y * tilewidth, tilewidth, tilewidth);
                    if (temp.Intersects(bullet))
                    {
                        Console.WriteLine(" dead ");
                        pack.playerExists[i] = false;
                        players[i].outOfGame = true;
                        players[id].score += killScore;
                        playersNoTeam[players[i].team]--;
                        Console.WriteLine(playersNoTeam[players[i].team]);
                        if (playersNoTeam[players[i].team] <= 0)
                        {
                            textBoxChat.Text += "\r\n game over";
                            pack.changesNo++;
                            pack.whatChanges.Add(9 + players[i].team); //round over message...
                            roundOver = true;
                        }
                        players[id].bullet.alive = false;
                        return true;
                    }
                }

            return false;
        }


        //functie care integreaza harta in variabilele locale
        void defineMapData(string str)
        {
            char[] delim = { ' ', '|', '\n' };
            string[] elems = str.Split(delim);
            //textBoxChat.Text += "\r\n" + str;
            mapsize = Int32.Parse(elems[0]);
           // textBoxChat.Text += "\r\nmapsize: " + mapsize;
            for(int i=0; i<mapsize; i++)
                for (int j = 0; j < mapsize; j++)
                {
                    tiles[j, i] = Int32.Parse(elems[i * mapsize + j + 1]);
                    tileRect[j, i] = new Rectangle(j * tilewidth, i * tilewidth, tilewidth, tilewidth);

                    //textBoxChat.Text += "\r\nPatratica " + i + " " + j + " are culoarea " + tiles[i, j]
                      //  +"\r\n\tcoordonate dreptunghi: "+tileRect[i,j].X+" "+tileRect[i,j].Y;
                }

        }

        //
        Boolean outOfBounds(Rectangle tancu)
        {
            //Console.WriteLine(tancu.X + " " + tancu.Y +  "   --  " + mapsize);
            if ((tancu.X < 0) || (tancu.Y < 0) 
                || (tancu.X + tilewidth  >= mapsize * tilewidth+5) 
                || (tancu.Y + tilewidth >= mapsize * tilewidth+5)
                )
                return true;
            return false;
        }

        //functie care verifica daca se interesecteaza cu zidurile
        Boolean checkIfIntersectsWalls(Rectangle tancu, int id, serverPacket pack)
        {
            //return false;
            if (pwuSolidOn && tancu.Intersects(new Rectangle((int)xpwuSolid * tilewidth, (int)ypwuSolid * tilewidth,  tilewidth, tilewidth)))
            {
                pack.changesNo++;
                pack.whatChanges.Add(8);
                pwuSolidOn = false;
                players[id].pwuSolid = true;
            }
            if (pwuWaterOn && tancu.Intersects(new Rectangle((int)xpwuWater * tilewidth, (int)ypwuWater * tilewidth, tilewidth, tilewidth)))
            {
                pack.changesNo++;
                pack.whatChanges.Add(6);
                pwuWaterOn = false;
                players[id].pwuWater = true;
            }


            for (int i = 0; i < mapsize; i++)
                for (int j = 0; j < mapsize; j++)
                        if (tiles[i, j] == 2 && tileRect[i, j].Intersects(tancu) == true)
                        {
                            if (players[id].pwuWater == true)
                                return false;
                            return true;
                        }
                        else
                            if ((tiles[i, j] != 0) && (tiles[i, j] != 3) && (tileRect[i, j].Intersects(tancu) == true))
                                return true;
            return false;
        }

        //fucntie care verifica daca se intersecteaza cu alte tancuri
        Boolean checkIfIntersectsOtherTanks(Rectangle tancu, int index)
        {
            Rectangle t;
            //return false;
            for (int i = 0; i < noPlayers; i++)
            {
                if (i == index) continue;
                else
                {
                    t = new Rectangle((int)(players[i].x * tilewidth), (int)(players[i].y * tilewidth), tilewidth, tilewidth);
                    if (t.Intersects(tancu))
                        return true;
                }
            }
            return false;
        }


        /* 
         * Make the current instance server
         */
        private void makeMeServerButton_Click(object sender, EventArgs e)
        {
            //verificare daca e harta selectata, si daca nu, EXIT cu FAIL
            if (comboBox2.Text.Contains(".map") == false)
            {
                MessageBox(new IntPtr(0), "No map file selected: "+comboBox2.SelectedText, "Map Error", 1);
                return;
            }
            try
            {
                //citirea hartii in string
                LocalMaps lm = new LocalMaps();
                if (comboBox2.Text.ToLower().Contains("map1"))
                {
                   // mapstring = dbmanager.getMap(1); if (mapstring == null)
                        mapstring = lm.map1;
                }
                else if (comboBox2.Text.ToLower().Contains("map2"))
                {
                    //mapstring = dbmanager.getMap(2);if (mapstring == null)
                        mapstring = lm.map2;
                }
                else if (comboBox2.Text.ToLower().Contains("map3"))
                {
                    //mapstring = dbmanager.getMap(3); if (mapstring == null)
                        mapstring = lm.map3;
                }
                //else if (comboBox2.Text.ToLower().Contains("map4"))
                //    mapstring = dbmanager.getMap(4);
                /*
                mapstring=mapstring.Insert(6, "|");
                char[] delim = { ' ' };
                char[] delim2 = { '|' };
                string[] el2 = mapstring.Split(delim2);
                string[] elms = el2[0].Split(delim);
                mapstring = elms[0] + "|" + el2[1];
                 */
                //definim harta locala
                textBoxChat.Text += "\r\nmapstring: " + mapstring;
                defineMapData(mapstring);
                //Debug.Write(mapstring);
            }
            catch (FileNotFoundException ex)
            {
                textBoxChat.Text += "\n\r Map Not Found\r\n";
                Console.WriteLine(ex.Message);
                return;
            }

            //daca s-a selectat harta, continuam cu crearea serverului
            try
            {
                pasiveSocket = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);

                // use ip address selected in the ComboBox1
                IPEndPoint ip1 = new IPEndPoint(IPAddress.Parse((comboBox1.SelectedItem).ToString()), Int32.Parse(textBox1.Text));
                pasiveSocket.Bind(ip1);
                pasiveSocket.Listen(MaxClients);

                textBoxChat.Text += "Ready ! Your IP is " + (comboBox1.SelectedItem).ToString() + "\r\n";
                textBoxChat.Refresh();
                Thread.Sleep(3000);

                Thread lt = new Thread(listenConn);
                lt.IsBackground = true;
                lt.Start();
                //WhileMethod();
            }
            catch (SocketException se)
            {
                textBoxChat.Text += "\n\r Could not create server\r\n";
                Console.WriteLine("Source : " + se.Source);
                Console.WriteLine("Message : " + se.Message);
            }
            catch (Exception se)
            {
                textBoxChat.Text += "\n\r Could not create server\r\n";
                Console.WriteLine("Source : " + se.Source);
                Console.WriteLine("Message : " + se.Message);
            }

            
        }

        /*
         *  Send Current configuration to clients
         */
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            serverPacket pack = new serverPacket();
            pack.Type = 0;
          
            for(int i=0; i<noPlayers; i++)
                //if the player is still in the game
                if (!players[i].outOfGame)
                {
                    pack.direction[i] = players[i].rotation / 90;
                    //Player p1 = new Player(players[i].id);
                    Rectangle temp;
                    // LAG logic 
                    if (players[i].nrFails > MAX_LAG)
                    {
                        players[i].outOfGame = true;
                        serverCM.ElementAt(i).Close();
                    }


                    //if the player is going up
                    if (players[i].pressed[1])
                    {
                        temp = new Rectangle((int)(players[i].x*tilewidth), (int)(players[i].y*tilewidth - dy), tilewidth, tilewidth);
                        if ((checkIfIntersectsWalls(temp,i,pack)==false) && (outOfBounds(temp) == false)
                            && checkIfIntersectsOtherTanks(temp,i)==false)
                            players[i].y -= (float)dy/tilewidth;
                        pack.direction[i] = UP;
                        players[i].rotation = 90;
                    }
                    else

                    //if the player is going down
                    if (players[i].pressed[2])
                    {
                        temp = new Rectangle((int)(players[i].x*tilewidth), (int)(players[i].y*tilewidth+dy), tilewidth, tilewidth);
                        if ((checkIfIntersectsWalls(temp, i, pack) == false) && (outOfBounds(temp) == false)
                            && checkIfIntersectsOtherTanks(temp, i)==false)
                            players[i].y += (float)dy/tilewidth;
                        pack.direction[i] = DOWN;
                        players[i].rotation = 270;
                    } else

                    //if the player is going left
                    if (players[i].pressed[3])
                    {
                        temp = new Rectangle((int)(players[i].x* tilewidth-dx), (int)(players[i].y*tilewidth), tilewidth, tilewidth);
                        if ((checkIfIntersectsWalls(temp, i, pack) == false) && (outOfBounds(temp) == false)
                            && checkIfIntersectsOtherTanks(temp, i)==false)
                            players[i].x -= (float)dx/tilewidth;
                        pack.direction[i] = LEFT;
                        players[i].rotation = 0;
                    } else 

                    //if the player is going right
                    if (players[i].pressed[4])
                    {
                        temp = new Rectangle((int)(players[i].x*tilewidth+dx), (int)(players[i].y*tilewidth), tilewidth, tilewidth);
                        if ((checkIfIntersectsWalls(temp, i, pack) == false) && (outOfBounds(temp) == false)
                            && checkIfIntersectsOtherTanks(temp, i)==false)
                            players[i].x += (float)dx/tilewidth;
                        pack.direction[i] = RIGHT;
                        players[i].rotation = 180;
                    }

                    //if player is shooting

                    //if the bullet is new
                    if (players[i].bullet.alive == false)
                    {
                        //if player is shooting
                        if (players[i].pressed[0])
                        //TODO bullet thingy
                        {
                            players[i].bullet.alive = true;
                            players[i].bullet.position.X = players[i].x + 0.3f;//(float)players[i].bullet.center.X / tilewidth;
                            players[i].bullet.position.Y = players[i].y + 0.3f;//(float)players[i].bullet.center.Y / tilewidth; ;
                            players[i].bullet.velocity = new Vector2((float)Math.Cos(MathHelper.ToRadians(players[i].rotation)), (float)Math.Sin(MathHelper.ToRadians(players[i].rotation))) * 9.0f / tilewidth;
                            pack.whatChanges.Add(0);
                            Console.WriteLine("Glont la " + players[i].bullet.position.X + " " + players[i].bullet.position.Y);
                            if (checkIfBulletIntersectsThingies(new Rectangle((int)(players[i].bullet.position.X * tilewidth),
                                                                    (int)(players[i].bullet.position.Y * tilewidth), 7, 7), pack, i) == true)
                            {
                                players[i].bullet.alive = false;
                                /*
                                pack.changesNo++;
                                pack.whatChanges.Add(2);
                                pack.xChanges.Add(players[i].bullet.position.X);
                                pack.yChanges.Add(players[i].bullet.position.Y);
                                 */
                            }
                            else
                            {
                                pack.changesNo++;
                                pack.whatChanges.Add(0);
                                pack.xChanges.Add(players[i].bullet.position.X);
                                pack.yChanges.Add(players[i].bullet.position.Y);
                            }
                        }
                    }
                    else
                    {

                        players[i].bullet.position -= players[i].bullet.velocity;

                        //verify if the bullet has exited the screen
                        if (players[i].bullet.position.X < 0 || players[i].bullet.position.X > mapsize ||
                            players[i].bullet.position.Y < 0 || players[i].bullet.position.Y > mapsize)
                        {
                            players[i].bullet.alive = false;
                            /*
                            pack.changesNo++;
                            pack.whatChanges.Add(2);
                            pack.xChanges.Add(players[i].bullet.position.X);
                            pack.yChanges.Add(players[i].bullet.position.Y);
                             */
                        }
                        if (checkIfBulletIntersectsThingies(new Rectangle((int)(players[i].bullet.position.X * tilewidth), 
                            (int)(players[i].bullet.position.Y * tilewidth), 7, 7), pack, i) == true)
                        {
                            players[i].bullet.alive = false;
                            /*
                            pack.changesNo++;
                            pack.whatChanges.Add(2);
                            pack.xChanges.Add(players[i].bullet.position.X);
                            pack.yChanges.Add(players[i].bullet.position.Y);
                             */
                        }
                        else
                        {
                            pack.changesNo++;
                            pack.whatChanges.Add(0);
                            pack.xChanges.Add(players[i].bullet.position.X);
                            pack.yChanges.Add(players[i].bullet.position.Y);
                        }
                    }

                    pack.xPlayer[i] = players[i].x;
                    pack.yPlayer[i] = players[i].y;
                    pack.playerExists[i] = true;
                }
                else
                {
                    pack.playerExists[i] = false;
                    //serverCM.ElementAt(i).Close();
                }

            // generate power-up
            //Console.WriteLine("pwuSolidOn " + pwuSolidOn + " timerSolid" + timerSolid);
            //                    "pwuWaterOn " + pwuWaterOn + " timerWater" + timerWater);
            if ((timerSolid = pwuUpdate(timerSolid)) == 0)
            {
                if (pwuSolidOn) // exista deja pwu de Solid - il dezactivam
                {
                    pack.changesNo++;
                    pack.whatChanges.Add(8);
                    pwuSolidOn = false;
                }
                else // activam pwu de solid
                {
                    pack.changesNo++;
                    pack.whatChanges.Add(7);
                    xpwuSolid = (rand.Next(mapsize) - 1);
                    ypwuSolid = (rand.Next(mapsize) - 1);
                    pack.xChanges.Add(xpwuSolid);
                    pack.yChanges.Add(ypwuSolid);
                    pwuSolidOn = true;
                }
            }

            if ((timerWater = pwuUpdate(timerWater)) == 0)
            {
                if (pwuWaterOn) // exista deja pwu de Solid - il dezactivam
                {
                    pack.changesNo++;
                    pack.whatChanges.Add(6);
                    pwuWaterOn = false;
                }
                else // activam pwu de solid
                {
                    pack.changesNo++;
                    pack.whatChanges.Add(5);
                    xpwuWater = (rand.Next(mapsize) - 1);
                    ypwuWater = (rand.Next(mapsize) - 1);
                    pack.xChanges.Add(xpwuWater);
                    pack.yChanges.Add(ypwuWater);
                    pwuWaterOn = true;
                }
            }


            //TODO verify if a wall or base have been destroyed

            //pack.changesNo = 0;

            // broadcast msg
            serverCM.ForEach(delegate(ConnectionManager cm)
            {
                int idx = cm.id;
                if (!players[idx].outOfGame)
                    players[idx].nrFails += cm.SendMessage(pack);
            });
        }

        /*
         *  Listen for possible connections
         */
        public void listenConn()
        {
                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();
            
                    pasiveSocket.BeginAccept(new AsyncCallback(OnClientJoined), null);
            
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            
        }

        /*
         *  Set in ComboBox1 the list with availabe ip4 addresses on the machine
         */
        private void GetIpList()
        {
            string[] ips = new string[100];
            IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());

            // determin the right ip - first IP of lenght <= length(255.255.255.255)
            string maxIP = "255.255.255.255";
            string ipv4;
            int i = 0;
            
            while (comboBox1.Items.Count==0)
            {
                try
                {
                    ipv4 = IPHost.AddressList[i].ToString();
                    if (ipv4.Length <= maxIP.Length)
                        comboBox1.Items.Add(ipv4);
                    //Console.WriteLine(ipv4 + " " + ipv4.Length);
                }
                catch (IndexOutOfRangeException ex)
                {
                    break;
                }
                i++;
            }
            comboBox1.SelectedIndex = 0;
            return;
        }

        /*
         *  New client is connected 
         */ 
        private void OnClientJoined(IAsyncResult asyncResult)
        {
               // Signal the main thread to continue
               allDone.Set();

               CheckForIllegalCrossThreadCalls = false;
               Socket s = pasiveSocket.EndAccept(asyncResult);
               newSocket.Add(s);
           
               textBoxChat.Text += "Client " + (newSocket.Count).ToString() + " connected\r\n";
               ConnectionManager cm = new ConnectionManager(s, serverCM.Count, true);
               serverCM.Add(cm);
               cm.serverMsgRecv += new serverMessageHandler(FromClient);

               // increase number of players 
               noPlayers++;

               cm.GetStarted();
         }

        //function that sends the map to the client no id
        public void SendMap(int id)
        {
            String newmap = mapsize + "|";
            for (int i = 0; i < mapsize; i++)
                for (int j = 0; j < mapsize; j++)
                    newmap += tiles[j, i] + " ";
            serverPacket sp = new serverPacket(1, newmap);
            sp.ID = id;

            serverCM.ElementAt<ConnectionManager>(id).SendMessage(sp);

            textBoxChat.Text += "\r\n"+mapstring;
        }

        public void GeneratePosition(int id)
        {
            Random r = new Random(17);
            bool ok = false;
            int x=0, y=0;
            while(!ok)
            {
                x = r.Next(mapsize-1);
                y = r.Next(mapsize-1);
                Rectangle t = new Rectangle(x * tilewidth, y * tilewidth, tilewidth, tilewidth);
                if(tiles[x,y] == 0 && checkIfIntersectsOtherTanks(t,-1)==false)
                    ok = true;
            }
            players[id].x = x;
            players[id].y = y;
            textBoxChat.Text += "\r\n" + x + " " + y;
        }

        public void FromClient(clientPacket pack, int id)
        {
            //textBoxChat.Text += "\r\nClient " + id.ToString() + ": got message " + pack.Type.ToString();
            //textBoxChat.SelectionStart = textBoxChat.Text.Length;
            //textBoxChat.ScrollToCaret();

            switch (pack.Type)
            {
                case 0:
                    {
                        //connection
                        //server receives credentials from client
                        textBoxChat.Text += "\r\nInfo: " + pack.Load;
                        
                        //parse the load of the message
                        string[] parts;
                        char[] delim = {'|'};
                        parts = pack.Load.Split(delim);

                        // verify password
                        /*
                        if (dbmanager.ValidateUser(parts[1], parts[2]) == false)
                        {
                            textBoxChat.Text += "\r\n\r\nInvalid User: " + parts[1] + " or Pass: " + parts[2];
                            serverCM.ElementAt(id).SendMessage(new serverPacket(2, " "));
                            return;
                        } 
                         */
                        textBoxChat.Text += "\r\n\r\nUser connected: " + parts[1];
                        
                        //update player
                        players[id].team = Int32.Parse(parts[0]);
                        playersNoTeam[players[id].team]++;
                        players[id].userName = parts[1];
                        players[id].password = parts[2];

                        //must send the map to the new player
                        SendMap(id);
                        
                        //Console.WriteLine("acum am " + noPlayers.ToString());
                        break;
                    }
                case 1:
                    {
                        //player exits
                        players[id].outOfGame = true;
                        break;
                    }
                case 2:
                    {
                        //player goes up
                        players[id].pressed[1] = true;
                        break;
                    }
                case 3:
                    {
                        //player stops going up
                        players[id].pressed[1] = false;
                        break;
                    }
                case 4:
                    {
                        //player goes down
                        players[id].pressed[2] = true;
                        break;
                    }
                case 5:
                    {
                        //player stops going down
                        players[id].pressed[2] = false;
                        break;
                    }
                case 6:
                    {
                        //player goes left
                        players[id].pressed[3] = true;
                        break;
                    }
                case 7:
                    {
                        //player stops going left
                        players[id].pressed[3] = false;
                        break;
                    }
                case 8:
                    {
                        //player goes right
                        players[id].pressed[4] = true;
                        break;
                    }
                case 9:
                    {
                        //player stops going right
                        players[id].pressed[4] = false;
                        break;
                    }
                case 10:
                    {
                        //player starts shooting
                        players[id].pressed[0] = true;
                        break;
                    }
                case 11:
                    {
                        //player stops shooting
                        players[id].pressed[0] = false;
                        break;
                    }
                case 12:
                    {
                        //server has received a "Join" message
                        //TODO analyze the packet and retrieve information from the client
                        players[id].outOfGame = false;
                        GeneratePosition(id);

                        //if it's the first client that has joined, start timer
                        if (firstClient)
                        {
                            aTimer.Enabled = true;
                            firstClient = false;
                        }

                        break;
                    }
                case 13:
                    {
                        //client has requested a pause

                        aTimer.Stop();
                        break;
                    }
                case 14:
                    {
                        //client has resumed the game

                        aTimer.Start();
                        break;
                    }
                default:
                    {
                        Console.WriteLine("What's this?");
                        break;
                    }
            }

        }
   
        private void button3_Click(object sender, EventArgs e)
        {
            textBoxChat.Text += "\r\nMe : " + textBox3.Text;
            string text = textBox3.Text;
            
            // broadcast msg
            serverCM.ForEach(delegate(ConnectionManager cm)
                    {
                        cm.SendMessage(new serverPacket(-1, text));
                    });
            

            textBox3.Text = "";
            textBoxChat.SelectionStart = textBoxChat.Text.Length;
            textBoxChat.ScrollToCaret();
        }
        
        private void textBox3_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)13)
            {
                button3_Click(sender, e);
            }
        }

        private int pwuUpdate(int pwu)
        {
            if (pwu == 0)
            {
                pwu = (int)((30 + rand.Next(15)) *  1000 / aTimer.Interval/3);
                return pwu; // 1 means change status
            }
            else
            {
                pwu--;
                return pwu; // no change
            }

        }
    }
}
