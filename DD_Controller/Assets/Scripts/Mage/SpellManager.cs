using System;
using System.Collections.Generic;
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
        public readonly List<SpellDirections> inputs;
        private readonly List<Action<Spell>> _spellEvents;
        private readonly List<Action<Spell>> _spellFailureEvents;
        
        public Spell(int id, string name, List<SpellDirections> inputs, bool canRecastWhileInCast)
        {
            this.id = id;
            this.name = name;
            this.inputs = inputs;
            this.canRecastWhileInCast = canRecastWhileInCast;
            isInCast = false;
            _spellEvents = new List<Action<Spell>>();
            _spellFailureEvents = new List<Action<Spell>>();
        }

        public void AddSpellListener(Action<Spell> spellEvent)
        {
            _spellEvents.Add(spellEvent);
        }

        // ReSharper disable Unity.PerformanceAnalysis
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
        public SpellManager()
        {
            List<Spell> test = new List<Spell>()
            {
                new Spell(
                    0,
                    "Grimoire",
                    new List<SpellDirections>(),
                    false
                ),
                new Spell(
                    1,
                    "Run",
                    new List<SpellDirections>() { SpellDirections.Up, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Up},
                    true
                    ),
                new Spell(
                    2,
                    "SkyView",
                    new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down },
                    true
                ),
                new Spell(
                    3,
                    "SkyViewExe",
                    new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down },
                    true
                ),
                new Spell(
                    4,
                    "Konami",
                    new List<SpellDirections>() { SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Down, SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Up },
                    true
                ),
            };
            
            this._spellList = test;
        }

        public Spell GetSpellById(int id)
        {
            return _spellList.Find(x => x.id == id);
        }

        public List<Spell> GetSpells()
        {
            return _spellList ?? new List<Spell>();
        }
    }
}