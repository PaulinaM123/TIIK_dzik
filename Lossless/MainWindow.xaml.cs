﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Lossless.Backend;
using Microsoft.Win32;

namespace Lossless
{
    public partial class MainWindow : Window
    {
        private DiagramWindow diagramWindow;
        private byte[] _FileToCompress;
        private byte[] __CompressedFile;
        private byte[] _FileToDecompress;
        private byte[] __DecompressedFile;
        private SFCompression _SFCompression;
        private SFDecompression _SFDecompression;

        public MainWindow()
        {
            InitializeComponent();
            CreateAndSetupSFInstances();       
        }

        #region configuration

        private void CreateAndSetupSFInstances()
        {
            _SFCompression = new SFCompression();
            _SFCompression.UpdateEvent += CompressLoading;
            _SFCompression.CompleteEvent += CompressComplete;

            _SFDecompression = new SFDecompression();
            _SFDecompression.UpdateEvent += DecompressLoading;
            _SFDecompression.CompleteEvent += DecompressComplete;
        }

        #endregion

        #region windowFunctions

        private void LoadFileStatisticView(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document"; 
            dlg.DefaultExt = ".txt"; 
            dlg.Filter = "Text documents (.txt)|*.txt"; 

            if (dlg.ShowDialog() == true)
            {
                loaded_text.Text = File.ReadAllText(dlg.FileName, Encoding.Default);
            }
        }
        
        private void CountCharactersStatisticView(object sender, RoutedEventArgs e)
        {
            //all characters
            char_num.Text = loaded_text.Text.Length.ToString();

            //loading characters to table
            var counted = Helpers.CountCharacters(loaded_text.Text);
            charactersTable.ItemsSource = Helpers.ReplaceWhiteCharactersWithName(counted);

            //unic characters
            distinct_txtbox.Text = counted.Count.ToString();

            //entropy
            entropy.Text= Helpers.Entropy(loaded_text.Text).ToString(); 
        }

        private void ShowChart(object sender, RoutedEventArgs e)
        {
            if (diagramWindow != null) diagramWindow.Close();
            diagramWindow = new DiagramWindow(loaded_text.Text);
            diagramWindow.Show();
        }

        #endregion

        private void CompressFile(object sender, RoutedEventArgs e)
        {
            if (_FileToCompress == null)
            {
                MessageBox.Show("Can't Compress. File not loaded");
            }
            else
            {
                byte[] compressedData = new byte[(uint)_FileToCompress.Length * 2];
               
                Task.Run(() => {

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    int compressedSize = _SFCompression.Compress(_FileToCompress, compressedData, (uint)_FileToCompress.Length);
                    sw.Stop();
                    __CompressedFile = new byte[compressedSize];
                    Array.Copy(compressedData, __CompressedFile, compressedSize);

                    Dispatcher.Invoke(() => {
                        c_CompressionTime.Content = sw.ElapsedMilliseconds + "ms";
                        c_SizeBeforeCompression.Content = _FileToCompress.Length + " bytes";
                        c_SizeAfterCompression.Content = __CompressedFile.Length + " bytes";
                        c_CompressionDegree.Content = (float)_FileToCompress.Length / (float)__CompressedFile.Length;
                    });
                });
            }  
        }

        private void DecompressFile(object sender, RoutedEventArgs e)
        {
            if (_FileToDecompress == null)
            {
                MessageBox.Show("Can't Decompress. File not loaded");
            }
            else
            {
                Task.Run(() => {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    __DecompressedFile = _SFDecompression.Decompress(_FileToDecompress); 
                    sw.Stop();
                    if(__DecompressedFile != null)
                    {
                        Dispatcher.Invoke(() => {
                            d_SizeBeforeDecompression.Content = _FileToDecompress.Length + " bytes";
                            d_SizeAfterDecompression.Content = __DecompressedFile.Length + " bytes";
                            d_DecompressionTime.Content = sw.ElapsedMilliseconds + "ms";
                            d_DecompressionDegree.Content = (float)_FileToDecompress.Length / (float)__DecompressedFile.Length;
                        });
                    }
                });
            }
        }

        #region LoadFile

        private void LoadFileToCompress(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _FileToCompress = File.ReadAllBytes(openFileDialog.FileName);
                AddToListFileToCompress(openFileDialog.SafeFileName, _FileToCompress.Length + " Bytes");
            }
        }

        private void LoadFileToDecompress(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _FileToDecompress = File.ReadAllBytes(openFileDialog.FileName);
                AddToListFileToDecompress(openFileDialog.SafeFileName, _FileToDecompress.Length + " Bytes");
            }
        }

        #endregion

        #region SaveFile

        public void SaveCompressedFile(object sender, RoutedEventArgs e)
        {
            if (__CompressedFile == null)
            {
                MessageBox.Show("Please load and compress the file");
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, __CompressedFile);
            }
        }

        public void SaveDecompressedFile(object sender, RoutedEventArgs e)
        {
            if (__DecompressedFile == null)
            {
                MessageBox.Show("Please load and decompress the file");
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, __DecompressedFile);
            }
        }

        #endregion

        #region events

        private void CompressLoading(float counter, float max)
        {
            Dispatcher.Invoke(() =>
            {
                var percents = (int)((counter * 100) / max);
                CompressionProgressBar.Value = percents;
                CompressionProgressBarLabel.Content = percents + "% ";
            });
        }

        private void CompressComplete()
        {
            Dispatcher.Invoke(() =>
            {
                CompressionProgressBar.Value = 100.0;
                CompressionProgressBarLabel.Content = 100 + "% ";
                SaveFileCompress.IsEnabled = true;
            });
        }

        private void DecompressLoading(float counter, float max)
        {
            Dispatcher.Invoke(() =>
            {
                var percents = (int)((counter * 100) / max);
                DecompressionProgressBar.Value = percents;
                DecompressionProgressBarLabel.Content = percents + "% ";
            });
        }

        private void DecompressComplete()
        {
            Dispatcher.Invoke(() =>
            {
                DecompressionProgressBar.Value = 100.0;
                DecompressionProgressBarLabel.Content = 100 + "% ";
                SaveFileDecompress.IsEnabled = true;
            });
        }

        #endregion

        #region LoadingsToLists

        private void AddToListFileToCompress(string FileName, string FileSize)
        {
            FilesToCompress.ItemsSource = new List<FileDto>() {
                new FileDto() { Name = FileName, Size = FileSize }
            };
        }
        private void AddToListFileToDecompress(string FileName, string FileSize)
        {
            FilesToDecompress.ItemsSource = new List<FileDto>() {
                 new FileDto() { Name = FileName, Size = FileSize }
            };
        }

        #endregion
    }
}
