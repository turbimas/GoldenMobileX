using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;
//using XamarinEntry;
//using XamarinEntry.Droid;
using GoldenMobileX.Views.Controls;
/*
[assembly: ExportRenderer(typeof(NumericInput), typeof(NumericInputRenderer))]
namespace GoldenMobileX.Droid.Renderer
{
    public class NumericInputRenderer : EntryRenderer
    {
        public NumericInputRenderer(Context context) : base(context)
        {

        }

        private EditText _native = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            _native = Control as EditText;
            _native.InputType = Android.Text.InputTypes.ClassNumber;
            if ((e.NewElement as NumericInput).AllowNegative == true)
                _native.InputType |= InputTypes.NumberFlagSigned;
            if ((e.NewElement as NumericInput).AllowFraction == true)
            {
                _native.InputType |= InputTypes.NumberFlagDecimal;
                _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890{0}", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            }
            if (e.NewElement.FontFamily != null)
            {
                var font = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, e.NewElement.FontFamily);
                _native.Typeface = font;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (_native == null)
                return;

            if (e.PropertyName == NumericInput.AllowNegativeProperty.PropertyName)
            {
                if ((sender as NumericInput).AllowNegative == true)
                {
                    // Add Signed flag
                    _native.InputType |= InputTypes.NumberFlagSigned;
                }
                else
                {
                    // Remove Signed flag
                    _native.InputType &= ~InputTypes.NumberFlagSigned;
                }
            }
            if (e.PropertyName == NumericInput.AllowFractionProperty.PropertyName)
            {
                if ((sender as NumericInput).AllowFraction == true)
                {
                    // Add Decimal flag
                    _native.InputType |= InputTypes.NumberFlagDecimal;
                    _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890{0}", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                }
                else
                {
                    // Remove Decimal flag
                    _native.InputType &= ~InputTypes.NumberFlagDecimal;
                    _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890"));
                }
            }
        }
    }
}


namespace GoldenMobileX.iOS.Renderer
{
    public class NumericInputRenderer : EntryRenderer
    {
        private UITextField _native = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            _native = Control as UITextField;

            _native.KeyboardType = UIKeyboardType.NumberPad;

            if ((e.NewElement as NumericInput).AllowNegative == true && (e.NewElement as NumericInput).AllowFraction == true)
            {
                _native.KeyboardType = UIKeyboardType.NumbersAndPunctuation;
            }
            else if ((e.NewElement as NumericInput).AllowNegative == true)
            {
                _native.KeyboardType = UIKeyboardType.NumbersAndPunctuation;
            }
            else if ((e.NewElement as NumericInput).AllowFraction == true)
            {
                _native.KeyboardType = UIKeyboardType.DecimalPad;
            }
            else
            {
                _native.KeyboardType = UIKeyboardType.NumberPad;
            }
            if (e.NewElement.FontFamily != null)
            {
                e.NewElement.FontFamily = e.NewElement.FontFamily.Replace(".ttf", "");
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (_native == null)
                return;
        }
    }
}

*/