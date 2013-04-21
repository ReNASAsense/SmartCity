using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using XmlParserLib.Entities;
using GMap.NET;

namespace XmlParserLib.Tools
{
    public class XmlParser
    {
        static string path = @"C:\Users\GPapamath\Desktop\NaSaEntities\";
        //////////////////////////////////////////////////////////
        //"http://localhost:1265/XmlFiles/NaSaChallengeData.xml"
        //////////////////////////////////////////////////////////
        static string httpPathLocation = "http://chemigallego.es:2013/SmartCity/GetData.aspx?lat={0}&lon={1}";
        static string httpPathDeviceData = "http://chemigallego.es:2013/SmartCity/GetData.aspx?device={0}&onlylast={1}";
        static string httpPathDevices = "http://chemigallego.es:2013/SmartCity/GetDeviceList.aspx";
        static string httpPathDeviceImage = "http://chemigallego.es:2013/SmartCity/GetImage.aspx?device={0}";
        //static string[] entities = { "Measurements", "Measurement" };

        enum entityType
        {
            Devices,
            Device,
            Measurements,
            Measurement
        }

        public List<Measurement> Data
        {
            get { return (List<Measurement>)createEntityListByHttpRequest(entityType.Measurements, Point); }
        }

        public List<Measurement> DeviceData
        {
            get { return (List<Measurement>)createEntityListByHttpRequest(entityType.Measurements, DeviceId, false); }
        }

        public List<Measurement> DeviceLastData
        {
            get { return (List<Measurement>)createEntityListByHttpRequest(entityType.Measurements, DeviceId, true); }
        }

        public List<string> Devices
        {
            get { return (List<string>)createEntityListByHttpRequest(entityType.Devices, Point); }
        }

        public string DeviceImage
        {
            get { return createEntityImageByHttpRequest(DeviceId); }
        }

        public PointLatLng Point { get; set; }

        public int DeviceId { get; set; }

        public XmlParser()
        {
        }

        string createEntityImageByHttpRequest(int deviceId)
        {
            string path = String.Format(httpPathDeviceImage, deviceId);

            StreamReader reader = Toolkit.CreateHttpRequest(path);
            string link = reader.ReadLine();
            reader.Close();

            return link;
        }

        IList createEntityListByHttpRequest(entityType type, int deviceId, bool onlyLast)
        {
            string path = String.Format(httpPathDeviceData, deviceId, onlyLast.ToString().ToLower());

            StreamReader reader = Toolkit.CreateHttpRequest(path);
            Deserializer dsr = new Deserializer();

            IList list = null;
            Type tp = null;

            switch (type)
            {
                case entityType.Measurement:
                    tp = typeof(Measurement);
                    list = new List<Measurement>();
                    break;
                case entityType.Measurements:
                    tp = typeof(Measurements);
                    list = new List<Measurement>();
                    break;
            }

            object obj = dsr.GetEntityData(tp, reader);
            Toolkit.CloseConnection();

            switch (type)
            {
                case entityType.Measurement:
                    list.Add(obj);
                    break;
                case entityType.Measurements:
                    list = (obj as Measurements).measurements;
                    break;
            }

            return list;
        }

        IList createEntityListByHttpRequest(entityType type, PointLatLng point)
        {
            string path;

            if (type == entityType.Devices || type == entityType.Device)
                path = httpPathDevices;
            else
                path = String.Format(httpPathLocation, point.Lat.ToString(), point.Lng.ToString());

            StreamReader reader = Toolkit.CreateHttpRequest(path);
            Deserializer dsr = new Deserializer();

            IList list = null;
            Type tp = null;

            switch (type)
            {
                case entityType.Devices:
                    tp = typeof(Devices);
                    list = new List<string>();
                    break;
                case entityType.Measurement:
                    tp = typeof(Measurement);
                    list = new List<Measurement>();
                    break;
                case entityType.Measurements:
                    tp = typeof(Measurements);
                    list = new List<Measurement>();
                    break;
            }

            object obj = dsr.GetEntityData(tp, reader);
            Toolkit.CloseConnection();

            switch (type)
            {
                case entityType.Device:
                    list.Add(obj);
                    break;
                case entityType.Devices:
                    list = (obj as Devices).devices;
                    break;
                case entityType.Measurement:
                    list.Add(obj);
                    break;
                case entityType.Measurements:
                    list = (obj as Measurements).measurements;
                    break;
            }

            return list;
        }


        IList createEntityListInFolder(entityType type)
        {
            FolderReader folder = new FolderReader(path);
            StreamReader reader;
            Deserializer dsr = new Deserializer();

            IList list = null;
            Type tp = null;
            
            switch (type)
            {
                case entityType.Measurement:
                    tp = typeof(Measurement);
                    list = new List<Measurement>();
                    break;
                case entityType.Measurements:
                    tp = typeof(Measurements);
                    list = new List<Measurement>();
                    break;
            }
            
            foreach (string file in folder.Files)
            {
                reader = new StreamReader(file);
                object obj = dsr.GetEntityData(tp, reader);
                reader.Close();

                switch (type)
                {
                    case entityType.Measurement:
                        list.Add(obj);
                        break;
                    case entityType.Measurements:
                        list = (obj as Measurements).measurements;
                        break;
                }
            }

            return list;
        }
    }

}
