using System;
using System.Collections.Generic;
using Company.Game.Board;
using Company.Game.Fusion;
using Company.Game.Waves;
using UnityEngine;

namespace Company.Game.Combat
{
    [DisallowMultipleComponent]
    public sealed class CombatResolver : MonoBehaviour
    {
        private const int ActionThreshold = 100;

        [SerializeField]
        private HexBoardService boardService;

        [SerializeField]
        private WaveDirector waveDirector;

        [SerializeField]
        private bool autoAdvanceWave = true;

        public event Action<CombatResult> CombatFinished;

        public CombatResult ResolveCurrentWave()
        {
            if (boardService == null || waveDirector == null)
            {
                Debug.LogWarning("CombatResolver missing dependencies.");
                return CombatResult.Empty;
            }

            IReadOnlyList<WaveConfig.SpawnEntry> enemyEntries = waveDirector.PrepareWave();
            List<Fighter> fighters = new List<Fighter>();
            PopulatePlayerFighters(fighters);
            PopulateEnemyFighters(fighters, enemyEntries);

            CombatResult result = SimulateCombat(fighters);
            CombatFinished?.Invoke(result);

            if (autoAdvanceWave)
            {
                waveDirector.AdvanceWave();
            }

            return result;
        }

        public void Configure(HexBoardService board, WaveDirector director)
        {
            boardService = board;
            waveDirector = director;
        }

        private void PopulatePlayerFighters(List<Fighter> buffer)
        {
            foreach (KeyValuePair<HexBoardService.AxialCoord, HexCellOccupant> pair in boardService.Occupants)
            {
                HexCellOccupant occupant = pair.Value;
                if (occupant == null || occupant.CellType == null)
                {
                    continue;
                }

                buffer.Add(Fighter.FromCell(pair.Key, occupant));
            }
        }

        private static void PopulateEnemyFighters(List<Fighter> buffer, IReadOnlyList<WaveConfig.SpawnEntry> entries)
        {
            if (entries == null)
            {
                return;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                WaveConfig.SpawnEntry entry = entries[i];
                if (entry?.Enemy == null)
                {
                    continue;
                }

                buffer.Add(Fighter.FromEnemy(entry));
            }
        }

        private CombatResult SimulateCombat(List<Fighter> fighters)
        {
            List<Fighter> players = fighters.FindAll(f => f.IsPlayer);
            List<Fighter> enemies = fighters.FindAll(f => !f.IsPlayer);

            if (players.Count == 0 || enemies.Count == 0)
            {
                return new CombatResult(players.Count > 0, new List<CombatEvent>());
            }

            List<CombatEvent> events = new List<CombatEvent>();
            int safety = 0;
            const int maxIterations = 1024;

            while (players.Exists(f => f.IsAlive) && enemies.Exists(f => f.IsAlive))
            {
                Fighter actor = GetNextActor(fighters);
                if (actor == null)
                {
                    break;
                }

                Fighter target = actor.IsPlayer ? FindFirstAlive(enemies) : FindFirstAlive(players);
                if (target == null)
                {
                    break;
                }

                ResolveStrike(actor, target, events);

                if (++safety > maxIterations)
                {
                    Debug.LogWarning("CombatResolver safety break hit.");
                    break;
                }
            }

            bool playersWin = players.Exists(f => f.IsAlive);
            return new CombatResult(playersWin, events);
        }

        private static Fighter GetNextActor(List<Fighter> fighters)
        {
            if (fighters.Count == 0)
            {
                return null;
            }

            while (true)
            {
                for (int i = 0; i < fighters.Count; i++)
                {
                    Fighter fighter = fighters[i];
                    if (!fighter.IsAlive)
                    {
                        continue;
                    }

                    fighter.Initiative += fighter.Speed;
                    if (fighter.Initiative >= ActionThreshold)
                    {
                        fighter.Initiative -= ActionThreshold;
                        return fighter;
                    }
                }
            }
        }

        private static Fighter FindFirstAlive(List<Fighter> fighters)
        {
            for (int i = 0; i < fighters.Count; i++)
            {
                Fighter fighter = fighters[i];
                if (fighter.IsAlive)
                {
                    return fighter;
                }
            }

            return null;
        }

        private static void ResolveStrike(Fighter attacker, Fighter defender, List<CombatEvent> events)
        {
            int armorReduction = defender.HasTrait(Combat.Traits.Armored.TraitId) ? Combat.Traits.Armored.DamageReduction : 0;
            if (armorReduction > 0 && attacker.HasTrait(Combat.Traits.Rending.TraitId))
            {
                armorReduction = Math.Max(0, armorReduction - Combat.Traits.Rending.ArmorPierce);
            }

            int damage = Math.Max(0, attacker.Attack - armorReduction);
            defender.CurrentHealth -= damage;
            if (defender.CurrentHealth <= 0)
            {
                defender.CurrentHealth = 0;
                defender.IsAlive = false;
                defender.HandleDefeat();
            }

            events.Add(new CombatEvent(attacker.Id, defender.Id, damage));
        }

        public readonly struct CombatEvent
        {
            public CombatEvent(string attackerId, string defenderId, int damage)
            {
                AttackerId = attackerId;
                DefenderId = defenderId;
                Damage = damage;
            }

            public string AttackerId { get; }
            public string DefenderId { get; }
            public int Damage { get; }
        }

        public readonly struct CombatResult
        {
            public static readonly CombatResult Empty = new CombatResult(false, Array.Empty<CombatEvent>());

            public CombatResult(bool playersWin, IReadOnlyList<CombatEvent> events)
            {
                PlayersWin = playersWin;
                Events = events;
            }

            public bool PlayersWin { get; }
            public IReadOnlyList<CombatEvent> Events { get; }
        }

        private sealed class Fighter
        {
            private readonly List<string> _traits;

            private Fighter(string id, bool isPlayer, int attack, int speed, int health, List<string> traits,
                HexCellOccupant occupant, WaveConfig.SpawnEntry spawnEntry)
            {
                Id = id;
                IsPlayer = isPlayer;
                Attack = attack;
                Speed = speed;
                CurrentHealth = health;
                _traits = traits ?? new List<string>();
                Occupant = occupant;
                SpawnEntry = spawnEntry;
                IsAlive = true;
            }

            public string Id { get; }
            public bool IsPlayer { get; }
            public int Attack { get; }
            public int Speed { get; }
            public int CurrentHealth { get; set; }
            public bool IsAlive { get; set; }
            public int Initiative { get; set; }
            public HexCellOccupant Occupant { get; }
            public WaveConfig.SpawnEntry SpawnEntry { get; }

            public bool HasTrait(string traitId)
            {
                return _traits.Exists(t => t == traitId);
            }

            public void HandleDefeat()
            {
                if (IsPlayer && Occupant != null && Occupant.Board != null)
                {
                    Occupant.Board.RemoveOccupant(Occupant);
                    Occupant.gameObject.SetActive(false);
                }
            }

            public static Fighter FromCell(HexBoardService.AxialCoord coord, HexCellOccupant occupant)
            {
                CellTypeDefinition type = occupant.CellType;
                List<string> traits = new List<string>();
                if (type?.Traits != null)
                {
                    for (int i = 0; i < type.Traits.Count; i++)
                    {
                        CellTraitDefinition trait = type.Traits[i];
                        if (trait != null && !string.IsNullOrEmpty(trait.TraitId))
                        {
                            traits.Add(trait.TraitId);
                        }
                    }
                }

                string id = $"Player_{coord.q}_{coord.r}";
                return new Fighter(id, true, type.Attack, type.Speed, type.BaseHealth, traits, occupant, null);
            }

            public static Fighter FromEnemy(WaveConfig.SpawnEntry entry)
            {
                EnemyDefinition enemy = entry.Enemy;
                List<string> traits = new List<string>();
                if (enemy.Traits != null)
                {
                    for (int i = 0; i < enemy.Traits.Count; i++)
                    {
                        CellTraitDefinition trait = enemy.Traits[i];
                        if (trait != null && !string.IsNullOrEmpty(trait.TraitId))
                        {
                            traits.Add(trait.TraitId);
                        }
                    }
                }

                string id = $"Enemy_{enemy.EnemyId}_{entry.SpawnOrder}";
                return new Fighter(id, false, enemy.Attack, enemy.Speed, enemy.Health, traits, null, entry);
            }
        }
    }
}
