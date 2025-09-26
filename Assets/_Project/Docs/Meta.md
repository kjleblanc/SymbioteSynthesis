# Meta (for Rule 1–4 compliance)

- **Editor baseline:** Unity 6 LTS (pin exact minor in `ProjectSettings/ProjectVersion.txt` on first commit).
- **Render:** Built-in 2D (URP deferred until justified).
- **Namespaces:** `Company.Game.*`
- **Core scenes:** `Boot`, `MainMenu`, `Game`, `Results`.

## Seeds & Generative
- **RNG seeds:** store in `Assets/_Project/Data/Seeds/RunSeed.asset` (int32).
- **Generative seeds (art/audio):** list with each prompt in **Art/Audio Direction §**.

---
**You Did:** Captured meta for Rule 1–4.  
**Expected:** New contributors find the canonical baselines quickly.  
**Quick Test:** Confirm `RunSeed.asset` path exists when created.  
**If Broken:** Recreate the asset; update TDD.  
**Log Note:** Meta centralized; duplicates avoided.
