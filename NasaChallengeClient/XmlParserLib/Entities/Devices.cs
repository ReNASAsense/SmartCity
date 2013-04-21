using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlParserLib.Entities
{
    [Serializable()]
    [XmlRoot("Devices")]
    public class Devices
    {
        [XmlElement(ElementName = "Device")]
        public List<string> devices { get; set; }
    }
}
