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
    /// Interaction logic for CastsView.xaml
    /// </summary>
    public partial class CastsView : UserControl
    {
#region Variables

        public class DisassemblerInfo
        {
            public string FileName { get; set; }
            public DateTime LastCastUpdate { get; set; }
        }

        #endregion

        #region Constructor

        public CastsView()
        {
            InitializeComponent();
            UpdateWidgets();
        }

        #endregion

        #region Methods

        private void btnSearchCast_Click(object sender, RoutedEventArgs e)
        {
            UpdateWidgets();
        }

        private void UpdateWidgets()
        {
            // Get saved disassembler list
            List<Disassembler> savedDisassemblersList = ControlManager.GetDisassemblerManager().GetSavedDisassemblersList();

            // Retrieve the list of saved disassemblers and their info
            List<DisassemblerInfo> disassemblersInfoList = new List<DisassemblerInfo>();

            // Fill this list with data
            foreach (Disassembler disassembler in savedDisassemblersList)
            {
                // Filter elements by search text
                string searchText = SearchTextBox.Text.Trim().ToLower();

                // Filter elements
                if (disassembler.GetFileName().ToLower().Contains(searchText)
                    || disassembler.GetDisassembleDateTime().ToString().ToLower().Contains(searchText)
                    || searchText.Length == 0)
                {
                    disassemblersInfoList.Add(new DisassemblerInfo()
                    {
                        FileName = disassembler.GetFileName(),
                        LastCastUpdate = disassembler.GetDisassembleDateTime()
                    });
                }
            }

            // Set the ItemsSource of the DataGrid
            CastsDataGrid.ItemsSource = disassemblersInfoList;
        }

        #endregion
    }
}
