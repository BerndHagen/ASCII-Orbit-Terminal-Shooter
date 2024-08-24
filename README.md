# ASCII-Orbit - Terminal Shooter

**ASCII-Orbit** is a console-based C# game that transforms a simple console window into an intense battlefield. In this game, players navigate an ASCII-rendered universe, dodging and destroying waves of alphabetical invaders as they descend toward the bottom of the screen. 

Players control an arrow symbol, tasked with defending against enemies represented by letters of the alphabet. As players progress through levels, the speed of the enemies increases, ramping up the challenge. The objective is to shoot down these invaders before they reach the bottom of the screen, as each enemy that gets through costs the player a life. The game continues to escalate in difficulty, testing your reflexes constantly more.

At the end of the game, your performance is evaluated based on your final score, with rankings ranging from **S** (the highest) to lower grades.

### **Technical Details**
- **Platform:** Windows
- **Framework:** [.NET Framework 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) or higher
- **Compatibility:** The game is developed exclusively for `Windows`. However, with [Mono](https://www.mono-project.com/download/stable/), it may run on `Linux`, though with some limitations:
  - Sound effects are not supported on Linux.
  - Console size adjustments might be required for optimal gameplay.

Start your journey in the ASCII universe and see how long you can survive the onslaught!

## Running ASCII-Orbit

To start playing **ASCII-Orbit**, you have two options: download the game directly from the releases or run it manually via the command line.

### **Option 1: Download and Play (Recommended)**

1. **Download the Game**: Go to the [Releases page](https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/releases) and download the latest version of the game as a ZIP file.

2. **Extract the ZIP File**: Once downloaded, extract the contents of the ZIP file to your desired location.

3. **Run the Game**: Double-click `ASCII Orbit.exe` to start the game.

> **Note:** Make sure you have the [**.NET Framework 4.7.2**](https://dotnet.microsoft.com/download/dotnet-framework) or later installed on your system to run the game.

## **Option 2: Run with Command Line (For Developers or Advanced Users)**

For users who prefer to clone the repository and run the game from their desktop via the command line, follow these steps:

1. **Open Command Prompt**: Open Command Prompt as an administrator.

2. **Clone the Repository**:
   ```bash
   git clone https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter.git

2. **Navigate to the Release Folder**:
   ```bash
   cd ASCII-Orbit-Terminal-Shooter\release

2. **Run the Application**:
   ```bash
   start cmd /k ".\ASCII Orbit.exe"

> **Note:** Launch the game in a new CMD window to prevent buffer overflow exceptions.

## Rank Requirements

In **ASCII-Orbit**, your performance at the end of the game is ranked based on your final score. Additionally, players earn `Extra Lives` by reaching specific score milestones, beginning at **10,000**. Each subsequent milestone doubles the previous one, progressing to **20,000**, **40,000** and so forth.
Below is the list of score thresholds required to achieve each rank:

| Rank | Score Required (Pts.) | Rank | Score Required (Pts.) |
|------|-----------------------|------|-----------------------|
|   F  |        0 Pts.         |  C+  |      26000 Pts.       |
|  F+  |       3500 Pts.       |  B-  |      28500 Pts.       |
|  E-  |       6000 Pts.       |   B  |      31000 Pts.       |
|   E  |       8500 Pts.       |  B+  |      33500 Pts.       |
|  E+  |      11000 Pts.       |  A-  |      36000 Pts.       |
|  D-  |      13500 Pts.       |   A  |      38500 Pts.       |
|   D  |      16000 Pts.       |  A+  |      41000 Pts.       |
|  D+  |      18500 Pts.       |  S-  |      43500 Pts.       |
|  C-  |      21000 Pts.       |   S  |      46000 Pts.       |
|   C  |      23500 Pts.       |  S+  |      48500 Pts.       |

# License Information

ASCII-Orbit is licensed under the MIT License. You are granted permission, free of charge, to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of this project and its associated documentation files, under the following conditions:

1. **Copyright Notice:** The above copyright notice and this permission notice must be included in all copies or substantial portions of the project.

2. **Attribution:** If you use this project, you should credit the original creator in your work, documentation, or any materials that incorporate or use this project. Additionally, please include a link to the original repository created by the author when giving attribution.

3. **No Warranty:** This project is provided "as is," without any warranties, whether express or implied, including but not limited to implied warranties of merchantability, fitness for a particular purpose, or non-infringement. In no case shall the author or copyright holder be liable for any claims, damages, or other liabilities, whether in a contract, tort, or otherwise, arising from the use of this project or any other dealings with the Software.

For complete license details, please refer to the [MIT License](LICENSE).

# Screenshots
Before downloading ASCII-Orbit, please review the screenshots below to preview its appearance. Each image displays a different screen of the Windows console application. Be aware that future updates may introduce additional features.

| ASCII-Orbit - Titlescreen    | ASCII-Orbit - In Game         | ASCII-Orbit - Game Over      |
|------------------------------|------------------------------|------------------------------|
| <img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/img/v1.0.0-ascii-orbit-title.png" width="300px"> | <img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/img/v1.0.0-ascii-orbit-game.png" width="300px"> | <img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/img/v1.0.0-ascii-orbit-gameover.png" width="300px"> |
