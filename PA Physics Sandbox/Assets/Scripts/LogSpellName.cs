using Mage;
using UnityEngine;

public class LogSpellName : MonoBehaviour, ISpellEvent
{
    public SpellManager spellManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var spell in spellManager.GetSpells())
        {
            spell.AddSpellEventListener(this);
        }
    }

    public void Cast(Spell spell)
    {
        Debug.Log(spell.name);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
