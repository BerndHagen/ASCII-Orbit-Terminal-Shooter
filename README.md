# ASCII-Orbit - Save The Galaxy

ASCII-Orbit is a console-based C# application that merges classic arcade gaming with a modern programming approach. This game transforms the simple console window into a dynamic battlefield, inviting players to navigate through an ASCII universe filled with invaders. With the game area defined by a width of 60 characters and a height of 25 lines, it centralizes the action in the console's window, providing a focused gameplay experience.

The core mechanics revolve around controlling a player character, represented by a simple ASCII character, to dodge and shoot down incoming invaders. These invaders, selected from the English alphabet, descend towards the player at varying speeds, challenging the player's reaction times and strategic planning. The player's objective is to survive as long as possible while eliminating invaders, thereby increasing their score, advancing through levels and enhancing the game's difficulty.

Crafted using Visual Studio 2022, ASCII Orbit is a console-based application tailored for `Windows`, featuring compatibility with `Linux` systems albeit with certain limitations. Specifically, the game operates on Linux without sound effects and requires adjustments to the console size due to the Windows-exclusive availability of the Beep function and specific console size adaptations. The application depends on the `.NET Framework 4.7.2` or later versions to ensure a seamless and effective user experience across various platforms.

# Gameplay Mechanics

In ASCII Orbit, the game mechanics are straightforward yet engaging, designed to keep the player immersed through various stages:

- **Player Movement**: Players control their character by moving left or right to target incoming invaders.

- **Invader Dynamics**: Invaders, represented by ASCII characters, move downwards towards the player. The game increases the number of invaders and their speed as the player progresses.

- **Scoring System**: Points are awarded for each invader destroyed. The total score reflects the player's success in eliminating threats.

- **Progression and Levels**: The game becomes more challenging as the player advances, with invaders moving faster and in more complex patterns.

- **Projectile Mechanics**: Shooting is the primary action for attacking invaders. Players must aim their shots to intersect with the invaders' paths.

- **Live Status Updates**: The game displays the current score, lives left and the level to keep the player informed of their progress.

- **Life and Survival**: Players start with a set number of lives, which are lost when hit by invaders. The game ends when all lives are depleted, leading to a game over screen that generates a rank based on the player's stats.

- **Extra Lives**: Players earn extra lives at score milestones (10,000, 20,000, 40,000, 80,000, etc.), helping them tackle the increasing difficulty.
  
If the Windows console application ASCII-Orbit receives updates in the future, it may include new game elements, improved game mechanics and necessary bug fixes.

# Copyright

This ASCII-Orbit application is licensed under the [MIT License](LICENSE).

**MIT License**
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files, to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense and/or sell copies of the Software, subject to the following conditions:

1. The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

2. Users of the Software should provide proper attribution to the original creator in their projects, documentation or any other materials that make use of this Software. Additionally, users should include a link to the original Repository created by me when providing attribution.

3. The Software is provided "as is," without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose, and non-infringement. In no event shall the authors or copyright holders be liable for any claim, damages, or other liability, whether in an action of contract, tort, or otherwise, arising from, out of, or in connection with the Software or the use or other dealings in the Software.

For more details, please refer to the [MIT License](LICENSE).

# Screenshots
Prior to downloading either the Setup or Project folder for the ASCII-Orbit Software, you have the option to preview its appearance through the screenshots provided below. These images highlight the visual characteristics of the windows console application. Please note that future updates may introduce additional features.

| StartUp Screen               | InGame Screen                | GameOver Screen              |
|------------------------------|------------------------------|------------------------------|
| <img src="https://github.com/BerndHagen/MetaEdit-Tag-Editor/raw/main/img/v1.0.0-metaedit_startup.png" width="300px"> | <img src="https://github.com/BerndHagen/MetaEdit-Tag-Editor/raw/main/img/v1.0.0-metaedit_inputs.png" width="300px"> | <img src="https://github.com/BerndHagen/MetaEdit-Tag-Editor/raw/main/img/v1.0.0-metaedit_generate.png" width="300px"> |
