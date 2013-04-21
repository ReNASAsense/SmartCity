using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCityServer
{
    public partial class PutData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.Request.HttpMethod != "POST")
                {
                    this.Response.Write("Request method must be POST");
                    return;
                }
                
                System.IO.BufferedStream bufStream = new System.IO.BufferedStream(this.Request.InputStream);
                System.IO.StreamReader read = new System.IO.StreamReader(bufStream);

                string postData = read.ReadToEnd();
                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                xmldoc.LoadXml(postData);
                
                NumberFormatInfo numFormat = new NumberFormatInfo();
                numFormat.NumberDecimalSeparator = ".";
                using (SmartCityEntities ctx = new SmartCityEntities())
                {
                    Sample newSample = new Sample();
                    newSample.device_id = Convert.ToInt32(xmldoc["Measurements"]["Measurement"]["deviceId"].InnerText.ToString());
                    newSample.sample_time = DateTime.Now;
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("latitude").Count > 0)
                    {
                        newSample.lat = Convert.ToDecimal(xmldoc["Measurements"]["Measurement"]["latitude"].InnerText.Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("longitude").Count > 0)
                    {
                        newSample.lon = Convert.ToDecimal(xmldoc["Measurements"]["Measurement"]["longitude"].InnerText.Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("temperature").Count > 0)
                    {
                        newSample.temperature = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["temperature"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("humidity").Count > 0)
                    {
                        newSample.humidity = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["humidity"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("pressure").Count > 0)
                    {
                        newSample.pressure = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["pressure"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("sound").Count > 0)
                    {
                        newSample.sound = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["sound"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("winddirection").Count > 0)
                    {
                        newSample.winddirection = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["winddirection"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("uv").Count > 0)
                    {
                        newSample.uv = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["uv"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("xacceleration").Count > 0)
                    {
                        newSample.xacceleration = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["xacceleration"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("yacceleration").Count > 0)
                    {
                        newSample.yacceleration = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["yacceleration"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("zacceleration").Count > 0)
                    {
                        newSample.zacceleration = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["zacceleration"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("xrotation").Count > 0)
                    {
                        newSample.xrotation = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["xrotation"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("yrotation").Count > 0)
                    {
                        newSample.yrotation = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["yrotation"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("zrotation").Count > 0)
                    {
                        newSample.zrotation = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["zrotation"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("xmagneticforce").Count > 0)
                    {
                        newSample.xmagneticforce = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["xmagneticforce"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("ymagneticforce").Count > 0)
                    {
                        newSample.ymagneticforce = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["ymagneticforce"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("zmagneticforce").Count > 0)
                    {
                        newSample.zmagneticforce = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["zmagneticforce"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    if (xmldoc["Measurements"]["Measurement"].GetElementsByTagName("accelerationmagnitude").Count > 0)
                    {
                        newSample.accelerationmagnitude = Convert.ToDouble(xmldoc["Measurements"]["Measurement"]["accelerationmagnitude"].InnerText.ToString().Replace(',', '.'), numFormat);
                    }
                    ctx.Sample.Add(newSample);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.ToString());
                return;
            }
        }
    }
}