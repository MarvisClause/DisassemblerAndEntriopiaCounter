using Syncfusion.UI.Xaml.Charts;
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

namespace DisEn
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class InspectionStatisticWindow : Window
    {
        public InspectionStatisticWindow(DisassemblerComparator disassemblerComparator)
        {
            InitializeComponent();

            // Update data in data grid
            DeltaDataGrid.ItemsSource = disassemblerComparator.GetDisassemblerCommandInfoDelta();
            // Update data in text blocks
            SizeOfCurrentFileTextBlock.Text = ByteConverter.ConvertByToMegaByteToString(disassemblerComparator.GetFirstDisassembler().GetFileSize());
            SizeOfCastFileTextBlock.Text = ByteConverter.ConvertByToMegaByteToString(disassemblerComparator.GetSecondDisassembler().GetFileSize());
            DeltaSizeTextBlock.Text = ByteConverter.ConvertByToMegaByteToString(disassemblerComparator.GetFileSizeDelta());
            // Add data to histogram
            DeltaDisassemblerHistogram.Series.Clear();
            AddDataToHistogram(DeltaDisassemblerHistogram, disassemblerComparator.GetDisassemblerCommandInfoDelta());
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
    }
}
