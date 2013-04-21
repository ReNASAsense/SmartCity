using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

namespace SmartCityServer
{
    public class ImageCompute
    {
        //static System.Drawing.Bitmap Computed(string xmlDat, HttpContext context)
        //{
        //    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
        //    xmlDocument.LoadXml(xmlDat);
        //    double temp = Convert.ToDouble(xmlDocument["Measurements"]["Measurement"]["temperature"].InnerText);
        //    System.Drawing.Image tempHot = System.Drawing.Image.FromFile(context.
        //    using (Bitmap newBitmap = new System.Drawing.Bitmap(64,64))
        //    {
        //        using (Graphics compositeGraphics = Graphics.FromImage(newBitmap))
        //        {
        //            compositeGraphics.CompositingMode = CompositingMode.SourceCopy;
        //            compositeGraphics.DrawImageUnscaled(frontImage, 0, 0);
        //            compositeGraphics.DrawImageUnscaled(backImage, 0, frontImage.Height);
        //            compositeImage.Save(context.Response.OutputStream, ImageFormat.Jpeg);
        //        }
        //    }
        //    return newBitmap;
        //}
    }
}