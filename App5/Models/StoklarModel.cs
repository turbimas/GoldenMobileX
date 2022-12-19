using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using System.ComponentModel;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoldenMobileX.Models
{
    public partial class CRD_GroupMembers
    {
        public Nullable<int> GroupID { get; set; }
        public Nullable<int> RecordID { get; set; }
    }
    public partial class CRD_GroupProperties
    {
        public int ID { get; set; }
        public Nullable<int> GroupID { get; set; }
        public string PropertyName { get; set; }
        public string LookupFilter { get; set; }
        public string Degerler { get; set; }
    }
    public partial class CRD_Groups
    {
        public int ID { get; set; }
        public string AuthCode { get; set; }
        public Nullable<int> GroupSourceID { get; set; }
        public Nullable<int> UpperGroupID { get; set; }
        public string GroupName { get; set; }
        public string GroupDesc { get; set; }
    }
    public partial class CRD_ItemProperties
    {
        public int ID { get; set; }
        public Nullable<int> ItemID { get; set; }
        public string ItemProperty { get; set; }
        public Nullable<double> MaxValue { get; set; }
        public Nullable<double> MinValue { get; set; }
        public Nullable<int> UnitID { get; set; }

        public virtual CRD_Items CRD_Items { get; set; }
    }
    public partial class CRD_Items
    {
        public CRD_Items()
        {
            this.CRD_ItemProperties = new HashSet<CRD_ItemProperties>();
        }

        public int ID { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> BOMID { get; set; }
        public string AuthCode { get; set; }
        public string Code { get; set; }
        public string OzelKod { get; set; }
        public string Code2 { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string MuhasebeKodu { get; set; }
        public string Ozellikler { get; set; }
        public string TradeMark { get; set; }
        [XmlElement, DefaultValue(11)]    
        public Nullable<int> UnitID
        {
            get
            {
                return (UnitID_?.ID).convInt();
            }
            set
            {
                UnitID_ = DataLayer.L_Units.AsParallel().Where(x => x.ID == value).FirstOrDefault();
            }
        }
        public L_Units UnitID_ { get; set; }
        public Nullable<int> Type
        {
            get
            {
                return (Type_?.Code).convInt();
            }
            set
            {
               Type_= DataLayer.x_types_stokkarti.AsParallel().Where(x => x.Code == value).FirstOrDefault();
            }
        }
        public X_Types Type_ { get; set; }
        public string GTIP { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<double> UnitPrice2 { get; set; }
        public Nullable<double> UnitPrice3 { get; set; }
        public Nullable<double> AlisFiyati { get; set; }
        public Nullable<int> DovizKodu { get; set; }
        public Nullable<double> KritikSeviye { get; set; }
        public Nullable<double> MaksimumSeviye { get; set; }
        public Nullable<double> DefaultAmount { get; set; }
        public Nullable<double> PakettekiMiktar { get; set; }
        public Nullable<double> BalyadakiPaket { get; set; }
        public Nullable<double> AgirlikGr { get; set; }
        public Nullable<int> UrunPuan { get; set; }
        public Nullable<double> IskontoluMinSatisTutari { get; set; }
        public Nullable<bool> BedelsizVerilebilir { get; set; }
        public Nullable<int> ItemGroupID { get; set; }
        public Nullable<bool> DigerFirmaUrunu { get; set; }
        public Nullable<double> Bonus { get; set; }
        public string UrunGrubu { get; set; }
        public Nullable<double> MinPrice { get; set; }
        public Nullable<double> EnCm { get; set; }
        public Nullable<double> BoyCm { get; set; }
        public Nullable<double> TaxRate { get; set; }
        public Nullable<double> TaxRateToptan { get; set; }
        public Nullable<int> StockWareHouseID { get; set; }
        public Nullable<int> StockRoomsID { get; set; }
        public Nullable<int> StockShelfID { get; set; }
        public Nullable<int> StockShelfOrder { get; set; }
        public Nullable<double> PacalMaliyet { get; set; }
        public Nullable<double> StokAdeti { get; set; }
        public Nullable<double> StokGiris { get; set; }
        public Nullable<double> StokCikis { get; set; }
        public Nullable<int> StoktaBeklemeSuresi { get; set; }
        public Nullable<bool> WebteGoster { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModiFiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<double> Agirlik { get; set; }
        public Nullable<double> Derinlik { get; set; }
  
        [OneToMany]
        public virtual ICollection<CRD_ItemProperties> CRD_ItemProperties { get; set; }
    }
    public class CRD_ItemBarcodes
    {
        public int ID { get; set; }
        public Nullable<int> UrunID { get; set; }
        public string Barkod { get; set; }
        public Nullable<double> Miktar { get; set; }
        public Nullable<double> Fiyat { get; set; }
        public Nullable<double> Fiyat2 { get; set; }
        public Nullable<double> Fiyat3 { get; set; }
        public Nullable<int> Beden { get; set; }
        public Nullable<int> Renk { get; set; }
        public Nullable<double> StokAdeti { get; set; }
        public string Aciklama { get; set; }
        public Nullable<bool> VaryantWebteGoster { get; set; }
        public Nullable<bool> VaryantWebInSale { get; set; }
        public Nullable<bool> VaryantWebCanShipped { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModiFiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

    }
    public partial class V_AllItems
    {
         public int ID { get; set; }


        public Nullable<int> UnitID
        {
            get
            {
                return (UnitID_?.ID).convInt();
            }
            set
            {
                UnitID_ = DataLayer.L_Units.AsParallel().Where(x => x.ID == value).FirstOrDefault();
            }
        }
        [ManyToOne]
        [NotMapped]
        public L_Units UnitID_ { get; set; }


        public Nullable<int> Type
        {
            get
            {
                return (Type_?.Code).convInt();
            }
            set
            {
                Type_ = DataLayer.x_types_stokkarti.AsParallel().Where(x => x.Code == value).FirstOrDefault();
            }
        }
        [ManyToOne]
        [NotMapped]
        public X_Types Type_ { get; set; }


        public Nullable<bool> Active { get; set; }
        public string AuthCode { get; set; }
        public string Code { get; set; }
        public string OzelKod { get; set; }
        public string Code2 { get; set; }
        public string Barcode { get; set; }
        public string MainBarcode { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string TradeMark { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<double> UnitPrice2 { get; set; }
        public Nullable<double> UnitPrice3 { get; set; }
        public Nullable<double> AlisFiyati { get; set; }
        public Nullable<double> PakettekiMiktar { get; set; }
        public Nullable<double> AgirlikGr { get; set; }
        public Nullable<int> ItemGroupID { get; set; }
        public string UrunGrubu { get; set; }
        public Nullable<double> TaxRate { get; set; }
        public Nullable<double> TaxRateToptan { get; set; }
        public Nullable<double> StokAdeti { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModiFiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> UrunRenk { get; set; }
        public int Beden { get; set; }
        public double Miktar { get; set; }
        public string Aciklama { get; set; }
        public Nullable<int> PakettekiAdet { get; set; }
        public Nullable<double> PacalMaliyet { get; set; }
        public int VaryantID { get; set; }
        public Nullable<bool> WebteGoster { get; set; }
        public string MainImageUrl { get; set; }
        public string MainThumImageUrl { get; set; }
        public int isUnderProduct { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public string UnitType { get; set; }
        public string TypeName { get; set; }
        public string ColorName { get; set; }
        public double WebAddFactor { get; set; }
        public Nullable<bool> WebInSale { get; set; }
        public Nullable<bool> WebCanShipped { get; set; }
    }
 

    public class CRD_StockWareHouse
    {
        public int ID { get; set; }
 
        public string AuthCode { get; set; }
        public string Name { get; set; }
        public Nullable<double> Kapasite { get; set; }
        public Nullable<int> KapasiteBirimi { get; set; }
        public Nullable<int> Type { get; set; }
        public string DepoNo { get; set; }
        public Nullable<int> Branch { get; set; }
    }

    public class TRN_Invoice
    {
        public TRN_Invoice()
        {
            CariID_ = new CRD_Cari();
            Type_ = new X_Types();
            WareHouseID_ = new CRD_StockWareHouse();
            Lines = new List<TRN_StockTransLines>();
        }
        public int ID { get; set; }
        public Guid Guid { get; set; }
        public Guid ZarfUUID { get; set; }
        public string AuthCode { get; set; }
        public Nullable<int> CariID
        {
            get
            {
                return (CariID_?.ID).convInt();
            }
            set
            {
                CariID_= DataLayer.Cariler.AsParallel().Where(x => x.ID == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public CRD_Cari CariID_ { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> WorkOrderID { get; set; }
        public Nullable<int> StockTransID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public string DocNumber { get; set; }
        public string SpeCode { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Branch { get; set; }

        public Nullable<int> Type
        {
            get
            {
                return (Type_?.Code).convInt();
            }
            set
            {
                Type_= DataLayer.x_types_Invoice.AsParallel().Where(x => x.Code == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Types Type_ { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> FaturaninGeldigiTarih { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Notes { get; set; }
        public string Notes2 { get; set; }
        public string Notes3 { get; set; }
        public Nullable<double> NetTotal { get; set; }
        public Nullable<double> Discount1 { get; set; }
        public Nullable<double> Discount2 { get; set; }
        public Nullable<double> Discount3 { get; set; }
        public Nullable<double> TotalDiscount { get; set; }
        public Nullable<double> TotalTax { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<int> KDVIstisnaKodu { get; set; }
        public Nullable<int> PaymentAccountID { get; set; }
        public Nullable<int> AccountFicheID { get; set; }
        public Nullable<int> OdemePlanID { get; set; }
        public Nullable<int> HireCount { get; set; }
        public Nullable<bool> VirtualInvoice { get; set; }
        public Nullable<bool> EInvoice { get; set; }
        public Nullable<double> BagkurPrim { get; set; }
        public Nullable<double> BorsaTasdik { get; set; }
        public Nullable<double> Nakliye { get; set; }
        public Nullable<double> NakitOdenen { get; set; }
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
        public Nullable<bool> Cancelled { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> GoldenSync { get; set; }
        public string SyncFicheNo { get; set; }
        public Nullable<int> SyncLogicalRef { get; set; }
        public string EInvoiceStatus { get; set; }
        public string EInvoiceXML { get; set; }
        public Nullable<bool> KabulRed { get; set; }
        public Nullable<int> EntegratorID { get; set; }
        
        public Nullable<int> WareHouseID
        {
            get
            {
                return (WareHouseID_?.ID).convInt();
            }
            set
            {
                WareHouseID_ = DataLayer.Depolar.Where(x => x.ID == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public CRD_StockWareHouse WareHouseID_ { get; set; }
        
        public Nullable<double> StopajOrani { get; set; }
        public Nullable<int> IhracatID { get; set; }
        public string CariCode { get; set; }
        public Nullable<int> TevkifatKodu { get; set; }
        public List<TRN_StockTransLines> Lines { get; set; }
    }

    public class TRN_StockTrans
    {
        public Nullable<int> CariID
        {
            get
            {
                return (CariID_?.ID).convInt();
            }
            set
            {
                CariID_= DataLayer.Cariler.Where(x => x.ID == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public CRD_Cari CariID_ { get; set; }
        public int ID { get; set; }
        public string FicheNo { get; set; }
        public Nullable<int> WorkOrderID { get; set; }
        public Nullable<int> InvoiceID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<int> Branch { get; set; }
        public string SpeCode { get; set; }
        public Nullable<int> KantarSorumlusu { get; set; }
        public string NakliyeUcreti { get; set; }
        public Nullable<int> NakliyeFirmasi { get; set; }
        public string Sehir { get; set; }
        public string SoforAdi { get; set; }
        public string SoforTelefon { get; set; }
        public string SoforTC { get; set; }
        public string SevkNo { get; set; }
        
        public Nullable<int> Type
        {
            get
            {
                return (Type_?.Code).convInt();
            }
            set
            {
                Type_ = DataLayer.x_types_stokfisi.Where(x => x.Code == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public X_Types Type_ { get; set; }

        
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
        public string OzelKod { get; set; }
        public Nullable<System.DateTime> TransDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Notes { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<double> TotalTax { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<double> CurrencyRate { get; set; }
        public string KonteynerNo { get; set; }
        public string KonteynerAracPlaka { get; set; }
        public Nullable<int> KonteynerTipID { get; set; }
        
        public Nullable<int> StockWareHouseID
        {
            get
            {
                return (StockWareHouseID_?.ID).convInt();
            }
            set
            {
               StockWareHouseID_ = DataLayer.Depolar.Where(x => x.ID == value).FirstOrDefault();
            }
        }
        
        public Nullable<int> DestStockWareHouseID
        {
            get
            {
                return (DestStockWareHouseID_?.ID).convInt();
            }
            set
            {
                DestStockWareHouseID_= DataLayer.Depolar.Where(x => x.ID == value).FirstOrDefault();
            }
        }
        [NotMapped]
        public CRD_StockWareHouse StockWareHouseID_ { get; set; }
        [NotMapped]
        public CRD_StockWareHouse DestStockWareHouseID_ { get; set; }

        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> Cancelled { get; set; }
        public string IrsaliyeNotlari { get; set; }

        public Nullable<int> DeliveryAdressID { get; set; }
        [OneToMany] 
        [NotMapped] //  [System.ComponentModel.DataAnnotations.Schema.ForeignKey("StockTransID")]
        public List<TRN_StockTransLines> Lines { get; set; }

    }

    public partial class TRN_StockTransLines
    {
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
        public int ID { get; set; }
        public Nullable<int> SatirNo { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> Direction { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<int> OrderLinesID { get; set; }
        public Nullable<int> WorkOrderID { get; set; }
        public Nullable<int> PromosyonID { get; set; }
        public Nullable<double> PromosyonTutari { get; set; }
        public string SpeCode { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> InvoiceID { get; set; }
        public Nullable<int> StockTransID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public string SeriNo { get; set; }
        public string SeriLot { get; set; }
        public string LotID { get; set; }
        public string PaketNo { get; set; }
        public string BalyaNo { get; set; }
        public Nullable<int> Beden { get; set; }
        public Nullable<int> Renk { get; set; }
        public Nullable<double> En { get; set; }
        public Nullable<double> Boy { get; set; }
        public Nullable<double> Derinlik { get; set; }
        public Nullable<double> Agirlik { get; set; }
        public Nullable<double> Yogunluk { get; set; }
        public Nullable<bool> Depoda { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> GuaranteeID { get; set; }
        public Nullable<double> PakettekiAdet { get; set; }
        public Nullable<bool> Guarantee { get; set; }
        public Nullable<int> ProductType { get; set; }
        public string LineExp { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> RealAmount { get; set; }
        public Nullable<double> OrderAmount { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<double> PerakendeFiyati { get; set; }
        public Nullable<double> LineTotal { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> DiscountRate { get; set; }
        public Nullable<double> Discount2 { get; set; }
        public Nullable<double> DiscountRate2 { get; set; }
        public Nullable<double> Discount3 { get; set; }
        public Nullable<double> DiscountRate3 { get; set; }
        public Nullable<System.DateTime> SonKullanmaTarihi { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<double> CurrencyRate { get; set; }
        public Nullable<double> TaxRate { get; set; }
        public Nullable<double> TotalTax { get; set; }
        public Nullable<int> AddTaxID { get; set; }
        public string TevkifatOrani { get; set; }
        public Nullable<int> WorkerOrOutServiceID { get; set; }
        public Nullable<double> AggCost { get; set; }
        public Nullable<int> Branch { get; set; }
        public Nullable<bool> GoldenSync { get; set; }
        public string SyncFicheNo { get; set; }
        public Nullable<int> SyncLogicalRef { get; set; }
        public Nullable<int> StockWareHouseID { get; set; }
        public Nullable<int> StockRoomsID { get; set; }
        public Nullable<int> StockShelfID { get; set; }
        public Nullable<int> DestStockWareHouseID { get; set; }
        public Nullable<int> DestStockRoomsID { get; set; }
        public Nullable<int> DestStockShelfID { get; set; }
        public Nullable<double> PacalMaliyet { get; set; }
        public string PacalMaliyetIslemi { get; set; }
        public Nullable<bool> Cancelled { get; set; }
        public string InvoiceNo { get; set; }
        public string DocNumber { get; set; }
        public Nullable<int> IhracatID { get; set; }
        public Nullable<int> CaseNo { get; set; }
        public Nullable<int> AuthBy { get; set; }
        public Nullable<bool> PosTrans { get; set; }
        public Nullable<double> MalKabulAmount { get; set; }
        public string CariCode { get; set; }
        public Nullable<double> Dara { get; set; }
        public string FisNo { get; set; }
        public Nullable<int> ZNo { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string TedarikciLotID { get; set; }
        public Nullable<int> SevkiyatPaketMiktari { get; set; }
        public string SevkiyatPaketTuru { get; set; }
        public string SevkiyatTeslimSekli { get; set; }
        public Nullable<int> SevkiyatTasimaYolu { get; set; }
        public Nullable<double> PuanOran { get; set; }

    }

    public class V_DepodakiLotlar
    {
        [Key]
        public Nullable<int> HareketID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> Branch { get; set; }
        public Nullable<int> UnitID { get; set; }
        public string ISLEM { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string LotID { get; set; }
        public string PaketNo { get; set; }
        public string BalyaNo { get; set; }
        public Nullable<double> Miktar { get; set; }
        public Nullable<int> Depo { get; set; }
        public Nullable<int> Direction { get; set; }
        public Nullable<double> PakettekiAdet { get; set; }
        public Nullable<double> En { get; set; }
        public Nullable<double> Boy { get; set; }
        public Nullable<double> Derinlik { get; set; }
        public Nullable<double> Agirlik { get; set; }
        public Nullable<double> Yogunluk { get; set; }
        public string SeriLot { get; set; }
        public Nullable<int> Renk { get; set; }
        public Nullable<int> Beden { get; set; }
        public string SeriNo { get; set; }
        public Nullable<int> WorkOrderID { get; set; }
        public string LineExp { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }


    public partial class TRN_EtiketBasim
    {
        public int ID { get; set; }
        public Nullable<int> Branch { get; set; }
        public string FisNo { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
        public string DegisiklikSebebi { get; set; }
        public Nullable<System.DateTime> PromosyonBaslangic { get; set; }
        public Nullable<System.DateTime> PromosyonBitis { get; set; }
        public Nullable<bool> PromosyonBitti { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> Uygulandi { get; set; }
        public Nullable<bool> GoldenSync { get; set; }
        [NotMapped]
        public List<TRN_EtiketBasimEmirleri> TRN_EtiketBasimEmirleri { get; set; }
    }

    public partial class TRN_EtiketBasimEmirleri
    {
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
        public int ID { get; set; }
        public Nullable<int> FisID { get; set; }
        public string FisNo { get; set; }
        public string Barkod { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
        public Nullable<double> Fiyat { get; set; }
        public Nullable<double> Fiyat2 { get; set; }
        public Nullable<double> Fiyat3 { get; set; }
        public Nullable<double> EskiFiyat { get; set; }
        public Nullable<bool> EtiketBasildi { get; set; }
        public Nullable<bool> Uygulandi { get; set; }
        public Nullable<double> SonAlisFiyati { get; set; }
        public Nullable<double> MainUnitPrice { get; set; }
        public Nullable<System.DateTime> PromosyonBaslangic { get; set; }
        public Nullable<System.DateTime> PromosyonBitis { get; set; }
        public string DegisiklikSebebi { get; set; }
        public Nullable<bool> PromosyonBitti { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModiFiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Aciklama { get; set; }
        public Nullable<int> EtiketSayisi { get; set; }
        public Nullable<bool> GoldenSync { get; set; }
    }
}
