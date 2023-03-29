using Xamarin.Forms;
namespace GoldenMobileX.Views.Controls
{

    public class GoldenEntry : Entry
    {



    }
}

/*
namespace GoldenMobileX.Views.Controls
{
 
    public class GoldenEntry : AbsoluteLayout
    {
        
        public Entry entry;
        public Label label;
        public Frame frame;

        public GoldenEntry()
        {
            Initialize();
     
        }

 

 
 


        public static readonly BindableProperty TextProperty =
           BindableProperty.Create(nameof(Text), typeof(string), typeof(GoldenEntry), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(GoldenEntry), null);

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(GoldenEntry), Color.Blue);
        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(GoldenEntry), Color.FromHex("2196F3"));

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public string Keyboard
        {
            get { return (string)GetValue(KeyboardProperty); }
            set { SetValue(KeyboardProperty, value); }
        }
        public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(GoldenEntry), null);

        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }
        public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(GoldenEntry), false);

        void Initialize()
        {
            this.Margin = 4;
            this.HeightRequest = 80;
          
            frame = new Frame();
            frame.BorderColor = Color.FromHex("2196F3");
            frame.BackgroundColor = Color.FromHex("ffffff");
            frame.SetBinding(Frame.BorderColorProperty, new Binding() { Path = "BorderColor", Source = this });
            frame.CornerRadius = 6;
            frame.Margin = new Thickness(0, 6, 0, 0);
            frame.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Fill, Expands = true };
            this.Children.Add(frame);
            //this.HorizontalOptions = new LayoutOptions() { Alignment= LayoutAlignment.Fill, Expands=true };
            entry = new Entry();
            entry.BindingContext = this;
          
            entry.Margin = new Thickness(4, 0, 4, -8);
            entry.BindingContext = TextProperty;
 
            entry.SetBinding(Entry.IsPasswordProperty, new Binding() { Path = "IsPassword", Source = this });
            entry.SetBinding(Entry.KeyboardProperty, new Binding() { Path = "Keyboard", Source = this });
            entry.SetBinding(Entry.PlaceholderProperty, new Binding() { Path = "Placeholder", Source = this });

            entry.SetBinding(Entry.TextProperty, new Binding() { Path = "Text", Mode = BindingMode.TwoWay, Source = this });
            entry.BackgroundColor = Color.Transparent;
            this.Children.Add(entry);
            label = new Label();

            label.FontSize = 12;
            label.Margin = new Thickness(10, 0, 0, 0);
            label.SetBinding(Label.TextProperty, new Binding() { Path = "Placeholder", Mode = BindingMode.TwoWay, Source = this });


            label.BackgroundColor = Color.FromHex("ffffff");
            label.TextColor = Color.Black;
            this.Children.Add(label);


            AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(frame, new Rectangle(0, 0, 1, 1));

            AbsoluteLayout.SetLayoutFlags(entry, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(entry, new Rectangle(0, 0, 1, 1));

 
        }

 
    }

}

 
*/