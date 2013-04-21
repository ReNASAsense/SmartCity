using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace XmlParserLib.Tools
{
    public static class Toolkit
    {
        static StreamReader reader;
        static Stream dataStream;
        static HttpWebResponse response;

        public static StreamReader CreateHttpRequest(string path)
        {
            WebRequest request = WebRequest.Create(path);
            response = (HttpWebResponse)request.GetResponse();

            dataStream = response.GetResponseStream();
            reader = new StreamReader(dataStream);
            // string responseFromServer = reader.ReadToEnd();

            return reader;
        }

        public static void CloseConnection()
        {
            reader.Close();
            dataStream.Close();
            response.Close();
        }

    }
}
