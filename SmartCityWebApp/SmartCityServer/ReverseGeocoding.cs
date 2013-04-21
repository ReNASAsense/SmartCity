using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SmartCityServer
{
    public class CityCountry
    {
        public string countryName;
        public string adminName1;
    }

    public static class ReverseGeocoding
    {
        public static CityCountry GetCityCountry(string lat, string lng)
        {

            try
            {

                //string geonamesCountryQuery = "http://api.geonames.org/countryCode?lat=" + lat + "&lng=" + lng + "&username=spaceappschallenge&type=JSON";
                //string countryResult = new WebClient().DownloadString(geonamesCountryQuery);
                //CityCountry country = Newtonsoft.Json.JsonConvert.DeserializeObject<CityCountry>(countryResult);
                CityCountry cc = null;
                using (WebClient webClient = new WebClient())
                {
                    string geonamesCityQuery = "http://api.geonames.org/countrySubdivisionJSON?lat=" + lat + "&lng=" + lng + "&username=spaceappschallenge";
                    string cityResult = webClient.DownloadString(geonamesCityQuery);
                    cc = Newtonsoft.Json.JsonConvert.DeserializeObject<CityCountry>(cityResult);
                }
                return cc;
            }
            catch (Exception ex)
            {
                return new CityCountry();
            }
        }
    }
}
