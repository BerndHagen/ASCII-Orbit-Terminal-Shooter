<p align="center">
  <img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/ascii-orbit-logo.png" alt="ASCII Orbit Logo" width="128" />
</p>
<h1 align="center">ASCII-Orbit - Terminal Shooter</h1>
<p align="center">
  <b>Navigate an ASCII-rendered universe and defend against waves of alphabetical invaders.</b><br>
  <b>Fast-paced terminal shooting action with escalating difficulty, power-ups and a ranking system.</b>
</p>
<p align="center">
  <a href="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/releases"><img src="https://img.shields.io/github/v/release/BerndHagen/ASCII-Orbit-Terminal-Shooter?include_prereleases&style=flat-square&color=CD853F" alt="Latest Release"></a>&nbsp;&nbsp;<a href="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/blob/main/LICENSE"><img src="https://img.shields.io/badge/License-MIT-blue?style=flat-square" alt="License"></a>&nbsp;&nbsp;<a href="https://dotnet.microsoft.com/en-us/download/dotnet/8.0"><img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square" alt=".NET Version"></a>&nbsp;&nbsp;<img src="https://img.shields.io/badge/Platform-Windows%20%7C%20Linux%20%7C%20macOS-0078D6?style=flat-square" alt="Platform">&nbsp;&nbsp;<img src="https://img.shields.io/badge/Status-Active-brightgreen?style=flat-square" alt="Status">&nbsp;&nbsp;<a href="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/issues"><img src="https://img.shields.io/github/issues/BerndHagen/ASCII-Orbit-Terminal-Shooter?style=flat-square&color=orange&label=Issues" alt="Issues"></a>
</p>

**ASCII-Orbit** is a console-based C# game that turns a plain terminal window into a battlefield. You pilot an arrow ship along the bottom of the screen and shoot down letters of the alphabet as they descend toward you. Every level introduces the next letter, from **A** all the way to **Z**, and each one moves differently - gentle drifters give way to weavers, homing hunters, divers, and finally fast, armoured reapers. Let one slip past and it costs you a life.

The game is built on a flicker-free, double-buffered renderer and runs anywhere .NET 8 runs: **Windows, Linux and macOS** terminals alike.

### **Key Features**

- **26 distinct invaders** - every letter A-Z has its own movement behaviour and difficulty.
- **Power-ups & bonus pickups** - rapid fire, spread shot, shield, extra life, instant-point gems and a rare screen-clearing blast, dropped by destroyed invaders.
- **Game A / Game B, 1 or 2 players** - pick normal or hard difficulty; in two-player the turn passes to the other player each time a ship is lost.
- **Cross-platform** - a single codebase for Windows, Linux and macOS terminals; the play field fills whatever terminal size you give it.
- **Escalating difficulty** - a gentle early curve that ramps into faster, armoured, homing invaders.
- **Arcade presentation** - a bezel frame, a 1UP / HIGH SCORE / 2UP header, a flickering title, an explode-and-respawn death, and a persistent top-five high-score table with three-letter initials entry.
- **Ranking system** - finish with a grade from **F** to **S+** based on your score.

## **Table of Contents**

1. [System Requirements](#system-requirements)
2. [Controls](#controls)
3. [Gameplay](#gameplay)
    - [The Alphabet of Invaders](#the-alphabet-of-invaders)
    - [Power-ups](#power-ups)
    - [Scoring & Extra Lives](#scoring--extra-lives)
4. [Rank Requirements](#rank-requirements)
5. [Running ASCII-Orbit](#running-ascii-orbit)
    - [Option 1: Download and Play](#option-1-download-and-play)
    - [Option 2: Build and Run from Source](#option-2-build-and-run-from-source)
    - [Diagnostics](#diagnostics)
6. [Troubleshooting](#troubleshooting)
7. [License Information](#license-information)
8. [Screenshots](#screenshots)

## **System Requirements**

- **Operating System:** Windows, Linux or macOS
- **Runtime:** [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (or the SDK to build from source)
- **Terminal size:** at least **80 x 30** characters (the game shows a prompt and waits if the window is smaller)
- **Audio:** sound effects use the PC speaker and are **Windows-only**; other platforms play silently

## **Controls**

| Key | Action |
|-----|--------|
| **← / →** (or **A / D**) | Move the ship left / right |
| **↑ / ↓** (or **W / S**) | Menu navigation; pick initials on a new high score |
| **Spacebar** | Fire (also: select menu / confirm / continue) |
| **P** | Pause / resume |
| **Esc** or **Q** | Quit to title (from the title screen, quit the game) |

## **Gameplay**

Invaders descend from the top of the play field. Destroy them with your shots before they reach the bottom - each one that gets through costs a life, and the game ends when you run out. Clear enough invaders to advance to the next level: **5** on level 1, and **2 more** for each level after (7, 9, 11, ...).

### **The Alphabet of Invaders**

Each level introduces the next letter of the alphabet, and the letters are grouped into behaviour bands that get progressively more dangerous:

| Letters | Band | Behaviour |
|---------|------|-----------|
| A - C | Drifter | Falls straight down at a steady pace |
| D - F | Weaver | Sways from side to side as it descends |
| G - I | Glider | Slides sideways, bouncing off the walls |
| J - L | Hunter | Creeps toward your column while falling |
| M - O | Diver | Cruises slowly, then accelerates into a dive |
| P - R | Stutterer | Descends in stop-and-go bursts |
| S - U | Zigzag | Fast, sharp side-to-side weaving (U is armoured) |
| V - X | Stalker | Fast homing pursuit (W and X are armoured) |
| Y - Z | Reaper | Fast diving hunters, armoured - the deadliest |

After **Z**, the letters stay at Z but the overall pace keeps climbing, so a deep run remains a real challenge.

### **Power-ups**

Destroying an invader has a small chance to drop a power-up that drifts down. Move underneath it to collect it:

| Symbol | Power-up | Effect |
|:------:|----------|--------|
| ♥ | **1-UP** | Grants an extra life |
| ★ | **Bonus** | Instant points (scales with your level) |
| » | **Rapid Fire** | Much faster firing for a short time |
| ≡ | **Spread Shot** | Fires three shots at once for a short time |
| ◆ | **Shield** | Blocks the next invader that would cost you a life |
| ◎ | **Blast** | Destroys every invader on screen at once (rare) |

### **Scoring & Extra Lives**

You score points for every invader destroyed; tougher letters and higher levels are worth more, and **Bonus** pickups award instant points. You also earn an **extra life** at **8,000** points, with each subsequent milestone doubling the previous one (**16,000**, **32,000**, and so on). The **top five high scores** are saved between sessions, and beating one lets you enter your three-letter initials.

## **Rank Requirements**

At the end of a run your performance is graded from **F** up to **S+** based on your final score. The thresholds are cumulative and grow by 750 points per rank:

| Rank | Score Required (Pts.) | Rank | Score Required (Pts.) |
|:----:|:---------------------:|:----:|:---------------------:|
|   F  |        0 Pts.         |  C+  |      63750 Pts.       |
|  F+  |       3000 Pts.       |  B-  |      74250 Pts.       |
|  E-  |       6750 Pts.       |   B  |      85500 Pts.       |
|   E  |      11250 Pts.       |  B+  |      97500 Pts.       |
|  E+  |      16500 Pts.       |  A-  |     110250 Pts.       |
|  D-  |      22500 Pts.       |   A  |     123750 Pts.       |
|   D  |      29250 Pts.       |  A+  |     138000 Pts.       |
|  D+  |      36750 Pts.       |  S-  |     153000 Pts.       |
|  C-  |      45000 Pts.       |   S  |     168750 Pts.       |
|   C  |      54000 Pts.       |  S+  |     185250 Pts.       |

## **Running ASCII-Orbit**

### **Option 1: Download and Play**

1. Download the latest release from the [Releases page](https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/releases) and extract the archive.
2. Make sure the [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) is installed.
3. Run the executable (`ASCII Orbit.exe` on Windows, or `./"ASCII Orbit"` on Linux/macOS) from a terminal window that is at least 80 x 30 characters.

### **Option 2: Build and Run from Source**

You need the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

```bash
git clone https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter.git
cd ASCII-Orbit-Terminal-Shooter
dotnet run --project "ASCII Orbit.csproj" -c Release
```

To produce a self-contained, single-file build for your platform (replace the runtime identifier as needed: `win-x64`, `linux-x64`, `osx-arm64`, ...):

```bash
dotnet publish "ASCII Orbit.csproj" -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

### **Diagnostics**

The game accepts a few command-line flags, useful for development and CI:

| Flag | Description |
|------|-------------|
| `--selftest` | Runs a headless simulation of thousands of frames and exits (no terminal required). |
| `--dump` | Renders representative frames (gameplay, title, game over) to plain text and exits. |
| `--mute` | Starts the game with sound disabled. |

## **Troubleshooting**

**"Terminal too small" message:**
- Resize your terminal window to at least 80 x 30 characters. The game resumes automatically once it fits.

**No sound effects:**
- Sound effects use the PC speaker and are available on Windows only; Linux and macOS run silently by design.

**Garbled box / arrow characters:**
- Use a terminal with a monospace font and UTF-8 support (Windows Terminal, modern macOS/Linux terminals). The game sets UTF-8 output automatically where possible.

## **License Information**

ASCII-Orbit is released under the **MIT License**. You are free to use, copy, modify, merge, publish, distribute, sublicense, and sell copies of the software, provided that the copyright notice and this permission notice are included in all copies or substantial portions of the software. The software is provided "as is", without warranty of any kind.

See the [LICENSE](LICENSE) file for the full text.

## **Screenshots**

If you'd like a preview of ASCII-Orbit before downloading, the screenshots below show the game's core screens. Note that future updates may introduce additional features.

<table>
  <tr>
    <th>ASCII-Orbit - Title</th>
    <th>ASCII-Orbit - In Game</th>
  </tr>
  <tr>
    <td><a href="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/screenshot-title.png" target="_blank" rel="noopener noreferrer"><img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/screenshot-title.png" alt="ASCII-Orbit Title" width="450"></a></td>
    <td><a href="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/screenshot-gameplay.png" target="_blank" rel="noopener noreferrer"><img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/screenshot-gameplay.png" alt="ASCII-Orbit In Game" width="450"></a></td>
  </tr>
  <tr>
    <th>ASCII-Orbit - High Score</th>
    <th>ASCII-Orbit - Game Over</th>
  </tr>
  <tr>
    <td><a href="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/screenshot-highscore.png" target="_blank" rel="noopener noreferrer"><img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/screenshot-highscore.png" alt="ASCII-Orbit High Score" width="450"></a></td>
    <td><a href="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/screenshot-gameover.png" target="_blank" rel="noopener noreferrer"><img src="https://github.com/BerndHagen/ASCII-Orbit-Terminal-Shooter/raw/main/images/screenshot-gameover.png" alt="ASCII-Orbit Game Over" width="450"></a></td>
  </tr>
</table>
