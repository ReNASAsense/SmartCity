using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCityServer
{
    public partial class PutImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //if (this.Request.HttpMethod != "POST")
                //{
                //    this.Response.Write("Request method must be POST");
                //    return;
                //}
                // handle as image
                //string device = this.Request.Form["deviceid"].ToString();
                using (SmartCityEntities ctx = new SmartCityEntities())
                {
                    SampledImage sImage = new SampledImage();
                    sImage.device_id = Convert.ToInt32(this.Request.QueryString["device"]);
                    sImage.image_timestamp = DateTime.Now;
                    System.IO.BufferedStream bufRead = new System.IO.BufferedStream(this.Request.Files[0].InputStream);
                    System.IO.BinaryReader binaryRead = new System.IO.BinaryReader(bufRead);
                    sImage.imagesample = binaryRead.ReadBytes(this.Request.Files[0].ContentLength);
                    ctx.SampledImage.Add(sImage);
                    ctx.SaveChanges();
                }
                return;
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.ToString());
                return;
            }
        }
    }
}