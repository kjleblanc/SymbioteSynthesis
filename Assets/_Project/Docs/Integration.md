# Unity 6 Integration Playbook (Human-First)

## Hex Tilemap
- Menu: *GameObject → 2D Object → Tilemap → Hexagonal (Point-Top)*
- Grid: Cell Size (1,1); Orientation Point-Top.
- Import: Sprites PPU=128; Filter=Point; Compression=Normal; Pivot=Center.

## URP (if later chosen)
- *Assets → Create → Rendering → URP Global Settings and URP 2D Renderer.*
- *Edit → Project Settings → Graphics → Render Pipeline Asset = URP.*
- Assign 2D Renderer; verify materials. **Rollback:** set pipeline asset to None.

## Input (if enabling Input System)
- Add package; `Assets/_Project/Input/InputActions.inputactions`
- Bind: Point, Click, Drag; Gamepad optional. **Rollback:** switch back to legacy.

## Build Settings
- Platform: *PC, Mac & Linux Standalone → Windows x86_64*.
- Scripting Backend: IL2CPP; .NET Standard.
- Resolution: 1920×1080 windowed; VSync off in Editor.
- Company/Product: set to deterministic names.

## Save Locations
- `Application.persistentDataPath/SymbioteSynthesis/{RunSaves,Meta,Logs}`
- Folder hygiene: no stray assets under `Assets/` root—use `_Project` strictly.

---
**You Did:** Followed menu paths; confirmed scene runs.  
**Expected:** Game scene loads; tilemap visible; merges work.  
**Quick Test:** Build & Run once.  
**If Broken:** Clear `Library`; reimport; check pipeline asset.  
**Log Note:** Steps are click-only; no code edits as required.
