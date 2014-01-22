using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServices
{
    public class GNFeatureViewModel
    {
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public string FclName { get; set; }
        public string CountryCode { get; set; }
        public string Lng { get; set; }
        public string FcodeName { get; set; }
        public string ToponymName { get; set; }
        public string Fcl { get; set; }
        public string Name { get; set; }
        public string Fcode { get; set; }
        public int GeonameId { get; set; }
        public string Lat { get; set; }
        public string AdminName1 { get; set; }
        public int Population { get; set; }
    }
}