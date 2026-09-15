# Belonging

A 3D third-person building and resource-gathering prototype developed in Unity and C#. The project explores modular construction, procedural world generation, persistence, and data-driven gameplay systems.

This is an unpublished solo project created in 2022. The original working title was *Belonging*.

## Features

- Modular building system with placement previews, snap points, collision checks, resource costs, rotation, deletion, and unlockable parts
- Seeded, chunk-based world generation using layered Perlin noise, dynamic chunk loading, and persistent world modifications
- Inventory and toolbar management with stackable resources and equippable tools
- Event-driven quest and progression system based on ScriptableObjects
- Multiple save profiles for player, inventory, quest, building, and world state
- Configurable key bindings, third-person camera controls, menus, tooltips, and gameplay feedback
- Day-night cycle, interactive snow, outline rendering, and Shader Graph effects

## Running the project

Clone the repository and open it with Unity. Some dependencies must be installed and/or purchased separately before opening the project:
- Polygon Fantasy Kingdom Asset Pack (https://syntystore.com/products/polygon-fantasy-kingdom) - used for the majority of character and environment models
- OdinInspector (https://odininspector.com/) - used for custom editor code and tooling
- LeanTween (https://github.com/dentedpixel/LeanTween) - used for user interface animations
- Simple Sky (https://syntystore.com/products/simple-sky-cartoon-assets) - used for the skybox

Start with [`Assets/Scenes/MainMenuScene.unity`](Assets/Scenes/MainMenuScene.unity); the playable scene is [`Assets/Scenes/GameScene.unity`](Assets/Scenes/GameScene.unity).
