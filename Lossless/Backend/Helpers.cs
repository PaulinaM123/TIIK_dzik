using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization.Charting;

namespace Lossless
{
    public static class Helpers
    {
        public static Dictionary<char, int> CountCharacters(string text)
        {
            Dictionary<char, int> dict = new Dictionary<char, int>();

            for (int i = 0; i < text.Length; i++)
            {
                if (dict.ContainsKey(text[i]))
                {
                    dict[text[i]]++;
                }
                else
                {
                    dict.Add(text[i], 1);
                }
            }
            return dict;
        }

        public static Dictionary<string,int> ReplaceWhiteCharactersWithName(Dictionary<char, int> dictionary)
        {
            Dictionary<string, int> seriesDictionary = new Dictionary<string, int>();

            foreach (var pair in dictionary)
            {
                if (pair.Key == ' ')
                {
                    seriesDictionary.Add("space", pair.Value);
                }
                else if (pair.Key == '\n')
                {
                    seriesDictionary.Add("\\n", pair.Value);
                }
                else if (pair.Key == '\r')
                {
                    seriesDictionary.Add("\\r", pair.Value);
                }
                else if (pair.Key == '\t')
                {
                    seriesDictionary.Add("\\t", pair.Value);
                }
                else
                {
                    seriesDictionary.Add(pair.Key.ToString(), pair.Value);
                }
            }
            return seriesDictionary;
        }


        public static void LoadDataToColumnSeries(DataPointSeries pointSeries, Dictionary<char, int> dictionary)
        {
            //generowanie wykresu
            //przepisanie słownika do nowego typu <string,int>
            //znaki białe zostają zastąpione napisem np \n jako "nowa linia", " " jako "spacja" itd
            var seriesDictionary = ReplaceWhiteCharactersWithName(dictionary);
            pointSeries.ItemsSource = seriesDictionary.ToList().OrderBy(x => x.Key);
        }

        public static int CountNonSpaceChars(String value)
        {
            int result = 0;
            foreach (char c in value)
            {
                if (!char.IsWhiteSpace(c))
                {
                    result++;
                }
            }
            return result;
        }

        public static double Entropy(string s)
        {
            var map = new Dictionary<char, int>();
            foreach (char c in s)
            {
                if (!map.ContainsKey(c))
                    map.Add(c, 1);
                else
                    map[c] += 1;
            }

            double result = 0.0;
            int len = s.Length;
            foreach (var item in map)
            {
                var frequency = (double)item.Value / len;
                result -= frequency * (Math.Log(frequency) / Math.Log(2));
            }

            return result;
        }
    }
}
