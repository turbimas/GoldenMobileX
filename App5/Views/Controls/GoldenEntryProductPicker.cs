using GoldenMobileX.Models;
using System.Linq;
using Xamarin.Forms;

namespace GoldenMobileX.Views.Controls
{
    class GoldenEntryProductPicker : DevExpress.XamarinForms.Editors.TextEdit
    {

        public GoldenEntryProductPicker()
        {
            this.Focused += GoldenEntryProductPicker_Focused;
            this.PropertyChanged += GoldenProductPicker_PropertyChanged;
            this.LabelText = "Ürün";
        }



        private void GoldenProductPicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedItem") && SelectedItem != null)
            {
                this.Text = SelectedItem.Name;
            }
        }
        public static readonly BindableProperty SelectedValueProperty =
BindableProperty.Create(nameof(SelectedValue), typeof(int), typeof(GoldenEntryProductPicker), 0);
        public int SelectedValue
        {
            get { return (int)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); this.Text = DataLayer.V_AllItems.Where(s => s.ID == value).FirstOrDefault().Name; }
        }

        public static readonly BindableProperty SelectedItemProperty =
    BindableProperty.Create(nameof(SelectedItem), typeof(V_AllItems), typeof(GoldenEntryProductPicker), default(V_AllItems));

        public V_AllItems SelectedItem
        {
            get { return (V_AllItems)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); this.Text = value.Name; }
        }

        private async void GoldenEntryProductPicker_Focused(object sender, FocusEventArgs e)
        {
            Stoklar fm = new Stoklar();


            fm.ItemSelected += (s2, e2) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.SelectedItem = fm.item;
                    this.Text = fm.item.Name;
                    Navigation.PopAsync();
                    this.Unfocus();
                });
            };
            await Navigation.PushAsync(fm);
        }
    }
}
