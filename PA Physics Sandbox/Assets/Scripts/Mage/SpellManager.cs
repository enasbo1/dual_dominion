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
    
    public interface ISpellEvent
    {
        public void Cast(Spell spell);
    }

    public struct Spell
    {
        public int id;
        public string name;
        public readonly bool schema;
        public readonly List<SpellDirections> inputs;
        
        private List<ISpellEvent> _onCastTrigger;
        
        public Spell(int id, string name, List<SpellDirections> inputs)
        {
            this.id = id;
            this.name = name;
            this.inputs = inputs;
            schema = false;
            _onCastTrigger = new List<ISpellEvent>();
        }

        public void Cast()
        {
            if (_onCastTrigger == null) return;
            foreach (ISpellEvent castTrigger in _onCastTrigger)
                castTrigger.Cast(this);
        }

        public void AddSpellEventListener(ISpellEvent spellListener)
        {
            _onCastTrigger ??= new List<ISpellEvent>();
            _onCastTrigger.Add(spellListener);
        }
    }
    
    public class SpellManager : MonoBehaviour
    {
        private readonly List<Spell> _spellList;
        public SpellManager()
        {
            List<Spell> test = new List<Spell>()
            {
                new Spell(0, "Grimoire", new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Left, SpellDirections.Up, SpellDirections.Right }),
                new Spell(1, "SkyView", new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down }),
                new Spell(1, "SkyViewExe", new List<SpellDirections>() { SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Up, SpellDirections.Up, SpellDirections.Down }),
                new Spell(1, "Konami", new List<SpellDirections>() { SpellDirections.Up, SpellDirections.Up, SpellDirections.Down, SpellDirections.Down, SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Right, SpellDirections.Left, SpellDirections.Up }),
            };
            
            this._spellList = test;
        }

        public List<Spell> GetSpells()
        {
            return _spellList ?? new List<Spell>();
        }
    }
}