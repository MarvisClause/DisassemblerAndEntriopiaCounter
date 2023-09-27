using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    internal class DisassemblerAnalyzer
    {
        // Analyzes disassembler data for authorship
        // Returns value of discrepancy criterion
        public int CalculateDiscrepancyCriterion(Dictionary<string, double> commandThresholdFilter, DisassemblerComparator disassemblerComparator)
        {
            int discrepancyCriterion = 0;

            // Iterate through disassembler comparator and calculate discrepancy criterion
            for (int i = 0; i < disassemblerComparator.GetDisassemblerCommandInfoDelta().Count; ++i)
            {
                // Check, if command is in the filter
                if (commandThresholdFilter.ContainsKey(disassemblerComparator.GetDisassemblerCommandInfoDelta()[i].Name))
                {
                    // Check, if delta of entropy exceeds threshold value
                    if (disassemblerComparator.GetDisassemblerCommandInfoDelta()[i].Entropy >=
                        commandThresholdFilter[disassemblerComparator.GetDisassemblerCommandInfoDelta()[i].Name])
                    {
                        discrepancyCriterion++;
                    }
                }
            }

            return discrepancyCriterion;
        }
    }
}
