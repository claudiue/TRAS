using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public static class Constants
    {
        public const string GeoNamesUrl = "http://api.geonames.org/search?";
        public const string GeoNamesUsername = "claudiuepure";
        private static string[] SpotsSearchOptions = { "S"
                                                    ,"AIRP"
                                                    ,"ARCH"
                                                    ,"BUSTP"
                                                    ,"BUSTP"
                                                    ,"CAVE"
                                                    ,"CMP"
                                                    ,"CH"
                                                    ,"CSTL"
                                                    ,"CSNO"
                                                    ,"FY"
                                                    ,"GDN"
                                                    ,"HSTS"
                                                    ,"HTL"
                                                    ,"MALL"
                                                    ,"MSQE"
                                                    ,"MSTY"
                                                    ,"MTRO"
                                                    ,"MUS"
                                                    ,"PAL"
                                                    ,"PYR"
                                                    ,"PYRS"
                                                    ,"REST"
                                                    ,"RSRT"
                                                    ,"RSTN"
                                                    ,"SPA"
                                                    ,"STDM"
                                                    ,"THTR"
                                                    ,"TMPL"
                                                    ,"TOWR"
                                                    ,"ZOO"
                                                    ,"SQR"
                                                   };

        private static string _featureCodes;
        public static string SpotsSearchFeatureCodesString
        {
            get
            {
                if (string.IsNullOrEmpty(_featureCodes))
                {
                    _featureCodes = string.Empty;
                    _featureCodes = string.Format("featureClass={0}", SpotsSearchOptions[0]);
                    foreach (string s in SpotsSearchOptions.Skip(1))
                    {
                        _featureCodes += string.Format("&featureCode={0}", s);
                    }
                }
                return _featureCodes;
            }
        }

    }
}
