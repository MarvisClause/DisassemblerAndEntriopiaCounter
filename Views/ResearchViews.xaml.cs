using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static DisEn.DisassemblerAnalyzer;

namespace DisEn.Views
{
    /// <summary>
    /// Interaction logic for ResearchViews.xaml
    /// </summary>
    public partial class ResearchViews : UserControl
    {
        #region Constructor

        public ResearchViews()
        {
            InitializeComponent();
            UpdateWidgets();
        }

        #endregion

        #region Methods

        private void btnDisassembleFile_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to find file to disassemble
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                // Disassemble the file
                ControlManager.GetDisassemblerManager().Disassemble(openFileDialog.FileName);
                // Update widgets
                UpdateWidgets();
            }
        }

        private void btnUpdateCastFile_Click(object sender, RoutedEventArgs e)
        {
            // Save current disassembler
            ControlManager.GetDisassemblerManager().SaveCurrentDisassemblerFile();
            // Show information about result
            string messageBoxText = "Cast was updated";
            string caption = "Disassembler information";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);
            // Hide update button
            UpdateCastFileButton.Visibility = Visibility.Collapsed;
        }

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
                AddDataToTable(CurrentDisassemblerDataGrid, ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo());
                // Add data to the histogram
                AddCurrentFileDataToThePieHistogram();
                AddCurrentFileDataToTheColumnHistorgram();

                // Show name of the current file
                CurrentFileName.Text = ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetFileName();
                // Show total amount of instructions of the current file
                CurrentFileTotalAmountOfCommands.Text = ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetTotalInstructionCounter().ToString();
                // Show size of the current file
                CurrentFileSize.Text = ByteConverter.ConvertByToMegaByteToString(ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetFileSize());

                // If current disassembler is different than the one we saved, show difference of them visually and change interface accordingly
                if (!ControlManager.GetDisassemblerManager().GetCurrentDisassembler().Equals(ControlManager.GetDisassemblerManager().GetSavedDisassembler()))
                {
                    // Add data to the table
                    AddDataToTable(SavedDisassemblerDataGrid, ControlManager.GetDisassemblerManager().GetSavedDisassembler().GetDisassemblerCommandsInfo());
                    // Add data to the histogram
                    AddSavedFileDataToTheColumnHistogram();

                    // Show name of the cast file
                    CastFileName.Text = ControlManager.GetDisassemblerManager().GetSavedDisassembler().GetFileName();
                    // Show total amount of instructions of the cast file
                    CastFileTotalAmountOfCommands.Text = ControlManager.GetDisassemblerManager().GetSavedDisassembler().GetTotalInstructionCounter().ToString();
                    // Show size of the cast file
                    CastFileSize.Text = ByteConverter.ConvertByToMegaByteToString(ControlManager.GetDisassemblerManager().GetSavedDisassembler().GetFileSize());

                    // Change cast data visibility
                    SetCastDataVisibility(true);

                    // If data is not equal, inform user about this
                    if (!ControlManager.GetDisassemblerComparator().CompareData(ControlManager.GetDisassemblerManager().GetCurrentDisassembler(),
                        ControlManager.GetDisassemblerManager().GetSavedDisassembler()))
                    {
                        // Train
                        ControlManager.GetDisassemblerAnalyzer().TrainNeuralNetworkByDiscrepancyCriterion(ControlManager.GetDisassemblerComparator());
                        // Predict
                        OwnershipAnalyze ownershipAnalyze = ControlManager.GetDisassemblerAnalyzer().CalculateDiscrepancyCriterionByNeuralNetwork(ControlManager.GetDisassemblerComparator());

                        // Show information about result
                        string messageBoxText;
                        string caption;
                        MessageBoxImage icon;
                        if (ownershipAnalyze.virusOwnerChance > ownershipAnalyze.authorOwnerChance)
                        {
                            messageBoxText = "There is a possibility of virus code injection" +
                                "\nDiscrepancy criterion = " + ownershipAnalyze.discrepancyCriterionCount +
                                "\nAuthor owner chance = " + ownershipAnalyze.authorOwnerChance +
                                "\nVirus owner chance = " + ownershipAnalyze.virusOwnerChance;
                            caption = "Discrepancy value is high";
                            icon = MessageBoxImage.Warning;
                        }
                        else
                        {
                            messageBoxText = "Discrepancy value is in normal range" +
                            "\nCode was changed by user. Discrepancy criterion = " + ownershipAnalyze.discrepancyCriterionCount +
                            "\nAuthor owner chance = " + ownershipAnalyze.authorOwnerChance +
                            "\nVirus owner chance = " + ownershipAnalyze.virusOwnerChance;
                            caption = "Code was changed by user";
                            icon = MessageBoxImage.Information;
                        }
                        MessageBoxButton button = MessageBoxButton.OK;
                        MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);
                    }
                }
                // If current disassembler is the same as the saved one, when there is no difference or this is the first time this file is processed by this program.
                // In this case, we will hide information about cast file, because there is no need to show it.
                else
                {
                    // Change cast data visibility
                    SetCastDataVisibility(false);
                }
            }
        }

        private void AddDataToTable(DataGrid dataGrid, List<DisassemblerCommandInfo> commandsInfo)
        {
            dataGrid.ItemsSource = commandsInfo;
        }

        private void AddCurrentFileDataToThePieHistogram()
        {
            // Fill series collection with data
            SeriesCollection seriesCollection = new SeriesCollection();
            // Fill series collection with data from disassembler
            for (int i = 0; i < ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo().Count; ++i)
            {
                PieSeries pieSeries = new PieSeries
                {
                    Title = ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo()[i].Name,
                    Values = new ChartValues<double> { ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo()[i].Entropy }
                };
                seriesCollection.Add(pieSeries);
            }
            // Set series collection to the histogram
            CurrentFileTopPieHistogram.Series = seriesCollection;
        }

        private void AddCurrentFileDataToTheColumnHistorgram()
        {
            // Fill series collection with data
            SeriesCollection seriesCollectionTop = new SeriesCollection();
            SeriesCollection seriesCollectionBottom = new SeriesCollection();
            // Fill series collection with data from disassembler
            for (int i = 0; i < ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo().Count; ++i)
            {
                // Add new series
                seriesCollectionTop.Add(new ColumnSeries
                {
                    Title = ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo()[i].Name,
                    Values = new ChartValues<double> { ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo()[i].Entropy }
                });
                seriesCollectionBottom.Add(new ColumnSeries
                {
                    Title = ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo()[i].Name,
                    Values = new ChartValues<double> { ControlManager.GetDisassemblerManager().GetCurrentDisassembler().GetDisassemblerCommandsInfo()[i].Entropy }
                });
            }
            // Set series collection to the histogram
            CurrentFileTopColumnHistogram.Series = seriesCollectionTop;
            CurrentFileBottomColumnHistogram.Series = seriesCollectionBottom;
        }

        private void AddSavedFileDataToTheColumnHistogram()
        {
            // Fill series collection with data
            SeriesCollection seriesCollection = new SeriesCollection();
            // Fill series collection with data from disassembler
            for (int i = 0; i < ControlManager.GetDisassemblerManager().GetSavedDisassembler().GetDisassemblerCommandsInfo().Count; ++i)
            {
                ColumnSeries columnSeries = new ColumnSeries
                {
                    Title = ControlManager.GetDisassemblerManager().GetSavedDisassembler().GetDisassemblerCommandsInfo()[i].Name,
                    Values = new ChartValues<double> { ControlManager.GetDisassemblerManager().GetSavedDisassembler().GetDisassemblerCommandsInfo()[i].Entropy }
                };
                // Add new series
                seriesCollection.Add(columnSeries);
            }
            // Set series collection to the histogram
            CastFileBottomColumnHistogram.Series = seriesCollection;
        }

        private void SetCastDataVisibility(bool IsVisibile)
        {
            if (IsVisibile)
            {
                // Change file info uniform grid rows
                FileInfoUniformGrid.Rows = 2;
                // Change cast file info grid visibility
                CastFileInfoGrid.Visibility = 
                UpdateCastFileButton.Visibility = Visibility.Visible;
                // Change histograms visibility
                CurrentFileTopColumnHistogram.Visibility = Visibility.Visible;
                CurrentFileTopPieHistogram.Visibility = Visibility.Collapsed;
                CurrentFileBottomColumnHistogram.Visibility = Visibility.Collapsed;
                CastFileBottomColumnHistogram.Visibility = Visibility.Visible;
            }
            else
            {
                // Change file info uniform grid rows
                FileInfoUniformGrid.Rows = 1;
                // Change cast file info grid visibility
                CastFileInfoGrid.Visibility =
                UpdateCastFileButton.Visibility = Visibility.Collapsed;
                // Change histograms visibility
                CurrentFileTopColumnHistogram.Visibility = Visibility.Collapsed;
                CurrentFileTopPieHistogram.Visibility = Visibility.Visible;
                CurrentFileBottomColumnHistogram.Visibility = Visibility.Visible;
                CastFileBottomColumnHistogram.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}
