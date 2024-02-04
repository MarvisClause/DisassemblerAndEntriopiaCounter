using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    // Analyzes disassembler results 
    // Used to define ownership of the changes (Author or virus)
    public class DisassemblerAnalyzer
    {
        #region Variables

        // Enum for more clear definition of ownership results
        public enum ChangeOwnership
        {
            Virus,
            Author
        }

        // Command threshold filter is a  filter for commands threshold.
        // If command's entropy value is bigger than than threshold, it might mean, that we have a virus.
        // Though, not all entropy values can overshoot threshold value, so we also can define a virus by number of threshold it overshoot.
        private Dictionary<string, double> _commandEntropyThresholdFilter;

        // Holds commands in order for neural network, so it can correctly work with given data
        private List<String> _commandNamesList;

        // Neural network is used to make command threshold filter as a more adaptive instrument.
        // It might learn on the results of the command threshold filter and then be more flexible.
        // It can also include learning mode, where user can teach network and adjust it to the requirments
        private NeuralNetwork _commandThresholdNeuralNetwork;

        #endregion

        #region Constructor

        public DisassemblerAnalyzer()
        {
            // Initialiaze command threshold filter
            _commandEntropyThresholdFilter = new Dictionary<string, double>();
            _commandEntropyThresholdFilter.Add("inc", 0.0060935);
            _commandEntropyThresholdFilter.Add("dec", 0.003465);
            _commandEntropyThresholdFilter.Add("neg", 0.0012695);
            _commandEntropyThresholdFilter.Add("jmp", 0.0085135);
            _commandEntropyThresholdFilter.Add("call", 0.0151115);
            _commandEntropyThresholdFilter.Add("ret", 0.005825);
            _commandEntropyThresholdFilter.Add("sub", 0.007995);
            _commandEntropyThresholdFilter.Add("sbb", 0.0040975);
            _commandEntropyThresholdFilter.Add("cmp", 0.01059);
            _commandEntropyThresholdFilter.Add("add", 0.021305);
            _commandEntropyThresholdFilter.Add("and", 0.010315);
            _commandEntropyThresholdFilter.Add("or", 0.0058755);
            _commandEntropyThresholdFilter.Add("xor", 0.0055585);
            _commandEntropyThresholdFilter.Add("not", 0.0011655);
            _commandEntropyThresholdFilter.Add("test", 0.0049205);
            _commandEntropyThresholdFilter.Add("push", 0.0064165);
            _commandEntropyThresholdFilter.Add("pop", 0.005588);
            _commandEntropyThresholdFilter.Add("nop", 0.0087785);
            _commandEntropyThresholdFilter.Add("jl", 0.003693);
            _commandEntropyThresholdFilter.Add("jle", 0.0015375);
            _commandEntropyThresholdFilter.Add("jge", 0.001325);
            _commandEntropyThresholdFilter.Add("jg", 0.0012025);
            _commandEntropyThresholdFilter.Add("jb", 0.0083675);
            _commandEntropyThresholdFilter.Add("jbe", 0.0027105);
            _commandEntropyThresholdFilter.Add("jae", 0.0090315);
            _commandEntropyThresholdFilter.Add("ja", 0.0010915);
            _commandEntropyThresholdFilter.Add("je", 0.0044205);
            _commandEntropyThresholdFilter.Add("jne", 0.0045995);
            _commandEntropyThresholdFilter.Add("js", 0.0024585);
            _commandEntropyThresholdFilter.Add("jns", 0.003481);
            _commandEntropyThresholdFilter.Add("int", 0.013317);
            _commandEntropyThresholdFilter.Add("mov", 0.0128165);
            _commandEntropyThresholdFilter.Add("xchg", 0.0023355);
            _commandEntropyThresholdFilter.Add("lea", 0.0132795);

            // Initialize commands names list
            _commandNamesList = new List<string>();
            _commandNamesList.Add("inc");
            _commandNamesList.Add("dec");
            _commandNamesList.Add("neg");
            _commandNamesList.Add("jmp");
            _commandNamesList.Add("call");
            _commandNamesList.Add("ret");
            _commandNamesList.Add("sub");
            _commandNamesList.Add("sbb");
            _commandNamesList.Add("cmp");
            _commandNamesList.Add("add");
            _commandNamesList.Add("and");
            _commandNamesList.Add("or");
            _commandNamesList.Add("xor");
            _commandNamesList.Add("not");
            _commandNamesList.Add("test");
            _commandNamesList.Add("push");
            _commandNamesList.Add("pop");
            _commandNamesList.Add("nop");
            _commandNamesList.Add("jl");
            _commandNamesList.Add("jle");
            _commandNamesList.Add("jge");
            _commandNamesList.Add("jg");
            _commandNamesList.Add("jb");
            _commandNamesList.Add("jbe");
            _commandNamesList.Add("jae");
            _commandNamesList.Add("ja");
            _commandNamesList.Add("je");
            _commandNamesList.Add("jne");
            _commandNamesList.Add("js");
            _commandNamesList.Add("jns");
            _commandNamesList.Add("int");
            _commandNamesList.Add("mov");
            _commandNamesList.Add("xchg");
            _commandNamesList.Add("lea");

            // Initialize neural network
            // Number of entry nodes is the same as number of threshold commands
            // Number of hidden nodes is half of the number of threshold commands
            // Number of outputs is two. It is a author change or author one.
            _commandThresholdNeuralNetwork = new NeuralNetwork(_commandEntropyThresholdFilter.Count, _commandEntropyThresholdFilter.Count / 2, 2, 0.1f, -0.3f, 0.3f);
        }

        #endregion

        #region Methods

        // Analyzes disassembler data for authorship
        // Returns value of discrepancy criterion. Number of thresholds, which were reached and overshoot
        public int CalculateDiscrepancyCriterionByThresholdFilter(DisassemblerComparator disassemblerComparator)
        {
            int discrepancyCriterion = 0;

            // Iterate through disassembler comparator and calculate discrepancy criterion
            for (int i = 0; i < disassemblerComparator.GetDisassemblerCommandInfoDelta().Count; ++i)
            {
                // Check, if command is in the filter
                if (_commandEntropyThresholdFilter.ContainsKey(disassemblerComparator.GetDisassemblerCommandInfoDelta()[i].Name))
                {
                    // Check, if delta of entropy exceeds threshold value
                    if (disassemblerComparator.GetDisassemblerCommandInfoDelta()[i].Entropy >=
                        _commandEntropyThresholdFilter[disassemblerComparator.GetDisassemblerCommandInfoDelta()[i].Name])
                    {
                        discrepancyCriterion++;
                    }
                }
            }

            return discrepancyCriterion;
        }

        // Analyzes disassembler data for authorship via neural network
        // Returns change ownership
        public ChangeOwnership CalculateDiscrepancyCriterionByNeuralNetwork(DisassemblerComparator disassemblerComparator)
        {
            List<float> inputDeltaList = new List<float>();
            // We iterate through disassembler command info delta and try to form array, which will preserve the same order of commands
            // thus neural network will always have same types of input nodes, which corresponds with given command
            for (int i = 0; i < _commandNamesList.Count; ++i)
            {
                inputDeltaList.Add(0.0f);
                for (int j = 0; j < disassemblerComparator.GetDisassemblerCommandInfoDelta().Count(); ++j)
                {
                    if (disassemblerComparator.GetDisassemblerCommandInfoDelta()[j].Name.Equals(_commandNamesList[i]))
                    {
                        inputDeltaList[i] = (float)disassemblerComparator.GetDisassemblerCommandInfoDelta()[j].Entropy;
                        break;
                    }
                }
            }

            // Analyze input through neural network
            List<float> authorshipAnalyze = _commandThresholdNeuralNetwork.Predict(inputDeltaList);

            // 0 - Author
            // 1 - Virus
            return authorshipAnalyze[0] > authorshipAnalyze[1] ? ChangeOwnership.Author : ChangeOwnership.Virus;
        }

        // Analyzes disassembler data for authorship via neural network
        // Returns change ownership
        public void TrainNeuralNetworkByDiscrepancyCriterion(DisassemblerComparator disassemblerComparator)
        {
            // We assume, that if half of discrepancy criterion is reached, changes are made by virus
            List<float> targetValuesList = new List<float>();
            // 0 - Author
            // 1 - Virus
            if (CalculateDiscrepancyCriterionByThresholdFilter(disassemblerComparator) > disassemblerComparator.GetDisassemblerCommandInfoDelta().Count / 2)
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


            List<float> inputDeltaList = new List<float>();
            // We iterate through disassembler command info delta and try to form array, which will preserve the same order of commands
            // thus neural network will always have same types of input nodes, which corresponds with given command
            for (int i = 0; i < _commandNamesList.Count; ++i)
            {
                inputDeltaList.Add(0.0f);
                for (int j = 0; j < disassemblerComparator.GetDisassemblerCommandInfoDelta().Count(); ++j)
                {
                    if (disassemblerComparator.GetDisassemblerCommandInfoDelta()[j].Name.Equals(_commandNamesList[i]))
                    {
                        inputDeltaList[i] = (float)disassemblerComparator.GetDisassemblerCommandInfoDelta()[j].Entropy;
                        break;
                    }
                }
            }

            _commandThresholdNeuralNetwork.Train(inputDeltaList, targetValuesList);
        }

        #endregion
    }
}
