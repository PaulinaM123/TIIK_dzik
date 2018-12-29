using System;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace Lossless
{
    public partial class MainWindow : Window
    {
        private byte[] loadedFileToCompress;
        private byte[] loadedFileToDecompress;
        private byte[] compressed;
        private byte[] decompressed;
        DiagramWindow diagramWindow;

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
        
        private void CountAll_Click(object sender, RoutedEventArgs e)
        {
            char_num.Text = loaded_text.Text.Length.ToString();
            var counted = Helpers.CountCharacters(loaded_text.Text);
            charactersTable.ItemsSource = Helpers.ReplaceWhiteCharactersWithName(counted);

            entropy.Text= Helpers.Entropy(loaded_text.Text).ToString();
            distinct_txtbox.Text = counted.Count.ToString();
        }

        private void LoadFile_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog()==true)
            {
                loadedFileToCompress = File.ReadAllBytes(fileDialog.FileName);
                FileName_label.Content = "File name: "+fileDialog.SafeFileName;
                FileSize_label.Content = "File size: "+loadedFileToCompress.Length;
            }     
        }

        private void Compress_btn_Click(object sender, RoutedEventArgs e)
        {
            if (loadedFileToCompress == null)
            {
                MessageBox.Show("No file loaded!");
            }
            
            byte[] originalData = loadedFileToCompress;
            uint originalDataSize = (uint)loadedFileToCompress.Length;
            byte[] compressedData = new byte[originalDataSize * (101 / 100) + 384];

            compressed = compressedData;
            int compressedSize = ShanoFanoCompression.Compress(originalData, compressedData, originalDataSize);

            compressedFileSize_label.Content = "Compressed file size: " + compressedSize;

            SaveFileDialog fileDialog = new SaveFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(fileDialog.FileName,compressedData);
            }
        }

        private void LoadFileDecompress_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == true)
            {
                loadedFileToDecompress = File.ReadAllBytes(fileDialog.FileName);
                FileNameDcmps_label.Content = "File name: " + fileDialog.SafeFileName;
                FileSizeDcmps_label.Content = "File size: " + loadedFileToCompress.Length;
            }
        }

        private void Decompress_btn_Click(object sender, RoutedEventArgs e)
        {
            uint originalDataSize = (uint)loadedFileToDecompress.Length;
            byte[] decompressedData = new byte[loadedFileToDecompress.Length];

            ShanoFanoDecompression.Decompress(compressed, decompressedData, (uint)compressed.Length, originalDataSize);

            compressedFileSize_label.Content = "Decompressed file size: " + decompressedData.Length;
        }

        public void ShowChart(object sender, RoutedEventArgs e)
        {
            diagramWindow = new DiagramWindow(loaded_text.Text);
            diagramWindow.Show();
        }
    }
}
