using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization.Charting;

namespace Lossless
{
    public static class Helpers
    {
        static string DeleteNonChars(String value)
        {
            value = value.Replace("\n", String.Empty);
            value = value.Replace("\r", String.Empty);
            value = value.Replace("\t", String.Empty);
            value = value.Replace(" ", String.Empty);

            return value;
        }

        public static Dictionary<char, int> CountCharacters(string text)
        {
            Dictionary<char, int> dict = new Dictionary<char, int>();

            text = DeleteNonChars(text);
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

        public static  void LoadDataToColumnSeries(DataPointSeries pointSeries, Dictionary<char, int> dictionary)
        {
            pointSeries.ItemsSource = dictionary.ToList().OrderBy(x => x.Key);
        }

        
    }
}
