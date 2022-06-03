using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                // Disassemble given file
                _disassembler.Disassemble(openFileDialog.FileName);
                // Show path to file
                PathToFileTextBlock.Text = openFileDialog.FileName;
                // Show total amount of instructions
                TotalNumOfCommandsTextBlock.Text = _disassembler.GetTotalInstructionCounter().ToString();
                // Show size of file
                FileInfo fileSize = new FileInfo(openFileDialog.FileName);
                double MB = fileSize.Length / 1048576.0;
                /*double MB = (fileSize.Length / 1024) / 1024;*/
                SizeFile.Text = MB.ToString("0.###") + " MB";
                // Get commands info
                List<DisassemblerCommandInfo> commandInfos = _disassembler.GetCommandsInfo();
                DisassemblerDataGrid.ItemsSource = commandInfos;
            }
        }
        private void BtnSaveFile_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog saveData = new SaveFileDialog();
            saveData.Filter = "Текст (*.txt)|*.txt";
            if (saveData.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(saveData.OpenFile(), Encoding.UTF8))
                {
                    sw.Write("Path to file: " + PathToFileTextBlock.Text);
                    sw.Write("\nTotal amout of commands: " + TotalNumOfCommandsTextBlock.Text);
                    sw.Write("\nSize of file: " + SizeFile.Text);

                    DisassemblerDataGrid.SelectAllCells();
                    DisassemblerDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                    ApplicationCommands.Copy.Execute(null, DisassemblerDataGrid);
                    DisassemblerDataGrid.UnselectAllCells();
                    string result = Clipboard.GetText(TextDataFormat.Text);
                    Clipboard.Clear();
                    sw.WriteLine("\n" + result);

                    sw.Close();
                }
            }
        }
    }
}