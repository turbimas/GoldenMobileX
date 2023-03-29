using GoldenMobileX.Models;
using Xamarin.Forms;

namespace GoldenMobileX.Views.Controls
{
    class GoldenEntryCariPicker : DevExpress.XamarinForms.Editors.TextEdit
    {

        public GoldenEntryCariPicker()
        {
            this.Focused += GoldenEntryPicker_Focused;
            this.PropertyChanged += GoldenPicker_PropertyChanged;
            this.LabelText = "Cari Hesap";

        }



        private void GoldenPicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedItem") && SelectedItem != null)
            {
                this.Text = SelectedItem.Name;
            }
        }

        public static readonly BindableProperty SelectedItemProperty =
    BindableProperty.Create(nameof(SelectedItem), typeof(CRD_Cari), typeof(GoldenEntryProductPicker), default(CRD_Cari));

        public CRD_Cari SelectedItem
        {
            get { return (CRD_Cari)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); this.Text = value.Name; }
        }

        private async void GoldenEntryPicker_Focused(object sender, FocusEventArgs e)
        {
            CariHesaplar fm = new CariHesaplar();



            fm.ItemSelected += (s2, e2) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.SelectedItem = fm.SelectedCari;
                    Navigation.PopAsync();
                    this.Unfocus();
                });
            };
            await Navigation.PushAsync(fm);
        }
    }
}
