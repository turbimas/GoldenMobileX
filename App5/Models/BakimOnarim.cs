using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GoldenMobileX.Models
{
    public partial class Kalite_KalibrasyonCihazlar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kalite_KalibrasyonCihazlar()
        {
            this.Kalite_KalibrasyonGirisi = new HashSet<Kalite_KalibrasyonGirisi>();
        }

        public int ID { get; set; }
        public Nullable<int> CihazTuru { get; set; }
        public Nullable<int> KullanimYeri { get; set; }
        public string CihazAdi { get; set; }
        public string CihazKodu { get; set; }
        public string Marka { get; set; }
        public string Sahibi { get; set; }
        public Nullable<int> KalibrasyonTipi { get; set; }
        public string OlcumHassasiyeti { get; set; }
        public string OlcumToleransi { get; set; }
        public Nullable<int> KalibrasyonPeriyodu { get; set; }
        public Nullable<int> CihazSorumlusu { get; set; }
        public Nullable<bool> Aktif { get; set; }
        public Nullable<int> OrtalamaOmru { get; set; }
        public Nullable<int> OmurBirimi { get; set; }
        public Nullable<int> Rota { get; set; }
        public Nullable<int> Istasyon { get; set; }

        [NotMapped]
        public virtual ICollection<Kalite_KalibrasyonGirisi> Kalite_KalibrasyonGirisi { get; set; }
    }


    public partial class Kalite_KalibrasyonCihazSarfListesi
    {
        public int ID { get; set; }
        public Nullable<int> CihazID { get; set; }
        public Nullable<int> ItemID { get; set; }
    }

    public partial class Kalite_KalibrasyonGirisi
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
        public Nullable<int> SarfFisID { get; set; }
        public Nullable<int> LabaratuvarID { get; set; }
        public Nullable<int> CihazID { get; set; }
        public Nullable<int> DegerlendirmeSonucu { get; set; }
        public string Notlar { get; set; }
        public Nullable<System.DateTime> BirSonrakiKalibrasyonTarihi { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModiFiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string KalibrasyonNo { get; set; }
        [NotMapped]
        public virtual Kalite_KalibrasyonCihazlar Kalite_KalibrasyonCihazlar { get; set; }
    }
}
