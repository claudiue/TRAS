using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleStore.Stardog
{
    public static class StardogConstants
    {
        public const string DefaultGraphName = @"tag:stardog:api:context:default";
        public const string AllGraphsName = @"tag:stardog:api:context:all"; 
    }

    public static class URIs
    {
        public const string XML = @"http://www.w3.org/XML/1998/namespace";
        public const string XSD = @"http://www.w3.org/2001/XMLSchema#";
        public const string RDF = @"http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        public const string RDFS = @"http://www.w3.org/2000/01/rdf-schema#";
        public const string OWL = @"http://www.w3.org/2002/07/owl#";
        public const string FOAF = @"http://xmlns.com/foaf/0.1/";
        public const string DC = @"http://purl.org/dc/elements/1.1/";
        public const string GEONAMES = @"http://www.geonames.org/ontology#";
        public const string TRAS = @"http://www.tras.org/ontology#";
    }
}
