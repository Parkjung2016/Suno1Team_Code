using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum UsePassiveSkillType
{
    Stat,
    WhenHit,
    WhenApplyDamage,
    Update,
}


public class PassiveSkill : Skill
{
    public List<UsePassiveSkillType> whenUsePassiveSkillTypes;

    public void TryUsePassiveSkill(Player target)
    {
        AttemptUseSkill();
    }
}