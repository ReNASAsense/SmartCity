using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCityServer
{
    public partial class GetData : System.Web.UI.Page
    {
        protected void LatOffset(double radius, double lat, double lon, out double latOff, out double lonOff)
        {
            //Position, decimal degrees             

             //Earth’s radius, sphere
             double R = 6378137;


             //Coordinate offsets in radians
             latOff = radius / R;
             lonOff = radius / (R * Math.Cos(Math.PI * lat / 180));

             //OffsetPosition, decimal degrees
             //latOff = dLat * 180 / Math.PI;
             //lonOff = dLon * 180 / Math.PI;
        }
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
                NumberFormatInfo numFormat = new NumberFormatInfo();
                numFormat.NumberDecimalSeparator = ".";
                using (SmartCityEntities ctx = new SmartCityEntities())
                {
                    List<Sample> samples = ctx.Sample.ToList();
                    if (this.Request.QueryString.AllKeys.Contains("device"))
                    {
                        int device = Convert.ToInt32(this.Request.QueryString["device"]);
                        samples = samples.Where(dev => dev.device_id == device).ToList();
                    }
                    bool isonlylast = false;
                    if (this.Request.QueryString.AllKeys.Contains("onlylast") & this.Request.QueryString["onlylast"] == "true")
                    {
                        isonlylast = true;
                        samples = samples.OrderBy(dev => dev.id_measurement).Take(1).ToList();
                    }
                    if (this.Request.QueryString.AllKeys.Contains("lat") & this.Request.QueryString.AllKeys.Contains("lon") & this.Request.QueryString.AllKeys.Contains("radius"))
                    {
                        double lat = Convert.ToDouble(this.Request.QueryString["lat"].Replace(',','.'), numFormat);
                        double lon = Convert.ToDouble(this.Request.QueryString["lon"].Replace(',','.'), numFormat);
                        double radius = Convert.ToDouble(this.Request.QueryString["radius"].Replace(',','.'), numFormat);
                        double latOff = 0.0;
                        double lonOff = 0.0;
                        decimal latd = (decimal)lat;
                        decimal lond = (decimal)lon;
                        LatOffset(radius, lat, lon, out latOff, out lonOff);
                        samples = samples.Where(dev => dev.lat > (latd - (decimal)latOff) & dev.lat < (latd + (decimal)latOff) & dev.lon > (lond - (decimal)lonOff) & dev.lon < (lond + (decimal)lonOff)).ToList();
                    }
                    foreach (var item in samples)
                    {
                        bld.AppendLine("<Measurement>");
                        bld.AppendLine(String.Format("<id>{0}</id>", item.id_measurement));
                        bld.AppendLine(String.Format("<deviceId>{0}</deviceId>", item.device_id));
                        bld.AppendLine(String.Format("<timestamp>{0}</timestamp>", item.sample_time));
                        bld.AppendLine(String.Format(numFormat, "<longitude>{0}</longitude>", item.lon));
                        bld.AppendLine(String.Format(numFormat, "<latitude>{0}</latitude>", item.lat));

                        //bool humset = false;
                        //bool tempset = false;
                        //bool presset = false;
                        //if (item.lat.HasValue & item.lon.HasValue & isonlylast)
                        //{
                        //    string lat = item.lat.ToString().Replace(',', '.');
                        //    string lon = item.lon.ToString().Replace(',', '.');
                        //    CityCountry cc = ReverseGeocoding.GetCityCountry(lat, lon);
                        //    bld.AppendLine(String.Format("<Country>{0}</Country>", cc.countryName));
                        //    bld.AppendLine(String.Format("<City>{0}</City>", cc.adminName1));

                        //    WeatherReading ww = WorldWeather.getWeatherAt(lat, lon);
                        //    bld.AppendLine(String.Format("<Humidity>{0}</Humidity>", ww.humidity.Replace(',', '.')));
                        //    bld.AppendLine(String.Format("<Temperature>{0}</Temperature>", ww.tempC.Replace(',', '.')));
                        //    bld.AppendLine(String.Format("<Windspeed>{0}</Windspeed>", ww.windspeed.Replace(',', '.')));
                        //    bld.AppendLine(String.Format("<Winddirection>{0}</Winddirection>", ww.winddirection.Replace(',', '.')));
                        //    bld.AppendLine(String.Format("<Visibility>{0}</Visibility>", ww.visibility.Replace(',', '.')));
                        //    bld.AppendLine(String.Format("<Pressure>{0}</Pressure>", ww.pressure.Replace(',', '.')));
                        //    bld.AppendLine(String.Format("<Cloudcover>{0}</Cloudcover>", ww.cloudcover.Replace(',', '.')));
                        //}
                        //else
                        //{
                            bld.AppendLine(String.Format(numFormat, "<humidity>{0}</humidity>", item.humidity));
                            bld.AppendLine(String.Format(numFormat, "<temperature>{0}</temperature>", item.temperature));
                            bld.AppendLine(String.Format(numFormat, "<pressure>{0}</pressure>", item.pressure));
                        //}
                        bld.AppendLine(String.Format(numFormat, "<sound>{0}</sound>", item.sound));
                        bld.AppendLine(String.Format(numFormat, "<uv>{0}</uv>", item.uv));
                        bld.AppendLine(String.Format(numFormat, "<xacceleration>{0}</xacceleration>", item.xacceleration));
                        bld.AppendLine(String.Format(numFormat, "<yacceleration>{0}</yacceleration>", item.yacceleration));
                        bld.AppendLine(String.Format(numFormat, "<zacceleration>{0}</zacceleration>", item.zacceleration));

                        bld.AppendLine(String.Format(numFormat, "<xrotation>{0}</xrotation>", item.xrotation));
                        bld.AppendLine(String.Format(numFormat, "<yrotation>{0}</yrotation>", item.yrotation));
                        bld.AppendLine(String.Format(numFormat, "<zrotation>{0}</zrotation>", item.zrotation));

                        bld.AppendLine(String.Format(numFormat, "<xmagneticforce>{0}</xmagneticforce>", item.xmagneticforce));
                        bld.AppendLine(String.Format(numFormat, "<ymagneticforce>{0}</ymagneticforce>", item.ymagneticforce));
                        bld.AppendLine(String.Format(numFormat, "<zmagneticforce>{0}</zmagneticforce>", item.zmagneticforce));

                        bld.AppendLine(String.Format(numFormat, "<accelerationmagnitude>{0}</accelerationmagnitude>", item.accelerationmagnitude));

                        //bld.AppendLine(String.Format("<longitude>{0}</longitude>", item.lon.ToString().Replace(',', '.')));
                        //bld.AppendLine(String.Format("<latitude>{0}</latitude>", item.lat.ToString().Replace(',', '.')));
                        //bld.AppendLine(String.Format("<humidity>{0}</humidity>", item.humidity.ToString().Replace(',', '.')));
                        //bld.AppendLine(String.Format("<temperature>{0}</temperature>", item.temperature.ToString().Replace(',', '.')));
                        bld.AppendLine(String.Format("</Measurement>"));
                    }
                }
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