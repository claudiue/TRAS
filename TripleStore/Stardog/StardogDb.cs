using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Storage;
using ViewModels;

namespace TripleStore.Stardog
{
    public class StardogDb
    {
        public const string DefaultName = "tras";
        public string Name {get; set;}

        #region Singleton
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
        #endregion

        private IGraph GetDefaultGraph()
        {
            IGraph g = new Graph();
            g.BaseUri = null; // new Uri(StardogConstants.DefaultGraphName);
            //g.NamespaceMap.AddNamespace("xsd", new Uri(URIs.XSD));
            //g.NamespaceMap.AddNamespace("rdf", new Uri(URIs.RDF));
            //g.NamespaceMap.AddNamespace("rdfs", new Uri(URIs.RDFS)); // by default
            g.NamespaceMap.AddNamespace("xml", new Uri(URIs.XML));
            g.NamespaceMap.AddNamespace("owl", new Uri(URIs.OWL));
            g.NamespaceMap.AddNamespace("foaf", new Uri(URIs.FOAF));
            g.NamespaceMap.AddNamespace("dc", new Uri(URIs.DC));
            g.NamespaceMap.AddNamespace("geonames", new Uri(URIs.GEONAMES));
            g.NamespaceMap.AddNamespace("tras", new Uri(URIs.TRAS));
            g.NamespaceMap.AddNamespace("", new Uri(URIs.TRAS));
            return g;
        }

        #region Person
        public void CreateOrUpdatePerson(PersonViewModel newPersonVM)
        {
            if (newPersonVM == null || newPersonVM.Id == null)
                return;

            NodeFactory factory = new NodeFactory();
            List<Triple> toAdd = new List<Triple>();
            List<Triple> toRemove = new List<Triple>();

            PersonViewModel oldPersonVM = GetPerson(newPersonVM.Id);

            var subject = factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, newPersonVM.Id)));

            if (oldPersonVM != null & oldPersonVM.Id == newPersonVM.Id)
            {
                toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(foaf("Person")))));
                if (oldPersonVM.Email != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("mbox"))), oldPersonVM.MailToEmail.ToLiteral(factory)));
                if (oldPersonVM.NickName != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("nickname"))), oldPersonVM.NickName.ToLiteral(factory)));
                if (oldPersonVM.FirstName != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("firstName"))), oldPersonVM.FirstName.ToLiteral(factory)));
                if (oldPersonVM.LastName != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("lastName"))), oldPersonVM.LastName.ToLiteral(factory)));
                if (oldPersonVM.Age > 0)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("age"))), oldPersonVM.Age.ToLiteral(factory)));
                if (oldPersonVM.Gender != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("gender"))), oldPersonVM.Gender.ToLiteral(factory)));
                if (oldPersonVM.Location != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("location"))), oldPersonVM.Location.ToLiteral(factory)));
            }
            
            toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(foaf("Person")))));
            if (newPersonVM.Email != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("mbox"))), newPersonVM.MailToEmail.ToLiteral(factory)));
            if (newPersonVM.NickName != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("nickname"))), newPersonVM.NickName.ToLiteral(factory)));
            if (newPersonVM.FirstName != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("firstName"))), newPersonVM.FirstName.ToLiteral(factory)));
            if (newPersonVM.LastName != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("lastName"))), newPersonVM.LastName.ToLiteral(factory)));
            if (newPersonVM.Age > 0)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("age"))), newPersonVM.Age.ToLiteral(factory)));
            if (newPersonVM.Gender != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("gender"))), newPersonVM.Gender.ToLiteral(factory)));
            if (newPersonVM.Location != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(foaf("location"))), newPersonVM.Location.ToLiteral(factory)));
            
            using(var connector = new StardogManager().GetConnector(DefaultName))
            {
                connector.UpdateGraph(new Uri(StardogConstants.DefaultGraphName), toAdd, toRemove);
            }
        }

        public PersonViewModel GetPerson(string id)
        {
            PersonViewModel personVM = new PersonViewModel();

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                IGraph result = connector.Query(string.Format("DESCRIBE {0}", string.Format("<{0}{1}>", URIs.TRAS, id))) as IGraph;

                if (result == null)
                    return null;

                personVM.Id = id;

                if (result.Triples != null && result.Triples.Count > 0)
                {
                    foreach (Triple t in result.Triples)
                    {
                        var predicate = t.Predicate.ToString();
                        var obj = t.Object.ToString();

                        if (predicate.Equals(foaf("mbox")))
                        {
                            personVM.Email = obj.Substring(obj.IndexOf(":") + 1, obj.IndexOf("^") - obj.IndexOf(":") - 1);
                        }

                        if (predicate.Equals(foaf("nickname")))
                        {
                            personVM.Id = obj.Substring(0, obj.IndexOf("^"));
                            personVM.NickName = obj.Substring(0, obj.IndexOf("^"));
                        }

                        if (predicate.Equals(foaf("firstName")))
                        {
                            personVM.FirstName = obj.Substring(0, obj.IndexOf("^"));
                        }

                        if (predicate.Equals(foaf("lastName")))
                        {
                            personVM.LastName = obj.Substring(0, obj.IndexOf("^"));
                        }

                        if (predicate.Equals(foaf("gender")))
                        {
                            personVM.Gender = obj.Substring(0, obj.IndexOf("^"));
                        }

                        if (predicate.Equals(foaf("age")))
                        {
                            var stringAge = obj.Substring(0, obj.IndexOf("^"));
                            int age = -1;
                            if(int.TryParse(stringAge, out age))
                            {
                                personVM.Age = age;
                            }
                        }

                        if (predicate.Equals(foaf("location")))
                        {
                            personVM.Location = obj.Substring(0, obj.IndexOf("^"));
                        }
                    }
                }
            }
            return personVM;
        }
        #endregion

        #region Itinerary
        public void CreateOrUpdateItinerary(ItineraryViewModel itinVM)
        {
            NodeFactory factory = new NodeFactory();
            List<Triple> toAdd = new List<Triple>();

            var subject = factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, itinVM.Id)));

            toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(tras("Itinerary")))));
            if (itinVM.Name != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), itinVM.Name.ToLiteral(factory)));
            if (itinVM.Rating > 0)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("rating"))), itinVM.Rating.ToLiteral(factory)));
            if (itinVM.Budget >= 0)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("budget"))), itinVM.Budget.ToLiteral(factory)));
            if (itinVM.Budget > 0)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("nrOfDays"))), itinVM.NrOfDays.ToLiteral(factory)));
            if(itinVM.StartDate > DateTime.MinValue)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("startDate"))), itinVM.StartDate.ToLiteral(factory)));
            if (itinVM.EndDate > DateTime.MinValue)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("endDate"))), itinVM.EndDate.ToLiteral(factory)));
            if (itinVM.Creator != null && itinVM.Creator.Id != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("creator"))), factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, itinVM.Creator.Id)))));

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                connector.UpdateGraph(new Uri(StardogConstants.DefaultGraphName), toAdd, null);
            }
        }

        public ItineraryViewModel GetItinerary(string id)
        {
            ItineraryViewModel itinVM = new ItineraryViewModel();

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                IGraph result = connector.Query(string.Format("DESCRIBE {0}", string.Format("<{0}{1}>", URIs.TRAS, id))) as IGraph;

                if (result == null)
                    return null;

                itinVM.Id = id;

                if (result.Triples != null && result.Triples.Count > 0)
                {
                    foreach (Triple t in result.Triples)
                    {
                        var predicate = t.Predicate.ToString();

                        if (t.Object.NodeType == NodeType.Literal)
                        {
                            var obj = t.Object.ToString();

                            if (predicate.Equals(tras("name")))
                            {
                                itinVM.Name = obj.Substring(0, obj.IndexOf("^"));
                            }

                            if (predicate.Equals(tras("rating")))
                            {
                                var stringRating = obj.Substring(0, obj.IndexOf("^"));
                                int rating = -1;
                                if (int.TryParse(stringRating, out rating))
                                {
                                    itinVM.Rating = rating;
                                }
                            }

                            if (predicate.Equals(tras("budget")))
                            {
                                var stringBudget = obj.Substring(0, obj.IndexOf("^"));
                                double budget = -1;
                                if (double.TryParse(stringBudget, out budget))
                                {
                                    itinVM.Budget = budget;
                                }
                            }

                            if (predicate.Equals(tras("nrOfDays")))
                            {
                                var stringNrOfDays = obj.Substring(0, obj.IndexOf("^"));
                                int nrOfDays = -1;
                                if (int.TryParse(stringNrOfDays, out nrOfDays))
                                {
                                    itinVM.NrOfDays = nrOfDays;
                                }
                            }

                            if (predicate.Equals(tras("startDate")))
                            {
                                var stringStartDate = obj.Substring(0, obj.IndexOf("^"));
                                DateTime startDate;
                                if (DateTime.TryParse(stringStartDate, out startDate))
                                {
                                    itinVM.StartDate = startDate;
                                }
                            }

                            if (predicate.Equals(tras("endDate")))
                            {
                                var stringEndDate = obj.Substring(0, obj.IndexOf("^"));
                                DateTime endDate;
                                if (DateTime.TryParse(stringEndDate, out endDate))
                                {
                                    itinVM.EndDate = endDate;
                                }
                            }
                        }

                        if (t.Object.NodeType == NodeType.Uri)
                        {
                            if (predicate.Equals(tras("creator")))
                            {
                                var creatorId = (t.Object as IUriNode).Uri.Fragment.Substring(1);
                                PersonViewModel creator = GetPerson(creatorId);
                                if (creator != null)
                                {
                                    itinVM.Creator = creator;
                                }
                            }
                        }
                    }
                }
            }
            return itinVM;
        }
        #endregion

        #region Namespace Helpers Methods
        public string foaf(string id)
        {
            return string.Format("{0}{1}", URIs.FOAF, id);
        }

        public string rdf(string id)
        {
            return string.Format("{0}{1}", URIs.RDF, id);
        }
        
        public string tras(string id)
        {
            return string.Format("{0}{1}", URIs.TRAS, id);
        }

        public string xsd(string id)
        {
            return string.Format("{0}{1}", URIs.XSD, id);
        }
        #endregion
    }
}
