﻿using GoldenMobileX.Models;
using Xamarin.Forms;
using System.Linq;
using System;
using System.Threading.Tasks;
namespace GoldenMobileX.Views.Controls
{
    public class NumericInput : Entry
    {
        public static BindableProperty AllowNegativeProperty = BindableProperty.Create("AllowNegative", typeof(bool), typeof(NumericInput), false, BindingMode.TwoWay);
        public static BindableProperty AllowFractionProperty = BindableProperty.Create("AllowFraction", typeof(bool), typeof(NumericInput), false, BindingMode.TwoWay);

        public NumericInput()
        {
            this.Keyboard = Keyboard.Numeric;
        }

        public bool AllowNegative
        {
            get { return (bool)GetValue(AllowNegativeProperty); }
            set { SetValue(AllowNegativeProperty, value); }
        }

        public bool AllowFraction
        {
            get { return (bool)GetValue(AllowFractionProperty); }
            set { SetValue(AllowFractionProperty, value); }
        }
    }
}

