using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mage
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
        private readonly List<Action<Spell>> _spellEvents;
        private readonly List<Action<Spell>> _spellFailureEvents;
        public readonly bool canRecastWhileInCast;
        public readonly int id;
        public readonly List<SpellDirections> inputs;
        public readonly string name;
        public readonly float recastDelay;

        public bool canBeCast;
        public float cooldown;
        public bool isActive;
        public bool isUnlockable;
        public bool isInCast;

        public Spell(int id, string name, List<SpellDirections> inputs, float recastDelay, bool canRecastWhileInCast,
            bool enableByDefault, bool isUnlockable = true)
        {
            this.id = id;
            this.name = name;
            this.canRecastWhileInCast = canRecastWhileInCast;
            this.isInCast = false;
            this.recastDelay = recastDelay;
            this.isActive = enableByDefault;
            this.isUnlockable = !enableByDefault && isUnlockable;

            this.inputs = inputs;

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
                    0,
                    false,
                    true
                ),
                new Spell(
                    1,
                    "Run",
                    new List<SpellDirections>
                    {
                        SpellDirections.Up, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down,
                        SpellDirections.Up
                    },
                    2,
                    true,
                    true
                ),
                new Spell(
                    5,
                    "Jump",
                    new List<SpellDirections>
                    {
                        SpellDirections.Down, SpellDirections.Down, SpellDirections.Left, SpellDirections.Right,
                        SpellDirections.Down, SpellDirections.Up
                    },
                    1,
                    true,
                    false
                ),
                new Spell(
                    6,
                    "UnnamedSpell",
                    new List<SpellDirections>
                        { SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down },
                    0,
                    true,
                    true
                ),
                new Spell(
                    7,
                    "UnnamedSpell 2",
                    new List<SpellDirections>
                    {
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down,
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down
                    },
                    0,
                    true,
                    false
                ),
                new Spell(
                    8,
                    "GrowingShot",
                    new List<SpellDirections>
                    {
                        SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down,
                        SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Up
                    },
                    5,
                    true,
                    false
                ),
                new Spell(
                    2,
                    "SkyView",
                    new List<SpellDirections>
                        { SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down },
                    1,
                    true,
                    true
                ),
                new Spell(
                    3,
                    "End SkyView",
                    new List<SpellDirections> { SpellDirections.Down },
                    0,
                    true,
                    false,
                    false
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
                    10,
                    true,
                    true
                )
            };

            _spellList = test;
            spellsAvailable = test.Where(spell => spell.isActive).ToList();
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
                spellsAvailable.ForEach(spell => spell.isActive = true);
                _forcedEnabled = true;
            }

            float timeIncrement = Time.deltaTime;

            for (int i = spellsAvailable.Count - 1; i >= 0; i--)
            {
                Spell spell = spellsAvailable[i];

                if (spell.cooldown < spell.recastDelay) spell.cooldown += timeIncrement;
                spell.canBeCast = (!spell.isInCast || spell.canRecastWhileInCast) &&
                                  spell.cooldown >= spell.recastDelay;

                if (!spell.isActive) spellsAvailable.RemoveAt(i);
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
            spellsAvailable.AddRange(_spellList.Where(spell => spell.isActive));
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