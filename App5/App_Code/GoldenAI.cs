using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


    public class GoldenAI
    {
    static List<string> decisions = new List<string>();
    static List<(string, Type)> types = new List<(string, Type)>();
  public  static string getPattern(string text)
    {
        if (text == null) return "";
        text = text.Replace("'", " ");
        string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (text.Contains("\t"))
            words = text.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

        int i = -1;
        types = new List<(string, Type)>();
        foreach (string word in words)
        {
            i++;

            if (word.Split('/').Length > 1)
            {
                if (Tools.IsInt(word.Split('/')[0]).ok && !Tools.IsInt(word.Split('/')[1]).ok)
                    text = text.Replace(word, word.Split('/')[1]);
            }
            var tran = Tools.IsTransaction(word);
            if (tran.ok)
            {
                text = text.Replace(word + "", "{tran}");
                types.Add((word, typeof(string)));
                continue;
            }

            var d = Tools.IsDate(word);
            if (d.ok)
            {
                text = text.Replace(word + "", "{date}");
                types.Add((word, typeof(DateTime)));
                continue;
            }

            var time = Tools.IsTime(word);
            if (time.ok)
            {
                text = text.Replace(word + "", "{time}");
                types.Add((word, typeof(TimeSpan)));
                continue;
            }

            var num = Tools.IsInt(word);
            if (num.ok)
            {
                text = text.Replace(word + "", "{int}");
                types.Add((word, typeof(int)));
                continue;
            }

            var money = Tools.IsMoney(word);
            if (money.ok)
            {
                text = text.Replace(word + "", "{money}");
                types.Add((word, typeof(decimal)));
                continue;
            }
            var dbl = Tools.IsDouble(word);
            if (dbl.ok)
            {
                text = text.Replace(word + "", "{double}");
                types.Add((word, typeof(double)));
                continue;
            }
            if (Tools.IsCurrency(word))
            {
                text = text.Replace(word + "", "{currency}");
                types.Add((word, typeof(char[])));
                continue;
            }
        }
        if (decisions.Count() > 0)
        {
            foreach (string dec in decisions)
            {
                var vstrings = Tools.OrtakOlmayanKelimeleriBul(text, dec);
                text = vstrings.changedText;
                foreach (string str in vstrings.varStrings)
                    types.Add((str, typeof(string)));
            }
        }
        return text;
    }
  public  class Tools
    {
        public static void OrtakKelimeleriBul(string t1, string t2)
        {
            string[] kelimeler1 = t1.Split(' ');
            string[] kelimeler2 = t1.Split(' ');

            var ortakKelimeler = kelimeler1.Intersect(kelimeler2);

            string sonuc = "Ortak kelimeler: ";

            foreach (string kelime in ortakKelimeler)
            {
                sonuc += kelime + " ";
            }

            Console.WriteLine(sonuc);
        }

        public static string DegiskenGir(string anametin)
        {
            Console.Write("Değişkeni yazınız ([Ç] - Çıkış): ");
            string s = Console.ReadLine();
            if (s == "Ç" || s == "ç")
            {

            }
            else
            {
                anametin = anametin.Replace(s, "{string}");
                DegiskenGir(anametin);
            }
            return anametin;
        }
        public static (string changedText, List<string> varStrings) OrtakOlmayanKelimeleriBul(string text1, string text2)
        {
            // 1. Adım: Metinleri kelimelere ayır
            string[] words1 = text1.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] words2 = text2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> vStrings = new List<string>();
            int i = 0;
            List<string> tumceler = new List<string>();
            List<string> tumce = new List<string>();
            foreach (string word in words1)
            {
                int index = Array.IndexOf(words2, word);
                if (index >= 0)
                {
                    if (tumce.Count() > 0)
                    {
                        tumceler.Add(string.Join(" ", tumce));
                        tumce.Clear();
                    }
                }
                else
                {
                    tumce.Add(word);
                }
                i++;
            }
            foreach (string s in tumceler)
            {
                text1 = text1.Replace(s, "{string}");
                vStrings.Add(s);
            }
            return ((text1, vStrings));
        }
        public static string ReplaceFirstOccurrence(string originalString, string searchString, string replaceString)
        {
            int index = originalString.IndexOf(searchString);
            if (index == -1)
            {
                return originalString;
            }

            return originalString.Substring(0, index) + replaceString + originalString.Substring(index + searchString.Length);
        }
        public static string sabitStringler(string text)
        {
            return text.Replace("{date}", ",").Replace("{time}", ",").Replace("{string}", ",").Replace("{int}", ",").Replace("{money}", ",");
        }
        public static List<(string DateText, DateTime date, int wordIndex)> FindDates(string text)
        {
            text = text.Replace("'", " ");
            var regex = new Regex(@"\d{1,2}\/\d{1,2}\/\d{4}|\d{4}\-\d{1,2}\-\d{1,2}|\d{1,2}\.\d{1,2}\.\d{4}|\d{1,2}\s+\w+\s+\d{4}");
            List<(string, DateTime, int)> dates = new List<(string, DateTime, int)>();

            int i = 0;
            string[] words = text.Split(' ');
            foreach (string word in words)
            {
                var _date = IsDate(word);
                if (_date.ok)
                {
                    dates.Add((word, _date.value, i));
                }
                i++;
            }
            return dates;
        }

        public static List<(string, TimeSpan, int)> FindTimes(string text)
        {
            text = text.Replace("'", " ");
            var times = new List<(string, TimeSpan, int)>();
         
            string[] words = text.Split(' ');
            int i = -1;
            foreach (string word in words)
            {
                i++;

                var m = IsTime(word);
                if (m.ok)
                    times.Add((word, m.value, i));
            }
            return times;
        }
        public static List<(string, decimal, int)> FindMoneys(string text)
        {
            var numbers = new List<(string, decimal, int)>();
            // Metni boşluklara göre ayırarak kelime dizisine dönüştür
            string[] words = text.Split(' ');
            int i = -1;
            foreach (string word in words)
            {
                i++;

                var m = IsMoney(word);
                if (m.ok)
                    numbers.Add((word, m.value, i));
            }
            return numbers;
        }
        public static List<(string, int, int)> FindInts(string text)
        {

            var numbers = new List<(string, int, int)>();
            string numberString = string.Empty;
            string[] words = text.Split(' ');
            int i = -1;
            foreach (string word in words)
            {
                i++;

                var m = IsInt(word);
                if (m.ok)
                    numbers.Add((word, m.value, i));

            }

            return numbers;
        }
        public static (DateTime value, bool ok) IsDate(string word)
        {
            var regex = new Regex(@"\d{1,2}\/\d{1,2}\/\d{4}|\d{4}\-\d{1,2}\-\d{1,2}|\d{1,2}\.\d{1,2}\.\d{4}|\d{1,2}\s+\w+\s+\d{4}");
            if (regex.Match(word).Success)
            {
                if (regex.Matches(word).Count >= 1)
                    if (DateTime.TryParseExact(word, new[] { "d/M/yyyy", "yyyy-M-d", "d.M.yyyy", "d MMM yyyy" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        return (date, true);

                    }
            }
            return (new DateTime(), false);
        }

        public static (TimeSpan value, bool ok) IsTime(string word)
        {
            var regex = new Regex(@"\d{1,2}:\d{1,2}(:\d{1,2})?\s*(AM|PM)?");
            if (regex.Match(word).Success)
            {
                if (TimeSpan.TryParseExact(word, new[] { "h:mm:ss tt", "h:mm tt", "H:mm:ss", "H:mm" },
                    CultureInfo.InvariantCulture, TimeSpanStyles.None, out var time))
                {
                    return (time, true);
                }
            }
            return (new TimeSpan(), false);
        }

        public static (double, int) FindNumberWithOrdinal(string text)
        {
            var regex = new Regex(@"\d{1,2}\/\d{1,2}\/\d{4}|\d{4}\-\d{1,2}\-\d{1,2}|\d{1,2}\.\d{1,2}\.\d{4}|\d{1,2}\s+\w+\s+\d{4}");
            var match = regex.Match(text);

            if (match.Success)
            {
                var numberString = match.Value.Replace(".", "").Replace(",", ".");

                if (double.TryParse(numberString, out double number))
                {
                    var ordinal = match.Index;
                    return (number, ordinal);
                }
            }

            return (double.NaN, -1);
        }

        public static (decimal value, bool ok) IsMoney(string word)
        {
            Regex regex = new Regex(@"[^0-9.,]");

            if (regex.IsMatch(word))
            {
                return (0, false);
            }
            else
            {
                if (!(word.Contains(".") || word.Contains(","))) return (0, false);
                string numberString = string.Empty;
                string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string thousandsSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                if (word.Contains("."))
                {
                    if (word.Split('.').Last().Length == 2)
                        numberString = word.Replace(",", "").Replace(".", decimalSeparator);
                }
                if (word.Contains(","))
                {
                    if (word.Split(',').Last().Length == 2)
                        numberString = word.Replace(".", "").Replace(",", decimalSeparator);
                }


                if (decimal.TryParse(numberString, out decimal parsedNumber))
                    return (parsedNumber, true);
                else
                    return (0, false);
            }
        }
        public static (double value, bool ok) IsDouble(string word)
        {
            Regex regex = new Regex(@"[^0-9.,]");

            if (regex.IsMatch(word))
            {
                return (0, false);
            }
            else
            {
                if (!(word.Contains(".") || word.Contains(","))) return (0, false);
                string numberString = string.Empty;
                string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string thousandsSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                if (word.Contains("."))
                {
                    if (word.Split('.').Last().Length == 3)
                        numberString = word.Replace(",", thousandsSeparator).Replace(".", decimalSeparator);
                }
                if (word.Contains(","))
                {
                    if (word.Split(',').Last().Length == 3)
                        numberString = word.Replace(".", thousandsSeparator).Replace(",", decimalSeparator);
                }
                if (word.Contains("."))
                {
                    if (word.Split('.').Last().Length > 3)
                        numberString = word.Replace(",", thousandsSeparator).Replace(".", decimalSeparator);
                }
                if (word.Contains(","))
                {
                    if (word.Split(',').Last().Length > 3)
                        numberString = word.Replace(".", thousandsSeparator).Replace(",", decimalSeparator);
                }


                if (double.TryParse(numberString, out double parsedNumber))
                    return (parsedNumber, true);
                else
                    return (0, false);
            }
        }
        public static (int value, bool ok) IsInt(string word)
        {
            Regex regex = new Regex(@"[^0-9]");

            if (regex.IsMatch(word))
            {
                return (0, false);
            }
            else
            {
                if (int.TryParse(word, out int parsedNumber))
                    return (parsedNumber, true);
                else
                    return (0, false);
            }
        }
        public static (double value, bool ok) IsPercent(string word)
        {

            Regex regex = new Regex(@"\b(\d+(\.\d+)?)%\b");

            if (regex.IsMatch(word))
            {
                return (Convert.ToDouble(word.Replace("%", "")), true);
            }
            else
            {
                return (0, false);
            }
        }
        public static (string value, bool ok) IsTransaction(string word)
        {

            string pattern = @"\b(?=\w*[A-Za-z])(?=\w*\d)[A-Za-z0-9]+\b";

            if (Regex.Match(word, pattern).Success)
            {
                return (word, true);
            }
            else
            {
                return (word, false);
            }
        }
        public static bool IsCurrency(string word)
        {
            string[] m = new string[] { "TL", "EUR", "EURO", "USD", "GPB", "AUD", "CHF", "JPY" };
            if (m.Contains(word.ToUpper()))
                return true;
            else
                return false;
        }

        public static void WriteLine(string s, ConsoleColor consoleColor = ConsoleColor.DarkCyan, int delay = 10)
        {
            if (s == null) return;
            if (consoleColor != ConsoleColor.DarkCyan)
                Console.ForegroundColor = consoleColor;
            foreach (char c in s)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
        public static void Write(string s, ConsoleColor consoleColor = ConsoleColor.DarkCyan, int delay = 10)
        {
            if (consoleColor != ConsoleColor.DarkCyan)
                Console.ForegroundColor = consoleColor;
            foreach (char c in s)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }

        }
        public static string ReadLine()
        {
            return Console.ReadLine();

        }
        static void BankaIslemi(string tarih, string banka, string alici, string tutar, string islem)
        {
            // Verileri dosyaya yaz
            string path = @"C:\banka_islemleri.txt";
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine($"{tarih} | {banka} | {alici} | {tutar} | {islem}");
            }

            // Önerileri dosyadan oku ve karşılaştır
            string suggestionPath = @"C:\banka_islem_onerileri.txt";
            string[] lines = File.ReadAllLines(suggestionPath);
            List<string> suggestions = new List<string>(lines);

            foreach (string suggestion in suggestions)
            {
                string[] parts = suggestion.Split('|');
                string suggestionTarih = parts[0];
                string suggestionBanka = parts[1];
                string suggestionAlici = parts[2];
                string suggestionTutar = parts[3];
                string suggestionIslem = parts[4];

                if (suggestionTarih == tarih && suggestionBanka == banka && suggestionAlici == alici && suggestionTutar == tutar && suggestionIslem == islem)
                {
                    Console.WriteLine("Benzer bir işlem yapılmış. Şu öneriyi düşünebilirsin: " + suggestion);
                    return;
                }
            }

            // Öneri bulunamadıysa yeni bir öneri oluştur ve dosyaya yaz
            string newSuggestion = $"{tarih} | {banka} | {alici} | {tutar} | {islem}";
            using (StreamWriter sw = File.AppendText(suggestionPath))
            {
                sw.WriteLine(newSuggestion);
            }

            Console.WriteLine("Yeni bir işlem yapıldı ve öneri kaydedildi: " + newSuggestion);
        }
    }
}

