using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    // Disassembler manager
    // Controls file saves and their management
    internal class DisassemblerManager
    {
        #region Variables

        // Disassembler 
        private Disassembler _disassembler;
        // Path to the instruction file
        private const string INSTRUCTION_FILTER_FILE_PATH = "instructions.txt";

        #endregion

        #region Constructor

        public DisassemblerManager()
        {
            // Initialize
            _disassembler = new Disassembler();
            // Prepare hash set
            HashSet<string> instructionFilter = new HashSet<string>();
            // Get instruction filter list
            string[] instructionReadArray = File.ReadAllLines(INSTRUCTION_FILTER_FILE_PATH);
            for (int i = 0; i < instructionReadArray.Length; ++i)
            {
                // Split string
                string[] splitString = instructionReadArray[i].Split(' ');
                for (int j = 0; j < splitString.Length; ++j)
                {

                    instructionFilter.Add(splitString[j]);
                }
            }
            // Set instruction filter hash set
            _disassembler.SetInstructionFilterHashSet(instructionFilter);
        }

        #endregion

        #region Methods

        public Disassembler GetDisassembler()
        {
            return _disassembler;
        }

        public void Disassemble(string filePath)
        {
            // Disassemble file
            FileInfo fileInfo = new FileInfo(filePath);
            // Temp directory path
            string tempDirectoryPath = String.Format("Temp\\{0}.txt", fileInfo.Name.Split('.')[0]);
            // Disassemble 
            _disassembler.DisassembleToTxtFile(filePath, tempDirectoryPath);
            // Parse disassemble
            _disassembler.ParseDisassembledTxtFile(tempDirectoryPath);
        }

        #endregion
    }
}
