# PROJECT RULES (verbatim for sub-agents)

Place this file at `Assets/_Project/Docs/PROJECT_RULES.md` and share with both agents.

## General
- **Determinism:** Provide seeds/params for RNG (spawns, events) and for generative outputs.
- **Reproducibility:** Every output lists exact paths, filenames, versions, settings, expected result, quick test, and rollback steps.
- **Atomic Tasks:** Split work into Task Cards that pass/fail cleanly; each ends with “You Did / Expected / Quick Test / If Broken / Log Note”.
- **Documentation:** Keep CHANGELOG, DECISIONS LOG, and FUTURE-SELF NOTES updated in every major output.

## Code (Coding Agent)
- **Unity 6 LTS;** URP only if explicitly justified. Use the Input System only if needed. Addressables/Localization are opt-in with justification.
- **Structure:** `Assets/_Project/{Scripts, Data, Prefabs, Scenes, Art, Audio, UI}`
- **Namespaces:** `Company.Game.*`
- **Core Scenes:** `Boot`, `MainMenu`, `Game`, `Results`
- **Style:** One public class per file; PascalCase public; _camelCase private with `[SerializeField]`; null-guard; profiler notes for hot paths.
- **Plugin Policy:** If proposing a plugin, include purpose, version, setup steps, API facade location (so it’s replaceable), and removal plan.

## Art/Audio (Visuals Agent)
- Do not assume art style upfront. First list readability/scale/contrast constraints for hex readability; then propose ≥2 style lanes with cost/benefit and pick one.
- Each asset includes export specs (size/PPU/padding/pivot/compression/bitrate/loop), consistent naming, seeds (if applicable), and a quick “in-engine expectation” note.

## Human Integrator
- Only click-level steps (no code edits). Provide exact Unity menu paths, import settings, and where to drag/drop prefabs/assets.

## PLUGINS/ASSETS POLICY (Unity 6)
Start Unity-first. If proposing a helper, evaluate case-by-case; examples (optional):
- **2D Tilemap Extras (Unity/open-source)** — hex helpers.
- **DOTween (free) or LeanTween** — UI/feedback polish; wrap in `Company.Game.Tweener` facade.
- **TextMeshPro, Cinemachine, Input System, Localization** — only if justified.

For each chosen item, produce a Plugins Manifest row: **Name, Version, License, Source URL, Purpose, Setup Steps, API Facade, Known Risks, Removal Plan.**

## Final Rallying Orders
- **Coding Agent:** scaffold services & ScriptableObjects per §7; implement TC-001..003 in order; keep profiler open; commit tests with seeds.
- **Visuals Agent:** produce placeholder set per §6 with listed seeds/specs; export to paths; include “in-engine expectation” notes.
- **Human Integrator:** follow §10; wire prefabs; run Quick Tests; log seeds and menu paths used.
