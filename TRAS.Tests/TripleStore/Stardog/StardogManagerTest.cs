using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TripleStore.Stardog;
using VDS.RDF.Storage;
using VDS.RDF;

namespace TRAS.Tests.TripleStore.Stardog
{
    [TestClass]
    public class StardogManagerTest
    {
        [TestMethod]
        public void ConnectToServer()
        {
            StardogManager manager = new StardogManager();
        }

        [TestMethod]
        public void ConnectToDatabase()
        {
            StardogManager manager = new StardogManager();
            StardogConnector connector = manager.GetConnector("users");
        }

        [TestMethod]
        public void AddUserTest()
        {
            StardogManager manager = new StardogManager();
            StardogConnector connector = manager.GetConnector("users");

            IGraph g = new Graph();
            g.BaseUri = UriFactory.Create("http://users.tras.com/");

            g.NamespaceMap.AddNamespace("rdf", UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            g.NamespaceMap.AddNamespace("rdfs", UriFactory.Create("http://www.w3.org/2000/01/rdf-schema#"));
            g.NamespaceMap.AddNamespace("foaf", UriFactory.Create("http://xmlns.com/foaf/0.1/"));


            //<foaf:Person>
            //   <foaf:name>David Banner</foaf:name>

            var s = g.CreateUriNode(UriFactory.Create("foaf:Person"));
            var p = g.CreateUriNode(UriFactory.Create("foaf:name"));
            var o = g.CreateLiteralNode("David Banner");

            Triple t = new Triple(s, p, o);

            g.Assert(t);

            connector.SaveGraph(g);
        }

        [TestMethod]
        public void AddUserProperties()
        {
            StardogManager manager = new StardogManager();
            StardogConnector connector = manager.GetConnector("users");

            IGraph g = new Graph();
            connector.LoadGraph(g, new Uri("http://users.tras.com/"));

            var s = g.CreateUriNode(UriFactory.Create("foaf:Person"));
            var p = g.CreateUriNode(UriFactory.Create("foaf:name"));
            var o = g.CreateLiteralNode("John Doe");

            Triple t = new Triple(s, p, o);

            g.Assert(t);

            connector.SaveGraph(g);
        }
    }
}
