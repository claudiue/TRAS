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
using ViewModels;

namespace WebServices
{
    public class GeoNamesAgent
    {
        private const int _geonamesLimit = 5000;

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


        public IList<FeatureViewModel> GetSpots(string query, string type = "json", int startRow = 0, int maxRows = 1000)
        {
            string url = string.Format("{0}q={1}&username={2}&type={3}&{4}&startRow={5}&maxRows={6}",
                Constants.GeoNamesUrl, query, Constants.GeoNamesUsername, type, Constants.SpotsSearchFeatureCodesString, startRow, maxRows);
            
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string response = client.DownloadString(url);
            var jobj = JObject.Parse(response);

            var items = JsonConvert.DeserializeObject<List<FeatureViewModel>>(jobj["geonames"].ToString());
            return items;
        }

        public IList<FeatureViewModel> SearchSpots(string query, string type = "json", int startRow = 0, int maxRows = 1000)
        {
            IList<FeatureViewModel> list = new List<FeatureViewModel>();

            string url = string.Format("{0}q={1}&username={2}&type={3}&{4}&startRow={5}&maxRows={6}", 
                Constants.GeoNamesUrl, query, Constants.GeoNamesUsername, type, Constants.SpotsSearchFeatureCodesString, startRow, maxRows);

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
                        return JsonConvert.DeserializeObject<List<FeatureViewModel>>(jobj["geonames"].ToString());
                    }
                    else 
                    {
                        foundSpots = foundSpots <= _geonamesLimit ? foundSpots : _geonamesLimit;
                        list = list.Concat(JsonConvert.DeserializeObject<List<FeatureViewModel>>(jobj["geonames"].ToString())).ToList();
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
