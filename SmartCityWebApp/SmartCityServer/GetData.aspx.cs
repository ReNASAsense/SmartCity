using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCityServer
{
    public partial class GetData : System.Web.UI.Page
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
                using (SmartCityEntities ctx = new SmartCityEntities())
                {
                    List<Sample> samples = ctx.Sample.ToList();
                    if (this.Request.QueryString.AllKeys.Contains("device"))
                    {
                        int device = Convert.ToInt32(this.Request.QueryString["device"]);
                        samples = samples.Where(dev => dev.device_id == device).ToList();
                    }
                    foreach (var item in samples)
                    {
                        bld.AppendLine("<Measurement>");
                        bld.AppendLine(String.Format("<id>{0}</id>", item.id_measurement));
                        bld.AppendLine(String.Format("<deviceId>{0}</deviceId>", item.device_id));
                        bld.AppendLine(String.Format("<timestamp>{0}</timestamp>", item.sample_time));
                        bld.AppendLine(String.Format("<longitude>{0}</longitude>", item.lon.ToString().Replace(',','.')));
                        bld.AppendLine(String.Format("<latitude>{0}</latitude>", item.lat.ToString().Replace(',', '.')));
                        bld.AppendLine(String.Format("<humidity>{0}</humidity>", item.humidity.ToString().Replace(',', '.')));
                        bld.AppendLine(String.Format("<temperature>{0}</temperature>", item.temperature.ToString().Replace(',', '.')));
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