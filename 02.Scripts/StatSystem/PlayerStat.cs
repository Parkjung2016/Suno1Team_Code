using System;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Stat/Player")]
public class PlayerStat : CharacterStat
{
    protected override void OnEnable()
    {
        base.OnEnable();

        Type playerStatType = typeof(PlayerStat);

        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            string fieldName = LowerFirstChar(statType.ToString());

            FieldInfo playerStatField = playerStatType.GetField(fieldName);
            if (playerStatField == null)
            {
                Debug.LogError($"There are no stat! error : {statType.ToString()}");
            }
            else
            {
                _statDictionary.Add(statType, playerStatField.GetValue(this) as Stat);
            }
        }
    }

    public Stat GetStatByType(StatType statType)
    {
        return _statDictionary[statType];
    }

    private string LowerFirstChar(string input)
    {
        return char.ToLower(input[0]) + input.Substring(1);
    }
}
