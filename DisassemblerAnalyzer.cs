using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace DisEn
{
    // Analyzes disassembler results 
    // Used to define ownership of the changes (Author or virus)
    public class DisassemblerAnalyzer
    {
        #region Variables

        // Enum for more clear definition of ownership results
        public struct OwnershipAnalyze 
        {
            public double authorOwnerChance;
            public double virusOwnerChance;
            public int discrepancyCriterionCount;
        }

        // Command threshold filter is a  filter for commands threshold.
        // If command's entropy value is bigger than than threshold, it might mean, that we have a virus.
        // Though, not all entropy values can overshoot threshold value, so we also can define a virus by number of threshold it overshoot.
        // Holds commands in order for neural network, so it can correctly work with given data
        private List<Tuple<string, double>> _commandEntropyThresholdFilterList;

        // Neural network is used to make command threshold filter as a more adaptive instrument.
        // It might learn on the results of the command threshold filter and then be more flexible.
        // It can also include learning mode, where user can teach network and adjust it to the requirments
        private NeuralNetwork _commandThresholdNeuralNetwork;

        // Folder for disassembler data files
        [NonSerialized]
        private const string NEURAL_NETWORK_DATA = "NeuralData.nnd";

        // Discrepancy criterion count was derived experimentally from the experiments.
        // If number of discrepancy criterions will be more than given number, than there is big possibility 
        // of non-author changes due to the drastic change of the program nature.
        [NonSerialized]
        private const int DISCREPANCY_CRITERION_COUNT = 22;

        #endregion

        #region Constructor

        public DisassemblerAnalyzer()
        {
            // Initialiaze command threshold filter
            _commandEntropyThresholdFilterList = new List<Tuple<string, double>>();
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("inc", 0.0060935));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("dec", 0.003465));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("neg", 0.0012695));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jmp", 0.0085135));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("call", 0.0151115));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("ret", 0.005825));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("sub", 0.007995));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("sbb", 0.0040975));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("cmp", 0.01059));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("add", 0.021305));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("and", 0.010315));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("or", 0.0058755));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("xor", 0.0055585));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("not", 0.0011655));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("test", 0.0049205));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("push", 0.0064165));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("pop", 0.005588));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("nop", 0.0087785));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jl", 0.003693));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jle", 0.0015375));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jge", 0.001325));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jg", 0.0012025));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jb", 0.0083675));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jbe", 0.0027105));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jae", 0.0090315));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("ja", 0.0010915));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("je", 0.0044205));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jne", 0.0045995));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("js", 0.0024585));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("jns", 0.003481));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("int", 0.013317));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("mov", 0.0128165));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("xchg", 0.0023355));
            _commandEntropyThresholdFilterList.Add(new Tuple<string, double>("lea", 0.0132795));


            // Initialize neural network
            // Number of entry nodes is the same as number of threshold commands
            // Number of hidden nodes is half of the number of threshold commands
            // Number of outputs is two. It is a author change or virus one.
            _commandThresholdNeuralNetwork = new NeuralNetwork(_commandEntropyThresholdFilterList.Count, _commandEntropyThresholdFilterList.Count / 2, 2, 0.1f, -0.3f, 0.3f);
            if (File.Exists(NEURAL_NETWORK_DATA))
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                using (Stream fStream = new FileStream(NEURAL_NETWORK_DATA, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (fStream.Length > 0)
                    {
                        _commandThresholdNeuralNetwork = (NeuralNetwork)binFormat.Deserialize(fStream);
                    }
                }
            }
        }

        ~DisassemblerAnalyzer()
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(NEURAL_NETWORK_DATA, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, _commandThresholdNeuralNetwork);
            }
        }

        #endregion

        #region Methods

        public List<Tuple<string, double>> GetCommandNamesList()
        {
            return _commandEntropyThresholdFilterList;
        }

        // Analyzes disassembler data for authorship
        // Returns value of discrepancy criterion. Number of thresholds, which were reached and overshoot
        public int CalculateDiscrepancyCriterionByThresholdFilter(DisassemblerComparator disassemblerComparator)
        {
            int discrepancyCriterion = 0;

            // Iterate through disassembler comparator and calculate discrepancy criterion
            for (int i = 0; i < disassemblerComparator.GetDisassemblerCommandInfoDelta().Count; ++i)
            {
                // Check, if command is in the filter
                for (int j = 0; j < _commandEntropyThresholdFilterList.Count; ++j)
                {
                    if (_commandEntropyThresholdFilterList[j].Item1.Equals(disassemblerComparator.GetDisassemblerCommandInfoDelta()[i].Name))
                    {
                        // Check, if delta of entropy exceeds threshold value
                        if (disassemblerComparator.GetDisassemblerCommandInfoDelta()[i].Entropy >=
                           _commandEntropyThresholdFilterList[j].Item2)
                        {
                            discrepancyCriterion++;
                        }
                    }
                }
            }

            return discrepancyCriterion;
        }

        // Analyzes disassembler data for authorship via neural network
        // Returns change ownership
        public OwnershipAnalyze CalculateDiscrepancyCriterionByNeuralNetwork(DisassemblerComparator disassemblerComparator)
        {
            List<double> inputDeltaList = new List<double>();
            // We iterate through disassembler command info delta and try to form array, which will preserve the same order of commands
            // thus neural network will always have same types of input nodes, which corresponds with given command
            for (int i = 0; i < _commandEntropyThresholdFilterList.Count; ++i)
            {
                inputDeltaList.Add(0.0f);
                for (int j = 0; j < disassemblerComparator.GetDisassemblerCommandInfoDelta().Count(); ++j)
                {
                    if (disassemblerComparator.GetDisassemblerCommandInfoDelta()[j].Name.Equals(_commandEntropyThresholdFilterList[i].Item1))
                    {
                        inputDeltaList[i] = (double)disassemblerComparator.GetDisassemblerCommandInfoDelta()[j].Entropy;
                        break;
                    }
                }
            }

            // Analyze input through neural network
            List<double> authorshipAnalyze = _commandThresholdNeuralNetwork.Predict(inputDeltaList);

            // 0 - Author
            // 1 - Virus
            OwnershipAnalyze ownershipAnalyze = new OwnershipAnalyze();
            ownershipAnalyze.authorOwnerChance = authorshipAnalyze[0];
            ownershipAnalyze.virusOwnerChance = authorshipAnalyze[1];
            ownershipAnalyze.discrepancyCriterionCount = CalculateDiscrepancyCriterionByThresholdFilter(disassemblerComparator);
            return ownershipAnalyze;
        }

        // Analyzes disassembler data for authorship via neural network
        // Returns change ownership
        public void TrainNeuralNetworkByDiscrepancyCriterion(DisassemblerComparator disassemblerComparator)
        {
            // We assume, that if half of discrepancy criterion is reached, changes are made by virus
            List<double> targetValuesList = new List<double>();
            // 0 - Author
            // 1 - Virus
            if (CalculateDiscrepancyCriterionByThresholdFilter(disassemblerComparator) > DISCREPANCY_CRITERION_COUNT)
            {
                // Author
                targetValuesList.Add(0.0f);
                // Virus
                targetValuesList.Add(1.0f);
            }
            else
            {
                // Author
                targetValuesList.Add(1.0f);
                // Virus
                targetValuesList.Add(0.0f);
            }

            List<double> inputDeltaList = new List<double>();
            // We iterate through disassembler command info delta and try to form array, which will preserve the same order of commands
            // thus neural network will always have same types of input nodes, which corresponds with given command
            for (int i = 0; i < _commandEntropyThresholdFilterList.Count; ++i)
            {
                inputDeltaList.Add(0.0f);
                for (int j = 0; j < disassemblerComparator.GetDisassemblerCommandInfoDelta().Count(); ++j)
                {
                    if (disassemblerComparator.GetDisassemblerCommandInfoDelta()[j].Name.Equals(_commandEntropyThresholdFilterList[i].Item1))
                    {
                        inputDeltaList[i] = (double)disassemblerComparator.GetDisassemblerCommandInfoDelta()[j].Entropy;
                        break;
                    }
                }
            }

            _commandThresholdNeuralNetwork.Train(inputDeltaList, targetValuesList);
        }

        #endregion
    }
}
