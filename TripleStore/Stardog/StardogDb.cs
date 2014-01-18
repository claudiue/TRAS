using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleStore.Stardog
{
    public class StardogDb
    {
        public string Name { get; set; }

        public StardogDb() { }
        public StardogDb(string Name)
        {
            this.Name = Name;
        }
    }
}
