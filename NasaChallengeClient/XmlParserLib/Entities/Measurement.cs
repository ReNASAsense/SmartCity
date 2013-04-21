using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlParserLib.Entities
{
    [Serializable()]
    public class Measurement
    {
        [System.Xml.Serialization.XmlElement("id")]
        public string id { get; set; }

        [System.Xml.Serialization.XmlElement("deviceId")]
        public string deviceId { get; set; }

        [System.Xml.Serialization.XmlElement("timestamp")]
        public string timestamp { get; set; }

        [System.Xml.Serialization.XmlElement("longitude")]
        public string longitude { get; set; }

        [System.Xml.Serialization.XmlElement("latitude")]
        public string latitude { get; set; }

        [System.Xml.Serialization.XmlElement("humidity")]
        public string humidity { get; set; }

        [System.Xml.Serialization.XmlElement("temperature")]
        public string temprature { get; set; }

        ///////////////new values//////////////////////////
        [System.Xml.Serialization.XmlElement("pressure")]
        public string pressure { get; set; }

        [System.Xml.Serialization.XmlElement("sound")]
        public string sound { get; set; }

        [System.Xml.Serialization.XmlElement("uv")]
        public string uv { get; set; }

        [System.Xml.Serialization.XmlElement("xacceleration")]
        public string xacceleration { get; set; }

        [System.Xml.Serialization.XmlElement("yacceleration")]
        public string yacceleration { get; set; }

        [System.Xml.Serialization.XmlElement("zacceleration")]
        public string zacceleration { get; set; }

        [System.Xml.Serialization.XmlElement("xrotation")]
        public string xrotation { get; set; }

        [System.Xml.Serialization.XmlElement("yrotation")]
        public string yrotation { get; set; }

        [System.Xml.Serialization.XmlElement("zrotation")]
        public string zrotation { get; set; }

        [System.Xml.Serialization.XmlElement("xmagneticforce")]
        public string xmagneticforce { get; set; }

        [System.Xml.Serialization.XmlElement("ymagneticforce")]
        public string ymagneticforce { get; set; }

        [System.Xml.Serialization.XmlElement("zmagneticforce")]
        public string zmagneticforce { get; set; }

        [System.Xml.Serialization.XmlElement("accelerationmagnitude")]
        public string accelerationmagnitude { get; set; }
    }
}
