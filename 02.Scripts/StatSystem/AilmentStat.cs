using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Ailment : int
{
    None = 0,
    Ignited = 1
}

public delegate void AilmentChange(Ailment oldAilment, Ailment newAilment);

public delegate void AilmentDotDamageEvent(Ailment ailmentType, int damage);

[Serializable]
public class AilmentStat
{
    private Dictionary<Ailment, float> _ailmentTimerDictionary;
    private Dictionary<Ailment, int> _ailmentDamageDictionary;


    public Ailment currentAilment;

    public event AilmentDotDamageEvent DotDamageEvent;
    public event AilmentChange AilmentChangeEvent;

    private float _igniteTimer;
    private float _igniteDamageCooldown = 1f;

    public AilmentStat()
    {
        _ailmentTimerDictionary = new();
        _ailmentDamageDictionary = new();

        foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
        {
            if (ailment != Ailment.None)
            {
                _ailmentTimerDictionary.Add(ailment, 0);
                _ailmentDamageDictionary.Add(ailment, 0);
            }
        }
    }

    public void UpdateAilment()
    {
        IgniteTimer();

        foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
        {
            if (ailment == Ailment.None) continue;

            _ailmentTimerDictionary[ailment] -= Time.deltaTime;
            if (_ailmentTimerDictionary[ailment] <= 0)
            {
                _ailmentDamageDictionary[ailment] = 0;
                Ailment oldAilment = currentAilment;
                currentAilment ^= ailment;
                AilmentChangeEvent?.Invoke(oldAilment, currentAilment);
                _ailmentTimerDictionary[ailment] = 0;
            }
        }
    }

    private void IgniteTimer()
    {
        if ((currentAilment & Ailment.Ignited) == 0) return;

        _igniteTimer += Time.deltaTime;

        if (_ailmentTimerDictionary[Ailment.Ignited] > 0 && _igniteTimer > _igniteDamageCooldown)
        {
            _igniteTimer = 0;
            DotDamageEvent?.Invoke(Ailment.Ignited, _ailmentDamageDictionary[Ailment.Ignited]);
        }
    }

    public bool HasAilment(Ailment ailment)
    {
        return (currentAilment & ailment) > 0;
    }

    public void ApplyAilments(Ailment value, float duration, int damage)
    {
        Ailment oldValue = currentAilment;
        if ((currentAilment & Ailment.Ignited) == 0)
        {
            _igniteTimer = 0;
        }

        currentAilment |= value;
        if (oldValue != currentAilment)
            AilmentChangeEvent?.Invoke(oldValue, currentAilment);

        if ((value & Ailment.Ignited) > 0)
        {
            SetAilment(Ailment.Ignited, duration: duration, damage: damage);
        }
    }

    private void SetAilment(Ailment ailment, float duration, int damage)
    {
        _ailmentTimerDictionary[ailment] += duration + 1;
        _ailmentDamageDictionary[ailment] = damage;
    }
}