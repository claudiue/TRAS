using Newtonsoft.Json.Linq;
using NGeo.GeoNames;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public class GeoNamesAgent
    {
        public Toponym Search(string query, string type)
        {
            //string url = string.Format("{0}q={1}&username={2}&startRow=0&maxRows=1&type={3}", Constants.GeoNamesUrl, query, Constants.GeoNamesUsername, type);
            //WebClient client = new WebClient();
            //client.Encoding = Encoding.UTF8;
            //string response = client.DownloadString(url);
            //var jobj = JObject.Parse(response);

            using (var geoNamesClient = new NGeo.GeoNames.GeoNamesClient())
            {
                SearchOptions searchOptions = 
                    new SearchOptions(SearchType.Query, query)
                    {
                        UserName = Constants.GeoNamesUsername,
                        StartRow = 0,
                        MaxRows = 1
                    };
                var toponym = geoNamesClient.Search(searchOptions).FirstOrDefault();
                return toponym;
            }
        }

        public ReadOnlyCollection<Toponym> FindNearBy(Toponym toponym)
        {
            using (var geoNamesClient = new NGeo.GeoNames.GeoNamesClient())
            {
                NearbyPlaceNameFinder finder =
                    new NearbyPlaceNameFinder()
                    {
                        Latitude = toponym.Latitude,
                        Longitude = toponym.Longitude,
                        UserName = Constants.GeoNamesUsername,
                        Language = "EN",
                        RadiusInKm = 5,
                        MaxRows = 100
                    };
                var res = geoNamesClient.FindNearbyPlaceName(finder);
                return res;
            }
        }

        public ReadOnlyCollection<Toponym> FindNearBy(double lat, double lng)
        {
            using (var geoNamesClient = new NGeo.GeoNames.GeoNamesClient())
            {
                NearbyPlaceNameFinder finder =
                    new NearbyPlaceNameFinder()
                    {
                        Latitude = lat,
                        Longitude = lng,
                        UserName = Constants.GeoNamesUsername,
                        Language = "EN",
                        RadiusInKm = 5,
                        MaxRows = 100
                    };
                var res = geoNamesClient.FindNearbyPlaceName(finder);
                return res;
            }
        }
    }
}
