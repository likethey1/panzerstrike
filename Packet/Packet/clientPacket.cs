using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packet
{
    [Serializable]
    public class clientPacket
    {
        private  int type;
        public   int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        private string load;
        public  string Load
        {
            get
            {
                return load;
            }
            set
            {
                load = value;
            }
        }

        public  clientPacket()
        {
            this.type = -1;
            this.load = "";
        }


        public  clientPacket(int type, string load)
        {
            this.type = type;
            this.load = load;
        }

        override
        public string  ToString()
        {
            return this.type.ToString() + " " + this.load;
        }
    }

    [Serializable]
    public class serverPacket
    {
        private int type;
        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        private string load;
        public  string Load
        {
            get
            {
                return load;
            }
            set
            {
                load = value;
            }
        }

        public float[] xPlayer;
        public float[] yPlayer;
        public bool[] playerExists;
        public int[] direction;
        public int changesNo;
        public List<int> xChanges;
        public List<int> yChanges;
        public List<int> whatChanges;

        public  serverPacket()
        {
            this.type = -1;
            this.load = "";
            xPlayer = new float[8];
            yPlayer = new float[8];
            playerExists = new bool[8];
            direction = new int[8];
            xChanges = new List<int>();
            yChanges = new List<int>();
            whatChanges = new List<int>();
            changesNo = 0;
        }


        public  serverPacket(int type, string load)
        {
            this.type = type;
            this.load = load;
        }

        override
        public string  ToString()
        {
            return this.type.ToString() + " " + this.load;
        }
        
    }
}
