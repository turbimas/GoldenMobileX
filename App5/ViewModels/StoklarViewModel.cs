using GoldenMobileX.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Diagnostics;
using System.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GoldenMobileX.ViewModels
{
    public class StoklarViewModel : BaseViewModel
    {
        public V_AllItems item
        {
            get; set;
        }



        public StoklarViewModel()
        {

            Title = "Stoklar";

        }


        public List<double> kdv
        {
            get
            {
                List<double> k = new List<double>();
                k.Add(0);
                k.Add(1);
                k.Add(2);
                k.Add(8);
                k.Add(18);
                return k;
            }
        }

        public List<V_AllItems> items
        {
            get; set;

        }
        public List<CRD_ItemBarcodes> varyantlar
        {
            get; set;

        }
        public CRD_ItemBarcodes varyant
        {
            get; set;

        }
        public List<L_Units> units
        {
            get
            {
                return DataLayer.L_Units ;
            }
        }

        public List<X_Types> stockTypes
        {
            get
            {
                return DataLayer.x_types_stokkarti;
            }
        }
        public List<TRN_Files> files
        {
            get;

            set;
        }

  
    }
}