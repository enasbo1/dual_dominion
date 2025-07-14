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
        private readonly bool _canBeCastOnUnlock;
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
            float recastDelay = 0,
            bool canBeCastOnUnlock = true,
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
            this._canBeCastOnUnlock = canBeCastOnUnlock;
            this.isHidden = isHidden;
            
            this.isInCast = false;
            this.cooldown = canBeCastOnUnlock ? recastDelay : 0;
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
            cooldown = 0;
            foreach (Action<Spell> action in _spellEvents)
                action.Invoke(this);
        }

        public void CastFailure()
        {
            foreach (Action<Spell> action in _spellFailureEvents)
                action.Invoke(this);
        }

        public void ClearListeners()
        {
            _spellEvents.Clear();
        }

        public void UnlockSpell()
        {
            isUnlocked = true;
            isUnlockable = false;
            cooldown = _canBeCastOnUnlock ? recastDelay : 0;
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
            List<Spell> test = new List<Spell>()
            {
                new Spell(
                    0,
                    "Grimoire",
                    new List<SpellDirections>(),
                    canRecastWhileInCast: false
                ),
                new Spell(
                    1,
                    "Run",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down,
                        SpellDirections.Up
                    },
                    recastDelay: 4
                ),
                new Spell(
                    9,
                    "Run 2",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down,
                        SpellDirections.Up, SpellDirections.Down, SpellDirections.Up, SpellDirections.Up,
                        SpellDirections.Down, SpellDirections.Up, SpellDirections.Up
                    },
                    recastDelay: 8,
                    unlockedByDefault: false
                ),
                new Spell(
                    5,
                    "Jump",
                    new List<SpellDirections>
                    {
                        SpellDirections.Down, SpellDirections.Down, SpellDirections.Left, SpellDirections.Right,
                        SpellDirections.Down, SpellDirections.Up
                    },
                    recastDelay: 2,
                    unlockedByDefault: false
                ),
                new Spell(
                    6,
                    "UnnamedSpell",
                    new List<SpellDirections>
                        { SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down }
                ),
                new Spell(
                    7,
                    "UnnamedSpell 2",
                    new List<SpellDirections>
                    {
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down,
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down
                    },
                    unlockedByDefault: false
                ),
                new Spell(
                    8,
                    "GrowingShot",
                    new List<SpellDirections>
                    {
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down,
                        SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Up
                    },
                    recastDelay: 15,
                    unlockedByDefault: false
                ),
                new Spell(
                    2,
                    "SkyView",
                    new List<SpellDirections>
                        { SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down },
                    recastDelay: 4
                ),
                new Spell(
                    3,
                    "End SkyView",
                    new List<SpellDirections> { SpellDirections.Down },
                    unlockedByDefault: false,
                    isUnlockable: false
                ),
                new Spell(
                    4,
                    "Konami",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Down,
                        SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Right,
                        SpellDirections.Left, SpellDirections.Up
                    },
                    recastDelay: 6,
                    isHidden: true
                ),
                new Spell(
                    10,
                    "Healing",
                    new List<SpellDirections>
                    {
                        SpellDirections.Left, SpellDirections.Right, SpellDirections.Down, SpellDirections.Up
                    },
                    recastDelay: 20,
                    canBeCastOnUnlock: false,
                    canRecastWhileInCast: false
                ),
                new Spell(
                    11,
                    "Healing 2",
                    new List<SpellDirections>
                    {
                        SpellDirections.Left, SpellDirections.Right, SpellDirections.Down, SpellDirections.Up,
                        SpellDirections.Right, SpellDirections.Left, SpellDirections.Up, SpellDirections.Down
                    },
                    recastDelay: 30,
                    canBeCastOnUnlock: false,
                    canRecastWhileInCast: false,
                    unlockedByDefault: false
                )
            };

            _spellList = test;
            spellsAvailable = test.Where(spell => spell.isUnlocked).ToList();
            spellsToUnlock = test.Where(spell => spell.isUnlockable).ToList();
            defaultSpell = GetSpellById(0);
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

        public Spell GetSpellById(int id)
        {
            return _spellList.Find(x => x.id == id);
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
            foreach (Spell spell in _spellList) spell.ClearListeners();
        }
    }
}