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
                    newSample.device_id = 0;
                    newSample.sample_time = DateTime.Now;
                    newSample.lat = Convert.ToDecimal(xmldoc["Measurements"]["Measurement"]["latitude"].InnerText.ToString());
                    newSample.lon = Convert.ToDecimal(xmldoc["Measurements"]["Measurement"]["longitude"].InnerText.ToString());
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