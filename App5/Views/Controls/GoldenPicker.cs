using GoldenMobileX.Models;
using Xamarin.Forms;
using System.Linq;
using System;

namespace GoldenMobileX.Views.Controls
{
    class  GoldenPicker : Picker
    {
        public static EventHandler ItemSelected;

        public GoldenPicker()
        {
            this.Focused += GoldenPicker_Focused;
    
        }

        private void GoldenPicker_Focused(object sender, FocusEventArgs e)
        {
            Stoklar fm = new Stoklar();
 

            fm.ItemSelected += (s2, e2) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    this.SelectedItem = fm.item;
                    this.Unfocus();
                });
            };
            Navigation.PushAsync(fm);
            e.VisualElement.IsVisible = false;
        }
    }
}
