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
        private UInt32 _totalInstructionCounter;
        // List of instructions
        private HashSet<String> _instructionFilterHashSet;
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
            _instructionFilterHashSet = new HashSet<string>();
            _instructionsDict = new Dictionary<string, UInt32>();
            // Reset number to zero
            _totalInstructionCounter = 0;
            // Get instruction filter list
            string[] instructionReadArray = File.ReadAllLines(INSTRUCTION_FILTER_FILE_PATH);
            for (int i = 0; i < instructionReadArray.Length; ++i)
            {
                // Split string
                string[] splitString = instructionReadArray[i].Split(' ');
                for (int j = 0; j < splitString.Length; ++j)
                {
                    _instructionFilterHashSet.Add(splitString[j]);
                }
            }
        }

        #endregion

        #region Methods

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
        public List<DissamblerCommandInfo> GetCommandsInfo()
        {
            // Initialize list with command and entropy value pair
            List<DissamblerCommandInfo> commandsEntropy = new List<DissamblerCommandInfo>();
            List<string> instructionFilter = new List<string>(_instructionFilterHashSet);
            for (int i = 0; i < instructionFilter.Count; ++i)
            {
                // Check, if this element exist in map
                if (_instructionsDict.ContainsKey(instructionFilter[i]))
                {
                    DissamblerCommandInfo dissamblerCommandInfo;
                    dissamblerCommandInfo.Name = instructionFilter[i];
                    dissamblerCommandInfo.Count = _instructionsDict[instructionFilter[i]];
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
                    if (_instructionFilterHashSet.Contains(instruction.Mnemonic))
                    {
                        // Add new instruction
                        if (!_instructionsDict.ContainsKey(instruction.Mnemonic))
                        {
                            _instructionsDict.Add(instruction.Mnemonic, 1);
                        }
                        else
                        {
                            // Increment instruction count
                            _instructionsDict[instruction.Mnemonic]++;
                        }
                        // Increment total instruction counter
                        _totalInstructionCounter++;
                    }
                }
            }
        }

        #endregion
    }
}