using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCityServer
{
    public partial class GetDeviceList : System.Web.UI.Page
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

                System.Text.StringBuilder bld = new System.Text.StringBuilder();
                bld.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");
                bld.Append("<Devices>");
                NumberFormatInfo numFormat = new NumberFormatInfo();
                numFormat.NumberDecimalSeparator = ".";
                using (SmartCityEntities ctx = new SmartCityEntities())
                {
                    List<int?> tt = ctx.Sample.Select(d => d.device_id).Distinct().ToList();
                    foreach (int? item in tt)
                    {
                        if (item.HasValue)
                        {
                            bld.Append(String.Format("<Device>{0}</Device>", item.Value));
                        }
                    }
                }
                bld.AppendLine("</Devices>");
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