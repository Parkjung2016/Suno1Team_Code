using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AilmentColor
{
    public Ailment type;
    public Color color;
}

public class GameConfig
{
    private static readonly List<AilmentColor> _ailmentColors = new()
    {
        new AilmentColor { type = Ailment.Ignited, color = Color.red },
    };


    public static Color GetAilmentColor(Ailment type) => _ailmentColors.Find(x => x.type == type).color;
}