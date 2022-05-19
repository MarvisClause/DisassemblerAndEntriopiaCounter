using System.Collections.Generic;
using System.Text;
using System.Windows;

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

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|(*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Disassemble given file
                _disassembler.Disassemble(openFileDialog.FileName);
                // Show path to file
                PathToFileTextBlock.Text = openFileDialog.FileName;
                // Show total amount of instructions
                TotalNumOfCommandsTextBlock.Text = _disassembler.GetTotalInstructionCounter().ToString();
                // Get commands info
                List<DissamblerCommandInfo> commandInfos = _disassembler.GetCommandsInfo();
                DisassemblerDataGrid.ItemsSource = commandInfos;
            }
        }
    }
}
