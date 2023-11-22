# Key things to know about the game
The player character is fixed to the centre of the screen and the player has to use the mouse to rotate the character to aim and shoot.
## Control Scheme:
These are the controls:

WASD – movement

Mouse pointer – aim

Left click – primary fire

Right click – secondary fire

R – use spin ability

G – toggle god mode (god mode will turn the health bar blue if it is activated)
## UI Layout:

Annotated screenshot of main game UI

The UI is designed so that it is easy to intuit what all the different icons stand for. For example, red is a very common colour used for health in games and therefore would make it obvious to most gamers what the bar is for.
## Hidden weapon
There is a “hidden” shotgun weapon that the player can switch for their main weapon by going up to the large yellow square in the main level and activating the spin move (pressing “R”). 

Screenshot of the trigger box.
### Spikes
The floor spikes are green when safe (retracted) and red when dangerous (extended). They flash just before they extend to give the player a warning. 

Screenshot of the two different spike states.
## Highlighted feature: secondary fire

Screenshot of the secondary fire, the most prominent mechanic in the game.
# Expanded mechanics
## Mechanic 1: Two Unique abilities
### Ability 1
The first ability that has been implemented is the secondary fire (right-click). This is a raycast based attack that cleaves through enemies and will do more damage if there is charge in the charge meter (right side of the screen).
### How to use:
Attack enemies using the primary fire to gain charge. Once you have some charge accumulated, press right click to fire a raycasted shot. 
### Notes on implementation:
This ability was implemented by using a sphereCast instead of a rayCast, as it is much more forgiving for the player, reducing the need for them to aim as precisely. Even though the hit itself is casted, the visual is still a projectile but it moves extremely fast, which could cause problems if collision calculations were to be attempted on it.

The weapon’s charge is used as a multiplier for both the damage and radius of the attack, meaning higher charged attacks are easier to hit.
### Ability 2
The second ability is the spin kick move (R), this move is designed to knock enemies away from the player, helping them against being swarmed.
### How to use:
Press the “R” key and the player character will spin. This will knock nearby enemies away.
### Notes on implementation:
This ability uses the OverlapSphere function to find all the colliders within a certain radius of the player. These can then be iterated over and can have their tags checked to see if they are enemies, at which point the attack can be initiated.
### Deviation from original proposal:
The “cooldown reset” mechanic was removed in favour of simply putting a rather short cooldown on the ability. This is because the enemies frequently knock into each other when hit by the kick, greatly dampening its area clearing capabilities. This means that if they don’t have access to a clearing move quickly, the player will be easily overwhelmed.
## Mechanic 2: Powerups
The three powerups that can drop from enemies ended up being:

Green – health regeneration

Yellow – charge regeneration

Red – damage boost

These are similar to the original proposal except the green one has been changed to a simple regeneration to allow the implementation to be based on a simple timer rather than having to track damage.

All the powerups operate by setting a timer float in PlayerController.cs when they are collected and the effect will continue until the timer reaches 0 after having time.deltatime subtracted each frame.
## Mechanic 3: Environment interaction
For environment interaction, there are three features. The first is a set of rotating spawn points for the enemies in the main level, with a different door being randomly opened every few seconds to release more enemies. The second is a spike trap that has a random timer and will hurt the player and enemies who stand on it. The third is a hidden shotgun weapon that can be activated by using the kick move near a yellow trigger.

