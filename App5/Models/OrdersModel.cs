using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace GoldenMobileX.Models
{

    public class TRN_Orders
    {
         public int ID { get; set; }
        public Guid UniqueID { get; set; }
        public Nullable<int> TalepID { get; set; }
        public Nullable<int> TeklifID { get; set; }
        public Nullable<int> Branch { get; set; }

        public Nullable<int> Status
        {
            get
            {
                return Status_.Code;
            }
            set
            {
                Status_ = DataLayer.x_types_satisdurum.Where(x => x.Code == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Types Status_ { get; set; }
        public Nullable<int> OrderType
        {
            get
            {
                return OrderType_.Code;
            }
            set
            {
                OrderType_ = DataLayer.x_types_siparisTuru.Where(x => x.Code == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Types OrderType_ { get; set; }
        public string FicheNo { get; set; }
        public string OzelKod { get; set; }
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
        public string AuthorizedPeople { get; set; }
        public Nullable<double> KarMarji { get; set; }
        public Nullable<double> IndirimOrani { get; set; }
        public Nullable<double> ToplamIndirim { get; set; }
        public Nullable<double> MasrafOrani { get; set; }
        public Nullable<double> ToplamMasraf { get; set; }
        public Nullable<bool> MasrafIndirimKDVDahil { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> PaymentAccountID { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<int> DeliveryAdressID { get; set; }

        public Nullable<double> Total { get; set; }
        public Nullable<int> CurrencyID
        {
            get
            {
                return CurrencyID_.CurrencyNumber;
            }
            set
            {
                CurrencyID_ = DataLayer.X_Currency.Where(x => x.CurrencyNumber == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Currency CurrencyID_ { get; set; }
        public Nullable<double> CurrencyRate { get; set; }
        public string OrderNotes { get; set; }
        public string InvoiceNotes { get; set; }
        public string MusteriNotlari { get; set; }
        public string FaturaNo { get; set; }
        public Nullable<bool> Onaylandi { get; set; }
        public Nullable<int> OnaylayanID { get; set; }
        public Nullable<bool> Iptal { get; set; }
        public Nullable<bool> SevkeGonder { get; set; }
        public Nullable<bool> Faturalanacak { get; set; }
        public Nullable<int> Vade { get; set; }
        public string DeliveryAdress { get; set; }
        public Nullable<System.DateTime> OnayTarihi { get; set; }
        public Nullable<bool> SevkEdildi { get; set; }
        public Nullable<int> StockTransID { get; set; }
        public Nullable<System.DateTime> FiiliSevkTarihi { get; set; }
        public Nullable<int> TeslimSuresi { get; set; }
        public Nullable<bool> KDVHaric { get; set; }
        public Nullable<int> SatisSorumlusu { get; set; }
        public Nullable<System.DateTime> IlkOkunmaTarihi { get; set; }
        public Nullable<System.DateTime> GecerlilikTarihi { get; set; }
        public Nullable<bool> NakliyeDahil { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [NotMapped]
        public List<TRN_OrderLines> Lines { get; set; }
    }
    public class TRN_OrderLines
    {
        public int ID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<int> SatirNo { get; set; }
        public string PONumber { get; set; }
        public string SeriNo { get; set; }
        public Nullable<int> Renk { get; set; }
        public Nullable<int> Beden { get; set; }
        public Nullable<int> ProductID
        {
            get
            {
                return (ProductID_?.ID).convInt();
            }
            set
            {
                ProductID_ = DataLayer.V_AllItems.Where(x => x.ID == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public V_AllItems ProductID_ { get; set; }
        public Nullable<int> ProductionID { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<int> UnitID
        {
            get
            {
                return (UnitID_?.ID).convInt();
            }
            set
            {
                UnitID_ = DataLayer.L_Units.Where(x => x.ID == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public L_Units UnitID_ { get; set; }
        public Nullable<double> IndirimsizBirimFiyat { get; set; }
        public Nullable<double> Masraf { get; set; }
        public Nullable<double> Indirim { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<double> CurrencyRate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> VatRate { get; set; }
        public Nullable<int> RouteID { get; set; }
        public Nullable<double> AlisFiyati { get; set; }
        public string Notes { get; set; }
        public string MusteriNotlari { get; set; }
        public Nullable<bool> Bedelsiz { get; set; }
        public Nullable<double> Bonus { get; set; }
        public Nullable<bool> Iptal { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<System.DateTime> TerminTarihi { get; set; }
        public Nullable<bool> KDVDahil { get; set; }
        public Nullable<double> En { get; set; }
        public Nullable<double> Boy { get; set; }
        public Nullable<double> Derinlik { get; set; }
        public Nullable<double> Agirlik { get; set; }
        public Nullable<double> Yogunluk { get; set; }
        public string Ozellik1 { get; set; }
        public string Ozellik2 { get; set; }
        public string Ozellik3 { get; set; }
        public Nullable<int> PromosyonID { get; set; }
        public Nullable<double> PromosyonTutari { get; set; }
    }
}
