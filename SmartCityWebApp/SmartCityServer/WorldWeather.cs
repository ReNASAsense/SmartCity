using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;

namespace SmartCityServer
{
    public class WeatherReading
    {
        public String tempC = String.Empty;
        public String humidity = String.Empty;
        public String windspeed = String.Empty;
        public String winddirection = String.Empty;
        public String visibility = String.Empty;
        public String pressure = String.Empty;
        public String cloudcover = String.Empty;            
    }

    /// <summary>
    /// Get weather data using World Weather API
    /// </summary>
    public class WorldWeather
    {
        public static String key = "hwucsnjf2hb3bv69c23cjrms";
        public static String url = "http://api.worldweatheronline.com/free/v1/weather.ashx";

        protected static String httpGetRequest(string lat, string lon)
        {
            //Int32 parameterCount = 0;
            //String address = url;
            string responseText = String.Empty;
            //if (parameters != null)
            //{
            //    foreach (KeyValuePair<String, String> parameter in parameters)
            //    {
            //        if (parameterCount == 0)
            //        {
            //            address += "?" + parameter.Key + "=" + parameter.Value;
            //        }
            //        else
            //        {
            //            address += "&" + parameter.Key + "=" + parameter.Value;
            //        }
            //        parameterCount++;
            //    }
            //}

            string address = String.Format("http://api.worldweatheronline.com/free/v1/weather.ashx?q={0}%2C{1}&format=xml&num_of_days=1&key=hwucsnjf2hb3bv69c23cjrms", lat, lon);
            using (WebClient client = new WebClient())
            {
                responseText = client.DownloadString(address);
            }

            return responseText;
        }
        
        /// <summary>
        /// Return current weather on given longitude and latitude
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns>Tuple of format(longitude, latitude, tempC, humidity, windspeed, winddirection)</returns>
        public static WeatherReading getWeatherAt(string longitude, string latitude)
        {
            String longitudeStr = longitude;
            String latitudeStr = latitude;
            String response = String.Empty;
            //Dictionary<String, String> parameters = new Dictionary<String, String>();
            //parameters.Add("q", String.Format("{0:0.##}", longitude) + "%2C" + String.Format("{0:0.##}", latitude));
            //parameters.Add("format", "xml");
            //parameters.Add("num_of_days", "1");
            //parameters.Add("fx", "no");
            //parameters.Add("cc", "yes");
            //parameters.Add("includelocation", "yes");
            //parameters.Add("show_comments", "no");
            //parameters.Add("key", key);
            response = httpGetRequest(latitudeStr, longitudeStr);

            System.Xml.XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(response);
            WeatherReading cc = new WeatherReading();

            if (xmldoc.GetElementsByTagName("data").Count > 0 && 
                xmldoc.GetElementsByTagName("current_condition").Count> 0 && 
                xmldoc.GetElementsByTagName("error").Count == 0)
            {
                if (xmldoc["data"]["current_condition"].GetElementsByTagName("temp_C").Count > 0)
                    cc.tempC = xmldoc["data"]["current_condition"]["temp_C"].InnerText;
                if (xmldoc["data"]["current_condition"].GetElementsByTagName("humidity").Count > 0)
                    cc.humidity = xmldoc["data"]["current_condition"]["humidity"].InnerText;
                if (xmldoc["data"]["current_condition"].GetElementsByTagName("windspeedKmph").Count > 0)
                    cc.windspeed = xmldoc["data"]["current_condition"]["windspeedKmph"].InnerText;
                if (xmldoc["data"]["current_condition"].GetElementsByTagName("winddirDegree").Count > 0)
                    cc.winddirection = xmldoc["data"]["current_condition"]["winddirDegree"].InnerText;
                if (xmldoc["data"]["current_condition"].GetElementsByTagName("visibility").Count > 0)
                    cc.visibility = xmldoc["data"]["current_condition"]["visibility"].InnerText;
                if (xmldoc["data"]["current_condition"].GetElementsByTagName("pressure").Count > 0)
                    cc.pressure = xmldoc["data"]["current_condition"]["pressure"].InnerText;
                if (xmldoc["data"]["current_condition"].GetElementsByTagName("cloudcover").Count > 0)
                    cc.cloudcover = xmldoc["data"]["current_condition"]["cloudcover"].InnerText;
            }
            return cc;
        }
    }
}
