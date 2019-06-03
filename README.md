# Project Everest

### Round 2 of Development (Senior Project Quarter 1)
___
#### Newly Implemented Features
##### Variable Player Number
***(Completed 4/30/19)***
After much work combing through all of the game's systems that were initially designed with only two players in mind, the code has been changed to allow for 2 to 4 players to play together at once. The number of players is now selected at the initial menu and carried into each scene using a static class. Previously, each player had a static reference to the other player that never changed, so transitioning from that to a system where different numbers of players could be playing was a challenge and a good deal of code.

##### Loading Screen Functionality
***(Completed 5/01/19)***
As a precaution to some computers being unable to run the game as fast as my own, I have built in placeholder loading screens that will eventually provide the player some indication that the game hasn't frozen while they wait for it to load.

##### Hitbox Refinement/Adjustments
***(Completed 5/10/19)***
Some of the weapons seemed to be hitting an invisible wall upon defeating a player, meaning that a player who was behind the recently defeated one could have time to escape. This was fixed to prevent unfair playstyles.

##### Cursor Design/Implementation
***(Completed 5/18/19)***
Navigating the main menu as it expands would be too difficult for someone to do without an indicator of where they were. A cursor was designed and implemented so that each player could control it if necessary, without having to rely on having a Player 1.

##### Options Menu
***(Completed 5/28)***
The options menu will still be a work in progress, but the volume of the sounds and music can now be controlled directly from the main menu.

##### Mid-game Options Menu
***(In progress)***

___
#### Features to be Developed
- additional controller support
- additional weapons
- additional levels
- animation refinement