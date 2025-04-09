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
        public readonly int id;
        public readonly string name;
        public readonly bool canRecastWhileInCast;
        public bool isInCast;
        public bool isActive;
        public readonly List<SpellDirections> inputs;
        private readonly List<Action<Spell>> _spellEvents;
        private readonly List<Action<Spell>> _spellFailureEvents;
        
        public Spell(int id, string name, List<SpellDirections> inputs, bool canRecastWhileInCast, bool enableByDefault)
        {
            this.id = id;
            this.name = name;
            this.inputs = inputs;
            this.canRecastWhileInCast = canRecastWhileInCast;
            isInCast = false;
            isActive = enableByDefault;
            _spellEvents = new List<Action<Spell>>();
            _spellFailureEvents = new List<Action<Spell>>();
        }

        public void AddSpellListener(Action<Spell> spellEvent)
        {
            _spellEvents.Add(spellEvent);
        }

        public void Cast()
        {
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
        public bool isIncanting;
        
        public SpellManager()
        {
            List<Spell> test = new List<Spell>()
            {
                new Spell(
                    0,
                    "Grimoire",
                    new List<SpellDirections>(),
                    false,
                    true
                ),
                new Spell(
                    1,
                    "Run",
                    new List<SpellDirections>() { SpellDirections.Up, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Up},
                    true,
                    true
                    ),
                new Spell(
                    5,
                    "Jump",
                    new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Down, SpellDirections.Left, SpellDirections.Right, SpellDirections.Down, SpellDirections.Up },
                    true,
                    false
                ),
                new Spell(
                    6,
                    "UnnamedSpell",
                    new List<SpellDirections>() { SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down },
                    true,
                    true
                ),
                new Spell(
                    7,
                    "UnnamedSpell 2",
                    new List<SpellDirections>() { SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down, SpellDirections.Left, SpellDirections.Up, SpellDirections.Right, SpellDirections.Down },
                    true,
                    false
                ),
                new Spell(
                    2,
                    "SkyView",
                    new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down },
                    true,
                    true
                ),
                new Spell(
                    3,
                    "End SkyView",
                    new List<SpellDirections>() { SpellDirections.Down },
                    true,
                    false
                ),
                new Spell(
                    4,
                    "Konami",
                    new List<SpellDirections>() { SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Down, SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Up },
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
    }
}