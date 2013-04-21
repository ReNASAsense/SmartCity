using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms;
using XmlParserLib.Tools;
using XmlParserLib.Entities;
using System.IO;
using System.Diagnostics;

namespace NaSaChallengeApp
{
    public partial class Form1 : Form
    {
        XmlParser parserXml;
        GMapOverlay markersOverlay;
        List<Measurement> Data = new List<Measurement>();

        public Form1()
        {
            InitializeComponent();
            
            try
            {
                System.Net.IPHostEntry e =
                     System.Net.Dns.GetHostEntry("www.google.com");
            }
            catch
            {
                gMapControl1.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection avaible, going to CacheOnly mode.",
                      "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
            }
            
            gMapControl1.MapProvider = GMap.NET.MapProviders.GMapProviders.GoogleMap; //GMapProviders.OpenSeaMapHybrid;
            GMapProviders.GoogleSatelliteMap.APIKey = "AIzaSyCXbHqNPuwAVl6cYiLRPAicAqL8OBs3WqU";
            gMapControl1.Position = new PointLatLng(54.6961334816182, 25.2985095977783);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 9;
            gMapControl1.Manager.Mode = GMap.NET.AccessMode.ServerOnly;
            

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gMapControl1.Click += new EventHandler(gMapControl1_Click);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);

            CreateMarkers();

            parserXml = new XmlParser();

            LoadDevices();

            dataGridView1.DataSource = Data;
        }

        void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
            }
        }

        void gMapControl1_Click(object sender, EventArgs e)
        {
           // pictureBox1.ImageLocation = "http://www.micoequipment.com/products/large/MF_260_l.jpg";
           // pictureBox1.Update();
           // pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            /*
            PointLatLng point = gMapControl1.FromLocalToLatLng((e as MouseEventArgs).X, (e as MouseEventArgs).Y);

            var data = parserXml.Data;

            if (data != null)
            {
                this.dataGridView1.DataSource = data;
            }
             */
        }

        void LoadDevices()
        {
            var devices = parserXml.Devices;

            if (devices != null)
            {
                foreach(string item in devices)
                {
                    checkedListBox1.Items.Add(item);
                }
            }
        }

        List<Measurement> RequestDeviceData(int deviceId, bool onlyLast)
        {
            parserXml.DeviceId = deviceId;
            var deviceData = new List<Measurement>();

            if (onlyLast)
            {
                deviceData = parserXml.DeviceLastData;
            }
            else
            {
                deviceData = parserXml.DeviceData;

                if (deviceData != null)
                {
                    foreach (Measurement item in deviceData)
                    {
                        Data.Add(item);
                    }
                }
            }

            return deviceData;
        }

        void RemoveDataPerDevice(int deviceId)
        {
            List<Measurement> tempList = new List<Measurement>(Data.Where(d => d.deviceId == deviceId.ToString()));

            foreach (Measurement item in tempList)
            {
                Data.Remove(item);
            }
        }
       
        void CreateMarkers()
        {
            markersOverlay = new GMapOverlay(gMapControl1, "markers");
            markersOverlay.IsVisibile = true;
            gMapControl1.Overlays.Add(markersOverlay);

            gMapControl1.ZoomAndCenterMarkers("markers");
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int deviceId = Convert.ToInt32(checkedListBox1.Items[e.Index]);
            if (e.NewValue == CheckState.Unchecked)
            {
                foreach (GMapMarker item in markersOverlay.Markers)
                {
                    if (Convert.ToInt32(item.Tag) == deviceId)
                    {
                        markersOverlay.Markers.Remove(item);
                        break;
                    }
                }

                RemoveDataPerDevice(deviceId);
                //pictureBox1.ImageLocation = parserXml.DeviceImage;
                //pictureBox1.Update();
            }
            if (e.NewValue == CheckState.Checked)
            {
                RequestDeviceData(deviceId, false);
                List<Measurement> last = RequestDeviceData(deviceId, true);
                //String filepath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Images\smile.png";
                //CustomMarker marker = new CustomMarker(new PointLatLng(34.6961334816182, 25.2985095977783), new Bitmap(filepath));
                //marker.Size = new Size(11, 11);
                System.Globalization.NumberFormatInfo nmf = new System.Globalization.NumberFormatInfo();
                nmf.NumberDecimalSeparator = ".";
                if (last.Count > 0)
                {
                    if (Convert.ToDouble(last[0].temprature) > 25.0)
                    {
                        //GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMapMarker(new PointLatLng(Convert.ToDouble(last[0].latitude, nmf), Convert.ToDouble(last[0].longitude, nmf)));

                        
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMapMarkerGoogleRed(new PointLatLng(Convert.ToDouble(last[0].latitude, nmf), Convert.ToDouble(last[0].longitude, nmf)));
                        marker.Tag = deviceId;
                        markersOverlay.Markers.Add(marker);
                    }
                    else
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMapMarkerGoogleGreen(new PointLatLng(Convert.ToDouble(last[0].latitude, nmf), Convert.ToDouble(last[0].longitude, nmf)));
                        marker.Tag = deviceId;
                        markersOverlay.Markers.Add(marker);
                    }
                }
                pictureBox1.ImageLocation = String.Format("http://chemigallego.es:2013/SmartCity/GetImage.aspx?device={0}", deviceId);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Update();
            }

            foreach (GMapMarker item in markersOverlay.Markers)
            {
                gMapControl1.UpdateMarkerLocalPosition(item);
            }

            gMapControl1.ZoomAndCenterMarkers("markers");

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Data;
            dataGridView1.Refresh(); // Make sure this comes first
            dataGridView1.Parent.Refresh(); // Make sure this comes second
        }

        private void checkedListBox1_Click(object sender, EventArgs e)
        {

        }

    }
}
