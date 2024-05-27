using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VerCheck.Windows
{
    /// <summary>
    /// Interaction logic for ChangeHistoryWindow.xaml
    /// </summary>
    public partial class DisassemblerHistoryWindow : Window
    {
        public DisassemblerHistoryWindow(string disassemblerName)
        {
            InitializeComponent();
            UpdateWidgets(disassemblerName);
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

        // Updates widget with new information
        private void UpdateWidgets(string disassemblerName)
        {
            // Add data to the table
            AddHistoryDataToTheColumnHistorgram(disassemblerName);

            // Show name of the the file
            FileName.Text = disassemblerName;
        }

        private void AddHistoryDataToTheColumnHistorgram(string disassemblerName)
        {
            int latestVersionNumber = 1;
            DisassemblerManager.ReadDisassemblerLatestVersionNumberFromInfoFile(disassemblerName, out latestVersionNumber);

            List<string> versionLabels = new List<string>();
            ChartValues<int> totalNumberOfCommandsValues = new ChartValues<int>();
            ChartValues<double> totalEntropyValues = new ChartValues<double>();

            for (int i = 1; i <= latestVersionNumber; ++i)
            {
                Disassembler disassembler = DisassemblerManager.GetDeserializedDisassemblerByVersion(disassemblerName, i);
                versionLabels.Add(i.ToString());
                totalNumberOfCommandsValues.Add(disassembler.GetTotalInstructionCounter());
                totalEntropyValues.Add(disassembler.GetFileTotalEntropyValue());
            }

            HistoryTotalNumberOfCommandsLineChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Number of Commands",
                    Values = totalNumberOfCommandsValues,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                }
            };

            HistoryTotalEntropyLineChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Entropy",
                    Values = totalEntropyValues,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                }
            };

            // Initialize AxisX before setting Labels
            HistoryTotalNumberOfCommandsLineChart.AxisX.Add(new Axis
            {
                Title = "Version Number",
                Labels = versionLabels
            });

            HistoryTotalEntropyLineChart.AxisX.Add(new Axis
            {
                Title = "Version Number",
                Labels = versionLabels
            });

            var tooltipTotalNumberOfCommandsLineChart = (DefaultTooltip)HistoryTotalNumberOfCommandsLineChart.DataTooltip;
            tooltipTotalNumberOfCommandsLineChart.SelectionMode = TooltipSelectionMode.OnlySender;

            var tooltipTotalEntropyLineChart = (DefaultTooltip)HistoryTotalEntropyLineChart.DataTooltip;
            tooltipTotalEntropyLineChart.SelectionMode = TooltipSelectionMode.OnlySender;
        }

        #endregion
    }
}
