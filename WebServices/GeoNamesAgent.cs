using Newtonsoft.Json;
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


        public IList<dynamic> GetSpots(string query, string type = "json", int startRow = 0, int maxRows = 1000)
        {
            string url = string.Format("{0}q={1}&username={2}&type={3}&featureClass=S&startRow={4}&maxRows={5}", Constants.GeoNamesUrl, query, Constants.GeoNamesUsername, type, startRow, maxRows);
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string response = client.DownloadString(url);
            var jobj = JObject.Parse(response);

            var items = JsonConvert.DeserializeObject<List<dynamic>>(jobj["geonames"].ToString());
            return items;
        }

        public IList<dynamic> SearchSpots(string query, string type = "json", int startRow = 0, int maxRows = 1000)
        {
            IList<dynamic> list = new List<dynamic>();

            string url = string.Format("{0}q={1}&username={2}&type={3}&featureClass=S&startRow={4}&maxRows={5}", Constants.GeoNamesUrl, query, Constants.GeoNamesUsername, type, startRow, maxRows);
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string response = client.DownloadString(url);
            var jobj = JObject.Parse(response);


            int foundSpots = 0;
            int difference = 0;

            if (jobj["totalResultsCount"] != null)
            {
                if (int.TryParse(jobj["totalResultsCount"].ToString(), out foundSpots))
                {
                    if (foundSpots <= 1000)
                    {
                        return JsonConvert.DeserializeObject<List<dynamic>>(jobj["geonames"].ToString());
                    }
                    else 
                    {
                        list = list.Concat(JsonConvert.DeserializeObject<List<dynamic>>(jobj["geonames"].ToString())).ToList();
                        do
                        {
                            startRow += maxRows;
                            difference = foundSpots - startRow;
                            if (difference > 0)
                            {
                                list = list.Concat(GetSpots(query, type, startRow, maxRows)).ToList();
                            }
                        } while (startRow < foundSpots);
                    }
                    
                }
            }

            return list;
        }


    }
}
