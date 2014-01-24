using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TripleStore.Stardog;
using VDS.RDF.Storage;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace TRAS.Tests.TripleStore.Stardog
{
    [TestClass]
    public class StardogManagerTest
    {

        private const string _testDbName = "test";

        [TestMethod]
        public void ConnectToServer()
        {
            StardogManager manager = new StardogManager();
        }

        [TestMethod]
        public void ConnectToDatabase()
        {
            StardogManager manager = new StardogManager();
            StardogConnector connector = manager.GetConnector(_testDbName);
        }

        [TestMethod]
        public void AddUserTest()
        {
            StardogManager manager = new StardogManager();
            StardogConnector connector = manager.GetConnector(_testDbName);

            IGraph g = new Graph();
            g.BaseUri = UriFactory.Create("http://users.tras.com/");

            g.NamespaceMap.AddNamespace("rdf", UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            g.NamespaceMap.AddNamespace("rdfs", UriFactory.Create("http://www.w3.org/2000/01/rdf-schema#"));
            g.NamespaceMap.AddNamespace("foaf", UriFactory.Create("http://xmlns.com/foaf/0.1/"));


            //<foaf:Person>
            //   <foaf:name>David Banner</foaf:name>

            var s = g.CreateUriNode("foaf:Person");
            var p = g.CreateUriNode("foaf:name");
            var o = g.CreateLiteralNode("David Banner");

            Triple t = new Triple(s, p, o);

            g.Assert(t);

            connector.SaveGraph(g);
        }

        [TestMethod]
        public void AddUserProperties()
        {
            StardogManager manager = new StardogManager();
            StardogConnector connector = manager.GetConnector("edu");
            var x = connector.ListGraphs();

            IGraph g = new Graph();
            //g.BaseUri = new Uri(@"http://www.tras.org/ontology");
            //RdfXmlParser parser = new RdfXmlParser();
            //parser.Load(g, "tras.rdf");

            //TurtleParser parser = new TurtleParser();
            //parser.Load(g, "tras.ttl");

            var stringUri = @"http://www.tras.org/ontology";
            var uri = new Uri(stringUri);
            connector.LoadGraph(g, uri);

            //connector.LoadGraph(g, new Uri("http://users.tras.com/"));


            //g.NamespaceMap.AddNamespace("rdf", UriFactory.Create(@"http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            //g.NamespaceMap.AddNamespace("rdfs", UriFactory.Create(@"http://www.w3.org/2000/01/rdf-schema#"));
            //g.NamespaceMap.AddNamespace("foaf", UriFactory.Create(@"http://xmlns.com/foaf/0.1/"));


            //var s = g.CreateUriNode("foaf:Person");
            //var p = g.CreateUriNode("foaf:name");
            //var o = g.CreateLiteralNode("John Doee");

            //Triple t = new Triple(s, p, o);

            //g.Assert(t);

            connector.SaveGraph(g);

            RdfXmlWriter writer = new RdfXmlWriter();
            writer.Save(g, "tras2.rdf");
        }

        [TestMethod]
        public void LoadGraph()
        {
            string trasOntologyUri = "http://www.tras.org/ontology";

            StardogManager manager = new StardogManager();
            StardogConnector connector = manager.GetConnector("tras");

            IGraph g = new Graph();
            connector.LoadGraph(g, new Uri(trasOntologyUri));

        }
    }
}
