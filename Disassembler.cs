using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DisEn
{
    // Command info struct
    [Serializable]
    public class DisassemblerCommandInfo
    {
        public string Name { get; set; }
        public Int32 Count { get; set; }
        public double Entropy { get; set; }
    }

    // Class for disassembling
    [Serializable]
    public class Disassembler
    {
        #region Variables

        // File name
        private String _fileName;
        // Parsed file code
        private String _parsedFileCode;
        // File path to disassembled file
        [NonSerialized]
        private String _filePath;
        // Total instruction counter
        private Int32 _totalInstructionCounter;
        // Total entropy value
        private double _totalEntropyValue;
        // Size of the file in bytes
        private double _fileSize;
        // List of instructions
        private HashSet<string> _instructionFilterHashSet;
        // Dictionary of instructions
        private Dictionary<string, Int32> _instructionsDict;
        // Commands info list
        private List<DisassemblerCommandInfo> _disassemblerCommandsInfo;

        // Temporary folder for disassembler temp files
        [NonSerialized]
        private const string TEMP_DISASSEMBLER_FOLDER = "DISTEMP";

        #endregion

        #region Constructor

        // Constructor
        public Disassembler()
        {
            // Initialize containers
            _instructionFilterHashSet = new HashSet<string>();
            _instructionsDict = new Dictionary<string, Int32>();
            _disassemblerCommandsInfo = new List<DisassemblerCommandInfo>();
            // Reset number to zero
            _totalInstructionCounter = 0;
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is Disassembler objectType)
            {
                return this._fileName.CompareTo(objectType._fileName) == 0
                    && this._parsedFileCode.CompareTo(objectType._parsedFileCode) == 0
                    && this._fileSize == objectType._fileSize
                    && this._totalInstructionCounter == objectType._totalInstructionCounter
                    && this._totalEntropyValue == objectType._totalEntropyValue
                    && CompareDisInfo(objectType.GetDisassemblerCommandsInfo());
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Set instruction hash set
        public void SetInstructionFilterHashSet(HashSet<string> InstructionFilterHashSet)
        {
            _instructionFilterHashSet = InstructionFilterHashSet;
        }

        // Get file name
        public String GetFileName()
        {
            return _fileName;
        }

        // Get file path
        public String GetFilePath()
        {
            return _filePath;
        }

        // Get instruction hash set
        public HashSet<string> GetInstructionFilterHashSet()
        {
            return _instructionFilterHashSet;
        }

        // Total quantity of instructions
        public Int32 GetTotalInstructionCounter()
        {
            return _totalInstructionCounter;
        }

        // Total file size
        public double GetFileSize()
        {
            return _fileSize;
        }

        // Returns file entropy value
        public double GetFileTotalEntropyValue()
        {
            return _totalEntropyValue;
        }

        public List<DisassemblerCommandInfo> GetDisassemblerCommandsInfo()
        {
            return _disassemblerCommandsInfo;
        }

        public static void Serialize(String path, Disassembler disassembler)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(path,FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, disassembler);
            }
        }
        public static Disassembler Deserialize(String path)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return (Disassembler)binFormat.Deserialize(fStream);
            }
        }


        // Disassembles file
        public void DisassembleFile(String filePath)
        {
            // Save file path and its name
            FileInfo fileInfo = new FileInfo(filePath);
            _fileName = fileInfo.Name.Split('.')[0];
            _filePath = filePath;
            _fileSize = fileInfo.Length;
            // Disassemble and parse txt file
            if (fileInfo.Extension.CompareTo(".exe") == 0)
            {
                // Temp disassembler directory
                String _tempDisassembledFileFolder = TEMP_DISASSEMBLER_FOLDER;
                Directory.CreateDirectory(_tempDisassembledFileFolder);
                String _tempDisassembledFilePath = _tempDisassembledFileFolder + "\\" + _fileName + ".txt";
                // Disassemble file and parse it
                DisassembleExeToTxtFile(filePath, _tempDisassembledFilePath);
                ParseDisassembledTxtFile(_tempDisassembledFilePath);
                // Remove disassembled file and folder
                if (Directory.Exists(_tempDisassembledFileFolder))
                {
                    Directory.Delete(_tempDisassembledFileFolder, true);
                }
            }
            // Parse .txt file
            else if (fileInfo.Extension.CompareTo(".txt") == 0)
            {
                ParseDisassembledTxtFile(filePath);
            }
            else
            {
                return;
            }
            // Form and calculate data
            FormCommandsInfo();
            CalculateFileTotalEntropyValue();
        }

        // Exports disassembled file to .txt file
        public void ExportDisassembledCodeToTxtFile(String fileExportPath)
        {
            File.WriteAllText(fileExportPath, _parsedFileCode);
        }

        // Parses disassembled txt file
        private void ParseDisassembledTxtFile(String filePath)
        {
            if (File.Exists(filePath))
            {
                // Read all strings from file
                ParseDisassemble(File.ReadAllLines(filePath));
            }
        }

        // Disassembles file to get map of instructions (instruction, number of encounters)
        private void DisassembleExeToTxtFile(String filePath, String fileSavePath)
        {
            // Create dictionary of instructions
            _instructionsDict = new Dictionary<string, Int32>();
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
            catch (Exception) { }
        }

        // Forms commands info
        private void FormCommandsInfo()
        {
            // Initialize list with command and entropy value pair
            _disassemblerCommandsInfo = new List<DisassemblerCommandInfo>();
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
                    _disassemblerCommandsInfo.Add(dissamblerCommandInfo);
                }
            }
        }

        // Calculates file total entropy value
        public void CalculateFileTotalEntropyValue()
        {
            List<string> instructionFilter = new List<string>(_instructionFilterHashSet);
            // Calculate entropy
            _totalEntropyValue = 0;
            for (int i = 0; i < instructionFilter.Count; ++i)
            {
                // Check, if this element exist in map
                if (_instructionsDict.ContainsKey(instructionFilter[i]))
                {
                    // Count entropy of this element
                    _totalEntropyValue += EntropyCalculation(_instructionsDict[instructionFilter[i]], _totalInstructionCounter);
                }
            }
        }

        // Parse disassemble
        private void ParseDisassemble(string[] dissFileLines)
        {
            StringBuilder codeStringBuilder = new StringBuilder();
            _totalInstructionCounter = 0;
            _instructionsDict.Clear();
            // Go through all instructions
            foreach (string line in dissFileLines)
            {
                // Add line to command string builder
                codeStringBuilder.AppendLine(line);
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
            // Copy string builder to string variable
            _parsedFileCode = codeStringBuilder.ToString();
        }

        // Counts entropy
        private double EntropyCalculation(double elementsCount, double allElementsCount)
        {
            if (allElementsCount == 0) { return 0; }
            return -elementsCount / allElementsCount * Math.Log(elementsCount / allElementsCount, 2);
        }

        private bool CompareDisInfo(List<DisassemblerCommandInfo> disComInfo)
        {
            if (_disassemblerCommandsInfo.Count != disComInfo.Count) { return false; }
            for (int i = 0; i < disComInfo.Count; ++i)
            {
                for (int j = 0; j < _disassemblerCommandsInfo.Count; ++j)
                {
                    if (_disassemblerCommandsInfo[j].Name.Equals(disComInfo[i]))
                    {
                        if (_disassemblerCommandsInfo[j].Count != disComInfo[i].Count
                            || _disassemblerCommandsInfo[j].Entropy != disComInfo[i].Entropy)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        #endregion
    }
}