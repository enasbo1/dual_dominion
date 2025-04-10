using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        public readonly int id;
        public readonly string name;
        public readonly bool canRecastWhileInCast;
        public bool isInCast;
        public readonly float recastDelay;
        public bool isActive;
        public readonly List<SpellDirections> inputs;
        
        private readonly List<Action<Spell>> _spellEvents;
        private readonly List<Action<Spell>> _spellFailureEvents;
        
        public bool canBeCast;
        public float cooldown;
        
        public Spell(int id, string name, List<SpellDirections> inputs, float recastDelay, bool canRecastWhileInCast, bool enableByDefault)
        {
            this.id = id;
            this.name = name;
            this.canRecastWhileInCast = canRecastWhileInCast;
            isInCast = false;
            this.recastDelay = recastDelay;
            isActive = enableByDefault;
            
            this.inputs = inputs;
            
            _spellEvents = new List<Action<Spell>>();
            _spellFailureEvents = new List<Action<Spell>>();
        }

        public void AddSpellListener(Action<Spell> spellEvent)
        {
            _spellEvents.Add(spellEvent);
        }

        public void Cast()
        {
            this.cooldown = 0;
            foreach (Action<Spell> action in _spellEvents)
                action.Invoke(this);
        }
        
        public void CastFailure()
        {
            foreach (Action<Spell> action in _spellFailureEvents)
                action.Invoke(this);
        }
    }
    
    public class SpellManager : MonoBehaviour
    {
        private readonly List<Spell> _spellList;
        public readonly List<Spell> spellsAvailable;
        public readonly Spell defaultSpell;
        public Spell spellToCast;
        [DoNotSerialize] public bool isIncanting;
        
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
                    new List<SpellDirections>() { SpellDirections.Up, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Up},
                    2,
                    true,
                    true
                    ),
                new Spell(
                    5,
                    "Jump",
                    new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Down, SpellDirections.Left, SpellDirections.Right, SpellDirections.Down, SpellDirections.Up },
                    1,
                    true,
                    false
                ),
                new Spell(
                    6,
                    "UnnamedSpell",
                    new List<SpellDirections>() { SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down },
                    0,
                    true,
                    true
                ),
                new Spell(
                    7,
                    "UnnamedSpell 2",
                    new List<SpellDirections>() { SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down, SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down },
                    0,
                    true,
                    false
                ),
                new Spell(
                    2,
                    "SkyView",
                    new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down },
                    1,
                    true,
                    true
                ),
                new Spell(
                    3,
                    "End SkyView",
                    new List<SpellDirections>() { SpellDirections.Down },
                    0,
                    true,
                    false
                ),
                new Spell(
                    4,
                    "Konami",
                    new List<SpellDirections>() { SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Down, SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Up },
                    10,
                    true,
                    true
                ),
            };
            
            this._spellList = test;
            this.spellsAvailable = test.Where(spell => spell.isActive).ToList();
            this.defaultSpell = this.GetSpellById(0);
            this.spellToCast = defaultSpell;
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
            this.spellsAvailable.Clear();
            this.spellsAvailable.AddRange(_spellList.Where(spell => spell.isActive));
        }

        private void FixedUpdate()
        {
            float timeIncrement = Time.deltaTime;
            
            for (int i = spellsAvailable.Count - 1; i >= 0; i--)
            {
                Spell spell = spellsAvailable[i];

                if (spell.cooldown < spell.recastDelay) spell.cooldown += timeIncrement;
                spell.canBeCast = (!spell.isInCast || spell.canRecastWhileInCast) && spell.cooldown >= spell.recastDelay;

                if (!spell.isActive) spellsAvailable.RemoveAt(i);
            }
        }
    }
}