using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using PacketL;


namespace ui
{
    /*        
     *       Declarare delegate pentru mesaje 
     */
    public delegate void serverMessageHandler(clientPacket cp, int id);
    public delegate void clientMessageHandler(serverPacket sp, int id);

    public class ConnectionManager
    {
        /*
         *        Declarare eveniment de tipul delegate-ului de mai sus
         */
        public event clientMessageHandler clientMsgRecv;
        public event serverMessageHandler serverMsgRecv;

        NetworkStream networkStream;
        Thread thread;
        public int id = -1;
        public bool isServer = false;

        public ConnectionManager(Socket socket)
        {
            networkStream = new NetworkStream(socket);

            if (this.isServer == true)
                thread = new Thread(ServerGetMessage);
            else
                thread = new Thread(ClientGetMessage);
            thread.IsBackground = true;
        }


        public ConnectionManager(Socket socket, int id, bool isServer)
        {
            networkStream = new NetworkStream(socket);
            this.isServer = isServer;

            if (this.isServer == true)
                thread = new Thread(ServerGetMessage);
            else
                thread = new Thread(ClientGetMessage);
            thread.IsBackground = true;
            this.id = id;
        }

        public void GetStarted()
        {
            thread.Start();
        }

        // the same for both server and client
        // 0 - no error
        // 1 - could not transmit
        public int SendMessage(Object o)
        {
            BinaryFormatter sender = new BinaryFormatter();
            try
            {
                sender.Serialize(this.networkStream, o);
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine("Could not send data\r\n");
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
            return 0;
        }

        public void Close()
        {
            thread.Abort();
        }


        // return clientPacket for the server - isServer = true
        public void ServerGetMessage()
        {
            BinaryFormatter getter = new BinaryFormatter();

            while (true)
            {
                Thread.Sleep(10);
                clientPacket cp = null;
                if (networkStream.DataAvailable)
                {
                    try
                    {
                        cp = (clientPacket)getter.Deserialize(networkStream);
                        /*
                         *   Declanseaza un eveniment 
                         */
                        serverMsgRecv(cp, id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        cp = null;
                    }
                    finally
                    {
                        networkStream.Flush();
                    }
                }
            } // end while
        }


        // returns serverPacket for the client - isServer = false
        public void ClientGetMessage()
        {
            BinaryFormatter getter = new BinaryFormatter();

            while (true)
            {
                //Thread.Sleep(10);
                serverPacket sp = null;
                if (networkStream.DataAvailable)
                {
                    try
                    {
                        sp = (serverPacket)getter.Deserialize(networkStream);
                        /*
                         *   Declanseaza un eveniment 
                         */
                        clientMsgRecv(sp, id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        sp = null;
                    }
                    finally
                    {
                        networkStream.Flush();
                    }
                }
            } // end while
        }

        internal bool isAllive()
        {
            return thread.IsAlive;
        }
    }
}
