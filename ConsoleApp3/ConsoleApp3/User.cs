using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    internal class User
    {
        public int ID { get; set; }
        private string Name, Rank; 

        public string name { get { return Name; }  set { Name = value; } }
        public string rank { get { return Rank; } set { Rank = value; } }
        public int lvl { get; set; }

        public User() { }
        public User(string Name, int lvl, string Rank)
        {
            this.Name = Name;
            this.lvl = lvl;
            this.Rank = Rank;
        }
    }
}
