# ASCII-Orbit - Terminal Shooter

**ASCII-Orbit** is a console-based C# game that transforms a simple console window into a battlefield. In this game, players navigate an ASCII-rendered universe, dodging and destroying waves of alphabetical invaders as they descend toward the bottom of the screen. 

Players control an arrow symbol, tasked with defending against enemies represented by letters of the alphabet. As players progress through levels, the speed of the enemies increases, ramping up the challenge. The objective is to shoot down these invaders before they reach the bottom of the screen, as each enemy that gets through costs the player a life. The game continues to escalate in difficulty, testing your reflexes constantly more.

At the end of the game, your performance is evaluated based on your final score, with rankings ranging from **S+** to lower grades.

## **Technical Details**

- **Platform:** Windows
- **Development Environment:** Visual Studio 2022
- **Framework:** [.NET Framework 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) or higher
- **Compatibility:** The game is developed exclusively for `Windows`. However, with Mono, it may run on `Linux`, though with some limitations:
  - Sound effects are not supported on Linux.
  - Console size adjustments might be required for optimal gameplay.

## Running ASCII-Orbit

To start playing **ASCII-Orbit**, you have two options: download the game directly from the releases or run it manually via the command line.

### **Option 1: Download and Play (Recommended)**

1. **Download the Game**: Go to the [Releases page](https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/releases) and download the latest version of the game as a **ZIP** file.

2. **Extract the ZIP File**: Once downloaded, extract the contents of the ZIP file to your desired location.

3. **Run the Game**: Double-click `ASCII Orbit.exe` to start the game.

### **Option 2: Run with Command Line (For Developers or Advanced Users)**

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

Make sure you launch the game in a **new** CMD window to prevent buffer overflow exceptions.

## Rank Requirements

In **ASCII-Orbit**, your performance at the end of the game is ranked based on your final score. Additionally, players earn `Extra Lives` by reaching specific score milestones, beginning at **10,000**. Each subsequent milestone doubles the previous one, progressing to **20,000**, **40,000** and so forth.
Below is the list of score thresholds required to achieve each rank:

| Rank | Score Required (Pts.) | Rank | Score Required (Pts.) |
|------|-----------------------|------|-----------------------|
|   F  |        0 Pts.         |  C+  |      63750 Pts.       |
|  F+  |       3000 Pts.       |  B-  |      74250 Pts.       |
|  E-  |       6750 Pts.       |   B  |      85500 Pts.       |
|   E  |       11250 Pts.       |  B+  |      97500 Pts.       |
|  E+  |      16500 Pts.       |  A-  |      110250 Pts.       |
|  D-  |      22500 Pts.       |   A  |      123750 Pts.       |
|   D  |      29250 Pts.       |  A+  |      138000 Pts.       |
|  D+  |      36750 Pts.       |  S-  |      153000 Pts.       |
|  C-  |      45000 Pts.       |   S  |      168750 Pts.       |
|   C  |      54000 Pts.       |  S+  |      185250 Pts.       |

Players score points by hitting invaders. Each invader hit grants the player **50 points**. As players advance through levels, the points earned per hit increase by an additional **50 points** for each new level. To reach the next level, players must defeat a specific number of invaders, which increases with each level by 2

For example:
- To progress from Level 1 to Level 2, a player needs to defeat **5** invaders.
- To progress from Level 2 to Level 3, a player needs to defeat **7** invaders.
- To progress from Level 3 to Level 4, a player needs to defeat **9** invaders.

## License Information

ASCII-Orbit is licensed under the MIT License. You are granted permission, free of charge, to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of this project and its associated documentation files, under the following conditions:

1. **Copyright Notice:** The above copyright notice and this permission notice must be included in all copies or substantial portions of the project.

2. **Attribution:** If you use this project, you should credit the original creator in your work, documentation, or any materials that incorporate or use this project. Additionally, please include a link to the original repository created by the author when giving attribution.

3. **No Warranty:** This project is provided "as is," without any warranties, whether express or implied, including but not limited to implied warranties of merchantability, fitness for a particular purpose, or non-infringement. In no case shall the author or copyright holder be liable for any claims, damages, or other liabilities, whether in a contract, tort, or otherwise, arising from the use of this project or any other dealings with the Software.

For complete license details, please refer to the [MIT License](LICENSE).

## Screenshots
Before downloading the project, you can review the screenshots below to preview its appearance. Each image displays a different screen of the Windows console application. Be aware that future updates may introduce additional features.

| ASCII-Orbit - Titlescreen    | ASCII-Orbit - In Game        | ASCII-Orbit - Game Over      |
|------------------------------|------------------------------|------------------------------|
| <img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/img/v1.0.0-ascii-orbit-title.png" width="300px"> | <img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/img/v1.0.0-ascii-orbit-game.png" width="300px"> | <img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/img/v1.0.0-ascii-orbit-gameover.png" width="300px"> |
