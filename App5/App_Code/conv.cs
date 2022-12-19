using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Xamarin.Forms;
/// <summary>
/// Summary description for conv
/// </summary>
public class conv
{
    public static int ToInt(object obj)
    {
        if (obj + "" == "")
            return 0;
        try
        {
            return Convert.ToInt32(obj);
        }
        catch
        {
            return 0;
        }
    }
    public static double ToDouble(object obj, int decimalPlaces=-1)
    {
        if (obj+"" == "")
            return 0;
        try
        {
            if (decimalPlaces > -1)
                return Math.Round(Convert.ToDouble(obj.ToString().Replace(".", ",")), decimalPlaces);
            else
                return Convert.ToDouble(obj.ToString().Replace(".", ","));
        }
        catch
        {
            return 0;
        }
    }
    public static float ToFloat(object obj)
    {
        if (obj + "" == "")
            return 0;
        try
        {
            return (float)Convert.ToDouble(obj.ToString().Replace(".", ","));
        }
        catch
        {
            return 0;
        }
    }
    public static string ToString(object obj)
    {
        return obj + "";
    }
    public static DateTime ToDateTime(object obj)
    {
        string tarih = obj + "";
        if (tarih.Split("-".ToCharArray()).Length == 3)
            tarih = tarih.Split("-".ToCharArray())[2] + "." + tarih.Split("-".ToCharArray())[1] + "." + tarih.Split("-".ToCharArray())[0];
        if (obj == null)
            return Convert.ToDateTime("01.01.1900");
        try
        {
            return Convert.ToDateTime(tarih);
        }
        catch
        {
            return Convert.ToDateTime("01.01.1900");
        }
    }
    public static string ToSqlDate(object obj)
    {
        if (obj == null)
            return "1900-01-01";
        try
        {
            return Convert.ToDateTime(obj).ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        catch
        {
            return "1900-01-01";
        }
    }
    public static string ToShortSqlDate(object obj)
    {
        if (obj+"" == "")
            return "1900-01-01";
        try
        {
            return Convert.ToDateTime(obj).ToString("yyyy-MM-dd");
        }
        catch
        {
            return "1900-01-01";
        }
    }
    public static int ToBit(object obj)
    {
        try
        {
            return Convert.ToInt32(obj);
        }
        catch
        {
            return 0;
        }
    }
    public static bool ToBool(object obj)
    {
        try
        {
            return Convert.ToBoolean(obj);
        }
        catch
        {
            return false;
        }
    }


    static readonly string PasswordHash = "P@@Sw0rd";
    static readonly string SaltKey = "S@LT&KEY";
    static readonly string VIKey = "@1B2c3D4e5F6g7H8";

    public static string Encrypt(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
        var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
        var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

        byte[] cipherTextBytes;

        using (var memoryStream = new MemoryStream())
        {
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                cipherTextBytes = memoryStream.ToArray();
                cryptoStream.Close();
            }
            memoryStream.Close();
        }
        return Convert.ToBase64String(cipherTextBytes);
    }
    public static string Decrypt(string encryptedText)
    {
        byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
        byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
        var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

        var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
        var memoryStream = new MemoryStream(cipherTextBytes);
        var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];

        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        memoryStream.Close();
        cryptoStream.Close();
        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
    }

    public static string DecimalToText(decimal _decimal)
    {

        string sTutar = _decimal.ToString("F2").Replace('.', ','); // Replace('.',',') ondalık ayracının . olma durumu için            
        string lira = sTutar.Substring(0, sTutar.IndexOf(',')); //tutarın tam kısmı
        string kurus = sTutar.Substring(sTutar.IndexOf(',') + 1, 2);
        string yazi = "";

        string[] birler = { "", "BİR", "İKİ", "ÜÇ", "DÖRT", "BEŞ", "ALTI", "YEDİ", "SEKİZ", "DOKUZ" };
        string[] onlar = { "", "ON", "YİRMİ", "OTUZ", "KIRK", "ELLİ", "ALTMIŞ", "YETMİŞ", "SEKSEN", "DOKSAN" };
        string[] binler = { "KATRİLYON", "TRİLYON", "MİLYAR", "MİLYON", "BİN", "" }; //KATRİLYON'un önüne ekleme yapılarak artırabilir.

        int grupSayisi = 6; //sayıdaki 3'lü grup sayısı. katrilyon içi 6. (1.234,00 daki grup sayısı 2'dir.)
        //KATRİLYON'un başına ekleyeceğiniz her değer için grup sayısını artırınız.

        lira = lira.PadLeft(grupSayisi * 3, '0'); //sayının soluna '0' eklenerek sayı 'grup sayısı x 3' basakmaklı yapılıyor.            

        string grupDegeri;

        for (int i = 0; i < grupSayisi * 3; i += 3) //sayı 3'erli gruplar halinde ele alınıyor.
        {
            grupDegeri = "";

            if (lira.Substring(i, 1) != "0")
                grupDegeri += birler[Convert.ToInt32(lira.Substring(i, 1))] + "YÜZ"; //yüzler                

            if (grupDegeri == "BİRYÜZ") //biryüz düzeltiliyor.
                grupDegeri = "YÜZ";

            grupDegeri += onlar[Convert.ToInt32(lira.Substring(i + 1, 1))]; //onlar

            grupDegeri += birler[Convert.ToInt32(lira.Substring(i + 2, 1))]; //birler                

            if (grupDegeri != "") //binler
                grupDegeri += binler[i / 3];

            if (grupDegeri == "BİRBİN") //birbin düzeltiliyor.
                grupDegeri = "BİN";

            yazi += grupDegeri;
        }


        return yazi;
    }
}

public static class convExtensions
{
    public static string DecimalSeparator
    {
        get
        {
            return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        }

    }

    public static string GroupSeparator
    {
        get
        {
            return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
        }
    }
    public static string convSQLNumber(this object obj)
    {
        if (obj + "" == "")
            return "0";
        try
        {
            return (obj + "").Replace(",", ".");
        }
        catch
        {
            return "0";
        }
    }
    public static string convSQLString(this object obj)
    {

        try
        {
            return " '"+obj + "' ";
        }
        catch
        {
            return "";
        }
    }
    public static int DecimalDigits(this decimal n)
    {
        return n.ToString(System.Globalization.CultureInfo.InvariantCulture)
                .SkipWhile(c => c != '.')
                .Skip(1)
                .Count();
    }

    public static bool IsDeletion(this TextChangedEventArgs e)
    {
        return !string.IsNullOrEmpty(e.OldTextValue) && e.OldTextValue.Length > e.NewTextValue.Length;
    }

    public static int convInt(this object obj)
    {
        if (obj+"" == "")
            return 0;
        try
        {
            return Convert.ToInt32(obj);
        }
        catch
        {
            return 0;
        }
    }
    public static Int16 convInt16(this object obj)
    {
        if (obj + "" == "")
            return 0;
        try
        {
            return Convert.ToInt16(obj);
        }
        catch
        {
            return 0;
        }
    }
    public static double convDouble(this object obj, int decimalPlaces = -1)
    {

        if (obj + "" == "")
            return 0;
        try
        {
            if (decimalPlaces > -1)
                return Convert.ToDouble(Convert.ToDouble(obj).ToString("N" + decimalPlaces));
            else
                return Convert.ToDouble(obj.ToString().Replace(GroupSeparator, DecimalSeparator));
        }
        catch
        {
            return 0;
        }
    }
 
    public static decimal convDecimal(this object obj, int decimalplace = 4)
    {
        return Convert.ToDecimal(obj.convDouble().ToString("F" + decimalplace));
    }
    public static string convString(this object obj)
    {
        return obj + "";
    }
    public static DateTime convDateTime(this object obj)
    {
        string tarih = obj + "";
        if (tarih.Split("-".ToCharArray()).Length == 3)
            tarih = tarih.Split("-".ToCharArray())[2] + "." + tarih.Split("-".ToCharArray())[1] + "." + tarih.Split("-".ToCharArray())[0];
        if (obj == null)
            return Convert.ToDateTime("01.01.1900");
        try
        {
            return Convert.ToDateTime(tarih);
        }
        catch
        {
            return Convert.ToDateTime("01.01.1900");
        }
    }

    public static TimeSpan convTime(this object obj)
    {
        string tarih = obj + "";

        if (obj == null)
            return Convert.ToDateTime("01.01.1900").TimeOfDay;
        try
        {
            return DateTime.ParseExact(obj + "", "HH:mm:ss",
                                        System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;
        }
        catch
        {
            return Convert.ToDateTime("01.01.1900").TimeOfDay;
        }
    }

    public static string convSqlDate(this object obj)
    {
        if (obj + "" == "")
            return "1900-01-01";
        try
        {
            return Convert.ToDateTime(obj).ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        catch
        {
            return "1900-01-01";
        }
    }
    public static string convShortSqlDate(this object obj)
    {
        if (obj + "" == "")
            return "1900-01-01";
        try
        {
            return Convert.ToDateTime(obj).ToString("yyyy-MM-dd");
        }
        catch
        {
            return "1900-01-01";
        }
    }
    public static int convBit(this object obj)
    {
        try
        {
            return Convert.ToInt32(obj);
        }
        catch
        {
            return 0;
        }
    }
    public static Guid convGuid(this string obj)
    {
        try
        {
            return Guid.Parse(obj);
        }
        catch
        {
            return new Guid();
        }
    }
    public static bool convBool(this object obj)
    {
        if (obj + "" == "")
            return false;
        try
        {
            if ((obj + "").ToLower() == "true" || (obj + "") == "1")
                return true;
            return Convert.ToBoolean(obj);
        }
        catch
        {
            return false;
        }
    }
    public static Stream convStream(this object obj)
    {
        try
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(obj + "");
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        catch
        {
            return new MemoryStream();
        }
    }
    public static byte[] convStreamToByteArray(this Stream input)
    {
        input.Position = 0;
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}

