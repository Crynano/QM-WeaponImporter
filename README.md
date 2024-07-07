# QM-WeaponImporter
A mod for Quasimorph that allows to add custom weapons to the game.

It provides a function and custom classes to aid in the creation of weapons via code, as well as a JSON file to add weapons manually without the need to create another mod.

## Usage
*NOTE: Currently the weapons are only obtainable via Console Commands.

### For users
- Upon installation via Steam Workshop, execute the game once.
- In the workshop folder of the mod (../SteamLibrary/steamapps/workshop/content/2059170/3284022031) a JSON file named "import_weapons.json" will be created, containing an example of usage.
- Add as many weapons wished
- Execute the game and play!

### For developers
- Create an instance of WeaponTemplate
- Modify as desired
- Call "CreateNamedWeapon" with the instance as parameter
- Success! Now the weapon is added to the game upon start.

## Planned/In the works
· Allow weapons to be added to Corporation's weapon pool or rewards.
· Introduce support to localize name and descriptions for every language.

## Feedback
For feedback, please use the mod's workshop page or the official Quasimorph Discord, using the appropiate modding category/forum.
