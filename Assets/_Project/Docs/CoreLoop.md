# Core Loop & Pillars (explicit turn structure)

## Turn (5 steps)
1) **Spawn:** N random Tier-1 cells (seeded) onto free hexes (respect anti-dup).
2) **Player Merges:** Exactly **3 merges** (drag A onto B).
3) **Combat Resolution:** Same board, initiative/tick.
4) **Cleanup:** Remove dead; apply end-of-turn effects.
5) **Map Choice:** Branch to next node (battle/elite/event).

## ASCII Loop Diagram
```
[Spawn] -> [Merge x3] -> [Combat] -> [Cleanup] -> [Map Choice] -> (repeat)
    ^                                                               |
    |---------------------------------------------------------------|
```

---
**Paths/Files:** `Assets/_Project/Docs/CoreLoop.md`

**You Did:** Documented loop.  
**Expected:** Loop diagram present (ASCII ok).  
**Quick Test:** Count 5 steps; 3 merges fixed.  
**If Broken:** Update; notify Coding Agent.  
**Log Note:** Directly mirrors canonical concept to prevent drift.
