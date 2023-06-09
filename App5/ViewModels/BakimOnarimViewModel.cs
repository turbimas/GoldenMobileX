﻿using GoldenMobileX.Models;
using System.Collections.Generic;

namespace GoldenMobileX.ViewModels
{
    public class BakimOnarimViewModel : BaseViewModel
    {
        public V_AllItems item
        {
            get; set;
        }



        public BakimOnarimViewModel()
        {

            Title = "Bakım Onarım";

        }



        public List<V_AllItems> items
        {
            get; set;

        }

        public List<L_Units> units
        {
            get
            {
                return DataLayer.L_Units;
            }
        }

        public List<Kalite_KalibrasyonGirisi> BakimOnarimListesi
        {
            get; set;
        }
        public List<Kalite_KalibrasyonCihazlar> CihazListesi
        {
            get; set;
        }
        public List<Kalite_KalibrasyonCihazSarfListesi> SarfListesi
        {
            get; set;
        }
        public List<TRN_Files> files
        {
            get;

            set;
        }


    }
}