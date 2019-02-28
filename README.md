# Project Hierarchy Breakdown
Because it was pointed out to me that the code and project that I have designed could use more explanation, here is an explanation of how the project is structured. Explanations of the code will be included as comments in future versions of the code.

___

## CameraShaker
This is the GameObject that stores the Camera and all objects that need to move along with the Camera. It is a wrapper for the MainCamera that allows effects to be applied without affecting the Camera's position.

### MainCamera
This is the basic Camera that came with the scene. It uses the script called Camera to move along with the Players.

#### Killboxes
This GameObject stores the four boxes that enclose the Camera. Each one of these boxes is a Trigger collider that will kill a Player on contact.

#### Sunlight
This is a light that is used to light the Cave level even further than it already is. This is disabled in all levels and likely will be removed in the future, so don't worry about it too much.

#### Background
This is the GameObject that stores each of the individual Background sprites. The sprites are unordered within this GameObject and are instead attached with the Parallax script and given a speed at which to move with the Camera. A speed of 1 means the background will move perfectly with the camera, and a speed of 0 means the background will not move at all.

#### Minimap
This is a Canvas GameObject that contains a RawImage component that takes in input from the Minimap Camera and uses it to render the minimap in the corner of the screen.

#### Minimap Camera
This is another camera that renders the scene. Instead of rendering directly to the screen, it channels its output to the Render Texture Minimap, which is then used as the texture to a RawImage in the Minimap component. This camera uses a special culling mask to avoid rendering anything that does not belong to the Minimap layer. The Main Camera does the opposite, rendering everything except the Minimap layer.

___

## CameraPath
This is the GameObject that stores the points for the Main Camera to follow. Each of these points is a GameObject that can be placed at any position, and the GameControl script will pick it up and use it as a waypoint for the Main Camera. **IMPORTANT: These GameObjects are order-dependant, so the first GameObject that the Camera should go to must be placed first in the hierarchy, and the last one as last, etc.**

___

### Player
Honestly, Player 1 and Player 2 are essentially the same thing, just tinted slightly different colors and using different inputs, so I will be grouping them together here. Each Player currently has a Sprite Renderer that displays and tints the Sprite used, a Rigidbody2D for physics with Z-rotation frozen to prevent unwanted rotation, a script called stickman, a BoxCollider2D for collisions, and an Animator for the basic running animation. For the values that are attached to the stickman script, all are necessary and most should not be changed in the Unity window except for testing purposes. The ones that can and should be changed as necessary are Player Speed, Jump Strength, Wall Jump Strength, Throw Speed, and Initial Health. These could potentially be rolled into the GameControl script at some point, but are separated by Player at the moment to allow for individual Player modifications.

#### Holding
This is an empty GameObject that is placed at the rough position of the Player's hand. This is where any picked up weapons go.

#### CameraShake
This is an empty GameObject that is marked as inactive. **IMPORTANT: Do not mark as active, it is inactive on purpose.** This object is set to shake the screen as soon as it is active in the scene. It has a script attached that makes it active for a short amount of time, making the Main Camera shake whenever the Player lands from a jump.

#### Minimap Icon
This is currently a sphere that is only rendered by the Minimap Camera to show where the player is on the Minimap texture in the corner.

#### Trail
This is just a nice effect that adds a trail to the Player whenever they move. Feel free to tweak the values if you think it looks better.

#### P#_Canvas
This is a Canvas for displaying UI elements at the Player's position. This is currently limited to the health bar, but could be utilized for future improvements.

##### Slider
This is the slider that acts as the health bar above each Player. It has basic functionality that is currently run by the stickman script, and the color of its Fill is set by the Fill GameObject in its children.

#### Glow
This is a basic Point Light that I added to make the Player easier to see at some times. It could be removed easily and was just added to make debugging a little easier on the eyes.

___

## Platforms
This is a GameObject that stores all of the Platforms for the scene. They may be organized rather haphazardly on the inside, but as long as they are all in this GameObject, it should work just fine.

### Fight Platform
This is the special platform that goes at the end of the scene. It can be duplicated and arranged in any layout, it was just nice to know which one was which.

___

## Walls
This is a GameObject that stores all of the Walls for the scene. It performs the same function as the Platforms GameObject: it allows the hierarchy to be more organized.

___

## Pickups
This is a GameObject that stores all of the weapons that are available to pick up in the level. If an object is picked up, it is removed from this GameObject and attached to the respective Player's Holding GameObject. When an object is dropped, it is added back into this GameObject.

___

## Text Elements
This is the GameObject used for storing all of the text elements that are not tied to a Player's movement.

### WorldText
This is a GameObject that stores all Canvases that place their text through World Space, meaning the Text has a position in the World rather than on screen. Any Text elements in here should have their own Canvas so as to be positioned individually, and each of these Canvases should have their Render Mode set to World Space.

### ScreenText
This is a GameObject that stores all Canvases that place their text through Screen Space, meaning the Text has a position on the screen rather than in the World. Any Text in here should share the same Canvas, as they all need to share the same screen for displaying.

#### Play Again
This is the Button that appears once the game is over that asks if you'd like to play again.

#### Quit
This is the Button that appears once the game is over or when it has been paused that allows the Player to return to the main menu.

___

## EventSystem
Honestly I'm not too sure what this does other than allow the controllers to be used on the Menus, but let's leave it in.

___

## Ambient Light
This is the main lighting for the scene. It allows for making the background lighting lighter and darker. **IMPORTANT: for this and any light source, Render Mode must be set to Important in order to not have any conflicts with other light sources.**

___

## GameController
This is the GameObject with the GameControl script attached. There are many variables to modify in here, and I'll be sure to go into plenty of detail in the commented code. For easy changes, you're going to want to edit one of these values: Scale Delay, Camera Speed, Adjust Speed, Max Player Speed, Flamethrower Damage, Avalanche Damage, Platform Sprite, and Wall Sprite.
___

## Level-specific Components
Each of the current scenes has at least one unique GameObject attached, which will be explained here.

### Cave Level
##### Cave Entrance
This GameObject stores a Particle System that produces little dust particles that slowly fall from the ceiling to provide some extra details for the scene.

### Mountain Level
##### Snow
This is a Particle System that drops snow from the sky. It could be updated in the future with a snowflake texture, but it has none for now.

##### Avalancher
This is a GameObject that contains a Particle System to simulate an avalanche and a Trigger Box Collider to trigger that avalanche should a player enter the collider.

___

I'll make sure to update this periodically and please make sure to update this with any changes you make to the Project or its structure that wouldn't work as a comment in the code.
