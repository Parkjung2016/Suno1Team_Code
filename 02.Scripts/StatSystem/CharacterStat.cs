using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStat : ScriptableObject
{
    [Header("Major stat")] public Stat guardTime;
    public Stat jumpPower;
    public Stat moveSpeed;
    public Stat damage;
    public Stat attackSpeed;
    public Stat knockBackPowerX;
    public Stat knockBackPowerY;
    public Stat knockBackDuration;


    [Header("Defensive stats")] public Stat maxHealth;

    protected Player _owner;

    protected Dictionary<StatType, Stat> _statDictionary;

    public virtual void SetOwner(Player owner)
    {
        _owner = owner;
    }

    public virtual void IncreaseStatBy(int modifyValue, float duration, Stat statToModify)
    {
        _owner.StartCoroutine(StatModifyCoroutine(modifyValue, duration, statToModify));
    }

    private IEnumerator StatModifyCoroutine(int modifyValue, float duration, Stat statToModify)
    {
        statToModify.AddModifier(modifyValue);
        yield return new WaitForSeconds(duration);
        statToModify.RemoveModifier(modifyValue);
    }

    protected virtual void OnEnable()
    {
        _statDictionary = new Dictionary<StatType, Stat>();
    }


    public float GetMaxHealth()
    {
        return maxHealth.GetValue();
    }
}