using Microsoft.Win32;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DisEn
{
    public partial class MainWindow : Window
    {
        private Disassembler _disassembler;

        public MainWindow()
        {
            InitializeComponent();

            _disassembler = new Disassembler();
        }
        #region File
        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                // Disassemble given file
                _disassembler.Disassemble(openFileDialog.FileName);
                // Show path to file
                PathToFileTextBlock.Text = openFileDialog.FileName;
                // Show total amount of instructions
                TotalNumOfCommandsTextBlock.Text = _disassembler.GetTotalInstructionCounter().ToString();
                // Show size of file
                FileInfo fileSize = new FileInfo(openFileDialog.FileName);
                double MB = fileSize.Length / 1048576.0;
                /*double MB = (fileSize.Length / 1024) / 1024;*/
                SizeFile.Text = MB.ToString("0.###") + " MB";
                // Get commands info
                List<DisassemblerCommandInfo> commandInfos = _disassembler.GetCommandsInfo();
                DisassemblerDataGrid.ItemsSource = commandInfos;

                ColumnSeries series = new ColumnSeries();
                series.ItemsSource = commandInfos;
                series.XBindingPath = "Name";
                series.YBindingPath = "Entropy";
                Histogram.Series.Add(series);
            }
        }
        private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveData = new SaveFileDialog();
            saveData.Filter = "Текст (*.txt)|*.txt";
            if (saveData.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(saveData.OpenFile(), Encoding.UTF8))
                {
                    sw.Write("Path to file: " + PathToFileTextBlock.Text);
                    sw.Write("\nTotal amout of commands: " + TotalNumOfCommandsTextBlock.Text);
                    sw.Write("\nSize of file: " + SizeFile.Text);

                    DisassemblerDataGrid.SelectAllCells();
                    DisassemblerDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                    ApplicationCommands.Copy.Execute(null, DisassemblerDataGrid);
                    DisassemblerDataGrid.UnselectAllCells();
                    string result = Clipboard.GetText(TextDataFormat.Text);
                    Clipboard.Clear();
                    sw.WriteLine("\n" + result);

                    sw.Close();
                }
            }
        }
        #endregion

        #region Cleaner
        private void BtnClearHistogram(object sender, RoutedEventArgs e)
        {   // Clean histogram data
            Histogram.Series.Clear();
        }
        // IN PROCESS
        private void BtnSaveHistogramImage(object sender, RoutedEventArgs e)
        {
            // Make screenshot of program   

            //Desktop (-_-) screenshot
            string filename = "ScreenCapture-" + DateTime.Now.ToString("dd.MM.yyyy-hh.mm.ss") + ".png";
            //string filename = "ScreenCapture-" + DateTime.Now.ToString("U",CultureInfo.CreateSpecificCulture("eu-US")) + ".png";
            // Size
            int screenLeft = (int)SystemParameters.VirtualScreenLeft;
            int screenTop = (int)SystemParameters.VirtualScreenTop;
            int screenWidth = (int)SystemParameters.VirtualScreenWidth;
            int screenHeight = (int)SystemParameters.VirtualScreenHeight;

            Bitmap bitmap_Screen = new Bitmap(screenWidth, screenHeight);
            Graphics g = Graphics.FromImage(bitmap_Screen);

            g.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap_Screen.Size);

            bitmap_Screen.Save(filename);

            //UI Histogram screenshot
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string imageFilesFilter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg,*.jpeg)|*.jpg;*.jpeg|Gif (*.gif)|*.gif|TIFF(*.tiff)|*.tiff|PNG(*.png)|*.png|WDP(*.wdp)|*.wdp|Xps file (*.xps)|*.xps|All files (*.*)|*.*";
            saveFileDialog.Filter = imageFilesFilter;
            //saveFileDialog.FileName = "ScreenCapture-" + DateTime.Now;   
            string name = Path.GetFileName(PathToFileTextBlock.Text);
            saveFileDialog.FileName = "ScreenCapture-" + name + " " + DateTime.Now.ToString("dd.MM.yyyy-hh.mm.ss");

            if (saveFileDialog.ShowDialog() == true)
            {
                Histogram.Save(saveFileDialog.FileName);
            }

        }
        #endregion
    }
}