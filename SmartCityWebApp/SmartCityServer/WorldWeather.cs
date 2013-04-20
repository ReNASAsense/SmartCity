using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;

namespace TesterApplication
{
    /// <summary>
    /// Get weather data using World Weather API
    /// </summary>
    public class WorldWeather
    {
        public static String key = "hwucsnjf2hb3bv69c23cjrms";
        public static String url = "http://api.worldweatheronline.com/free/v1/weather.ashx";

        protected static String httpGetRequest(String url, Dictionary<String, String> parameters)
        {
            Int32 parameterCount = 0;
            String address = url, responseText = String.Empty;
            if (parameters != null)
            {
                foreach (KeyValuePair<String, String> parameter in parameters)
                {
                    if (parameterCount == 0)
                    {
                        address += "?" + parameter.Key + "=" + parameter.Value;
                    }
                    else
                    {
                        address += "&" + parameter.Key + "=" + parameter.Value;
                    }
                    parameterCount++;
                }
            }

            using (WebClient client = new WebClient())
            {
                responseText = client.DownloadString(address);
            }

            return responseText;
        }

        /// <summary>
        /// Return current weather on given longitude and latitude,
        /// if quota exceeded or not found return null;
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns>Tuple of format(longitude, latitude, tempC, humidity)</returns>
        public static Tuple<String, String, String, String> getWeatherAt(Double longitude, Double latitude)
        {
            String tempC = null;
            String humidity = null;
            String longitudeStr = String.Format("{0:0.##}", longitude);
            String latitudeStr = String.Format("{0:0.##}", latitude);
            String response = String.Empty;
            Boolean tempFound = false;
            Boolean humidityFound = false;
            Dictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("q", String.Format("{0:0.##}", longitude) + "%2C" + String.Format("{0:0.##}", latitude));
            parameters.Add("format", "xml");
            parameters.Add("num_of_days", "1");
            parameters.Add("fx", "no");
            parameters.Add("cc", "yes");
            parameters.Add("includelocation", "yes");
            parameters.Add("show_comments", "no");
            parameters.Add("key", key);
            response = httpGetRequest(url, parameters);

            using (XmlReader reader = XmlReader.Create(new StringReader(response)))
            {
                if (reader.ReadToFollowing("temp_C"))
                {
                    tempC = reader.ReadInnerXml();
                    tempFound = true;
                }

                if (reader.ReadToFollowing("humidity"))
                {
                    humidity = reader.ReadInnerXml();
                    humidityFound = false;
                }
            }
            return (humidityFound && tempFound) ? 
                new Tuple<String, String, String, String>(longitudeStr, latitudeStr, tempC, humidity) : null;
        }
    }
}
