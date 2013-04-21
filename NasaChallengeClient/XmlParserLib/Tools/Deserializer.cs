using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XmlParserLib.Tools
{
    public class Deserializer
    {
        public Deserializer()
        {
        }

        public object GetEntityData(Type entityType, StreamReader reader)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(entityType);
            object obj = serializer.Deserialize(reader);

            return obj;
        }
    }
}
