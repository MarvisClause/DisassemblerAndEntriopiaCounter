﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DisEn
{
    // Disassembler manager
    // Controls file saves and their management
    public class DisassemblerManager
    {
        #region Variables

        // Current disassembler
        private Disassembler _currentDisassembler = new Disassembler();

        // Saved disassembler
        private Disassembler _savedDisassembler = new Disassembler();

        // Path to the instruction file
        private const string INSTRUCTION_FILTER_FILE_PATH = "instructions.txt";

        // Instruction filter
        HashSet<string> instructionFilter;

        #endregion

        #region Constructor

        public DisassemblerManager()
        {
            // Prepare hash set
            instructionFilter = new HashSet<string>();
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
        }

        #endregion

        #region Methods

        public Disassembler GetCurrentDisassembler()
        {
            return _currentDisassembler;
        }

        public Disassembler GetSavedDisassembler()
        {
            return _savedDisassembler;
        }

        public bool IsSavedDisassemblerExistInData()
        {
            return File.Exists(GetSavedDisassemblerPath());
        }

        public string GetSavedDisassemblerPath()
        {
            return String.Format("Data\\{0}\\{0}.asdat", _savedDisassembler.GetFileName());
        }

        public void SaveCurrentDisassemblerFile()
        {
            String directoryPath = String.Format("Data\\{0}", _savedDisassembler.GetFileName());
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataDirectoryPath = String.Format("{0}\\{1}.asdat", directoryPath, _savedDisassembler.GetFileName());
            if (File.Exists(dataDirectoryPath))
            {
                File.Delete(dataDirectoryPath);
            }
            Disassembler.Serialize(dataDirectoryPath, _currentDisassembler);
        }

        public void LoadSavedDisassemblerFile()
        {
            _savedDisassembler = Disassembler.Deserialize(GetSavedDisassemblerPath());
        }

        public void Disassemble(string filePath)
        {
            // Set instruction filter hash set
            _currentDisassembler.SetInstructionFilterHashSet(instructionFilter);
            _savedDisassembler.SetInstructionFilterHashSet(instructionFilter);
            // Disassemble files
            _currentDisassembler.DisassembleFile(filePath);
            _savedDisassembler.DisassembleFile(filePath);
            // Temp directory path
            if (!Directory.Exists("Temp"))
            {
                Directory.CreateDirectory("Temp");
            }
            string tempDirectoryPath = String.Format("Temp\\{0}.txt", _currentDisassembler.GetFileName());
            // Export current file to temp
            _currentDisassembler.ExportDisassembledCodeToTxtFile(tempDirectoryPath);
            // Check, if file exist in the data directory
            if (IsSavedDisassemblerExistInData())
            {
                LoadSavedDisassemblerFile();
            }
            else
            {
                SaveCurrentDisassemblerFile();
            }
            // Open txt file
            if (File.Exists(tempDirectoryPath))
            {
                // Open file 
                ProcessStartInfo txtInfo = new ProcessStartInfo();
                txtInfo.FileName = tempDirectoryPath;
                Process txtProcess = Process.Start(txtInfo);
            }
        }

        #endregion
    }
}