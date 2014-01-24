using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Storage;
using ViewModels;

namespace TripleStore.Stardog
{
    public class StardogDb
    {
        public const string DefaultName = "tras";
        public string Name {get; set;}    

        private StardogConnector _connector = null;
        private static StardogDb _instance;

        private StardogDb(string dbName)
        {
            this.Name = dbName;
        }

        public static StardogDb GetInstance(string dbName = DefaultName)
        {
            if (_instance == null)
            {
                _instance = new StardogDb(dbName);
            }
            return _instance;
        }


        public void Update(AgentViewModel agentVM)
        {
            using (_connector = new StardogManager().GetConnector(Name))
            {
                NodeFactory factory = new NodeFactory();
                List<Triple> toRemove = new List<Triple>();
                List<Triple> toAdd = new List<Triple>();

                IGraph g = new Graph();
                _connector.LoadGraph(g, new Uri("http://users.tras.com/"));


                factory.CreateUriNode(new Uri("http://subject"));
                factory.CreateUriNode(new Uri("http://predicate"));
                factory.CreateUriNode(new Uri("http://object"));
                
                //toRemove.Add(new Triple())
                
                


                _connector.UpdateGraph(new Uri("http://graph"), null, toRemove);
            }
        }
    }
}
