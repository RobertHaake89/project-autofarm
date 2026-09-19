# Project Autofarm

![Project Autofarm](Images/main.png)

## About
A C# console-based farming simulation featuring procedural terrain generation, crops, resources, and (later) autonomous NPCs.

## Features

- Procedurally generated terrain, with trees, foliage, and misc objects
- 2D tile-based world
- Char-texture system loaded from external data
- Console-based rendering

## Technologies

- C#
- .NET
- .sc file for texture I/O
- Console Application

## Project Structure

As I am writing everything from scratch without any blueprints, structural changes may happen, but the overal foundations are already working without major flaws.

project-autofarm/
│
├── Images/
│   └── main.png
│
├── ProjectAutofarm.sln
├── README.md
├── LICENSE
│
└── ProjectAutofarm/
    ├── ProjectAutofarm.csproj
    ├── Program.cs
    ├── Renderer.cs
    ├── MainLoop.cs
    ├── Ressources.cs
    ├── Structures.cs
    ├── Interfaces.cs
    ├── Position.cs
    ├── Terrain.cs
    ├── Tiles.cs
    ├── Textures.cs
    └── Resources/
        └── Textures.sc

## How It Works

### Terrain
A 2D-array grid with tiles is created at start in a 16:9 ratio.

### Explain briefly how the terrain/grid is represented.
Each tile contains information such as texture name, icon, position, and potential ressource such as wheat.
There are whole patterns of glassland that are created with "controlled-randomness" and use additional noise to make them look more organic.
Foliage and smaller objects are randonmly created at bottom, logically depending on texture.

### Textures
The textures are made of retro but charming ASCII / Unicode characters.

### How are textures loaded and assigned to tiles.
The texture I/O is what makes me most happy so far, as it works flawlessly and intuitive.
A .sc file, containing texturename.charIcon is deserialized into a dictionary.
Now comes the clue, I only have to add the texturename (Dict <TKey>) and it automatically adds the Icon to the texture.
It was an experiment for me and works flawlessly!

### Rendering
Two nested for-loops (x,y) are writing the whole terrain in a stringbuilder to offer a better performance and quality than printing every letter at once.
Each round, a freshly updated output gets displayed as a complete frame.

## What I Learned
This project has already helped me alot so far to fortify my C# knowledge but the things I would like to point out are:
-Intuitive File I/O for textures
-Procedural generation with noise effects / working with multiple parameters
-Console rendering

## Future Plans
 Even more sophisticated terrain generation/simmulation
 NPCs are next on the list!
 Player movement
 Farming/foraging mechanics

## Getting Started

### Requirements
.NET 10 SDK
Run
git clone https://github.com/RobertHaake89/project-autofarm.git
cd project-autofarm
dotnet run

## License

This project is licensed under the MIT License.

## Progress

10-09-26 Initial commit
11-09-26 Added classes, world gen WIP
12-09-26 Refactoring of texture class, problems with dictionary's char/string mechanism and changing texture
14-09-26 Continuing with primal terrain generation with fields - Noise generator WIP
15-09-26 Several Problems with grasspattern forming and noise creation
16-09-26 Finished primal terrain generation, added random objects (flowers, stones, etc) ... and a Farmhouse!
17-09-26 Finished farm mechanic with independently growing crops
18-09-26 Added random pine trees and mushrooms at top and an oak at bottom.
19-09-26 Finally fixed flickering (sneaky WriteLine, only using \n, within nested render loops cause the flicker-jumps)! General cleanup, turned growth factor into a switch expression. Added proper README.md with image.