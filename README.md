# DisEn - Executable File Code Modification Identifier

## Overview

DisEn is a powerful software tool developed by Makarova L.M., Kaminsky S.S, and Bryzgalov M.V. at Nikolaev, Admiral Makarov National University of Shipbuilding. This tool is designed to identify modifications made to executable file code. It provides a user-friendly interface for analyzing executable files, disassembling them, calculating entropy values, and comparing files to determine authorship and changes.

![DisEn Screenshot](https://github.com/MarvisClause/DisassemblerAndEntriopiaCounter/blob/main/ShowCase/ResearchPage.jpg)

## Features

- **Executable Disassembly**: DisEn can disassemble executable files using the dumpbin.exe utility, resulting in a text file containing the program's code.

- **Entropy Calculation**: Calculate entropy values for each command in the disassembled file to assess the level of code complexity.

- **Hashing**: Compute and store SHA-1 hashes of files for future comparisons, serving as unique file fingerprints.

- **Entropy Threshold Determination**: Automatically determine entropy thresholds for each command within the current file.

- **Entropy Difference Calculation**: Measure the difference in entropy between the current and new versions of a file.

- **File Comparison**: Compare files using entropy differences and hash comparisons to identify modifications.

## Usage

To get started with DisEn, follow these steps:

1. **Download and Install**: Download the DisEn software from [here](download-link) and install it on your computer.

2. **Open and Load File**: Launch DisEn and open the executable file you want to analyze.

3. **Analyze**: Click the "Analyze" button to disassemble the file, calculate entropy, and save the results.

4. **Compare Files**: If you have a new version of the file, load it and use DisEn to compare it to the original version.

5. **Review Results**: DisEn will display differences between the two files, helping you identify modifications.

## Customization

We have plans to add the capability of customizing disassembly techniques and specifying a set of commands for a more adaptable interaction with the software in future updates.

## Requirements

- .NET Framework
- Microsoft Visual Studio (for dumpbin.exe utility)

## References

- [What is code injection in Windows?](https://www.thefastcode.com/uk-uah/article/what-is-code-injection-on-windows)
- [Shannon's formula](http://um.co.ua/8/8-16/8-168268.html)
- [Algorithm development to accelerate file comparison](https://www.inter-nauka.com/uploads/public/14694535994336.pdf)
- [SHA-1 is a Shambles First Chosen-Prefix Collision on SHA-1 and Application to the PGP Web of Trust](https://eprint.iacr.org/2020/014.pdf)
- [Microsoft Visual Studio disassembler "dumpbin"](https://learn.microsoft.com/ru-ru/cpp/build/reference/dumpbin-reference?view=msvc-170)
