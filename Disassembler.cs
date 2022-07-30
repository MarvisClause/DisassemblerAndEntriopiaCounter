using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DisEn
{
    // Command info struct
    public class DisassemblerCommandInfo
    {
        public string Name { get; set; }
        public UInt32 Count { get; set; }
        public double Entropy { get; set; }

    }

    // Class for disassembling
    public class Disassembler
    {
        #region Variables

        // Total instruction counter
        private UInt32 _totalInstructionCounter;
        // List of instructions
        private HashSet<string> _instructionFilterHashSet;
        // Dictionary of instructions
        private Dictionary<string, UInt32> _instructionsDict;

        #endregion

        #region Constructor

        // Constructor
        public Disassembler()
        {
            // Initialize containers
            _instructionFilterHashSet = new HashSet<string>();
            _instructionsDict = new Dictionary<string, UInt32>();
            // Reset number to zero
            _totalInstructionCounter = 0;
        }

        #endregion

        #region Methods

        // Set instruction hash set
        public void SetInstructionFilterHashSet(HashSet<string> InstructionFilterHashSet)
        {
            _instructionFilterHashSet = InstructionFilterHashSet;
        }

        // Get instruction hash set
        public HashSet<string> GetInstructionFilterHashSet()
        {
            return _instructionFilterHashSet;
        }

        // Total quantity of instructions
        public UInt32 GetTotalInstructionCounter()
        {
            return _totalInstructionCounter;
        }

        // Returns file entropy value
        public double GetFileEntropyValue()
        {
            List<string> instructionFilter = new List<string>(_instructionFilterHashSet);
            // Calculate entropy
            double entropySum = 0;
            for (int i = 0; i < instructionFilter.Count; ++i)
            {
                // Check, if this element exist in map
                if (_instructionsDict.ContainsKey(instructionFilter[i]))
                {
                    // Count entropy of this element
                    entropySum += EntropyCalculation(_instructionsDict[instructionFilter[i]], _totalInstructionCounter);
                }
            }
            return entropySum;
        }

        // Returns list with command name and it's count
        public List<DisassemblerCommandInfo> GetCommandsInfo()
        {
            // Initialize list with command and entropy value pair
            List<DisassemblerCommandInfo> commandsEntropy = new List<DisassemblerCommandInfo>();
            List<string> instructionFilter = new List<string>(_instructionFilterHashSet);
            for (int i = 0; i < instructionFilter.Count; ++i)
            {
                // Check, if this element exist in map
                if (_instructionsDict.ContainsKey(instructionFilter[i]))
                {
                    DisassemblerCommandInfo dissamblerCommandInfo = new DisassemblerCommandInfo();
                    dissamblerCommandInfo.Name = instructionFilter[i];
                    dissamblerCommandInfo.Count = _instructionsDict[instructionFilter[i]];
                    dissamblerCommandInfo.Entropy = EntropyCalculation(dissamblerCommandInfo.Count, _totalInstructionCounter);
                    // Count entropy of this element
                    commandsEntropy.Add(dissamblerCommandInfo);

                }
            }
            return commandsEntropy;
        }

        // Disassembles file to get map of instructions (instruction, number of encounters)
        public void DisassembleToTxtFile(String filePath, String fileSavePath)
        {
            // Create dictionary of instructions
            _instructionsDict = new Dictionary<string, UInt32>();
            _totalInstructionCounter = 0;
            // Open dumpbin
            ProcessStartInfo dumpInfo = new ProcessStartInfo();
            dumpInfo.FileName = @"Dumpbin\dumpbin.exe";
            dumpInfo.Arguments = String.Format(@"/disasm /out:{0} {1}", fileSavePath, filePath);
            Process dumpBinProcess = Process.Start(dumpInfo);
            // Check, if process still running
            try
            {
                while (Process.GetProcessById(dumpBinProcess.Id) != null) { }
            }
            catch (Exception ex) { }
            if (File.Exists(fileSavePath))
            {
                // Open file 
                ProcessStartInfo txtInfo = new ProcessStartInfo();
                txtInfo.FileName = fileSavePath;
                Process txtProcess = Process.Start(txtInfo);
            }
        }

        // Parses 
        public void ParseDisassembledTxtFile(String filePath)
        {
            if (File.Exists(filePath))
            {
                // Read all strings from file
                ParseDisassemble(File.ReadAllLines(filePath));
            }
        }

        // Parse disassemble
        private void ParseDisassemble(string[] dissFileLines)
        {
            // Go through all instructions
            foreach (string line in dissFileLines)
            {
                foreach (string word in line.Split(' '))
                {
                    if (_instructionFilterHashSet.Contains(word))
                    {
                        // Add new instruction
                        if (!_instructionsDict.ContainsKey(word))
                        {
                            _instructionsDict.Add(word, 1);
                        }
                        else
                        {
                            // Increment instruction count
                            _instructionsDict[word]++;
                        }
                        // Increment total instruction counter
                        _totalInstructionCounter++;
                    }
                }
            }
        }

        // Counts entropy
        private double EntropyCalculation(double elementsCount, double allElementsCount)
        {
            if (allElementsCount == 0) { return 0; }
            return -elementsCount / allElementsCount * Math.Log(elementsCount / allElementsCount, 2);
        }

        #endregion
    }
}