using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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



   
        private void GenerateChart_Click(object sender, RoutedEventArgs e)
        {
            
            var x = Helpers.CountCharacters(loaded_text.Text);
            Helpers.LoadDataToColumnSeries((DataPointSeries)mcChart.Series[0], x);
            tabItems.SelectedItem = ChartTabItem;

        }

        private void CountAll_Click(object sender, RoutedEventArgs e)
        {
            char_num.Text = loaded_text.Text.Length.ToString();
            var counted = Helpers.CountCharacters(loaded_text.Text);
            charactersTable.ItemsSource = Helpers.ReplaceWhiteCharactersWithName(counted);

            entropy.Text= Helpers.Entropy(loaded_text.Text).ToString();
        }
    }
}
