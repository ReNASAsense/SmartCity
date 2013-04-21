using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlParserLib.Entities
{
    [Serializable()]
    [XmlRoot("Measurements")]
    public class Measurements
    {
        [XmlElement(ElementName = "Measurement")]
        public List<Measurement> measurements { get; set; }
    }
}
