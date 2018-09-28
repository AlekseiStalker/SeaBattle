# SeaBattle

Classic Sea Battle game with computer performed in a console application.

### Description of implementation
The game implemented using MVC pattern. 
M - Main game logic contains in Model folder. <br />
V - `ViewManager.cs` class can always be replaced on any other ClassView for deployment game on some other platform. <br />
C - Сontroller functions are performed by the `GameControll.cs` class. 
<br />Tests folder contains NUnit tests of the game.

## Game features
- Ability to save/continue game (the logic of serialization is implemented in 3 versions of `Xml`, `JSON`, `Binary`)
- During the game, you can always go to the menu and see the current game statistics
- If computer hit your ship, he will shoot until kills your ship completely.
- When you press the `Q` key, you can see the computer game board =)

![ship](https://user-images.githubusercontent.com/29926552/46195196-518cb580-c30c-11e8-8ccf-d6ada6a60196.gif)

