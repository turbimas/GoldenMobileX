using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenMobileX.Models
{
    public partial class CRD_Cari
    {
        public CRD_Cari()
        {
            this.CRD_CariAdres = new HashSet<CRD_CariAdres>();
        }

        public int ID { get; set; }
        public Nullable<bool> Active { get; set; }
        public string AuthCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Yetkili { get; set; }
        public Nullable<System.DateTime> BornDate { get; set; }
        [NotMapped]
        public X_Types CardType_ { get; set; }
        public Nullable<short> CardType
        {
            get
            {
                return (CardType_?.Code).convInt16();
            }
            set
            {
               CardType_ = DataLayer.x_types_carihesap.AsParallel().Where(x => x.Code == value).FirstOrDefault();
            }
        }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string WebUrl { get; set; }
        public string Adress { get; set; }
        public Nullable<int> TownID { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string PostCode { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public Nullable<bool> isPerson { get; set; }
        public string TCKNo { get; set; }
        public Nullable<bool> EFatura { get; set; }
        public string Senaryo { get; set; }
        public string GondericiBirimEtiketi { get; set; }
        public string AliciBirimEtiketi { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<int> CurrencyRateNumber { get; set; }
        public Nullable<System.DateTime> GirisTarihi { get; set; }
        public Nullable<System.DateTime> CikisTarihi { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModiFiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Notes { get; set; }
        public string OzelKod1 { get; set; }
        public string OzelKod2 { get; set; }
        public string OzelKod3 { get; set; }
        public Nullable<bool> Cinsiyet { get; set; }
        public Nullable<int> Uyruk { get; set; }

        public Nullable<double> Bakiye { get; set; }
        public Nullable<double> DovizliBakiye { get; set; }
        public Nullable<double> CekSenetBakiye { get; set; }
        public Nullable<double> PuanBakiye { get; set; }
        public string Password { get; set; }
        public Nullable<double> USDBakiye { get; set; }
        public Nullable<double> EUROBakiye { get; set; }



        public virtual ICollection<CRD_CariAdres> CRD_CariAdres { get; set; }
    }
    public partial class CRD_CariAdres
    {
        public int ID { get; set; }
        public string AdresBasligi { get; set; }
        public Nullable<int> CariID { get; set; }
        public Nullable<bool> Varsayilan { get; set; }
        public string AuthCode { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string WebUrl { get; set; }
        public string Fax { get; set; }
        public string Adress { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> TownID { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string PostCode { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public Nullable<bool> isPerson { get; set; }
        public string TCKNo { get; set; }
        public string Ref { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModiFiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Notes { get; set; }

        public virtual CRD_Cari CRD_Cari { get; set; }
    }
    public class V_CariHareketler
    {
 
        [NotMapped]
        public X_Currency CurrencyID_ { get; set; }

        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> CurrencyID { get {return  CurrencyID_.CurrencyNumber; } set { CurrencyID_ = DataLayer.X_Currency.Where(s => s.CurrencyNumber == value).FirstOrDefault(); } }
        public int ID { get; set; }
        public Nullable<int> CariID { get; set; }
        public Nullable<double> Tutar { get; set; }
        public Nullable<double> ParaPuan { get; set; }
        public string DocNumber { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Modul { get; set; }
        public System.Drawing.Color ModulColor { get { return DataLayer.moduleColor(Modul); } }
        public Nullable<double> CurrencyRate { get; set; }
        public Nullable<double> DovizTutari { get; set; }

        public string OzelKod { get; set; }
        public Nullable<double> Bakiye { get; set; }
        public Nullable<double> DovizBakiye { get; set; }
        public Nullable<double> PuanBakiye { get; set; }
    }
}
