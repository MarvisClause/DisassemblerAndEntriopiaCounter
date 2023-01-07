using Microsoft.Win32;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DisEn
{
    public partial class MainWindow : Window
    {
        private DisassemblerManager _disassemblerManager;

        private DisassemblerComparator _disassemblerComparator;

        public MainWindow()
        {
            InitializeComponent();
            // Initialize disassembler
            _disassemblerManager = new DisassemblerManager();
            _disassemblerComparator = new DisassemblerComparator();
            // Hide additional interface
            SetCastDataVisibility(false);
        }

        #region File

        private void OpenFileAction(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                // Disassemble given file
                _disassemblerManager.Disassemble(openFileDialog.FileName);
                // Show path to file
                PathToFileTextBlock.Text = openFileDialog.FileName;
                // Show total amount of instructions
                TotalNumOfCommandsTextBlock.Text = _disassemblerManager.GetCurrentDisassembler().GetTotalInstructionCounter().ToString();
                // Show size of file
                SizeFile.Text = ByteConverter.ConvertByToMegaByteToString(_disassemblerManager.GetCurrentDisassembler().GetFileSize());

                // Add info to the interface
                CurrentDisassemblerHistogram.Series.Clear();
                AddDataToTable(CurrentDisassemblerDataGrid, _disassemblerManager.GetCurrentDisassembler().GetDisassemblerCommandsInfo());
                AddDataToHistogram(CurrentDisassemblerHistogram, _disassemblerManager.GetCurrentDisassembler().GetDisassemblerCommandsInfo());

                if (!_disassemblerManager.GetCurrentDisassembler().Equals(_disassemblerManager.GetSavedDisassembler()))
                {
                    SetCastDataVisibility(true);

                    // Show total amount of instructions
                    TotalNumOfCommandsCastTextBlock.Text = _disassemblerManager.GetSavedDisassembler().GetTotalInstructionCounter().ToString();
                    // Show size of file
                    SizeCastFile.Text = ByteConverter.ConvertByToMegaByteToString(_disassemblerManager.GetSavedDisassembler().GetFileSize());

                    CastDisassemblerHistogram.Series.Clear();
                    AddDataToTable(SavedDisassemblerDataGrid, _disassemblerManager.GetSavedDisassembler().GetDisassemblerCommandsInfo());
                    AddDataToHistogram(CastDisassemblerHistogram, _disassemblerManager.GetSavedDisassembler().GetDisassemblerCommandsInfo());

                    // If data is not equal, inform user about this
                    if (!_disassemblerComparator.CompareData(_disassemblerManager.GetCurrentDisassembler(), _disassemblerManager.GetSavedDisassembler()))
                    {
                        OpenStatisticWindowAction(sender, e);
                    }
                }
                else
                {
                    SetCastDataVisibility(false);
                }
            }
        }


        private void SaveFileAction(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveData = new SaveFileDialog();
            saveData.Filter = "Текст (*.txt)|*.txt";
            if (saveData.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(saveData.OpenFile(), Encoding.UTF8))
                {
                    sw.Write("Path to file: " + PathToFileTextBlock.Text);
                    sw.Write("\nTotal amount of commands: " + TotalNumOfCommandsTextBlock.Text);
                    sw.Write("\nSize of file: " + SizeFile.Text);

                    CurrentDisassemblerDataGrid.SelectAllCells();
                    CurrentDisassemblerDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                    ApplicationCommands.Copy.Execute(null, CurrentDisassemblerDataGrid);
                    CurrentDisassemblerDataGrid.UnselectAllCells();
                    string resultCurrent = Clipboard.GetText(TextDataFormat.Text);
                    Clipboard.Clear();

                    sw.WriteLine("\n" + resultCurrent);

                    if (CastDisassemblerHistogram.IsVisible)
                    {
                        sw.Write("Cast file: " + _disassemblerManager.GetSavedDisassembler().GetFileName());
                        sw.Write("\nTotal amount of commands: " + TotalNumOfCommandsCastTextBlock.Text);
                        sw.Write("\nSize of file: " + SizeCastFile.Text);

                        CurrentDisassemblerDataGrid.SelectAllCells();
                        CurrentDisassemblerDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                        ApplicationCommands.Copy.Execute(null, CurrentDisassemblerDataGrid);
                        CurrentDisassemblerDataGrid.UnselectAllCells();
                        string result = Clipboard.GetText(TextDataFormat.Text);
                        Clipboard.Clear();

                        sw.WriteLine("\n" + result);
                    }

                    sw.Close();
                }
            }
        }

        private void SaveHistogramAction(object sender, RoutedEventArgs e)
        {
            // Make screenshot of program   

            //Desktop screenshot
            string filename = "ScreenCapture-" + DateTime.Now.ToString("dd.MM.yyyy-hh.mm.ss") + ".png";
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
                CurrentDisassemblerHistogram.Save(saveFileDialog.FileName);
                if (CastDisassemblerHistogram.IsVisible)
                {
                    saveFileDialog.FileName = "ScreenCapture-" + name + " " + DateTime.Now.ToString("dd.MM.yyyy-hh.mm.ss") + "_CAST";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        CastDisassemblerHistogram.Save(saveFileDialog.FileName);
                    }
                }
            }
        }

        private void UpdateCastAction(object sender, RoutedEventArgs e)
        {
            // Save current disassembler
            _disassemblerManager.SaveCurrentDisassemblerFile();
            // Show information about result
            string messageBoxText = "Cast was updated";
            string caption = "Disassembler information";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);
            // Hide update button
            UpdateCastBorder.Visibility = Visibility.Collapsed;
        }

        private void AddDataToTable(DataGrid dataGrid, List<DisassemblerCommandInfo> commandsInfo)
        {
            dataGrid.ItemsSource = commandsInfo;
        }

        private void AddDataToHistogram(SfChart histogram, List<DisassemblerCommandInfo> commandInfos)
        {
            ColumnSeries currentSeries = new ColumnSeries();

            currentSeries.ItemsSource = commandInfos;
            currentSeries.XBindingPath = "Name";
            currentSeries.YBindingPath = "Entropy";

            ChartSeriesBase.SetSpacing(currentSeries, 0.0f);

            histogram.Series.Add(currentSeries);
        }

        private void SetCastDataVisibility(bool IsVisibile)
        {
            if (IsVisibile)
            {
                SavedDisassemblerDataGrid.Visibility =
                    UpdateCastBorder.Visibility =
                    CastDisassemblerHistogram.Visibility =
                    CastFileInfoGrid.Visibility = Visibility.Visible;
                FileInfoUniformGrid.Rows = HistogramUniformGrid.Rows = 2;
                StatisticMenuItem.IsEnabled = true;
            }
            else
            {
                SavedDisassemblerDataGrid.Visibility =
                    UpdateCastBorder.Visibility =
                CastDisassemblerHistogram.Visibility =
                CastFileInfoGrid.Visibility = Visibility.Collapsed;
                FileInfoUniformGrid.Rows = HistogramUniformGrid.Rows = 1;
                StatisticMenuItem.IsEnabled = false;
            }
        }

        #endregion

        #region Cleaner

        // Clear temp
        private void ClearTempDirectoryAction(object sender, RoutedEventArgs e)
        {
            DirectoryInfo folder = new DirectoryInfo("Temp\\");

            foreach (FileInfo file in folder.GetFiles())
            {
                file.Delete();
            }
        }

        #endregion

        #region Open Window  

        // Manual opening 
        private void OpenStatisticWindowAction(object sender, RoutedEventArgs e)
        {
            InspectionStatisticWindow inspectionStatisticWindow = new InspectionStatisticWindow(_disassemblerComparator);
            inspectionStatisticWindow.Show();
        }

        #endregion
    }
}