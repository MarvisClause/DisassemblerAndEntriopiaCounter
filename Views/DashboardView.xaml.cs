using LiveCharts;
using LiveCharts.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisEn.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        #region Constructor

        public DashboardView()
        {
            InitializeComponent();
            UpdateWidgets();
        }

        #endregion

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
            if (ControlManager.GetDisassemblerManager() != null)
            {
                // Add data to the table
                AddDataToTable(CurrentDisassemblerDataGrid, ControlManager.GetDisassemblerManager().GetLastDisassembler().GetDisassemblerCommandsInfo());
                // Add data to the histogram
                AddLastFileDataToThePieHistogram();
                AddLastFileDataToTheColumnHistorgram();

                // Show name of the current file
                CurrentFileName.Text = ControlManager.GetDisassemblerManager().GetLastDisassembler().GetFileName();
                // Show total amount of instructions of the current file
                CurrentFileTotalAmountOfCommands.Text = ControlManager.GetDisassemblerManager().GetLastDisassembler().GetTotalInstructionCounter().ToString();
                // Show size of the current file
                CurrentFileSize.Text = ByteConverter.ConvertByToMegaByteToString(ControlManager.GetDisassemblerManager().GetLastDisassembler().GetFileSize());
            }
        }

        private void AddDataToTable(DataGrid dataGrid, List<DisassemblerCommandInfo> commandsInfo)
        {
            dataGrid.ItemsSource = commandsInfo;
        }

        private void AddLastFileDataToThePieHistogram()
        {
            // Fill series collection with data
            SeriesCollection seriesCollection = new SeriesCollection();
            // Fill series collection with data from disassembler
            for (int i = 0; i < ControlManager.GetDisassemblerManager().GetLastDisassembler().GetDisassemblerCommandsInfo().Count; ++i)
            {
                PieSeries pieSeries = new PieSeries
                {
                    Title = ControlManager.GetDisassemblerManager().GetLastDisassembler().GetDisassemblerCommandsInfo()[i].Name,
                    Values = new ChartValues<double> { ControlManager.GetDisassemblerManager().GetLastDisassembler().GetDisassemblerCommandsInfo()[i].Entropy }
                };
                seriesCollection.Add(pieSeries);
            }
            // Set series collection to the histogram
            CurrentFileTopPieHistogram.Series = seriesCollection;
        }

        private void AddLastFileDataToTheColumnHistorgram()
        {
            // Fill series collection with data
            SeriesCollection seriesCollectionTop = new SeriesCollection();
            // Fill series collection with data from disassembler
            for (int i = 0; i < ControlManager.GetDisassemblerManager().GetLastDisassembler().GetDisassemblerCommandsInfo().Count; ++i)
            {
                // Add new series
                seriesCollectionTop.Add(new ColumnSeries
                {
                    Title = ControlManager.GetDisassemblerManager().GetLastDisassembler().GetDisassemblerCommandsInfo()[i].Name,
                    Values = new ChartValues<double> { ControlManager.GetDisassemblerManager().GetLastDisassembler().GetDisassemblerCommandsInfo()[i].Entropy }
                });
            }
            // Set series collection to the histogram
            CurrentFileBottomColumnHistogram.Series = seriesCollectionTop;
        }

        #endregion
    }
}
