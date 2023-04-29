using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GoldenMobileX.Models
{
    public class CRD_Bankalar
    {
        public CRD_Bankalar()
        {
            Bakiye = new Toplam();
        }
        public int ID { get; set; }
        public string AuthCode { get; set; }
        public string BankaKodu { get; set; }
        public string BankaAdi { get; set; }
        public Nullable<bool> Active { get; set; }
        public string Aciklamalar { get; set; }
        public string SwiftCode { get; set; }
        public Nullable<int> Branch { get; set; }
        public byte[] Layout { get; set; }
        public Nullable<int> CaseNo { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [NotMapped]
        public Toplam Bakiye { get; set; }
    }
    public class CRD_BankaHesaplari
    {
        public int ID { get; set; }
        public string AuthCode { get; set; }
        public Nullable<int> BankaID { get; set; }
        public string HesapNo { get; set; }
        public string MuhasebeKodu { get; set; }
        public string IBAN { get; set; }
        public Nullable<int> Turu
        {
            get
            {
                return Turu_.Code;
            }
            set
            {
                Turu_ = DataLayer.x_types_bankahesaplari.Where(x => x.Code == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Types Turu_ { get; set; }
        public Nullable<int> CekSenetTahsilHesapID { get; set; }
        public string HesapAdi { get; set; }
        public string Adres { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> TaksitSayisi { get; set; }
        public Nullable<System.DateTime> IlkTaksitOdemeTarihi { get; set; }
        public Nullable<double> BalonOdemeTaksitTutari { get; set; }
        public Nullable<double> KrediTutari { get; set; }
        public Nullable<double> BSMV { get; set; }
        public Nullable<double> KKDF { get; set; }
        public Nullable<double> FaizOrani { get; set; }
        public Nullable<double> Masraf { get; set; }
        public Nullable<double> ToplamOdenecekTutar { get; set; }

        public Nullable<int> CurrencyID
        {
            get
            {
                return (CurrencyID_?.CurrencyNumber).convInt();
            }
            set
            {
                CurrencyID_ = DataLayer.X_Currency.Where(x => x.CurrencyNumber == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Currency CurrencyID_ { get; set; }
        [NotMapped]
        public Nullable<decimal> HesapBakiye { get { return (CurrencyID == 0 ? Bakiye : DovizBakiye); } }
        public Nullable<decimal> Bakiye { get; set; }
        public Nullable<decimal> DovizBakiye { get; set; }
        public Nullable<System.DateTime> SonIslemTarihi { get; set; }
        public Nullable<double> KrediKartiLimit { get; set; }
        public Nullable<System.DateTime> KrediKartiHesapKesim { get; set; }
        public Nullable<System.DateTime> KrediKartiSonOdeme { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> Branch { get; set; }
        public Nullable<int> CaseNo { get; set; }
    }

    public class TRN_BankaHareketleri
    {

        public int ID { get; set; }
        public Nullable<int> Branch { get; set; }
        public string DocNumber { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
        public Nullable<int> TurKodu
        {
            get
            {
                return (TurKodu_?.Code).convInt();
            }
            set
            {
                TurKodu_ = DataLayer.x_types_BankaIslem.Where(x => x.Code == value)?.FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Types TurKodu_ { get; set; }
        public Nullable<int> FisID { get; set; }
        public Nullable<int> BankaID { get; set; }
        public Nullable<int> BankaHesapID
        {
            get
            {
                return (BankaHesapID_?.ID).convInt();
            }
            set
            {
                BankaHesapID_ = DataLayer.CRD_BankaHesaplari.Where(x => x.ID == value)?.FirstOrDefault();
            }
        }
        [NotMapped]
        public CRD_BankaHesaplari BankaHesapID_ { get; set; }
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
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> KarsiBankaID { get; set; }
        [NotMapped]
        public CRD_BankaHesaplari KarsiBankaHesapID_ { get; set; }
        public Nullable<int> KarsiBankaHesapID {
            get
            {
                return (KarsiBankaHesapID_?.ID).convInt();
            }
            set
            {
                KarsiBankaHesapID_ = DataLayer.CRD_BankaHesaplari.Where(x => x.ID == value)?.FirstOrDefault();
            }
        }
        public Nullable<int> KarsiKasaID { get; set; }
        public Nullable<int> MasrafID { get; set; }
        public string Aciklama { get; set; }
        public Nullable<decimal> Tutar { get; set; }
   
        public Nullable<int> DovizKodu { get; set; }
        public Nullable<double> DovizKuru { get; set; }
        public string OzelKod { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

    }
    public partial class TRN_DailyExchange
    {
        public int ID { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<double> Rate1 { get; set; }
        public Nullable<double> Rate2 { get; set; }
        public Nullable<double> Rate3 { get; set; }
        public Nullable<double> Rate4 { get; set; }
    }
}
