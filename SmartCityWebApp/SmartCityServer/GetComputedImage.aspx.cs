using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCityServer
{
    public partial class GetComputedImage : System.Web.UI.Page
    {
        System.Drawing.Bitmap Computed(string xmlDat)
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.LoadXml(xmlDat);
            double temp = Convert.ToDouble(xmlDocument["Measurements"]["Measurement"]["temperature"].InnerText);
            System.Drawing.Image tempHot = System.Drawing.Image.FromFile(MapPath("images/temphot.png"));
            System.Drawing.Image tempCold = System.Drawing.Image.FromFile(MapPath("images/termcold.png"));
            using (Bitmap newBitmap = new System.Drawing.Bitmap(640,640))
            {
                using (Graphics compositeGraphics = Graphics.FromImage(newBitmap))
                {
                    compositeGraphics.CompositingMode = CompositingMode.SourceCopy;
                    compositeGraphics.DrawImage(tempHot, 0, 0);
                    compositeGraphics.DrawImage(tempCold, 0, tempHot.Height);
                }
                return newBitmap;
            }
            return null;
        }
        public byte[] ImageToByte2(System.Drawing.Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder bld = new System.Text.StringBuilder();
                bld.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");
                bld.AppendLine("<Measurements>");
                NumberFormatInfo numFormat = new NumberFormatInfo();
                numFormat.NumberDecimalSeparator = ".";
                int device = 0;
                using (SmartCityEntities ctx = new SmartCityEntities())
                {

                    List<Sample> samples = ctx.Sample.ToList();
                    if (this.Request.QueryString.AllKeys.Contains("device"))
                    {
                        device = Convert.ToInt32(this.Request.QueryString["device"]);
                        samples = samples.Where(dev => dev.device_id == device).ToList();
                    }
                    samples = samples.OrderBy(dev => dev.id_measurement).Take(1).ToList();
                    foreach (var item in samples)
                    {
                        bld.AppendLine("<Measurement>");
                        bld.AppendLine(String.Format("<id>{0}</id>", item.id_measurement));
                        bld.AppendLine(String.Format("<deviceId>{0}</deviceId>", item.device_id));
                        bld.AppendLine(String.Format("<timestamp>{0}</timestamp>", item.sample_time));
                        bld.AppendLine(String.Format(numFormat, "<longitude>{0}</longitude>", item.lon));
                        bld.AppendLine(String.Format(numFormat, "<latitude>{0}</latitude>", item.lat));
                        bld.AppendLine(String.Format(numFormat, "<humidity>{0}</humidity>", item.humidity));
                        bld.AppendLine(String.Format(numFormat, "<temperature>{0}</temperature>", item.temperature));

                        bld.AppendLine(String.Format(numFormat, "<pressure>{0}</pressure>", item.pressure));
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

                        bld.AppendLine(String.Format("</Measurement>"));
                    }
                }
                bld.AppendLine("</Measurements>");
                string xmlDat = bld.ToString();
                System.Drawing.Image imgGo = Computed(xmlDat);
                byte[] imgArr = ImageToByte2(imgGo);
                this.Response.ContentType = "image/png";
                this.Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.png", device));
                this.Response.OutputStream.Write(imgArr, 0, imgArr.Count());
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