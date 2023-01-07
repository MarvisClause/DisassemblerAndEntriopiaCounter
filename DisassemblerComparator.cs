using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    public class DisassemblerComparator
    {
        #region Variables

        // First disassembler
        Disassembler _firstDisassembler = new Disassembler();
        // Seconds disassembler
        Disassembler _secondDisassembler = new Disassembler();

        // Total entropy data
        private double _totalEntropyDelta = 0;
        // Total instruction counter
        private UInt32 _totalInstructionCounterDelta = 0;
        // Size of the file in bytes
        private double _fileSizeDelta = 0;
        // Commands delta
        private List<DisassemblerCommandInfo> _disassemblerCommandInfoDelta = new List<DisassemblerCommandInfo>();

        #endregion

        #region Methods

        public Disassembler GetFirstDisassembler()
        {
            return _firstDisassembler;
        }

        public Disassembler GetSecondDisassembler()
        {
            return _secondDisassembler;
        }

        public double GetTotalEntropyDelta()
        {
            return _totalEntropyDelta;
        }

        public UInt32 GetTotalInstructionCounterDelta()
        {
            return _totalInstructionCounterDelta;
        }

        public double GetFileSizeDelta()
        {
            return _fileSizeDelta;
        }

        public List<DisassemblerCommandInfo> GetDisassemblerCommandInfoDelta()
        {
            return _disassemblerCommandInfoDelta;
        }

        // Return result data
        public bool CompareData(Disassembler firstDisassembler, Disassembler secondDisassembler)
        {
            if (firstDisassembler.Equals(secondDisassembler))
            {
                return true;
            }
            // Save disassembler objects
            _firstDisassembler = firstDisassembler;
            _secondDisassembler = secondDisassembler;
            // Calculate delta
            _totalEntropyDelta = Math.Abs(firstDisassembler.GetFileTotalEntropyValue() - secondDisassembler.GetFileTotalEntropyValue());
            _totalInstructionCounterDelta = (UInt32)Math.Abs(firstDisassembler.GetTotalInstructionCounter() - secondDisassembler.GetFileTotalEntropyValue());
            _fileSizeDelta = Math.Abs(firstDisassembler.GetFileSize() - secondDisassembler.GetFileSize());
            // Clear info array
            _disassemblerCommandInfoDelta.Clear();
            // Compare data
            for (int firstDisIndex = 0; firstDisIndex < firstDisassembler.GetDisassemblerCommandsInfo().Count; ++firstDisIndex)
            {
                for (int secondDisIndex = 0; secondDisIndex < secondDisassembler.GetDisassemblerCommandsInfo().Count(); ++secondDisIndex)
                {
                    if (firstDisassembler.GetDisassemblerCommandsInfo()[firstDisIndex].Name.Equals
                        (secondDisassembler.GetDisassemblerCommandsInfo()[secondDisIndex].Name))
                    {
                        if (firstDisassembler.GetDisassemblerCommandsInfo()[firstDisIndex].Count != secondDisassembler.GetDisassemblerCommandsInfo()[secondDisIndex].Count
                            || firstDisassembler.GetDisassemblerCommandsInfo()[firstDisIndex].Entropy != secondDisassembler.GetDisassemblerCommandsInfo()[secondDisIndex].Entropy)
                        {
                            DisassemblerCommandInfo disassemblerCommandInfo = new DisassemblerCommandInfo();
                            disassemblerCommandInfo.Name = firstDisassembler.GetDisassemblerCommandsInfo()[firstDisIndex].Name;
                            disassemblerCommandInfo.Count = (Int32)Math.Abs(firstDisassembler.GetDisassemblerCommandsInfo()[firstDisIndex].Count - secondDisassembler.GetDisassemblerCommandsInfo()[secondDisIndex].Count);
                            disassemblerCommandInfo.Entropy = Math.Abs(firstDisassembler.GetDisassemblerCommandsInfo()[firstDisIndex].Entropy - secondDisassembler.GetDisassemblerCommandsInfo()[secondDisIndex].Entropy);
                            _disassemblerCommandInfoDelta.Add(disassemblerCommandInfo);
                        }
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
