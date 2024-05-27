using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VerCheck
{
    // Disassembler manager
    // Controls file saves and their management
    public class DisassemblerManager
    {
        #region Variables

        // Last disassembler
        // Holds information about last disassembled file
        private Disassembler _lastDisassembler = new Disassembler();

        // Current disassembler
        private Disassembler _currentDisassembler = new Disassembler();

        // Saved disassembler
        private Disassembler _savedDisassembler = new Disassembler();

        // Instruction filter
        private HashSet<string> _instructionFilter;

        // File with info about last disassembled file
        [NonSerialized]
        private const string LAST_DISASSEMBLER_FILE = "LastDisassembledFile.asdat";

        // Folder for disassembler data files
        [NonSerialized]
        private const string DATA_DISASSEMBLER_FOLDER = "Data";

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
            List<Tuple<string, double>> commandFilterList = ControlManager.GetDisassemblerAnalyzer().GetCommandNamesList();
            for (int i = 0; i < commandFilterList.Count; ++i)
            {
                _instructionFilter.Add(commandFilterList[i].Item1);
            }
        }

        #endregion

        #region Methods

        public Disassembler GetLastDisassembler()
        {
            LoadLastDisassemblerFile();
            return _lastDisassembler;
        }

        public Disassembler GetCurrentDisassembler()
        {
            return _currentDisassembler;
        }

        public Disassembler GetSavedDisassembler()
        {
            return _savedDisassembler;
        }

        public static bool IsDisassemblerInfoFileExistInData(Disassembler disassembler)
        {
            return File.Exists(GetDisassemblerInfoFilePath(disassembler));
        }

        public static bool IsDisassemblerInfoFileExistInData(string name)
        {
            return File.Exists(GetDisassemblerInfoFilePath(name));
        }

        public static string GetDisassemblerInfoFilePath(Disassembler disassembler)
        {
            return String.Format("{0}\\{1}\\Info.txt", DATA_DISASSEMBLER_FOLDER, disassembler.GetFileName());
        }

        public static string GetDisassemblerInfoFilePath(string name)
        {
            return String.Format("{0}\\{1}\\Info.txt", DATA_DISASSEMBLER_FOLDER, name);
        }

        public static string GetDisassemblerDataPath(Disassembler disassembler, int versionNumber)
        {
            return String.Format("{0}\\{1}\\{2}\\{1}.asdat", DATA_DISASSEMBLER_FOLDER, disassembler.GetFileName(), versionNumber);
        }

        public static Disassembler GetLatestDeserializedDisassembler(string name)
        {
            int versionNumber = 1;
            Debug.Assert(ReadDisassemblerLatestVersionNumberFromInfoFile(name, out versionNumber));
            return Disassembler.Deserialize(String.Format("{0}\\{1}\\{2}\\{1}.asdat", DATA_DISASSEMBLER_FOLDER, name, versionNumber));
        }

        public static Disassembler GetDeserializedDisassemblerByVersion(string name, int versionNumber)
        {
            return Disassembler.Deserialize(String.Format("{0}\\{1}\\{2}\\{1}.asdat", DATA_DISASSEMBLER_FOLDER, name, versionNumber));
        }

        private static void WriteDisassemblerVersionToInfoFile(Disassembler disassembler, int versionNumber)
        {
            if (Directory.Exists(String.Format("{0}\\{1}", DATA_DISASSEMBLER_FOLDER, disassembler.GetFileName())))
            {
                File.WriteAllText(GetDisassemblerInfoFilePath(disassembler), versionNumber.ToString());
            }
        }

        public static bool ReadDisassemblerLatestVersionNumberFromInfoFile(Disassembler disassembler, out int outVersionNumber)
        {
            if (IsDisassemblerInfoFileExistInData(disassembler))
            {
                // Read all text from the file
                string fileContent = File.ReadAllText(GetDisassemblerInfoFilePath(disassembler));

                // Attempt to convert the content to an integer
                if (int.TryParse(fileContent, out int versionNumber))
                {
                    outVersionNumber = versionNumber;
                    return true;
                }
            }
            outVersionNumber = -1;
            return false;
        }

        public static bool ReadDisassemblerLatestVersionNumberFromInfoFile(string name, out int outVersionNumber)
        {
            if (IsDisassemblerInfoFileExistInData(name))
            {
                // Read all text from the file
                string fileContent = File.ReadAllText(GetDisassemblerInfoFilePath(name));

                // Attempt to convert the content to an integer
                if (int.TryParse(fileContent, out int versionNumber))
                {
                    outVersionNumber = versionNumber;
                    return true;
                }
            }
            outVersionNumber = -1;
            return false;
        }

        public void SaveCurrentDisassemblerFile()
        {
            // Check, if directory exists
            String directoryPath = String.Format("{0}\\{1}", DATA_DISASSEMBLER_FOLDER, _savedDisassembler.GetFileName());
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            // Check, if saved disassembler info exists
            int versionNumber = 1;
            if (IsDisassemblerInfoFileExistInData(_savedDisassembler))
            {
                Debug.Assert(ReadDisassemblerLatestVersionNumberFromInfoFile(_savedDisassembler, out versionNumber));
                versionNumber++;
                WriteDisassemblerVersionToInfoFile(_savedDisassembler, versionNumber);
            }
            else
            {
                WriteDisassemblerVersionToInfoFile(_savedDisassembler, versionNumber);
            }
            // Create directory with according version number and save asdata there
            String dataDirectoryPath = String.Format("{0}\\{1}", directoryPath, versionNumber);
            Directory.CreateDirectory(dataDirectoryPath);
            String assemblerDataPath = String.Format("{0}\\{1}.asdat", dataDirectoryPath, _savedDisassembler.GetFileName());
            Disassembler.Serialize(assemblerDataPath, _currentDisassembler);
            // Set saved disassembler as the current one
            _savedDisassembler = _currentDisassembler;
        }

        private void SaveLastDisassemblerFile()
        {
            if (File.Exists(LAST_DISASSEMBLER_FILE))
            {
                File.Delete(LAST_DISASSEMBLER_FILE);
            }
            Disassembler.Serialize(LAST_DISASSEMBLER_FILE, _currentDisassembler);
        }

        public void LoadLastDisassemblerFile()
        {
            if (File.Exists(LAST_DISASSEMBLER_FILE))
            {
                _lastDisassembler = Disassembler.Deserialize(LAST_DISASSEMBLER_FILE);
            }
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
            _lastDisassembler = _currentDisassembler;
            // Save last disassembler file
            SaveLastDisassemblerFile();
            // Check, if file exist in the data directory
            if (IsDisassemblerInfoFileExistInData(_savedDisassembler))
            {
                _savedDisassembler = GetLatestDeserializedDisassembler(_savedDisassembler.GetFileName());
            }
            else
            {
                SaveCurrentDisassemblerFile();
            }
        }

        public List<Disassembler> GetSavedDisassemblersList()
        {
            List<Disassembler> savedDisassemblersList = new List<Disassembler>();

            string dataFolderPath = DATA_DISASSEMBLER_FOLDER; // Adjust the path as needed

            // Check if the data folder exists
            if (Directory.Exists(dataFolderPath))
            {
                // Get all directories in the data
                string[] directories = Directory.GetDirectories(dataFolderPath);

                foreach (string directoryPath in directories)
                {
                    // Get the directory name, it is the same as disassembler, which we need to retrieve
                    string directoryName = Path.GetFileName(directoryPath);

                    savedDisassemblersList.Add(GetLatestDeserializedDisassembler(directoryName));
                }
            }

            return savedDisassemblersList;
        }

        #endregion
    }
}
