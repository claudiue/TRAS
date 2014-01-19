using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Storage;
using VDS.RDF.Storage.Management;

namespace TripleStore.Stardog
{
    public class StardogManager
    {
        public const string DefaultAddress = "http://localhost:{0}";
        public const string DefaultPort = "5820";

        private const string DefaultUsername = "admin";
        private const string DefaultPassword = "admin";
        private const int MaximumNrOfDatabases = 10;

        private string _username;
        private string _password;
        
        private Dictionary<string, StardogConnector> _connectors = new Dictionary<string, StardogConnector>();

        public string Address { get; private set; }
        public string Port { get; private set; }
        public StardogServer Server { get; private set; }

        public StardogManager(string address = DefaultAddress, string port = DefaultPort, string username = DefaultUsername, string password = DefaultPassword)
        {
            _username = username;
            _password = password;
            Address = string.Format(DefaultAddress, DefaultPort);
            Server = new StardogServer(string.Format(address, port), username, password);
        }

        public StardogConnector GetConnector(string database, StardogReasoningMode reasoningMode = StardogReasoningMode.None)
         {
            if (string.IsNullOrEmpty(database))
            {
                return null;
            }

            if (_connectors.ContainsKey(database))
            {
                return _connectors[database];
            }

            if (_connectors.Keys.Count == MaximumNrOfDatabases)
            {
                return null;
            }

            using (StardogConnector connector = new StardogConnector(Address, database, reasoningMode, _username, _password))
            {
                _connectors.Add(database, connector);
                return connector;
            }
        }

    }

    
}
