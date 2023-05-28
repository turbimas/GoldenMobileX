using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
// getinnertex'te end text yok ise starttext ten sonrasını vermesi sağlandı,ignorecase removed _return = Regex.Split(text, starttext, RegexOptions.Compiled | RegexOptions.IgnoreCase)[1];
// 1.3.2.4 Encrypt, Decrypt eklendi
//1.3.2.6 Selectfilesfrompath eklendi
//1.3.3.0 Send to Excel, Send to CSV eklendi 
//1.3.3.2 DataTableToHTMLTable eklendi
//1.3.3.4 System Net Mail yerine system.web mail eklendi, attachment eklendi
//1.3.3.5 getinnertext onlyinner düzeltildi
//1.3.3.6 webclient'lere HttpRequestHeader.AcceptEncoding ve HttpRequestHeader.AcceptLanguage türkçeye uyun olarak eklendi.
//1.3.3.7 getTexts eklendi.
//1.3.3.8 getTexts düzenlendi
//1.3.3.9 DataTable getinnertexts eklendi
//1.3.4.0 addHTMLHeaders added CurrentURL added
//1.3.4.1 DirectoryCopy added
/// <summary>
/// Version 1.3.4.0 
/// 
/// </summary>
public class TurbimTools
{

    public static string SaveRemoteFile(string url, string savepath, bool showProgress = false)
    {
        if (savepath.Substring(savepath.Length - 5, 5).IndexOf(".") < 0)
            savepath = savepath + "/" + url.Split(@"/".ToCharArray())[url.Split(@"/".ToCharArray()).Length - 1];
        using (WebClient client = new WebClient())
        {

            client.Encoding = Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.AcceptLanguage, "tr-TR");
            client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            client.DownloadFile(url, savepath);
        }
        return savepath;
    }

    /// <summary>
    /// uses webclient
    /// </summary>
    /// <param name="url"></param>
    /// <param name="savepath"></param>
    /// <returns></returns>
    public static string GetRemotePage(string url, string cookies = "")
    {
        string _return = "";
        using (WebClient client = new WebClient())
        {
            if (cookies != "")
            {

                client.Headers.Add("Cookie", cookies);
            }
            //client.Headers.Add(HttpRequestHeader.AcceptLanguage, "tr-TR");
            //client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");

            //client.Encoding = Encoding.UTF8;  // Sonradan eklendi kodu bozabilir.
            StreamReader sr = new StreamReader(client.OpenRead(url), Encoding.UTF8, true);

            _return = sr.ReadToEnd();
        }
        return _return;
    }

    public static DataTable SelectFilesFromPath(string path)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("FileName");
        dt.Columns.Add("DateCreated");
        foreach (string file in System.IO.Directory.GetFiles(path))
        {
            dt.Rows.Add(file, System.IO.File.GetCreationTime(file));
        }
        return dt;
    }
    public static List<int> AllIndexesOf(string str, string value)
    {
        List<int> indexes = new List<int>();
        if (String.IsNullOrEmpty(value))
            return indexes;
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }


    public static string Readtextfile(string url)
    {
        string _return = "";
        System.IO.StreamReader re = System.IO.File.OpenText(url);
        _return = re.ReadToEnd();
        re.Close();
        re.Dispose();
        return _return;
    }
    /// <summary>
    /// Gets inner text in given text
    /// </summary>
    /// <param name="text"></param>
    /// <param name="starttext"></param>
    /// <param name="endtext"></param>
    /// <param name="onlyinner">if false it contains seperator texts</param>
    /// <returns></returns>
    public static string[] getinnertexts(string text, string starttext, string endtext, bool onlyinner)
    {
        string[] _return = new string[Regex.Split(text, starttext).Length];
        int i = 0;
        foreach (string str in Regex.Split(text, starttext))
        {
            _return[i] = Regex.Split(str, endtext)[0];
            if (onlyinner == false)
                _return[i] = starttext + _return[i] + endtext;
            i++;
        }
        _return[0] = "";
        return _return;
    }
    public static List<string> getinnertextsToList(string text, string starttext, string endtext, bool onlyinner)
    {
        List<string> _return = new List<string>();
        string _r = "";
        foreach (string str in Regex.Split(text, starttext))
        {
            _r = Regex.Split(str, endtext)[0];
            if (onlyinner == false)
                _r = starttext + _r + endtext;
            _return.Add(_r);
        }
        _return.RemoveAt(0);
        return _return;
    }
    public static List<string> getinnertexts(string text, char startchar, char endchar, bool onlyinner, int maxtextlength)
    {
        List<string> _return = new List<string>();

        if (text.Length == 0)
        {
            _return.Add("Text length larger then zero");
            return _return;
        }


        string selecttext = "";
        bool select = false;
        foreach (char c in text.ToCharArray())
        {
            if (c == endchar)
            {
                select = false;
                if (onlyinner)
                    _return.Add(selecttext);
                else
                    _return.Add(startchar + selecttext + endchar);
                selecttext = "";
            }
            if (select == true)
                selecttext += c;
            if (c == startchar)
                select = true;
        }
        return _return;
    }
    public static string getinnertext(string text, string starttext, string endtext, bool onlyinner)
    {
        string _return = "";
        if (text.IndexOf(starttext) > -1)
        {
            _return = Regex.Split(text, starttext, RegexOptions.Compiled)[1];
            if (text.IndexOf(endtext) > -1)
            {
                _return = Regex.Split(_return, endtext, RegexOptions.Compiled)[0];
                if (onlyinner == false)
                    _return = starttext + _return + endtext;
            }

        }

        return _return;
    }
    /// <summary>
    /// Gets text or html with hierarchically between texts in given content. Result contains start and end texts
    /// </summary>
    /// <param name="text"></param>
    /// <param name="starttext"></param>
    /// <param name="endtext"></param>
    /// <returns></returns>
    public static System.Collections.Generic.List<string> getTexts(string text, string starttext, string endtext)
    {
        System.Collections.Generic.List<string> _returnlist = new System.Collections.Generic.List<string>();
        string _return = "";
        string _text = text;
        int innernumber = 0;
        bool ekle = true;
        while (ekle)
        {
            if (_text.IndexOf(starttext) > -1 && _text.IndexOf(endtext) > -1 && _text.IndexOf(starttext) < _text.IndexOf(endtext))
            {
                _return += starttext;
                innernumber++;
                _text = _text.Substring(_text.IndexOf(starttext) + starttext.Length);
            }
            else
            {
                if (innernumber > 1)
                {
                    _return += Regex.Split(_text, endtext, RegexOptions.Compiled)[0] + endtext;
                    _text = _text.Substring(_text.IndexOf(endtext) + endtext.Length);
                    innernumber--;
                }
                if (innernumber == 1)
                {
                    _return += Regex.Split(_text, endtext, RegexOptions.Compiled)[0] + endtext;
                    _text = _text.Substring(_text.IndexOf(endtext) + endtext.Length);
                    _returnlist.Add(_return);
                    _return = "";
                    innernumber--;
                }
            }
            if (innernumber == 0 && (_text.IndexOf(starttext) == -1 || _text.IndexOf(endtext) == -1))
                ekle = false;
            if (_text.Length <= 0)
                ekle = false;
            if (_text.IndexOf(starttext) == -1 && _text.IndexOf(endtext) == -1)
                ekle = false;

        }
        return _returnlist;
    }
    public static string getinnerHTML(string text, string starttext, string endtext, bool onlyinner)
    {
        string _return = "";
        string _text = "";
        int innernumber = 0;
        bool ekle = true;
        if (text.IndexOf(starttext) > -1)
            _text = text.Substring(text.IndexOf(starttext) + starttext.Length);



        while (ekle)
        {
            if (_text.IndexOf(starttext) > -1 && _text.IndexOf(starttext) < _text.IndexOf(endtext))
            {
                innernumber++;
                _return += Regex.Split(_text, starttext, RegexOptions.Compiled)[0] + starttext;
                _text = _text.Substring(_text.IndexOf(starttext) + starttext.Length);
            }
            else
            {
                if (innernumber > 0)
                {
                    _return += Regex.Split(_text, endtext, RegexOptions.Compiled)[0] + endtext;
                    _text = _text.Substring(_text.IndexOf(endtext) + endtext.Length);
                    innernumber--;
                }
                else
                {
                    _return += Regex.Split(_text, endtext, RegexOptions.Compiled)[0];
                    innernumber--;
                }
            }
            if (innernumber < 0)
                ekle = false;
            if (_text.Length <= 0)
                ekle = false;
            if (_text.IndexOf(starttext) == -1 && _text.IndexOf(endtext) == -1)
                ekle = false;
        }


        if (onlyinner == true)
            return _return;
        else
            return starttext + _return + endtext;
    }
    public static string RemoveHTML(string str)
    {
        return Regex.Replace(str, "<.*?>", string.Empty).Trim();
    }

    public static string GetRFC822Date(string dtmDate)
    {
        string _return = dtmDate;
        try
        {
            _return = (Convert.ToDateTime(dtmDate).ToString("ddd, dd MMM yyyy hh:mm:ss") + " GMT");
        }
        catch { }
        return _return;

    }


    public static byte[] UncompressFile(byte[] docData)
    {
        byte[] zipsizData = { };
        MemoryStream zippedStream = new MemoryStream(docData);
        using (ZipArchive archive = new ZipArchive(zippedStream))
        {

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                MemoryStream ms = new MemoryStream();
                Stream zipStream = entry.Open();
                zipStream.CopyTo(ms);
                zipsizData = ms.ToArray();
            }

        }
        return zipsizData;
    }
    public static byte[] CompressFile(byte[] xml, object fileName)
    {


        MemoryStream zipStream = new MemoryStream();

        using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            ZipArchiveEntry zipElaman = zip.CreateEntry(fileName + ".xml");
            Stream entryStream = zipElaman.Open();
            entryStream.Write(xml, 0, xml.Length);
            entryStream.Flush();
            entryStream.Close();
        }
        zipStream.Position = 0;
        return zipStream.ToArray();

    }
    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            if (!File.Exists(temppath))
            {
                file.CopyTo(temppath, false);
            }
            else
            {
                if (overwrite)
                    file.CopyTo(temppath, true);
            }

        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs, overwrite);
            }
        }
    }



    public static DataTable CSVImport(string filepath)
    {
        DataTable dt = new DataTable();
        foreach (string str in Regex.Split(GetRemotePage(filepath), "\n")[0].Split(@";".ToCharArray()))
        {
            dt.Columns.Add(str);
        }
        string[] values = new string[dt.Columns.Count];
        int j = 0;
        foreach (string str in Regex.Split(GetRemotePage(filepath), "\n"))
        {
            int i = 0;
            foreach (string val in str.Split(@";".ToCharArray()))
            {
                values[i] = val;
                i++;
            }
            if (j > 0)
                dt.Rows.Add(values);
            j++;
        }
        return dt;
    }
    enum Dimensions
    {
        Width,
        Height
    }


    public static string Encrypt(string text, string key)
    {
        byte[] Key;
        byte[] IV = new byte[8];
        IV[0] = (byte)0xA9;
        IV[1] = (byte)0x9B;
        IV[2] = (byte)0xC8;
        IV[3] = (byte)0x32;
        IV[4] = (byte)0x56;
        IV[5] = (byte)0x35;
        IV[6] = (byte)0xE3;
        IV[7] = (byte)0x03;
        try
        {
            Key = System.Text.Encoding.UTF8.GetBytes(key.PadLeft(8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] girisDizisi;
            girisDizisi = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(Key, IV), CryptoStreamMode.Write);
            cs.Write(girisDizisi, 0, girisDizisi.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    public static string Decrypt(string text, string key)
    {
        byte[] bkey;
        byte[] IV = new byte[8];
        IV[0] = (byte)0xA9;
        IV[1] = (byte)0x9B;
        IV[2] = (byte)0xC8;
        IV[3] = (byte)0x32;
        IV[4] = (byte)0x56;
        IV[5] = (byte)0x35;
        IV[6] = (byte)0xE3;
        IV[7] = (byte)0x03;
        byte[] girisDizisi = new byte[text.Length];
        try
        {
            bkey = System.Text.Encoding.UTF8.GetBytes(key.PadLeft(8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            girisDizisi = Convert.FromBase64String(text);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(bkey, IV), CryptoStreamMode.Write);
            cs.Write(girisDizisi, 0, girisDizisi.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static string CompressString(string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        MemoryStream memoryStream = new MemoryStream();
        using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gZipStream.Write(buffer, 0, buffer.Length);
        }

        memoryStream.Position = 0;

        byte[] compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        byte[] gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        return Convert.ToBase64String(gZipBuffer);
    }

    /// <summary>
    /// Decompresses the string.
    /// </summary>
    /// <param name="compressedText">The compressed text.</param>
    /// <returns></returns>
    public static string DecompressString(string compressedText)
    {
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        using (MemoryStream memoryStream = new MemoryStream())
        {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

            byte[] buffer = new byte[dataLength];

            memoryStream.Position = 0;
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
        }
    }

    public static void SendToExcel(string filepath, DataTable datatable, int borderwidth)
    {

        System.IO.TextWriter tw = new System.IO.StreamWriter(filepath, false, Encoding.UTF8);
        tw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"application/vnd.ms-excel\">");
        tw.Write("<table border=" + borderwidth + "><tr>");
        foreach (DataColumn dc in datatable.Columns)
        {
            tw.Write("<th>" + dc.Caption + "</th>");
        }
        tw.Write("</tr>");
        foreach (DataRow dr in datatable.Rows)
        {
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                if (i == 0)
                    tw.WriteLine("<tr>");
                tw.Write("<td>" + dr[i] + "</td>");
                if (i == datatable.Columns.Count - 1)
                    tw.WriteLine("</tr>");


            }
        }
        tw.Write("</table>");

        tw.Close();

    }
    public static string DataTableToHTMLTable(DataTable datatable, int borderwidth)
    {

        string _return = ("<table border=" + borderwidth + "><tr>");

        foreach (DataColumn dc in datatable.Columns)
        {
            _return += ("<th>" + dc.Caption + "</th>");
        }
        _return += ("</tr>");
        foreach (DataRow dr in datatable.Rows)
        {
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                if (i == 0)
                    _return += ("<tr>");
                _return += ("<td>" + dr[i] + "</td>");
                if (i == datatable.Columns.Count - 1)
                    _return += ("</tr>");


            }
        }
        _return += ("</table>");

        return _return;

    }
    public static void SendToCSV(string filepath, DataTable datatable)
    {

        System.IO.TextWriter tw = new System.IO.StreamWriter(filepath, false, Encoding.UTF8);
        foreach (DataColumn dc in datatable.Columns)
        {
            tw.Write(dc.Caption + ";");
        }

        foreach (DataRow dr in datatable.Rows)
        {
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                if (i == 0)
                    tw.WriteLine(dr[i] + ";");
                else
                    tw.Write(dr[i] + ";");
            }
        }
        tw.Close();
    }




}

