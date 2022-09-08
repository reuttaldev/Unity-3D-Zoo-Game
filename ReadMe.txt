Dear Testers, 
Please enjoy the game.
I developed a 3D game set in a zoo.
I created this using Unity's Universal Render Pipeline, the new input system, free graphics assets, and the Terrain Tools to design an immersive environment.
I applied all aspects of game design, from start to finish - level design, scripting, lighting, sfx, and animations.

My implementation includes:
- Character Controller 
- I combined  the enemy task and  zone collision event system task to create:
       CustomEventSystem 
       Enemy with animations and the use of Navmesh.
       Player health system
I detect OnEnter collision between the enemy and player, and execute an event on impact using my custom event system.
The health system is listening to that event will reduce the player's health as needed.
The enemy follows the player when in close proximity, attacks, stops for some time, and tries to detect the character again. After 5 times it will die.
- Pause menu with resume, options, and exit
- Use of SFX 

My additions:
- Visual effects: water, wind, trees, grass, and flowers
- Cinemachine virtual camera controlled by mouse position
- Gamepad support for player movements
- Health bar for the player 
- Animated animals: bear, tiger, and spiders chasing you
- Using Git Large File Storage (LFS) 
- Texture compression for windows build 

Thank you,
Reut Tal