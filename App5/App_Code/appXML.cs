using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public static class appXML
{
    public static void SaveXML<T>(this T o)
    {
        o.Serialize().Save(o.XmlPath());
    }
    public static T ReadXML<T>(this T o)
    {
        return DeserializeXMLFileToObject<T>(o.XmlPath());
    }
    static XmlDocument Serialize<T>(this T o)
    {
        XmlDocument xml = null;

        XmlSerializer serializerObj = new XmlSerializer(typeof(T));
        using (MemoryStream ms = new MemoryStream())
        {
            serializerObj.Serialize(ms, o);

            ms.Position = 0;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            using (var xtr = XmlReader.Create(ms, settings))
            {
                xml = new XmlDocument();
                xml.Load(xtr);
            }
        }



        return xml;
    }

    static T DeserializeXMLFileToObject<T>(string XmlFilename)
    {
        T returnObject = (T)Activator.CreateInstance(typeof(T));
        if (string.IsNullOrEmpty(XmlFilename)) return returnObject;
        if (!File.Exists(XmlFilename)) return returnObject;

        try
        {
            byte[] b = File.ReadAllBytes(XmlFilename);
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            string xml = b.ByteToString();
            xml = removeXMLEmptyChar(xml);
            returnObject = (T)serializer.Deserialize(xml.convStream());
        }
        catch (Exception ex)
        {
            appSettings.UyariGoster(ex.Message);
        }
        return returnObject;
    }
    static string ByteToString(this byte[] obj)
    {
        return Encoding.UTF8.GetString(obj);
    }


    static string XmlPath<T>(this T o)
    {
        var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(basePath, o.GetType().Name + ".xml");
    }
    public static string removeXMLEmptyChar(string xml)
    {
        if (xml.Substring(0, 1) + "" != "<")
        {
            xml = xml.Substring(1, xml.Length - 1);
            removeXMLEmptyChar(xml);
        }
        return xml;
    }

}

