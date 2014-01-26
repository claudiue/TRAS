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
                if (oldPersonVM.Following != null && oldPersonVM.Following.Count > 0)
                {
                    foreach(var pers in oldPersonVM.Following)
                    {
                        toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("follows"))), factory.CreateUriNode(new Uri(tras(pers.Id)))));
                    }
                }
                if (oldPersonVM.Intersts != null && oldPersonVM.Intersts.Count > 0)
                {
                    foreach (var interest in oldPersonVM.Intersts)
                    {
                        toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("interestedIn"))), factory.CreateUriNode(new Uri(tras(interest.Id)))));
                    }
                }
                    
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
            if (newPersonVM.Following != null && newPersonVM.Following.Count > 0)
            {
                foreach (var pers in newPersonVM.Following)
                {
                    toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("follows"))), factory.CreateUriNode(new Uri(tras(pers.Id)))));
                }
            }
            if (newPersonVM.Intersts != null && newPersonVM.Intersts.Count > 0)
            {
                foreach (var interest in newPersonVM.Intersts)
                {
                    toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("interestedIn"))), factory.CreateUriNode(new Uri(tras(interest.Id)))));
                }
            }

            using(var connector = new StardogManager().GetConnector(DefaultName))
            {
                connector.UpdateGraph(new Uri(StardogConstants.DefaultGraphName), toAdd, toRemove);
            }
        }

        public PersonViewModel GetPerson(string id, bool recursive = true)
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
                        if (t.Object.NodeType == NodeType.Literal)
                        {
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
                                if (int.TryParse(stringAge, out age))
                                {
                                    personVM.Age = age;
                                }
                            }

                            if (predicate.Equals(foaf("location")))
                            {
                                personVM.Location = obj.Substring(0, obj.IndexOf("^"));
                            }
                        }
                        if (t.Object.NodeType == NodeType.Uri)
                        {
                            if (predicate.Equals(tras("follows")) && recursive)
                            {
                                var personId = (t.Object as IUriNode).Uri.Fragment.Substring(1);
                                PersonViewModel person = GetPerson(personId, false);
                                if (person != null)
                                {
                                    personVM.Following.Add(person);
                                }
                            }

                            if (predicate.Equals(tras("interestedIn")))
                            {
                                var thingId = (t.Object as IUriNode).Uri.Fragment.Substring(1);
                                ThingViewModel thing = GetThing(thingId);
                                if (thing != null)
                                {
                                    personVM.Intersts.Add(thing);
                                }
                            }
                        }
                    }
                }
            }
            return personVM;
        }
        #endregion

        #region Itinerary
        public void CreateOrUpdateItinerary(ItineraryViewModel newItinVM)
        {
            if (newItinVM == null || newItinVM.Id == null)
                return;

            NodeFactory factory = new NodeFactory();
            List<Triple> toAdd = new List<Triple>();
            List<Triple> toRemove = new List<Triple>();

            ItineraryViewModel oldItinVM = GetItinerary(newItinVM.Id);   

            var subject = factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, newItinVM.Id)));

            if (oldItinVM != null & oldItinVM.Id == newItinVM.Id)
            {
                toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(tras("Itinerary")))));
                if (oldItinVM.Name != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), oldItinVM.Name.ToLiteral(factory)));
                if (oldItinVM.Rating > 0)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("rating"))), oldItinVM.Rating.ToLiteral(factory)));
                if (oldItinVM.Budget >= 0)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("budget"))), oldItinVM.Budget.ToLiteral(factory)));
                if (oldItinVM.Budget > 0)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("nrOfDays"))), oldItinVM.NrOfDays.ToLiteral(factory)));
                if (oldItinVM.StartDate > DateTime.MinValue)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("startDate"))), oldItinVM.StartDate.ToLiteral(factory)));
                if (oldItinVM.EndDate > DateTime.MinValue)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("endDate"))), oldItinVM.EndDate.ToLiteral(factory)));
                if (oldItinVM.Creator != null && oldItinVM.Creator.Id != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("creator"))), factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, oldItinVM.Creator.Id)))));
                if (oldItinVM.Features != null && oldItinVM.Features.Count > 0)
                {
                    foreach (var feat in oldItinVM.Features)
                    {
                        toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("hasFeature"))), factory.CreateUriNode(new Uri(tras(feat.Id)))));
                    }
                }
            }

            toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(tras("Itinerary")))));
            if (newItinVM.Name != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), newItinVM.Name.ToLiteral(factory)));
            if (newItinVM.Rating > 0)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("rating"))), newItinVM.Rating.ToLiteral(factory)));
            if (newItinVM.Budget >= 0)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("budget"))), newItinVM.Budget.ToLiteral(factory)));
            if (newItinVM.Budget > 0)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("nrOfDays"))), newItinVM.NrOfDays.ToLiteral(factory)));
            if(newItinVM.StartDate > DateTime.MinValue)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("startDate"))), newItinVM.StartDate.ToLiteral(factory)));
            if (newItinVM.EndDate > DateTime.MinValue)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("endDate"))), newItinVM.EndDate.ToLiteral(factory)));
            if (newItinVM.Creator != null && newItinVM.Creator.Id != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("creator"))), factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, newItinVM.Creator.Id)))));
            if (newItinVM.Features != null && newItinVM.Features.Count > 0)
            {
                foreach (var feat in newItinVM.Features)
                {
                    toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("hasFeature"))), factory.CreateUriNode(new Uri(tras(feat.Id)))));
                }
            }

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                connector.UpdateGraph(new Uri(StardogConstants.DefaultGraphName), toAdd, toRemove);
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

                            if (predicate.Equals(tras("hasFeature")))
                            {
                                var featureId = (t.Object as IUriNode).Uri.Fragment.Substring(1);
                                FeatureViewModel feature = GetFeature(featureId);
                                if (feature != null)
                                {
                                    itinVM.Features.Add(feature);
                                }
                            }
                        }
                    }
                }
            }
            return itinVM;
        }

        public IList<ItineraryViewModel> GetItineraries(string creatorId = null)
        {
            IList<ItineraryViewModel> itineraries = new List<ItineraryViewModel>();
            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                string query = string.IsNullOrEmpty(creatorId)
                    ? "SELECT * WHERE { ?s <" + rdf("type") + "> <" + tras("Itinerary") + "> }"
                    : "SELECT * WHERE { ?s <" + rdf("type") + "> <" + tras("Itinerary") + "> . ?s <" + tras("creator") + "> <" + tras(creatorId) + "> }";
                SparqlResultSet result = connector.Query(query) as SparqlResultSet;

                if (result == null)
                    return null;

                if (result.Results != null)
                {
                    if (result.Results.Count == 0)
                        return itineraries;

                    foreach (var s in result.Results)
                    {
                        var itineraryId = (s[0] as IUriNode).Uri.Fragment.Substring(1);
                        var itinerary = GetItinerary(itineraryId);
                        if (itinerary != null)
                        {
                            itineraries.Add(itinerary);
                        }
                    }
                }

                //if (result.Triples != null && result.Triples.Count > 0)
                //{
                //    foreach (Triple t in result.Triples)
                //    {

                //        var predicate = t.Predicate.ToString();
                //    }
                //}
            }
            return itineraries;
        }
        #endregion

        #region Feature
        public void CreateOrUpdateFeature(FeatureViewModel newFeatureVM)
        {
            if (newFeatureVM == null || newFeatureVM.Id == null)
                return;

            NodeFactory factory = new NodeFactory();
            List<Triple> toAdd = new List<Triple>();
            List<Triple> toRemove = new List<Triple>();

            FeatureViewModel oldFeatureVM = GetFeature(newFeatureVM.Id);

            var subject = factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, newFeatureVM.Id)));

            if (oldFeatureVM != null & oldFeatureVM.Id == newFeatureVM.Id)
            {
                toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(tras("Feature")))));
                if (oldFeatureVM.Name != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), oldFeatureVM.Name.ToLiteral(factory)));
                if (oldFeatureVM.Lat != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("lat"))), oldFeatureVM.Lat.ToLiteral(factory)));
                if (oldFeatureVM.Long != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("long"))), oldFeatureVM.Long.ToLiteral(factory)));
                if (oldFeatureVM.Code != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("code"))), oldFeatureVM.Long.ToLiteral(factory)));
                if (oldFeatureVM.Parent != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("hasParent"))), factory.CreateUriNode(new Uri(tras(oldFeatureVM.Parent.Id)))));
            }

            toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(tras("Feature")))));
            if (newFeatureVM.Name != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), newFeatureVM.Name.ToLiteral(factory)));
            if (newFeatureVM.Lat != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("lat"))), newFeatureVM.Lat.ToLiteral(factory)));
            if (newFeatureVM.Long != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("long"))), newFeatureVM.Long.ToLiteral(factory)));
            if (newFeatureVM.Code != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("code"))), newFeatureVM.Long.ToLiteral(factory)));
            if (newFeatureVM.Parent != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("hasParent"))), factory.CreateUriNode(new Uri(tras(newFeatureVM.Parent.Id)))));

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                connector.UpdateGraph(new Uri(StardogConstants.DefaultGraphName), toAdd, toRemove);
            }
        }

        public FeatureViewModel GetFeature(string id)
        {
            FeatureViewModel featureVM = new FeatureViewModel();

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                IGraph result = connector.Query(string.Format("DESCRIBE {0}", string.Format("<{0}{1}>", URIs.TRAS, id))) as IGraph;

                if (result == null)
                    return null;

                featureVM.Id = id;

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
                                featureVM.Name = obj.Substring(0, obj.IndexOf("^"));
                            }

                            if (predicate.Equals(tras("lat")))
                            {
                                var stringLat = obj.Substring(0, obj.IndexOf("^"));
                                double lat = -1;
                                if (double.TryParse(stringLat, out lat))
                                {
                                    featureVM.Lat = lat;
                                }
                            }

                            if (predicate.Equals(foaf("long")))
                            {
                                var stringLong = obj.Substring(0, obj.IndexOf("^"));
                                double Long = -1;
                                if (double.TryParse(stringLong, out Long))
                                {
                                    featureVM.Long = Long;
                                }
                            }

                            if (predicate.Equals(tras("code")))
                            {
                                featureVM.Code = obj.Substring(0, obj.IndexOf("^"));
                            }
                        }
                        if (t.Object.NodeType == NodeType.Uri)
                        {
                            if (predicate.Equals(tras("creator")))
                            {
                                var placeId = (t.Object as IUriNode).Uri.Fragment.Substring(1);
                                PlaceViewModel parent = GetPlace(placeId);
                                if (parent != null)
                                {
                                    featureVM.Parent = parent;
                                }
                            }
                        }
                    }
                }
            }
            return featureVM;
        }
        #endregion

        #region Place
        public void CreateOrUpdatePlace(PlaceViewModel newPlaceVM)
        {
            if (newPlaceVM == null || newPlaceVM.Id == null)
                return;

            NodeFactory factory = new NodeFactory();
            List<Triple> toAdd = new List<Triple>();
            List<Triple> toRemove = new List<Triple>();

            PlaceViewModel oldPlaceVM = GetPlace(newPlaceVM.Id);

            var subject = factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, newPlaceVM.Id)));

            if (oldPlaceVM != null & oldPlaceVM.Id == newPlaceVM.Id)
            {
                toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(tras("Place")))));
                if (oldPlaceVM.Name != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), oldPlaceVM.Name.ToLiteral(factory)));
                if (oldPlaceVM.Lat != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("lat"))), oldPlaceVM.Lat.ToLiteral(factory)));
                if (oldPlaceVM.Long != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("long"))), oldPlaceVM.Long.ToLiteral(factory)));
            }

            toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(tras("Place")))));
            if (newPlaceVM.Name != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), newPlaceVM.Name.ToLiteral(factory)));
            if (newPlaceVM.Lat != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("lat"))), newPlaceVM.Lat.ToLiteral(factory)));
            if (newPlaceVM.Long != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("long"))), newPlaceVM.Long.ToLiteral(factory)));

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                connector.UpdateGraph(new Uri(StardogConstants.DefaultGraphName), toAdd, toRemove);
            }
        }

        public PlaceViewModel GetPlace(string id)
        {
            PlaceViewModel placeVM = new PlaceViewModel();

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                IGraph result = connector.Query(string.Format("DESCRIBE {0}", string.Format("<{0}{1}>", URIs.TRAS, id))) as IGraph;

                if (result == null)
                    return null;

                placeVM.Id = id;

                if (result.Triples != null && result.Triples.Count > 0)
                {
                    foreach (Triple t in result.Triples)
                    {
                        var predicate = t.Predicate.ToString();
                        var obj = t.Object.ToString();

                        if (predicate.Equals(tras("name")))
                        {
                            placeVM.Name = obj.Substring(0, obj.IndexOf("^"));
                        }

                        if (predicate.Equals(tras("lat")))
                        {
                            var stringLat = obj.Substring(0, obj.IndexOf("^"));
                            double lat = -1;
                            if (double.TryParse(stringLat, out lat))
                            {
                                placeVM.Lat = lat;
                            }
                        }

                        if (predicate.Equals(foaf("long")))
                        {
                            var stringLong = obj.Substring(0, obj.IndexOf("^"));
                            double Long = -1;
                            if (double.TryParse(stringLong, out Long))
                            {
                                placeVM.Long = Long;
                            }
                        }
                    }
                }
            }
            return placeVM;
        }
        #endregion

        #region Thing
        public void CreateOrUpdateThing(ThingViewModel newThingVM)
        {
            if (newThingVM == null || newThingVM.Id == null)
                return;

            NodeFactory factory = new NodeFactory();
            List<Triple> toAdd = new List<Triple>();
            List<Triple> toRemove = new List<Triple>();

            ThingViewModel oldThingVM = GetThing(newThingVM.Id);

            var subject = factory.CreateUriNode(new Uri(string.Format("{0}{1}", URIs.TRAS, newThingVM.Id)));

            if (oldThingVM != null & oldThingVM.Id == newThingVM.Id)
            {
                toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(owl("Thing")))));
                if (oldThingVM.Name != null)
                    toRemove.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), oldThingVM.Name.ToLiteral(factory)));
            }

            toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(rdf("type"))), factory.CreateUriNode(new Uri(tras("Thing")))));
            if (newThingVM.Name != null)
                toAdd.Add(new Triple(subject, factory.CreateUriNode(new Uri(tras("name"))), newThingVM.Name.ToLiteral(factory)));

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                connector.UpdateGraph(new Uri(StardogConstants.DefaultGraphName), toAdd, toRemove);
            }
        }

        public ThingViewModel GetThing(string id) 
        {
            ThingViewModel thingVM = new ThingViewModel();

            using (var connector = new StardogManager().GetConnector(DefaultName))
            {
                IGraph result = connector.Query(string.Format("DESCRIBE {0}", string.Format("<{0}{1}>", URIs.TRAS, id))) as IGraph;

                if (result == null)
                    return null;

                thingVM.Id = id;

                if (result.Triples != null && result.Triples.Count > 0)
                {
                    foreach (Triple t in result.Triples)
                    {
                        var predicate = t.Predicate.ToString();
                        var obj = t.Object.ToString();

                        if (predicate.Equals(tras("name")))
                        {
                            thingVM.Name = obj.Substring(0, obj.IndexOf("^"));
                        }
                    }
                }
            }
            return thingVM;
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

        public string owl(string id)
        {
            return string.Format("{0}{1}", URIs.OWL, id);
        }
        #endregion
    }
}
