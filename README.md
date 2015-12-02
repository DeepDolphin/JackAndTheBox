# JackAndTheBox
Jack and the boxes game

Current Version: 1.0.0302.1000

# Contributors
 -  David -> Lead Programmer
 -  Ben -> Programmer
 -  Jonathan -> Programmer

# Known Bugs
  - Crates can be placed "above" the wall
  - Hitboxes are placed incorrectly
  

# Changelog
1.0.0302.1000:
	Added Capabilities:
  - Made a more efficient game loop by seperating the winForm from the rest of the game
  - Draws healthbars correctly
  - Rooms draw themselves
  - GameObjects draw themselves

1.0.0301.1000:
	Added Capabilities:
  - Draws hitbox during debugging
  - Sorts GameObjects more efficiently
  - Added properties to GameObjects in an efficient manner
  - Moved to a custom double buffered solution as preparation for efficent drawing
	Fixed Bugs:
  - Placing crate while direction up doesn't work
  - Object jitter

1.0.0223.1000:
	Added Capabilities:
  - Placing ability is independent from player
  - Added debug capability
  - Started a comprehensive readme on gitHub
  - Uploaded existing code to gitHub
    Fixed Bugs:
  - Boxes not showing up in correct places
