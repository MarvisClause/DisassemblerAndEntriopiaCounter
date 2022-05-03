using System;
using System.IO;
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

namespace DisEn
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
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
                // Show information on screen
                StringBuilder stringBuilder = new StringBuilder();
                // Show file path
                stringBuilder.Append("File path: " + openFileDialog.FileName + "\n");
                // Get total entropy
                stringBuilder.Append("Total Entropy: " + _disassembler.GetFileEntropyValue() + "\n");
                // Get list of commands
                List < Tuple<String, double> > commandsEntropyList = _disassembler.GetCommandsEntropy();
                // Add them to the text
                for (int i = 0; i < commandsEntropyList.Count; i++)
                {
                    stringBuilder.Append(commandsEntropyList[i].Item1 + " " + commandsEntropyList[i].Item2 + "\n");
                }
                DisassemblerBox.Text = stringBuilder.ToString();
            }
               
                
        }
    }
}
