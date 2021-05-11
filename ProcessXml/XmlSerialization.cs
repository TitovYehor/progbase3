using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace ProcessXml
{
    public static class XmlSerialization
    {
        public static void Serialize<T>(T serializatedObj, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;

            settings.NewLineHandling = NewLineHandling.Entitize;

            XmlWriter writer = XmlWriter.Create(filePath, settings);

            serializer.Serialize(writer, serializatedObj);

            writer.Close();
        }

        public static void Deserialize<T>(ref T deserializedObj, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            StreamReader sr = new StreamReader(filePath);

            deserializedObj = (T)serializer.Deserialize(sr);

            sr.Close();
        }
    }
}