using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace Lossless
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static String ReadingFromFile(String path)
        {
            string s = File.ReadAllText(path, Encoding.Default);
            return s;
        }

        private void LoadText_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string path = dlg.FileName;
                loaded_text.Text = ReadingFromFile(path);
            }
        }

        static int CountNonSpaceChars(String value)
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

   
        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder(loaded_text.Text);
            char_num.Text = CountNonSpaceChars(builder.ToString()).ToString();
            var x = Helpers.CountCharacters(loaded_text.Text);
            Helpers.LoadDataToColumnSeries((DataPointSeries)mcChart.Series[0], x);
          
        }
    }
}
