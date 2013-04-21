using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCityServer
{
    public partial class GetData2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string response = System.IO.File.ReadAllText(MapPath("TestXml.xml"));
            //this.Page.Request.QueryString;
            //this.Response.Write(response);
            try
            {
                if (this.Request.HttpMethod != "GET")
                {
                    this.Response.Write("Request method must be GET");
                    return;
                }
                System.Text.StringBuilder bld = new System.Text.StringBuilder();
                bld.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");
                bld.AppendLine("<Measurements>");
                if (this.Request.QueryString.AllKeys.Contains("lat") & this.Request.QueryString.AllKeys.Contains("lon"))
                {
                    
                    string lat = this.Request.QueryString["lat"].ToString().Replace(',','.');
                    string lon = this.Request.QueryString["lon"].ToString().Replace(',', '.');
                    CityCountry cc = ReverseGeocoding.GetCityCountry(lat, lon);
                    bld.AppendLine(String.Format("<Country>{0}</Country>", cc.countryName));
                    bld.AppendLine(String.Format("<City>{0}</City>", cc.adminName1));

                    WeatherReading ww = WorldWeather.getWeatherAt(lat, lon);
                    bld.AppendLine(String.Format("<Humidity>{0}</Humidity>", ww.humidity.Replace(',', '.')));
                    bld.AppendLine(String.Format("<Temperature>{0}</Temperature>", ww.tempC.Replace(',', '.')));
                    bld.AppendLine(String.Format("<Windspeed>{0}</Windspeed>", ww.windspeed.Replace(',', '.')));
                    bld.AppendLine(String.Format("<Winddirection>{0}</Winddirection>", ww.winddirection.Replace(',', '.')));
                    bld.AppendLine(String.Format("<Visibility>{0}</Visibility>", ww.visibility.Replace(',', '.')));
                    bld.AppendLine(String.Format("<Pressure>{0}</Pressure>", ww.pressure.Replace(',', '.')));
                    bld.AppendLine(String.Format("<Cloudcover>{0}</Cloudcover>", ww.cloudcover.Replace(',', '.')));
                }
                //windspeed = xmldoc["data"]["current_condition"]["windspeedKmph"].InnerText;
                //winddirection = xmldoc["data"]["current_condition"]["winddirDegree"].InnerText;
                //visibility = xmldoc["data"]["current_condition"]["visibility"].InnerText;
                //pressure = xmldoc["data"]["current_condition"]["pressure"].InnerText;
                //cloudcover = xmldoc["data"]["current_condition"]["cloudcover"].InnerText;

                //using (SmartCityEntities ctx = new SmartCityEntities())
                //{
                //    List<Sample> samples = ctx.Sample.ToList();
                //    if (this.Request.QueryString.AllKeys.Contains("device"))
                //    {
                //        int device = Convert.ToInt32(this.Request.QueryString["device"]);
                //        samples = samples.Where(dev => dev.device_id == device).ToList();
                //    }
                //    foreach (var item in samples)
                //    {
                //        bld.AppendLine("<Measurement>");
                //        bld.AppendLine(String.Format("<id>{0}</id>", item.id_measurement));
                //        bld.AppendLine(String.Format("<deviceId>{0}</deviceId>", item.device_id));
                //        bld.AppendLine(String.Format("<timestamp>{0}</timestamp>", item.sample_time));
                //        bld.AppendLine(String.Format("<longitude>{0}</longitude>", item.lon.ToString().Replace(',', '.')));
                //        bld.AppendLine(String.Format("<latitude>{0}</latitude>", item.lat.ToString().Replace(',', '.')));
                //        bld.AppendLine(String.Format("<humidity>{0}</humidity>", item.humidity.ToString().Replace(',', '.')));
                //        bld.AppendLine(String.Format("<temperature>{0}</temperature>", item.temperature.ToString().Replace(',', '.')));
                //        bld.AppendLine(String.Format("</Measurement>"));
                //    }
                //}
                bld.AppendLine("</Measurements>");
                this.Response.Write(bld.ToString());
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.ToString());
                return;
            }
        }
    }
}