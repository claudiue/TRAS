using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{

    public class LocationData
    {
        public int GeoNameId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string ToponymName { get; set; }
        public object AlternateNames { get; set; }
        public object ChildCount { get; set; }
        public string FeatureClassCode { get; set; }
        public string FeatureClassName { get; set; }
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
        public object ContinentCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Admin1Code { get; set; }
        public string Admin1Name { get; set; }
        public object Admin2Code { get; set; }
        public object Admin2Name { get; set; }
        public object Admin3Code { get; set; }
        public object Admin3Name { get; set; }
        public object Admin4Code { get; set; }
        public object Admin4Name { get; set; }
        public object TimeZone { get; set; }
        public int Population { get; set; }
        public object Elevation { get; set; }
    }

    public class SpotData
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

    public class Data
    {
        public int parentId { get; set; }
        public SpotData spotData { get; set; }
    }

    public class Feature
    {
        public Data data { get; set; }
    }

    public class Location
    {
        public LocationData locationData { get; set; }
        public List<Feature> features { get; set; }
    }

    public class ItineraryClientViewModel
    {
        public string name { get; set; }
        public int budget { get; set; }
        public int days { get; set; }
        public int rating { get; set; }
        public List<Location> locations { get; set; }
    }

}
