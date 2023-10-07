using System;
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
        private HashSet<string> _instructionFilter;

        // Temporary folder for disassembler temp files
        [NonSerialized]
        private const string TEMP_DISASSEMBLER_FOLDER = "DISTEMP";

        #endregion

        #region Constructor

        public DisassemblerManager()
        {
            // Prepare hash set
            _instructionFilter = new HashSet<string>();
            // Get instruction filter list
            string[] instructionReadArray = File.ReadAllLines(INSTRUCTION_FILTER_FILE_PATH);
            for (int i = 0; i < instructionReadArray.Length; ++i)
            {
                // Split string
                string[] splitString = instructionReadArray[i].Split(' ');
                for (int j = 0; j < splitString.Length; ++j)
                {

                    _instructionFilter.Add(splitString[j]);
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
            _currentDisassembler.SetInstructionFilterHashSet(_instructionFilter);
            _savedDisassembler.SetInstructionFilterHashSet(_instructionFilter);
            // Create folder for temporary files and remove existing one
            if (Directory.Exists(TEMP_DISASSEMBLER_FOLDER))
            {
                Directory.Delete(TEMP_DISASSEMBLER_FOLDER, true);
            }
            Directory.CreateDirectory(TEMP_DISASSEMBLER_FOLDER);
            // Create folder for current temporary file
            String tempDisassembledFileFolder = TEMP_DISASSEMBLER_FOLDER;
            Directory.CreateDirectory(tempDisassembledFileFolder);
            _currentDisassembler.DisassembleFile(filePath, tempDisassembledFileFolder);
            _savedDisassembler = _currentDisassembler;
            // Check, if file exist in the data directory
            if (IsSavedDisassemblerExistInData())
            {
                LoadSavedDisassemblerFile();
            }
            else
            {
                SaveCurrentDisassemblerFile();
            }
        }

        public List<Disassembler> GetSavedDisassemblersList()
        {
            List<Disassembler> savedDisassemblersList = new List<Disassembler>();

            string dataFolderPath = "Data"; // Adjust the path as needed

            // Check if the data folder exists
            if (Directory.Exists(dataFolderPath))
            {
                // Get all directories in the data
                string[] directories = Directory.GetDirectories(dataFolderPath);

                foreach (string directoryPath in directories)
                {
                    // Get all files with the ".asdat" extension in the data folder
                    string[] disassemblerFiles = Directory.GetFiles(directoryPath, "*.asdat");

                    foreach (string filePath in disassemblerFiles)
                    {
                        try
                        {
                            // Deserialize each file into a Disassembler object
                            Disassembler disassembler = Disassembler.Deserialize(filePath);
                            savedDisassemblersList.Add(disassembler);
                        }
                        catch (Exception ex)
                        {
                            // Handle any exceptions that may occur during deserialization
                            Console.WriteLine($"Error loading disassembler from {filePath}: {ex.Message}");
                        }
                    }
                }
            }

            return savedDisassemblersList;
        }

        #endregion
    }
}
