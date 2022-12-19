using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

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
                CurrencyID_= DataLayer.X_Currency.Where(x => x.CurrencyNumber == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Currency CurrencyID_ { get; set; }
        public Nullable<double> Bakiye { get; set; }
        public Nullable<double> DovizBakiye { get; set; }
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
        public TRN_BankaHareketleri()
        {
            CariID_ = new CRD_Cari();
            TurKodu_ = new X_Types();

        }
        public int ID { get; set; }
        public Nullable<int> FirmaNo { get; set; }
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
                TurKodu_ = DataLayer.x_types_bankahesaplari.Where(x => x.Code == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Types TurKodu_ { get; set; }
        public Nullable<int> FisID { get; set; }
        public Nullable<int> BankaID { get; set; }
        public Nullable<int> BankaHesapID { get; set; }
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
        public Nullable<int> KarsiBankaHesapID { get; set; }
        public Nullable<int> KarsiKasaID { get; set; }
        public Nullable<int> MasrafID { get; set; }
        public string Aciklama { get; set; }
        public Nullable<double> Tutar { get; set; }
        public Nullable<int> TaksitNo { get; set; }
        public Nullable<double> OdenenAnaPara { get; set; }
        public Nullable<double> OdenenFaiz { get; set; }
        public Nullable<double> BSMV { get; set; }
        public Nullable<double> KKDF { get; set; }
        public Nullable<double> KalanAnaPara { get; set; }
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
        public Nullable<bool> Silindi { get; set; }
        public Nullable<int> AccountFicheID { get; set; }
        public Nullable<int> AuthBy { get; set; }
        public Nullable<int> ZNo { get; set; }
        public Nullable<int> CaseNo { get; set; }
        public Nullable<int> AcquirerID { get; set; }
        public Nullable<int> BatchID { get; set; }
        public Nullable<int> StanID { get; set; }
        public Guid Guid { get; set; }
    }
}
