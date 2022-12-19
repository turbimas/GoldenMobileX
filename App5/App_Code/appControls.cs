using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;

class GldnButton : Button
{
    Color backColor { get; set; }
    Color foreColor { get; set; }
    public GldnButton(string text, int Status)
    {
        backColor = Color.White;
        foreColor = Color.Black;
        switch (Status)
        {
            case -1:
                backColor = Color.FromRgb(102, 0, 0);
                foreColor = Color.White;
                break;
            case 1:
                backColor = Color.FromRgb(51, 102, 0);
                foreColor = Color.White;
                break;
            case 2:
                backColor = Color.FromRgb(0, 51, 102);
                foreColor = Color.White;
                break;
            case 3:
                backColor = Color.FromRgb(102, 0, 102);
                foreColor = Color.White;
                break;
            case 4:
                backColor = Color.FromRgb(0, 102, 102);
                foreColor = Color.White;
                break;
            case 5:
                backColor = Color.FromRgb(102, 51, 0);
                foreColor = Color.White;
                break;
            case 6:
                backColor = backColor = Color.FromRgb(102, 102, 0);
                foreColor = Color.White;
                break;
            case 7:
                backColor = Color.FromRgb(102, 0, 51);
                foreColor = Color.White;
                break;
        }

        this.Text = text;
        this.TextColor = foreColor;
        this.BackgroundColor = backColor;
        this.CornerRadius = 5;
 


    }

}
class GldnLabel : Label
{
    Color backColor { get; set; }
    Color foreColor { get; set; }
    public GldnLabel(string text, int Status)
    {
        backColor = Color.White;
        foreColor = Color.Black;
        switch (Status)
        {
            case -1:
                backColor = Color.FromRgb(102, 0, 0);
                foreColor = Color.White;
                break;
            case 1:
                backColor = Color.FromRgb(51, 102, 0);
                foreColor = Color.White;
                break;
            case 2:
                backColor = Color.FromRgb(0, 51, 102);
                foreColor = Color.White;
                break;
            case 3:
                backColor = Color.FromRgb(102, 0, 102);
                foreColor = Color.White;
                break;
            case 4:
                backColor = Color.FromRgb(0, 102, 102);
                foreColor = Color.White;
                break;
            case 5:
                backColor = Color.FromRgb(102, 51, 0);
                foreColor = Color.White;
                break;
            case 6:
                backColor = backColor = Color.FromRgb(102, 102, 0);
                foreColor = Color.White;
                break;
            case 7:
                backColor = Color.FromRgb(102, 0, 51);
                foreColor = Color.White;
                break;
        }

        this.Text = text;
        this.TextColor = foreColor;
        this.BackgroundColor = backColor;
 


    }

}
public class GldnStackLayout : StackLayout
{
    public Color backColor(int Status)
    {
        switch (Status)
        {
            case -1:
                return Color.FromRgb(102, 0, 0);
            case 1:
                return Color.FromRgb(51, 102, 0);
            case 2:
                return Color.FromRgb(0, 51, 102);
            case 3:
                return Color.FromRgb(102, 0, 102);
            case 4:
                return Color.FromRgb(0, 102, 102);
            case 5:
                return Color.FromRgb(102, 51, 0);
            case 6:
                return  Color.FromRgb(102, 102, 0);
            case 7:
                return Color.FromRgb(102, 0, 51);
            case 11:
                return Color.FromRgb(51, 0, 102);
        }
        return Color.FromRgb(51, 0, 102);
    }
    public Color foreColor(int Status)
    {
        return Color.White;
    }
    public string txt{get;set;}

 
}