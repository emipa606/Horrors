# Copilot Instructions for RimWorld Modding Project

## Mod Overview and Purpose
The mod introduces a new biome, "Horror Wastes," along with a series of horrifying creatures and structures that interact and enhance the gameplay experience in RimWorld. It aims to challenge players with new enemies, environmental hazards, and dynamic events driven by unique AI behaviors and spawning mechanics.

## Key Features and Systems
- **Horror Wastes Biome**: A unique biome home to various terrors, adding new challenges for survival.
- **New Structures**: Includes structures like **Horror Dens** and **Horror Hives** that spawn terrifying creatures and have dynamic ecosystem interactions.
- **Advanced AI Mechanics**: Through the use of compilers and AI behaviors, such as **CompHorrorHatcher**, entities exhibit intelligent and strategic decision-making.
- **Noise Generation**: Utilizes classes like `TurboNoise` for procedural generation of environmental features like cave systems.
- **New Incidents and Events**: Introduces events such as the **Horror Infestation**, which dynamically alter the gameplay and require strategic planning.

## Coding Patterns and Conventions
The mod uses several coding conventions to maintain clarity and efficiency:
- **Class Naming**: Classes are named intuitively with prefixes/postfixes indicating their functionality, like `Comp*` for component-based classes and `JobGiver_*` for AI-related behaviors.
- **Encapsulation**: Methods are generally kept private unless they need to be accessed externally, to maintain class integrity.
- **Method Naming**: Descriptive method names reflect their purpose, such as `CalculateNextHiveSpawnTick()` and `TrySpawnChildHive()`.
- **Interfaces and Inheritance**: Utilizes inheritance effectively, for instance, `Building_Burrow` inheriting from `Building_CryptosleepCasket`.

## XML Integration
XML files are used to define gameplay data configurations:
- **ThingDefs and PawnDefs**: Define new entities and their attributes.
- **BiomeDefs**: Configure the new Horror Wastes biome with its unique parameters.
- **IncidentDefs**: Integrate new incidents into the game's event system.

There should be consistent alignment between XML data and C# logic to ensure seamless execution. It's crucial to double-check XML tags and attributes to align with C# class members.

## Harmony Patching
Harmony is essential in this mod for runtime code patching:
- **Patch Structure**: Patches are organized into nested classes within `HarmonyPatches` to keep related patches grouped.
- **Patch Types**: Various patches are used, including Prefix, Postfix, and Transpiler, depending on the necessary modifications.
- **Method Targeting**: Patches target specific methods identified by their method base, ensuring precise modifications with minimal conflict with the base game or other mods.

## Suggestions for Copilot

- **Code Completion**: Encourage Copilot to suggest method bodies, particularly for complex procedural content like noise generation or AI decision trees.
- **XML Data Suggestions**: Generate suggestions for XML setup based on existing data patterns—for example, suggesting new ThingDefs based on other creature classes.
- **Harmony Patch Templates**: Offer skeleton code for common patch types (Prefix, Postfix) to streamline the process of creating new patches.
- **Efficiency Improvements**: Suggest optimizations in method implementations, especially in loops and process-heavy methods like `layeredNoise`.

By following these directives, GitHub Copilot can effectively assist in expanding and refining your RimWorld mod development, enhancing both technical execution and creative game design.
