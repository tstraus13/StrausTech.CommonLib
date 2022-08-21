using System.Xml;
using System.Xml.Serialization;


namespace CommonLib;

/// <summary>
/// A class for manipuling Xml data
/// </summary>
public static class Xml
{
    /// <summary>
    /// Take an object and convert it to Xml
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object to convert to Xml</param>
    /// <returns>An Xml document</returns>
    public static XmlDocument SerializeObjectToXml<T>(T obj)
    {
        XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
        XmlDocument doc = new XmlDocument();

        StringWriter sww = new StringWriter();
        XmlWriter writer = XmlWriter.Create(sww);
        xsSubmit.Serialize(writer, obj);
        doc.LoadXml(sww.ToString());
        return doc;
    }

    /// <summary>
    /// Save object to Xml file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object to save</param>
    /// <param name="filePath">The path to save the Xml file</param>
    public static void SerializeObjectToXmlFile<T>(T obj, string filePath)
    {
        var doc = SerializeObjectToXml(obj);
        doc.Save(filePath);
    }

    /// <summary>
    /// Take an Xml file and convert it to an object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath">The path to the Xml file</param>
    /// <returns>The object that was in the Xml file</returns>
    public static T? DeserializeXMLFileToObject<T>(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        return (T?)serializer.Deserialize(new XmlTextReader(filePath));
    }
}