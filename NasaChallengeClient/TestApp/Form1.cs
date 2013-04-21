using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using XmlParserLib.Tools;
using System.IO;
using XmlParserLib.Entities;

namespace TestApp
{
    public partial class Form1 : Form
    {
        static string path = @"C:\Users\GPapamath\Desktop\NaSaEntities\";
        static string httpPath = "http://chemigallego.es:2013/SmartCity/GetData.aspx";
        //////////////////////////////////////////////////////////
        //"http://localhost:1265/XmlFiles/NaSaChallengeData.xml"
        //////////////////////////////////////////////////////////
        static string []entities = {"Measurements", "Measurement"};

        enum entityType
        {
            Measurements,
            Measurement
        }

        public Form1()
        {
            InitializeComponent();

            //var list = createEntityListInFolder(entityType.Measurements);
            var list = createEntityListByHttpRequest(entityType.Measurements);
        }

        static IList createEntityListByHttpRequest(entityType type)
        {
            StreamReader reader = Toolkit.CreateHttpRequest(httpPath);
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


        static IList createEntityListInFolder(entityType type)
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
