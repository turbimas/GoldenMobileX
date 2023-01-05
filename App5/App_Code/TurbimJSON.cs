using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

/// <summary>
/// Summary description for TurbimJSON
/// </summary>
public static class TurbimJSON
{
    public static string serializeObject<T>(T data)
    {
        return JsonConvert.SerializeObject(data);
    }

    public static T deSerializeObject<T>(string URL, object data = null)
    {
        setURL(ref URL);
  
        try
        {
            return JsonConvert.DeserializeObject<T>(GetJObjectFromUrl(URL, data)["data"] + "");
  /*
            return JsonConvert.DeserializeObject<T>(GetJObjectFromUrl(URL,data)["data"] + "", new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
  */
        }
        catch(Exception ex)
        {
            return default(T);
        }
    }
    public static void SaveJSON<T>(this T o)
    {
        File.WriteAllText(o.JsonPath(), JsonConvert.SerializeObject(o), Encoding.UTF8);
      
    }
    public static T Read<T>(this T o)
    {
        T returnObject = (T)Activator.CreateInstance(typeof(T));
        if (string.IsNullOrEmpty(o.JsonPath())) return returnObject;
        if (!File.Exists(o.JsonPath())) return returnObject;

        try
        {
            string json = File.ReadAllText(o.JsonPath(), Encoding.UTF8);

         

            returnObject = JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            appSettings.UyariGoster(ex.Message);
        }
        return returnObject;
    }

    public static  JObject GetJObjectFromUrl(string URL,  object data = null)
    {
        setURL(ref URL);
        string _return = "";
        using (WebClient client = new WebClient())
        {
            client.Headers.Add("content-type", "application/json");
            if ((data + "").Contains("{"))
                _return = client.UploadString(URL, (data + ""));
            else
                _return = client.UploadString(URL, serializeObject(data));
            JsonLoadSettings s = new JsonLoadSettings();


            return JObject.Parse(_return, s);
        }

 
    }
    public static string Post(string URL, object data = null)
    {
        setURL(ref URL);
        string _return = "";
        using (WebClient client = new WebClient())
        {
            client.Headers.Add("content-type", "application/json");
            if ((data + "").Contains("{"))
                _return = client.UploadString(URL, (data + ""));
            else
                _return = client.UploadString(URL, serializeObject(data));
     
            return _return;
        }


    }


    public static  string setURL(ref string URL)
    {
        if (!URL.Contains("http")) URL = TurbimSQLHelper.server +  (URL.EndsWith("/") ? "" : "/") + "api"  + (URL.StartsWith("/") ? "" : "/") + URL;
    
        return URL;
    }


  public  static string JsonPath<T>(this T o)
    {
        var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(basePath, appSettings.CurrentFirm + "_" + appSettings.User.ID+"_" + o.GetType().Name + ".json");
    }
}


