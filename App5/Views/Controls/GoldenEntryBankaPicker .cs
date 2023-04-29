using GoldenMobileX.Models;
using Xamarin.Forms;

namespace GoldenMobileX.Views.Controls
{
    class GoldenEntryBankaPicker : DevExpress.XamarinForms.Editors.TextEdit
    {

        public GoldenEntryBankaPicker()
        {
            this.Focused += GoldenEntryPicker_Focused;
            this.PropertyChanged += GoldenPicker_PropertyChanged;
            this.LabelText = "Banka Hesabı";

        }



        private void GoldenPicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedItem") && SelectedItem != null)
            {
                this.Text = SelectedItem.HesapAdi;
            }
        }

        public static readonly BindableProperty SelectedItemProperty =
    BindableProperty.Create(nameof(SelectedItem), typeof(CRD_BankaHesaplari), typeof(GoldenEntryBankaPicker), default(CRD_BankaHesaplari));

        public CRD_BankaHesaplari SelectedItem
        {
            get { return (CRD_BankaHesaplari)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); this.Text = value.HesapAdi; }
        }

        private async void GoldenEntryPicker_Focused(object sender, FocusEventArgs e)
        {
            Bankalar fm = new Bankalar();
            fm.OnlySelect = true;
            fm.ItemSelected += (s2, e2) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.SelectedItem = fm.SelectedItem;
                    Navigation.PopAsync();
                    this.Unfocus();
                });
            };
            await Navigation.PushAsync(fm);
        }
    }
}
