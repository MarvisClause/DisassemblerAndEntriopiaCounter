using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace VerCheck.Windows
{
    /// <summary>
    /// Interaction logic for DisassemblerComparisonWindow.xaml
    /// </summary>
    public partial class DisassemblerComparisonWindow : Window
    {
        public DisassemblerComparisonWindow()
        {
            InitializeComponent();
            UpdateWidgets();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else this.WindowState = WindowState.Normal;
        }

        #region Methods

        private void btnPieExpend_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnColumExpend_Click(object sender, RoutedEventArgs e)
        {

        }

        // Updates widget with new information
        private void UpdateWidgets()
        {
            if (ControlManager.GetDisassemblerComparator() != null)
            {
                // Add data to the table
                AddDataToTable(ComparisonDisassemblerDataGrid, ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta());
                // Add data to the histogram
                AddComparisonToThePieHistogram();
                AddComparisonTheColumnHistorgram();

                // Show name of the file
                ComparisonFileName.Text = ControlManager.GetDisassemblerComparator().GetFirstDisassembler().GetFileName();
                // Show total amount of instructions of the file
                ComparisonFileTotalAmountOfCommands.Text = ControlManager.GetDisassemblerComparator().GetTotalInstructionCounterDelta().ToString();
                // Show size of the file
                ComparisonFileSize.Text = ByteConverter.ConvertByToMegaByteToString(ControlManager.GetDisassemblerComparator().GetFileSizeDelta());
            }
        }

        private void AddDataToTable(DataGrid dataGrid, List<DisassemblerCommandInfo> commandsInfo)
        {
            dataGrid.ItemsSource = commandsInfo;
        }

        private void AddComparisonToThePieHistogram()
        {
            // Fill series collection with data
            SeriesCollection seriesCollection = new SeriesCollection();
            List<Color> Colors = Utility.GenerateColors(ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta().Count);
            // Fill series collection with data from disassembler
            for (int i = 0; i < ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta().Count; ++i)
            {
                PieSeries pieSeries = new PieSeries
                {
                    Title = ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta()[i].Name,
                    Values = new ChartValues<double> { ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta()[i].Entropy },
                    Fill = new SolidColorBrush(Colors[i])
                };
                seriesCollection.Add(pieSeries);
            }
            // Set series collection to the histogram
            ComparisonTopPieHistogram.Series = seriesCollection;
            var Tooltip = (DefaultTooltip)ComparisonTopPieHistogram.DataTooltip;
            Tooltip.SelectionMode = LiveCharts.TooltipSelectionMode.OnlySender;
        }

        private void AddComparisonTheColumnHistorgram()
        {
            // Fill series collection with data
            SeriesCollection seriesCollectionTop = new SeriesCollection();
            List<Color> Colors = Utility.GenerateColors(ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta().Count);
            // Fill series collection with data from disassembler
            for (int i = 0; i < ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta().Count; ++i)
            {
                // Add new series
                seriesCollectionTop.Add(new ColumnSeries
                {
                    Title = ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta()[i].Name,
                    Values = new ChartValues<double> { ControlManager.GetDisassemblerComparator().GetDisassemblerCommandInfoDelta()[i].Entropy },
                    Fill = new SolidColorBrush(Colors[i])
                });
            }
            // Set series collection to the histogram
            ComparisonBottomColumnHistogram.Series = seriesCollectionTop;
            var Tooltip = (DefaultTooltip)ComparisonBottomColumnHistogram.DataTooltip;
            Tooltip.SelectionMode = LiveCharts.TooltipSelectionMode.OnlySender;
        }

        #endregion
    }
}
