using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    // Gives control over main features of the disassembler.
    // Main point of this class is to hold static variable, which be used across all view models.
    static class ControlManager
    {
        static private DisassemblerManager _disassemblerManager;
        static private DisassemblerComparator _disassemblerComparator;

        static public DisassemblerManager GetDisassemblerManager()
        {
            if (_disassemblerManager == null)
            {
                _disassemblerManager = new DisassemblerManager();
            }

            return _disassemblerManager;
        }

        static public DisassemblerComparator GetDisassemblerComparator()
        {
            if (_disassemblerComparator == null)
            {
                _disassemblerComparator = new DisassemblerComparator();
            }

            return _disassemblerComparator;
        }
    }
}
