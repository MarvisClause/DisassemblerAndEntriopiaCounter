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
using System.Runtime.InteropServices;
using System.Windows.Interop;

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
            //SetCastDataVisibility(false);


            
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr wParam = new IntPtr(2); // Convert 2 to IntPtr
            IntPtr lParam = IntPtr.Zero;   // Use IntPtr.Zero for the third parameter
            SendMessage(helper.Handle, 161, wParam, lParam);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        #region File

        //private void OpenFileAction(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "All files (*.*)|*.exe";
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        // Disassemble given file
        //        _disassemblerManager.Disassemble(openFileDialog.FileName);
        //        // Show path to file
        //        PathToFileTextBlock.Text = openFileDialog.FileName;
        //        // Show total amount of instructions
        //        TotalNumOfCommandsTextBlock.Text = _disassemblerManager.GetCurrentDisassembler().GetTotalInstructionCounter().ToString();
        //        // Show size of file
        //        SizeFile.Text = ByteConverter.ConvertByToMegaByteToString(_disassemblerManager.GetCurrentDisassembler().GetFileSize());

        //        // Add info to the interface
        //        CurrentDisassemblerHistogram.Series.Clear();
        //        AddDataToTable(CurrentDisassemblerDataGrid, _disassemblerManager.GetCurrentDisassembler().GetDisassemblerCommandsInfo());
        //        AddDataToHistogram(CurrentDisassemblerHistogram, _disassemblerManager.GetCurrentDisassembler().GetDisassemblerCommandsInfo());

        //        if (!_disassemblerManager.GetCurrentDisassembler().Equals(_disassemblerManager.GetSavedDisassembler()))
        //        {
        //            SetCastDataVisibility(true);

        //            // Show total amount of instructions
        //            TotalNumOfCommandsCastTextBlock.Text = _disassemblerManager.GetSavedDisassembler().GetTotalInstructionCounter().ToString();
        //            // Show size of file
        //            SizeCastFile.Text = ByteConverter.ConvertByToMegaByteToString(_disassemblerManager.GetSavedDisassembler().GetFileSize());

        //            CastDisassemblerHistogram.Series.Clear();
        //            AddDataToTable(SavedDisassemblerDataGrid, _disassemblerManager.GetSavedDisassembler().GetDisassemblerCommandsInfo());
        //            AddDataToHistogram(CastDisassemblerHistogram, _disassemblerManager.GetSavedDisassembler().GetDisassemblerCommandsInfo());

        //            // If data is not equal, inform user about this
        //            if (!_disassemblerComparator.CompareData(_disassemblerManager.GetCurrentDisassembler(), _disassemblerManager.GetSavedDisassembler()))
        //            {
        //                // Check data through analyzer for the authorship

        //                // #################################################### TEST VALUES ####################################################

        //                DisassemblerAnalyzer disassemblerAnalyzer = new DisassemblerAnalyzer();

        //                Dictionary<string, double> commandThresholdFilter = new Dictionary<string, double>();
        //                commandThresholdFilter.Add("inc", 0.0060935);
        //                commandThresholdFilter.Add("dec", 0.003465);
        //                commandThresholdFilter.Add("neg", 0.0012695);
        //                commandThresholdFilter.Add("jmp", 0.0085135);
        //                commandThresholdFilter.Add("call", 0.0151115);
        //                commandThresholdFilter.Add("ret", 0.005825);
        //                commandThresholdFilter.Add("sub", 0.007995);
        //                commandThresholdFilter.Add("sbb", 0.0040975);
        //                commandThresholdFilter.Add("cmp", 0.01059);
        //                commandThresholdFilter.Add("add", 0.021305);
        //                commandThresholdFilter.Add("and", 0.010315);
        //                commandThresholdFilter.Add("or", 0.0058755);
        //                commandThresholdFilter.Add("xor", 0.0055585);
        //                commandThresholdFilter.Add("not", 0.0011655);
        //                commandThresholdFilter.Add("test", 0.0049205);
        //                commandThresholdFilter.Add("push", 0.0064165);
        //                commandThresholdFilter.Add("pop", 0.005588);
        //                commandThresholdFilter.Add("nop", 0.0087785);
        //                commandThresholdFilter.Add("jl", 0.003693);
        //                commandThresholdFilter.Add("jle", 0.0015375);
        //                commandThresholdFilter.Add("jge", 0.001325);
        //                commandThresholdFilter.Add("jg", 0.0012025);
        //                commandThresholdFilter.Add("jb", 0.0083675);
        //                commandThresholdFilter.Add("jbe", 0.0027105);
        //                commandThresholdFilter.Add("jae", 0.0090315);
        //                commandThresholdFilter.Add("ja", 0.0010915);
        //                commandThresholdFilter.Add("je", 0.0044205);
        //                commandThresholdFilter.Add("jne", 0.0045995);
        //                commandThresholdFilter.Add("js", 0.0024585);
        //                commandThresholdFilter.Add("jns", 0.003481);
        //                commandThresholdFilter.Add("int", 0.013317);
        //                commandThresholdFilter.Add("mov", 0.0128165);
        //                commandThresholdFilter.Add("xchg", 0.0023355);
        //                commandThresholdFilter.Add("lea", 0.0132795);

        //                int discrepancyCriterion = disassemblerAnalyzer.CalculateDiscrepancyCriterion(commandThresholdFilter, _disassemblerComparator);

        //                // Show information about result
        //                string messageBoxText;
        //                string caption;
        //                MessageBoxImage icon;
        //                if (discrepancyCriterion > commandThresholdFilter.Count / 2)
        //                {
        //                    messageBoxText = "There is a possibility of virus code injection";
        //                    caption = "Discrepancy value is high";
        //                    icon = MessageBoxImage.Warning;
        //                }
        //                else
        //                {
        //                    messageBoxText = "Discrepancy value is in normal range";
        //                    caption = "Code was changed by user";
        //                    icon = MessageBoxImage.Information;
        //                }
        //                MessageBoxButton button = MessageBoxButton.OK;
        //                MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);

        //                // #####################################################################################################################

        //                OpenStatisticWindowAction(sender, e);
        //            }
        //        }
        //        else
        //        {
        //            SetCastDataVisibility(false);
        //        }
        //    }
        //}


        //private void SaveFileAction(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog saveData = new SaveFileDialog();
        //    saveData.Filter = "Текст (*.txt)|*.txt";
        //    if (saveData.ShowDialog() == true)
        //    {
        //        using (StreamWriter sw = new StreamWriter(saveData.OpenFile(), Encoding.UTF8))
        //        {
        //            sw.Write("Path to file: " + PathToFileTextBlock.Text);
        //            sw.Write("\nTotal amount of commands: " + TotalNumOfCommandsTextBlock.Text);
        //            sw.Write("\nSize of file: " + SizeFile.Text);

        //            CurrentDisassemblerDataGrid.SelectAllCells();
        //            CurrentDisassemblerDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
        //            ApplicationCommands.Copy.Execute(null, CurrentDisassemblerDataGrid);
        //            CurrentDisassemblerDataGrid.UnselectAllCells();
        //            string resultCurrent = Clipboard.GetText(TextDataFormat.Text);
        //            Clipboard.Clear();

        //            sw.WriteLine("\n" + resultCurrent);

        //            if (CastDisassemblerHistogram.IsVisible)
        //            {
        //                sw.Write("Cast file: " + _disassemblerManager.GetSavedDisassembler().GetFileName());
        //                sw.Write("\nTotal amount of commands: " + TotalNumOfCommandsCastTextBlock.Text);
        //                sw.Write("\nSize of file: " + SizeCastFile.Text);

        //                CurrentDisassemblerDataGrid.SelectAllCells();
        //                CurrentDisassemblerDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
        //                ApplicationCommands.Copy.Execute(null, CurrentDisassemblerDataGrid);
        //                CurrentDisassemblerDataGrid.UnselectAllCells();
        //                string result = Clipboard.GetText(TextDataFormat.Text);
        //                Clipboard.Clear();

        //                sw.WriteLine("\n" + result);
        //            }

        //            sw.Close();
        //        }
        //    }
        //}

        //private void SaveHistogramAction(object sender, RoutedEventArgs e)
        //{
        //    // Make screenshot of program   

        //    //Desktop screenshot
        //    string filename = "ScreenCapture-" + DateTime.Now.ToString("dd.MM.yyyy-hh.mm.ss") + ".png";
        //    // Size
        //    int screenLeft = (int)SystemParameters.VirtualScreenLeft;
        //    int screenTop = (int)SystemParameters.VirtualScreenTop;
        //    int screenWidth = (int)SystemParameters.VirtualScreenWidth;
        //    int screenHeight = (int)SystemParameters.VirtualScreenHeight;

        //    Bitmap bitmap_Screen = new Bitmap(screenWidth, screenHeight);
        //    Graphics g = Graphics.FromImage(bitmap_Screen);

        //    g.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap_Screen.Size);

        //    bitmap_Screen.Save(filename);

        //    //UI Histogram screenshot
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    string imageFilesFilter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg,*.jpeg)|*.jpg;*.jpeg|Gif (*.gif)|*.gif|TIFF(*.tiff)|*.tiff|PNG(*.png)|*.png|WDP(*.wdp)|*.wdp|Xps file (*.xps)|*.xps|All files (*.*)|*.*";
        //    saveFileDialog.Filter = imageFilesFilter;
        //    //saveFileDialog.FileName = "ScreenCapture-" + DateTime.Now;   
        //    string name = Path.GetFileName(PathToFileTextBlock.Text);
        //    saveFileDialog.FileName = "ScreenCapture-" + name + " " + DateTime.Now.ToString("dd.MM.yyyy-hh.mm.ss");

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        CurrentDisassemblerHistogram.Save(saveFileDialog.FileName);
        //        if (CastDisassemblerHistogram.IsVisible)
        //        {
        //            saveFileDialog.FileName = "ScreenCapture-" + name + " " + DateTime.Now.ToString("dd.MM.yyyy-hh.mm.ss") + "_CAST";
        //            if (saveFileDialog.ShowDialog() == true)
        //            {
        //                CastDisassemblerHistogram.Save(saveFileDialog.FileName);
        //            }
        //        }
        //    }
        //}

        //private void UpdateCastAction(object sender, RoutedEventArgs e)
        //{
        //    // Save current disassembler
        //    _disassemblerManager.SaveCurrentDisassemblerFile();
        //    // Show information about result
        //    string messageBoxText = "Cast was updated";
        //    string caption = "Disassembler information";
        //    MessageBoxButton button = MessageBoxButton.OK;
        //    MessageBoxImage icon = MessageBoxImage.Information;
        //    MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);
        //    // Hide update button
        //    UpdateCastBorder.Visibility = Visibility.Collapsed;
        //}

        //private void AddDataToTable(DataGrid dataGrid, List<DisassemblerCommandInfo> commandsInfo)
        //{
        //    dataGrid.ItemsSource = commandsInfo;
        //}

        //private void AddDataToHistogram(SfChart histogram, List<DisassemblerCommandInfo> commandInfos)
        //{
        //    ColumnSeries currentSeries = new ColumnSeries();

        //    currentSeries.ItemsSource = commandInfos;
        //    currentSeries.XBindingPath = "Name";
        //    currentSeries.YBindingPath = "Entropy";

        //    ChartSeriesBase.SetSpacing(currentSeries, 0.0f);

        //    histogram.Series.Add(currentSeries);
        //}

        //private void SetCastDataVisibility(bool IsVisibile)
        //{
        //    if (IsVisibile)
        //    {
        //        SavedDisassemblerDataGrid.Visibility =
        //            UpdateCastBorder.Visibility =
        //            CastDisassemblerHistogram.Visibility =
        //            CastFileInfoGrid.Visibility = Visibility.Visible;
        //        FileInfoUniformGrid.Rows = HistogramUniformGrid.Rows = 2;
        //        StatisticMenuItem.IsEnabled = true;
        //    }
        //    else
        //    {
        //        SavedDisassemblerDataGrid.Visibility =
        //            UpdateCastBorder.Visibility =
        //        CastDisassemblerHistogram.Visibility =
        //        CastFileInfoGrid.Visibility = Visibility.Collapsed;
        //        FileInfoUniformGrid.Rows = HistogramUniformGrid.Rows = 1;
        //        StatisticMenuItem.IsEnabled = false;
        //    }
        //}

        #endregion


    }
}