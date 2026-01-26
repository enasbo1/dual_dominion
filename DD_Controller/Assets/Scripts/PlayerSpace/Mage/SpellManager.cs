using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayerSpace.Mage
{
    public enum SpellDirections
    {
        None,
        Up,
        Right,
        Down,
        Left
    }

    public class Spell
    {
        public readonly int id;
        public readonly string name;
        public readonly List<SpellDirections> inputs;
        public readonly float recastDelay;
        public readonly bool canRecastWhileInCast;
        public readonly bool isHidden;

        public bool canBeCast;
        public bool isInCast;
        public float cooldown;
        public bool isUnlocked;
        public bool isUnlockable;
        
        private readonly List<Action<Spell>> _spellEvents;
        private readonly List<Action<Spell>> _spellFailureEvents;

        public Spell(
            int id,
            string name,
            List<SpellDirections> inputs,
            float recastDelay = 0.2f,
            bool canRecastWhileInCast = true,
            bool unlockedByDefault = true,
            bool isUnlockable = true,
            bool isHidden = false)
        {
            this.id = id;
            this.name = name;
            this.inputs = inputs;
            this.recastDelay = recastDelay;
            this.canRecastWhileInCast = canRecastWhileInCast;
            this.isHidden = isHidden;
            
            this.isInCast = false;
            this.cooldown = recastDelay;
            this.isUnlocked = unlockedByDefault;
            this.isUnlockable = !unlockedByDefault && isUnlockable;
            
            this._spellEvents = new List<Action<Spell>>();
            this._spellFailureEvents = new List<Action<Spell>>();
        }

        public void AddSpellListener(Action<Spell> spellEvent)
        {
            _spellEvents.Add(spellEvent);
        }
        
        public void AddSpellFailureListener(Action<Spell> spellEvent)
        {
            _spellFailureEvents.Add(spellEvent);
        }

        public void Cast()
        {
            cooldown = 0f;
            foreach (Action<Spell> action in _spellEvents)
                action.Invoke(this);
        }

        public void CastFailure()
        {
            foreach (Action<Spell> action in _spellFailureEvents)
                action.Invoke(this);
        }

        public void UnlockSpell()
        {
            isUnlocked = true;
            isUnlockable = false;
            cooldown = recastDelay;
        }

        public void OnDestroy()
        {
            _spellEvents.Clear();
            _spellFailureEvents.Clear();
        }
    }

    public class SpellManager : MonoBehaviour
    {
        [SerializeField] public bool forceEnableSpell;
        private readonly List<Spell> _spellList;
        public readonly Spell defaultSpell;
        public readonly List<Spell> spellsAvailable;
        public readonly List<Spell> spellsToUnlock;
        private bool _forcedEnabled;

        [NonSerialized] public bool isIncanting;
        public Spell spellToCast;

        public SpellManager()
        {
            int id = 0;
            _spellList = new List<Spell>()
            {
                #region Default Spell

                new Spell(
                    id++,
                    "Grimory",
                    new List<SpellDirections>(),
                    canRecastWhileInCast: false
                ),

                #endregion

                #region Movements Spells

                new Spell(
                    id++,
                    "Haste",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Up
                    },
                    recastDelay: 3f
                ),
                new Spell(
                    id++,
                    "Haste 2",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Up
                    },
                    recastDelay: 6f,
                    unlockedByDefault: false
                ),
                new Spell(
                    id++,
                    "Haste 3",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Up,
                        SpellDirections.Left, SpellDirections.Right, SpellDirections.Up
                    },
                    recastDelay: 9f,
                    unlockedByDefault: false
                ),
                new Spell(
                    id++,
                    "Jump",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Down, SpellDirections.Up, SpellDirections.Up
                    },
                    recastDelay: 5f,
                    unlockedByDefault: false
                ),

                #endregion

                #region Offensives Spells

                new Spell(
                    id++,
                    "Spiral Shot",
                    new List<SpellDirections>
                        { SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down }
                ),
                new Spell(
                    id++,
                    "Spiral Shot 2",
                    new List<SpellDirections>
                    {
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down,
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down
                    },
                    unlockedByDefault: false
                ),
                new Spell(
                    id++,
                    "Growing Shot",
                    new List<SpellDirections>
                    {
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down,
                        SpellDirections.Up, SpellDirections.Up, SpellDirections.Up
                    },
                    recastDelay: 15f,
                    unlockedByDefault: false
                ),

                #endregion

                #region Defensives Spells

                new Spell(
                    id++,
                    "Healing",
                    new List<SpellDirections>
                    {
                        SpellDirections.Right, SpellDirections.Left, SpellDirections.Up, SpellDirections.Down
                    },
                    recastDelay: 20f,
                    canRecastWhileInCast: false
                ),
                new Spell(
                    id++,
                    "Healing 2",
                    new List<SpellDirections>
                    {
                        SpellDirections.Right, SpellDirections.Left, SpellDirections.Up, SpellDirections.Down,
                        SpellDirections.Left, SpellDirections.Right, SpellDirections.Down, SpellDirections.Up
                    },
                    recastDelay: 30f,
                    canRecastWhileInCast: false,
                    unlockedByDefault: false
                ),
                new Spell(
                    id++,
                    "Repulsing Field",
                    new List<SpellDirections>
                    {
                        SpellDirections.Right, SpellDirections.Left, SpellDirections.Up, SpellDirections.Up,
                        SpellDirections.Right, SpellDirections.Left, SpellDirections.Down
                    },
                    recastDelay: 60f,
                    canRecastWhileInCast: false,
                    unlockedByDefault: false
                ),

                #endregion

                #region Mischievalous Spells

                new Spell(
                    id++,
                    "Sky View",
                    new List<SpellDirections>
                    {
                        SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down
                    },
                    recastDelay: 5f
                ),
                new Spell(
                    id++,
                    "Sky View End",
                    new List<SpellDirections>
                    {
                        SpellDirections.Down
                    },
                    unlockedByDefault: false,
                    isUnlockable: false
                ),

                #endregion

                #region Cheats

                new Spell(
                    id,
                    "Konami",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Down,
                        SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Right,
                        SpellDirections.Left, SpellDirections.Up
                    },
                    recastDelay: 5,
                    isHidden: true
                ),

                #endregion
            };
            
            _spellList.Sort((spellA, spellB) =>
            {
                List<SpellDirections> inputsA = spellA.inputs;
                List<SpellDirections> inputsB = spellB.inputs;
                
                int minLen = inputsA.Count < inputsB.Count ? inputsA.Count : inputsB.Count;
                for (int i = 0; i < minLen; i++)
                {
                    if (inputsA[i] == inputsB[i]) continue;
                    return inputsA[i] - inputsB[i];
                }
                
                return inputsA.Count - inputsB.Count;
            });

            spellsAvailable = _spellList.Where(spell => spell.isUnlocked).ToList();
            spellsToUnlock = _spellList.Where(spell => spell.isUnlockable).ToList();
            defaultSpell = GetSpellByName("Grimory");
            spellToCast = defaultSpell;
        }

        private void FixedUpdate()
        {
            if (forceEnableSpell && !_forcedEnabled)
            {
                spellsAvailable.Clear();
                spellsAvailable.AddRange(_spellList);
                spellsAvailable.ForEach(spell => spell.isUnlocked = true);
                _forcedEnabled = true;
            }

            float timeIncrement = Time.deltaTime;

            for (int i = spellsAvailable.Count - 1; i >= 0; i--)
            {
                Spell spell = spellsAvailable[i];

                if (spell.cooldown < spell.recastDelay) spell.cooldown += timeIncrement;
                spell.canBeCast = (!spell.isInCast || spell.canRecastWhileInCast) &&
                                  spell.cooldown >= spell.recastDelay;

                if (!spell.isUnlocked) spellsAvailable.RemoveAt(i);
            }
        }

        public Spell GetSpellByName(string name)
        {
            return _spellList.Find(x => string.Equals(x.name, name));
        }

        public List<Spell> GetSpells()
        {
            return _spellList ?? new List<Spell>();
        }

        public void SetSpellsAvailable(List<Spell> spellsAvailable)
        {
            this.spellsAvailable.Clear();
            this.spellsAvailable.AddRange(spellsAvailable);
        }

        public void ResetSpellsAvailable()
        {
            spellsAvailable.Clear();
            spellsAvailable.AddRange(_spellList.Where(spell => spell.isUnlocked));
        }
        
        public void SetSpellsToUnlock(List<Spell> spellsToUnlock)
        {
            this.spellsToUnlock.Clear();
            this.spellsToUnlock.AddRange(spellsToUnlock);
        }

        public void ResetSpellsToUnlock()
        {
            spellsToUnlock.Clear();
            spellsToUnlock.AddRange(_spellList.Where(spell => spell.isUnlockable));
        }

        private void OnDestroy()
        {
            foreach (Spell spell in _spellList) spell.OnDestroy();
        }
    }
}