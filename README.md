# \# \\# PC Performance Benchmark (C DLL + C# WPF)

# 

# \# 

# 

# \# A small desktop application that measures \\\*\\\*hardware information\\\*\\\* and runs a few \\\*\\\*performance tests\\\*\\\* on a PC.  

# 

# \# The core measurements are implemented in a \\\*\\\*C/C++ DLL\\\*\\\* (CPUID + benchmarks), and the interface is a \\\*\\\*C# WPF GUI\\\*\\\* that calls the DLL using \\\*\\\*P/Invoke\\\*\\\*.

# 

# \# 

# 

# \# ---

# 

# \# 

# 

# \# \\## Features

# 

# \# \\- \\\*\\\*CPU Info (CPUID)\\\*\\\*

# 

# \# \&nbsp; - Producer name (ex: GenuineIntel / AuthenticAMD)

# 

# \# \&nbsp; - Brand string (full CPU name)

# 

# \# \&nbsp; - Family / Model / Stepping / Type

# 

# \# \&nbsp; - Address size: physical + virtual bits

# 

# \# \&nbsp; - Frequencies (base / max / bus) when supported

# 

# \# \&nbsp; - Cache info for all levels (L1/L2/L3): line size, ways, sets, inclusive, etc.

# 

# \# \\- \\\*\\\*System Info (Windows API)\\\*\\\*

# 

# \# \&nbsp; - Architecture (x86 / x64 / ARM)

# 

# \# \&nbsp; - Logical processors

# 

# \# \&nbsp; - Total RAM (MB)

# 

# \# \\- \\\*\\\*Performance Tests\\\*\\\*

# 

# \# \&nbsp; - \\\*\\\*Memory random access\\\*\\\* (pointer-chasing, low locality, measures memory latency behavior)

# 

# \# \&nbsp; - \\\*\\\*Gauss int\\\*\\\* (integer-heavy workload using Gauss elimination on an NxN matrix)

# 

# \# \&nbsp; - \\\*\\\*Floating numbers\\\*\\\* (Jacobi iterations on a 2D grid, floating-point workload)

# 

# \# 

# 

# \# ---

# 

# \# 

# 

# \# \\## Technologies

# 

# \# \\- \\\*\\\*C / C++\\\*\\\* (DLL, CPUID + benchmarks)

# 

# \# \\- \\\*\\\*C#\\\*\\\*

# 

# \# \\- \\\*\\\*WPF\\\*\\\* (GUI)

# 

# \# \\- \\\*\\\*P/Invoke\\\*\\\* (DllImport calls from C# into the C DLL)

# 

# \# \\- \\\*\\\*Windows API\\\*\\\* (SYSTEM\\\_INFO, GlobalMemoryStatusEx)

# 

# \# 

# 

# \# ---

# 

# \# 

# 

# \# \\## Project Structure

# 

# \# \\- `Proiect\\\_SSC/` – C/C++ DLL project

# 

# \# \&nbsp; - exports functions like `cpu\\\_get\\\_brand`, `cpu\\\_get\\\_cache\\\_all\\\_levels`, `test\\\_memory\\\_random\\\_access`, etc.

# 

# \# \\- `PerformanceGUI/` – C# WPF GUI project

# 

# \# \&nbsp; - lists tests, runs “Run once” / “Run average”, displays results in MessageBox

# 

# \# \\- `\\\*.sln` – solution that contains both projects

# 

# \# 

# 

# \# ---

# 

# \# 

# 

# \# \\## Build \\\& Run (Visual Studio)

# 

# \# > Important: this project uses inline assembly (`\\\_asm`) for CPUID, so it must run on \\\*\\\*x86 / Win32\\\*\\\*.

# 

# \# 

# 

# \# 1\\) Open the solution (`\\\*.sln`) in Visual Studio  

# 

# \# 2\\) Set platforms:

# 

# \# \&nbsp;  - `PerformanceGUI` → \\\*\\\*x86\\\*\\\*

# 

# \# \&nbsp;  - `Proiect\\\_SSC` → \\\*\\\*Win32\\\*\\\*

# 

# \# 3\\) Build Solution  

# 

# \# 4\\) Make sure `Proiect\\\_SSC.dll` is next to the GUI executable:

# 

# \# \&nbsp;  - `PerformanceGUI\\\\bin\\\\x86\\\\Debug\\\\PerformanceGUI.exe`

# 

# \# \&nbsp;  - `PerformanceGUI\\\\bin\\\\x86\\\\Debug\\\\Proiect\\\_SSC.dll`

# 

# \# 5\\) Run `PerformanceGUI`

# 

# \# 

# 

# \# ---

# 

# \# 

# 

# \# \\## Notes

# 

# \# \\- \\\*\\\*Run once\\\*\\\* runs the selected test a single time.

# 

# \# \\- \\\*\\\*Run average\\\*\\\* runs multiple times and shows average/min/max time.

# 

# \# \\- For \\\*\\\*Memory random access\\\*\\\*, `stepsPerElement` controls how many pointer jumps are performed per element (more steps → more stable timing).

# 

# \# 

# 

# \# ---

# 

# 

# 



