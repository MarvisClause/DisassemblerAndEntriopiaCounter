using Gee.External.Capstone;
using Gee.External.Capstone.X86;
using System;
using System.Collections.Generic;
using System.IO;

namespace DisEn
{
    // Command info struct
    public struct DissamblerCommandInfo
    {
        public string Name;
        public UInt32 Count;
        public double Entropy;
    }

    // Class for disassembling
    public class Disassembler
    {
        #region Variables

        // Total instruction counter
        private int _totalInstructionCounter;
        // List of instructions
        private List<String> _instructionFilter;
        // Dictionary of instructions
        private Dictionary<String, UInt32> _instructionsDict;
        // Path to the instruction file
        private const string INSTRUCTION_FILTER_FILE_PATH = "instructions.txt";


        #endregion

        #region Constructor

        // Constructor
        public Disassembler()
        {
            // Initialize containers
            _instructionFilter = new List<string>();
            _instructionsDict = new Dictionary<string, UInt32>();
            // Reset number to zero
            _totalInstructionCounter = 0;
            // Get instruction filter list
            string[] instructionReadArray = File.ReadAllLines(INSTRUCTION_FILTER_FILE_PATH);
            for (int i = 0; i < instructionReadArray.Length; ++i)
            {
                _instructionFilter.AddRange(instructionReadArray[i].Split(' '));
            }
        }

        #endregion

        #region Methods

        // Returns file entropy value
        public double GetFileEntropyValue()
        {
            // Calculate entropy
            double entropySum = 0;
            for (int i = 0; i < _instructionFilter.Count; ++i)
            {
                // Check, if this element exist in map
                if (_instructionsDict.ContainsKey(_instructionFilter[i]))
                {
                    // Count entropy of this element
                    entropySum += EntropyCalculation(_instructionsDict[_instructionFilter[i]], _totalInstructionCounter);
                }
            }
            return entropySum;
        }

        // Returns list with command name and it's count
        public List<DissamblerCommandInfo> GetCommandsInfo()
        {
            // Initialize list with command and entropy value pair
            List<DissamblerCommandInfo> commandsEntropy = new List<DissamblerCommandInfo>();
            for (int i = 0; i < _instructionFilter.Count; ++i)
            {
                // Check, if this element exist in map
                if (_instructionsDict.ContainsKey(_instructionFilter[i]))
                {
                    DissamblerCommandInfo dissamblerCommandInfo;
                    dissamblerCommandInfo.Name = _instructionFilter[i];
                    dissamblerCommandInfo.Count = _instructionsDict[_instructionFilter[i]];
                    dissamblerCommandInfo.Entropy = EntropyCalculation(dissamblerCommandInfo.Count, _totalInstructionCounter);
                    // Count entropy of this element
                    commandsEntropy.Add(dissamblerCommandInfo);
                }
            }
            return commandsEntropy;
        }

        // Counts entropy
        private double EntropyCalculation(double elementsCount, double allElementsCount)
        {
            if (allElementsCount == 0) { return 0; }
            return -elementsCount / allElementsCount * Math.Log(elementsCount / allElementsCount, 2);
        }

        // Disassembles file to get map of instructions (instruction, number of encounters)
        public void Disassemble(String filePath)
        {
            // Create dictionary of instructions
            _instructionsDict = new Dictionary<string, UInt32>();
            _totalInstructionCounter = 0;
            // Define type of disassemble
            if (!CapstoneDisassembler.IsX86Supported) { return; }
            const X86DisassembleMode disassembleMode = X86DisassembleMode.Bit64;
            using (CapstoneX86Disassembler disassembler = CapstoneDisassembler.CreateX86Disassembler(disassembleMode))
            {
                // Enables disassemble details, which are disabled by default, to provide more detailed information on
                disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
                // Enable skip data mode to get all instructions
                disassembler.EnableSkipDataMode = true;
                // Enable instruction details
                disassembler.EnableInstructionDetails = true;
                // Read all bytes from file
                byte[] binaryCode = File.ReadAllBytes(filePath);
                // Read through all instructions
                X86Instruction[] instructions = disassembler.Disassemble(binaryCode);
                // Go through all instructions
                foreach (X86Instruction instruction in instructions)
                {
                    for (int i = 0; i < _instructionFilter.Count; ++i)
                    {
                        if (_instructionFilter[i].CompareTo(instruction.Mnemonic) == 0)
                        {
                            // Add new instruction
                            if (!_instructionsDict.ContainsKey(_instructionFilter[i]))
                            {
                                _instructionsDict.Add(_instructionFilter[i], 1);
                            }
                            // Increment instruction count
                            _instructionsDict[_instructionFilter[i]]++;
                            // Increment total instruction counter
                            _totalInstructionCounter++;
                        }
                    }
                }
            }
        }

        #endregion
    }
}