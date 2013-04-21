using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XmlParserLib.Tools;
using Artem;
using XmlParserLib.Entities;
using System.Globalization;

namespace NaSaChallenge
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           /* XmlParser parserXml = new XmlParser(string.Empty, null);
            var data = parserXml.Data;

            if (data != null)
            {
                gvData.DataSource = data;
                gvData.DataBind();
            }

            CreateMarkers(data);
            * */
        }

        protected void Button_Click(object sender, EventArgs e)
        {
            //new HttpRequest("", "", "");
            Response.Redirect("Default.aspx");
        }

        void CreateMarkers(List<Measurement> data)
        {
            foreach (Measurement m in data)
            {
                Artem.Google.UI.Marker marker = new Artem.Google.UI.Marker();
                marker.Position = new Artem.Google.UI.LatLng(double.Parse(m.latitude, CultureInfo.CurrentCulture),
                                                             double.Parse(m.longitude, CultureInfo.CurrentCulture));

                string info = String.Format("Device: {0} <br/> Time: {1} <br/> Address: {2} <br/> {3} {4}",
                                            m.deviceId, m.timestamp, marker.Address, 
                                            htmlColorBlock(ColorCalcTemp(Convert.ToDouble(m.temprature)), "Temprature: " + m.temprature),
                                            htmlColorBlock(ColorCalcHumidity(Convert.ToDouble(m.humidity)), "Humidity: " + m.humidity));

                marker.Info = info;
                GoogleMap1.Markers.Add(marker);

            }
        }

        string htmlColorBlock(string color, string data)
        {
            string html = String.Format("<table><tr><td style=\"background-color:{0};\" width=\"5\" height=\"20\"></td><td>{1}</td></tr></table>",
                                         color, data);
            return html;
        }


        string ColorCalcTemp(double temp)
        {
            double maxTemp = 50.0;
        
            double perc = temp / 50.0;
            int _R = (int)(255 * perc);

            string hexOutput;
            if (_R == 0)
            {
                hexOutput = "00";
            }
            else
            {
                hexOutput = String.Format("{0:X}", _R);
            }
            string hexColor = "#" + hexOutput + "0b0b";

            return hexColor;
        }

        string ColorCalcHumidity(double humidity)
        {

            int _B = (int)(800 * humidity * 0.01);

            string hexOutput;
            if (_B == 0)
            {
                hexOutput = "00";
            }
            else
            {
                hexOutput = String.Format("{0:X}", _B);
            }
            string hexColor = "#0bbd" + hexOutput;

            return hexColor;
        }
    }
}
