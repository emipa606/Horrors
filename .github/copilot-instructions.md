# GitHub Copilot Instructions for Horrors (Continued) Mod

## Mod Overview and Purpose
**Horrors (Continued)** is an update to the original Viceroys mod for RimWorld. It introduces a new hostile faction called Horrors, which includes a swarm of unknown creatures that infest and attack player colonies. The mod aims to offer a more complex and intelligent enemy than vanilla infestations, with a dynamic hive behavior controlled by an overarching AI.

## Key Features and Systems
- **A new hostile faction, the Horrors**: This faction includes various monster types and an overarching AI that controls the behavior of the hive.
- **Prowling Monsters**: These monsters defend against the player's colony.
- **Attackable Hive sites**: Players can assault and eradicate the hives.
- **Infestations**: The Horrors infest the player's colony and build up their hive over several tiers.
- **Multiple different structures used by the Horrors**: These structures are part of the hive's expansion.
- **Dynamic hive behavior**: The hive expands, hardens itself, and defends itself, offering an immersive experience.

## Coding Patterns and Conventions
- **Coding Language**: C# is used for the mod's core logic and behavior.
- **XML Integration**: XML files are used to define various aspects of the mod, such as pawn kinds, bodies, and biome definitions.
- **Harmony Patching**: The mod uses Harmony (brrainz.harmony) to patch vanilla RimWorld code for integration and functionality.

## XML Integration
- **XML Files**: There are 60 XML files in the mod, each defining different aspects of the Horrors faction, including biome definitions, body definitions, and pawn kind definitions.
- **XML Structure**: Each XML file contains multiple `Defs`, such as `BiomeDef`, `BodyDef`, `PawnKindDef`, and `RulePackDef`.
- **Example XML File**: `Biomes_Arid_Horrors.xml` defines a new biome for the Horrors.

## Harmony Patching
- **Purpose**: Harmony patching is used to modify vanilla RimWorld behavior to support the Horrors faction.
- **Patches**: There are multiple Harmony patches in the mod, each modifying specific aspects of the game's behavior.
- **Example Patch**: `CompSpawnerHorrorDen.cs` includes a Harmony patch to modify the behavior of a comp that spawns horror hives.

## Suggestions for Copilot
1. **Feature Requests**:
   - **Expand Hive**: Add more tiers to the hive's expansion.
   - **Increase Unique Horrors**: Add more unique horror creatures.
   - **Variety in Raids**: Introduce different types of raids.
   - **Expand AI**: Enhance the hive's AI for better behavior.
   - **Additional Hive Defense**: Add more defense mechanisms for the hive.
   - **Hive Passification**: Implement a way to passify the hive through technological means.
   - **Horror Biome**: Create a new biome specifically for the Horrors.
   - **Horror Diseases**: Introduce new diseases related to the Horrors.
   - **Beneficial Resources**: Add resources that can be gained from hives.

2. **Bug Fixes**:
   - **NullPointerException**: Fix the issue with the nullpointer exception on the first encounter with Horrors.
   - **Human Horrors**: Remove the requirement for a human leader for the faction.
   - **Thieving Horrors**: Modify the behavior of thieving horrors to avoid hardcoded theft.
   - **Faction Bases**: Improve the faction bases for the Horrors.

3. **Code Enhancements**:
   - **Refactor Code**: Improve the mod's code structure and readability.
   - **Optimize Performance**: Optimize the mod's performance to reduce lag.
   - **Add Comments**: Add more comments to the code for better understanding.

4. **Localization**:
   - **Chinese Translation**: Continue working on the Chinese translation.

5. **User Interface**:
   - **Improve UI**: Enhance the user interface for better interaction with the mod.

By following these guidelines, developers can contribute to the Horrors (Continued) mod and help improve the overall experience for players.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

