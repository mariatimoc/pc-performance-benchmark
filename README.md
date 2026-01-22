\# PC Performance Benchmark (C DLL + C# WPF)



Small desktop app that shows hardware information and runs a few performance tests.

The core logic is implemented in a C/C++ DLL (CPUID + benchmarks), and the UI is a C# WPF app that calls the DLL using P/Invoke.



---



\## Features



\### CPU Info (CPUID)

\- Producer name (GenuineIntel / AuthenticAMD)

\- Brand string (full CPU name)

\- Family / Model / Stepping / Type

\- Address size: physical + virtual bits

\- Frequencies (base / max / bus) when supported

\- Cache info for all levels (L1/L2/L3): line size, ways, sets, inclusive, etc.



\### System Info (Windows API)

\- Architecture (x86 / x64 / ARM)

\- Logical processors

\- Total RAM (MB)



\### Performance Tests

\- Memory random access (pointer-chasing, low locality; emphasizes memory latency behavior)

\- Gauss int (integer workload using Gauss elimination on an NxN matrix)

\- Floating numbers (Jacobi iterations on a 2D grid; floating-point workload)



---



\## Technologies

\- C / C++ (DLL, CPUID + benchmarks)

\- C# + WPF (GUI)

\- P/Invoke (DllImport)

\- Windows API (SYSTEM\_INFO, GlobalMemoryStatusEx)



---



\## Project Structure

\- `Proiect\_SSC/` - C/C++ DLL project

\- `PerformanceGUI/` - C# WPF GUI project

\- `pc-performance-benchmark.sln` - solution containing both projects



---



\## Build \& Run (Visual Studio)



Important: CPUID uses inline assembly (`\_asm`), so the DLL must be built as x86/Win32.



1\. Open `pc-performance-benchmark.sln` in Visual Studio

2\. Set platforms:

&nbsp;  - `PerformanceGUI` -> x86

&nbsp;  - `Proiect\_SSC` -> Win32

3\. Build Solution

4\. Make sure the DLL is next to the GUI executable (same folder):

&nbsp;  - `PerformanceGUI\\bin\\x86\\Debug\\PerformanceGUI.exe`

&nbsp;  - `PerformanceGUI\\bin\\x86\\Debug\\Proiect\_SSC.dll`

5\. Run `PerformanceGUI`



---



\## Notes

\- Run once: runs the selected test one time.

\- Run average: runs multiple times and shows average/min/max time.

\- For Memory random access, `stepsPerElement` controls how many pointer jumps are performed per element.



