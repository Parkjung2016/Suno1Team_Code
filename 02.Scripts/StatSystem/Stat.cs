using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float _baseValue;

    public List<float> modifiers;
    public float GetValue()
    {
        float finalValue = _baseValue;
        foreach (float value in modifiers)
        {
            finalValue += value;
        }
        return finalValue;
    }

    public void AddModifier(float value)
    {
        //Debug.Log(value);
        if (value != 0)
            modifiers.Add(value);
    }

    public void RemoveModifier(float value)
    {
        if (value != 0)
            modifiers.Remove(value);
    }

    public void SetDefaultValue(float value)
    {
        _baseValue = value;
    }
}
