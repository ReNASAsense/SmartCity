using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCityServer
{
    public partial class GetImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.Request.HttpMethod != "GET")
                {
                    this.Response.Write("Request method must be GET");
                    return;
                }
                this.Response.ContentType = "multipart/form-data";
                using (SmartCityEntities ctx = new SmartCityEntities())
                {
                    int deviceid = Convert.ToInt32(this.Request.QueryString["device"].ToString());

                    SampledImage sImage = ctx.SampledImage.OrderByDescending(pimg => pimg.id_images).FirstOrDefault(devid => devid.device_id == deviceid);                   
                    if(sImage != null)
                    {
                        this.Response.ContentType = "image/jpg";
                        this.Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.jpg", deviceid));
                        this.Response.OutputStream.Write(sImage.imagesample, 0, sImage.imagesample.Count());
                        return;
                    }
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