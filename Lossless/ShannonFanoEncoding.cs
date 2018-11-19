using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lossless
{
    class ShannonFanoEncoding
    {

        public string Text { get; set; }

        public ShannonFanoEncoding(string text)
        {
            this.Text = text;
        }

        public Dictionary<char, int> CountFrequency(string text)
        {
            Dictionary<char, int> dict = new Dictionary<char, int>();
            int textLength = text.Length;


            for (int i = 0; i < textLength; i++)
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

        public Dictionary<char,int> SortByFrequency(Dictionary<char, int> dictionary)
        {
            var items = from pair in dictionary
                orderby
                    pair.Value descending
                select pair;

            return items.ToDictionary(pair=> pair.Key,pair=>pair.Value);
        }

        public void SplitCollection(Dictionary<char, int> dictionary)
        {
            int total = 0;
            int sum = 0;
            foreach (var pair in dictionary)
            {
                total += pair.Value;
            }
            
            int breakpoint = Text.Length / 2;
            
            foreach (var pair in dictionary)
            {
                
            }

        }


    }
}
