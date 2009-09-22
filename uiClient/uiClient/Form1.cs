using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using PacketL;
//using GameLibrary;


namespace uiClient
{
    public partial class Form1 : Form
    {

        //VAR--------------------------------------
        Game1 game = null;
        Thread gameThread;
        bool gameSuspended = false; // used for pause/resume 

        public Form1()
        {
            InitializeComponent();
            //GetIpList();

            // doar pentru statiile unde se testeaza local atat serverul si clientul
            //textBox1.Text = (comboBox1.SelectedItem).ToString();
            this.Text = "Game Client";

            comboBox1.Items.Add("The Yanks");
            comboBox1.Items.Add("The Commies");
            comboBox1.SelectedIndex = 0;

            textBox5.Text = PORT.ToString();
        }

        public static int PORT = 7033;
        
        /*
         *  Vars used by client
         */
        Socket clientSocket;
        ConnectionManager clientCM;

        //functie de debug de la joc
        public void MessageFromGame(String s)
        {
            textBoxChat.Text += "\r\nJOC: " + s;
        }

        public void ShowToTextBoxChat(serverPacket sp, int id)
        {
            textBoxChat.Text += "\r\nFriend : " + sp.ToString();
        }

        /*
         *  Handler for packets received from server
         */
        public void FromServer(serverPacket pack, int id)
        {
            //textBoxChat.Text += "\r\n " + pack.Type.ToString();
            //textBoxChat.SelectionStart = textBoxChat.Text.Length;
            //textBoxChat.ScrollToCaret();

            switch (pack.Type)
            {
                // console message ( debbuging )
                case -1:
                    //textBoxChat.Text += "\r\n " + pack.ToString();
                    break;

                // Sending periodic updates        
                case 0:
                    //MessageFromGame("Primit update: " + pack.xPlayer[0]);
                    game.getUpdate(pack);
                    break;

                // Sending Map
                case 1:
                    gameThread.Start();
                    while (game == null)
                    { } 
                    game.getMap(pack);
                    break;

                // user was rejected
                case 2:
                    while (game == null)
                    { }
                    game.Exit();
                    //textBoxChat.Text += "\r\n " + pack.Type.ToString();
                    break;
            
                // Pauze
                case 3:
                    game.gamePaused = true;
                    break;

                //Resume
                case 4:
                    game.gamePaused = false;
                    break;
            
            }
        }

        /*
         *  Client connects to server
         */
        private void connectButton_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            clientSocket = new Socket(
            AddressFamily.InterNetwork,
                          SocketType.Stream,
             ProtocolType.Tcp);
            try
            {
                IPEndPoint ip1 = new IPEndPoint(IPAddress.Parse(textBox1.Text), Int32.Parse(textBox5.Text));
                clientSocket.Connect(ip1);
            }
            catch (System.FormatException ex)
            {
                textBoxChat.Text += "\r\n Invalid IP and/or PORT \r\n";
                return;
            }

            catch (System.Net.Sockets.SocketException ex)
            {
                textBoxChat.Text += "\r\n Could not connect to server \r\n";
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            
            clientLabel.Text = "Connect !";
            clientCM = new ConnectionManager(clientSocket);

            //clientCM.clientMsgRecv += new clientMessageHandler(ShowToTextBoxChat);
            clientCM.clientMsgRecv += new clientMessageHandler(FromServer);
            clientCM.GetStarted();


            clientCM.SendMessage(new clientPacket(0, comboBox1.SelectedIndex+"|"+textBox2.Text+"|"+textBox4.Text));

            gameThread = new Thread(myfunc);
            gameThread.IsBackground = true;
            //gameThread.Start();
        }

        /*
         *  Launch game
         */
        private void myfunc()
        {
            using ( game = new Game1(clientCM, this))
            {
                game.Run();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBoxChat.Text += "\r\nMe : " + textBox3.Text;
            string text = textBox3.Text;
        
            clientCM.SendMessage(new clientPacket(0, textBox3.Text));
            
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

        private void Form1_Load(object sender, EventArgs e)
        {
            // no need for timer - the server send game updates - no need for keep alives
           // Timer1 timer = new Timer1();
        }
    }
}
