using Gee.External.Capstone;
using Gee.External.Capstone.X86;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    // Class for dissasembling
    public class Disassembler
    {
        #region Variables

        // Total instruction counter
        private int _totalInstructionCounter;
        // List of instructions
        private List<String> _insturctionFilter;
        // Dictionary of instructions
        Dictionary<String, int> _instructionsDict;
        // Path to the instruction file
        const string INSTRUCTION_FILTER_FILE_PATH = "instructions.txt";

      
        #endregion

        #region Constructor

        // Constructor
        public Disassembler()
        {
            // Initialize containers
            _insturctionFilter = new List<string>();
            _instructionsDict = new Dictionary<string, int>();
            // Reset number to zero
            _totalInstructionCounter = 0;
            // Get instruction filter list
            string[] instructionReadArray = File.ReadAllLines(INSTRUCTION_FILTER_FILE_PATH);
            for (int i = 0; i < instructionReadArray.Length; ++i)
            {
                _insturctionFilter.AddRange(instructionReadArray[i].Split(' '));
            }
        }

        #endregion

        #region Methods

        // Returns file entropy value
        public double GetFileEntropyValue()
        {
            // Calculate entropy
            double entropySum = 0;
            for (int i = 0; i < _insturctionFilter.Count; ++i)
            {
                // Check, if this element exist in map
                if (_instructionsDict.ContainsKey(_insturctionFilter[i]))
                {
                    // Count entropy of this element
                    entropySum += EntropyCalculation(_instructionsDict[_insturctionFilter[i]], _totalInstructionCounter);
                }
            }
            return entropySum;
        }

        // Returns list with command name and it's entropy value
        public List<Tuple<String, double>> GetCommandsEntropy()
        {
            // Initialize list with command and entropy value pair
            List<Tuple<String, double>> commandsEntropy = new List<Tuple<String, double>>();
            for (int i = 0; i < _insturctionFilter.Count; ++i)
            {
                // Check, if this element exist in map
                if (_instructionsDict.ContainsKey(_insturctionFilter[i]))
                {
                    // Count entropy of this element
                    commandsEntropy.Add(new Tuple<String, double>(_insturctionFilter[i],
                        EntropyCalculation(_instructionsDict[_insturctionFilter[i]], _totalInstructionCounter)));
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

        // Disassembles file to get map of instructions (insturction, number of encounters)
        public void Disassemble(String filePath)
        {
            // Create dictionary of instructions
            _instructionsDict = new Dictionary<string, int>();
            // Define type of dissasemble
            const X86DisassembleMode disassembleMode = X86DisassembleMode.Bit64;
            using (CapstoneX86Disassembler disassembler = CapstoneDisassembler.CreateX86Disassembler(disassembleMode))
            {
                // Enables disassemble details, which are disabled by default, to provide more detailed information on
                // disassembled binary code.
                disassembler.EnableInstructionDetails = true;
                disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
                // Read all bytes from file
                byte[] binaryCode = File.ReadAllBytes(filePath);
                // Read through all instructions
                X86Instruction[] instructions = disassembler.Disassemble(binaryCode);
                foreach (X86Instruction instruction in instructions)
                {
                    for (int i = 0; i < _insturctionFilter.Count; ++i)
                    {
                        if (_insturctionFilter[i].CompareTo(instruction.Mnemonic) == 0)
                        {
                            // Add new instruction
                            if (!_instructionsDict.ContainsKey(_insturctionFilter[i]))
                            {
                                _instructionsDict.Add(_insturctionFilter[i], 1);
                            }
                            // Increment instruction count
                            _instructionsDict[_insturctionFilter[i]]++;
                            // Increment total instruction counter
                            _totalInstructionCounter++;
                        }
                    }
                }
                #endregion
            }
        }
    }
}