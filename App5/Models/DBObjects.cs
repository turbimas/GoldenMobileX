
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace GoldenMobileX.Models
{
    [NotMapped]
    public class Toplam
    {
        public Nullable<decimal> Value { get; set; }
    }
    public partial class Bilanco
    {
        public string HESAP { get; set; }
        public Nullable<decimal> TUTAR { get; set; }
    }
    public partial class TRN_Files
    {
        public int ID { get; set; }
        public string TableName { get; set; }
        public byte[] File { get; set; }
        public Nullable<int> RecordID { get; set; }
        public Xamarin.Forms.ImageSource img { get { return Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(File)); } }
        public string Name { get; set; }
        public string FileName { get; set; }
        public Nullable<bool> Main { get; set; }
        public string DocType { get; set; }
        public string Keywords { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    }
    public class CRD_Kasalar
    {
        public int ID { get; set; }
        public Nullable<int> FirmaNo { get; set; }
        public string AuthCode { get; set; }
        public string KasaKodu { get; set; }
        public string KasaAciklama { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> SatisKasasi { get; set; }
        public string Baglanti { get; set; }
        public Nullable<System.DateTime> SonSenkronize { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string MuhasebeKodu { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

    }
    public class TRN_KasaHareketleri
    {
        public int ID { get; set; }
        public Nullable<int> KasaFisID { get; set; }
        public Nullable<int> FirmaNo { get; set; }
        public Nullable<int> Branch { get; set; }
        public string DocNumber { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
        public Nullable<System.DateTime> EvrakTarihi { get; set; }
        public Nullable<int> TurKodu { get; set; }
        public Nullable<int> MasrafID { get; set; }
        public Nullable<int> KasaID { get; set; }
        public Nullable<int> CariID
        {
            get
            {
                return (CariID_?.ID).convInt();
            }
            set
            {
                CariID_ = DataLayer.Cariler.Where(x => x.ID == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public CRD_Cari CariID_ { get; set; }
        public string Aciklama { get; set; }
        public string Aciklama2 { get; set; }
        public string Aciklama3 { get; set; }
        public Nullable<double> Tutar { get; set; }
        public Nullable<int> DovizKodu { get; set; }
        public Nullable<double> DovizKuru { get; set; }
        public string OzelKod { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> KontrolEdildi { get; set; }
        public Nullable<System.DateTime> KontrolTarihi { get; set; }
        public Nullable<bool> GoldenSync { get; set; }
        public string AktarimMesaji { get; set; }
        public Nullable<int> ProjeID { get; set; }
        public Nullable<bool> Silindi { get; set; }
        public Nullable<int> AccountFicheID { get; set; }
        public Nullable<int> AuthBy { get; set; }
        public Nullable<int> ZNo { get; set; }
        public Nullable<int> CaseNo { get; set; }
    }



    public class x_Settings
    {
        public int ID { get; set; }
        public string SettingCategory { get; set; }
        public string SettingDesc { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public string SettingType { get; set; }
        public string SettingChoices { get; set; }
        public string SettingGroup { get; set; }
    }
    public class X_Types
    {
        public int ID { get; set; }
        public string TableName { get; set; }
        public string ColumnsName { get; set; }
        public Nullable<int> Code { get; set; }
        public string Name { get; set; }
        public Nullable<int> Direction { get; set; }
        public string MuhasebeKodu { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        [NotMapped]
        public System.Drawing.Color rowColor { get { return DataLayer.statusbackColor(Code.convInt()); } }
    }
    public class X_Users
    {
        public int ID { get; set; }
        public Nullable<int> UpperUserID { get; set; }
        public string LogonName { get; set; }
        public string Password { get; set; }
        public string Branch { get; set; }
        public string FirmNr { get; set; }
        public string NameSirname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Nullable<int> UserLevel { get; set; }
        public string AuthCode { get; set; }
        public string Firms { get; set; }
        public string DefaultFirmBranch { get; set; }
        public Nullable<bool> Aktif { get; set; }
        public Nullable<bool> OnayVerebilir { get; set; }
        public Nullable<int> Departman { get; set; }
        public Nullable<int> Gorevi { get; set; }
        public Nullable<int> AylikKota { get; set; }
        public Nullable<bool> EkIskontoVerebilir { get; set; }
        public string DomainName { get; set; }
        public string Terminals { get; set; }
        public Nullable<int> WareHouseID { get; set; }
    }
    public class L_Units
    {
        public int ID { get; set; }
        public string UnitCode { get; set; }

    }
    public class X_Currency
    {
        public Nullable<int> CurrencyNumber { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
    }

    public class V_OrderLines
    {
        public int ID { get; set; }
        public string Product { get; set; }
        public X_Types Status { get; set; }

        public Nullable<double> Amount { get; set; }
        public string Unit { get; set; }
        public string Cari { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }

    }

    public class CRD_Projects
    {
        public int ID { get; set; }
        public Nullable<int> UpperProjectID { get; set; }
        public string AuthCode { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Baslangic { get; set; }
        public Nullable<System.DateTime> Bitis { get; set; }
        public string Aciklamalar { get; set; }
    }
    public class X_Firms
    {
        public Nullable<int> FirmNr { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string DataBase { get; set; }
    }

    public class X_UserSettings
    {
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string SettingCategory { get; set; }
        public string SettingDesc { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public string SettingType { get; set; }
        public string SettingChoices { get; set; }
    }
    public class offlineData
    {
        public offlineData()
        {
            SQLListToRun = new List<string>();
            JSON_TRN_StockTransListToRun = new List<string>();
            TRN_StockTransLines = new List<TRN_StockTransLines>();
        }
        public List<string> SQLListToRun
        {
            get; set;
        }


        public List<string> JSON_TRN_StockTransListToRun
        {
            get; set;
        }

        public List<TRN_StockTransLines> TRN_StockTransLines
        { get; set; }
    }

    public class localSettings
    {
        List<X_Firms> _Firms;
        public List<X_Firms> Firms { get { if (_Firms == null) _Firms = new List<X_Firms>(); return _Firms; } set { _Firms = value; } }

        List<X_UserSettings> _UserSettings;
        public List<X_UserSettings> UserSettings { get { if (_UserSettings == null) _UserSettings = new List<X_UserSettings>(); return _UserSettings; } set { _UserSettings = value; } }


    }

    public class X_Reports
    {
        public int ID { get; set; }
        public string ReportName { get; set; }
        public string ReportModule { get; set; }
        public byte[] ReportFile { get; set; }

    }
    public class AI_Patterns
    {
        public int ID { get; set; }
        public string Pattern { get; set; }
        public string LastString { get; set; }
        public string XmlInfo { get; set; }
    }
    public class HttpResponse
    {
        public string Message { get; set; }
        public Nullable<int> StatusCode { get; set; }
    }
    public class CredentialInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }

}

