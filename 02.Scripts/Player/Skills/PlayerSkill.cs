using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public enum PlayerSkillType
{
    Guard,
    BurningLeech,
    Frenzy,
    ImpendingDoom,
    KineticShift,
    BloodHunt,
    BloodPact,
    HealthGuard,
    SurvivalInstinct,
    BombSword,
    DamageUpHPDown,
    FireSword,
    IceSword,
    LateDamage,
    RegenerateRest
}

public class PlayerSkill : NetworkBehaviour
{
    private Dictionary<Type, Skill> _skills;
    private PassiveSkill[] _passiveSkillsWhenUpdate;


    public void Init(Player owner)
    {
        _skills = new Dictionary<Type, Skill>();

        foreach (PlayerSkillType skill in Enum.GetValues(typeof(PlayerSkillType)))
        {
            Skill skillComponent = GetComponent($"{skill}Skill") as Skill;
            Type type = skillComponent.GetType();
            _skills.Add(type, skillComponent);
            skillComponent.Init(owner);
        }

        _passiveSkillsWhenUpdate = FindPassiveSkillS(UsePassiveSkillType.Update);

        owner.GuardCoolTimeUI.RegisterGuardSkillCooldownEvent(GetSkill<GuardSkill>());
    }

    private void Update()
    {
        UsePassiveUpdateSkill();
    }

    private void UsePassiveUpdateSkill()
    {
        if (_passiveSkillsWhenUpdate == null) return;
        for (int i = 0; i < _passiveSkillsWhenUpdate.Length; i++)
        {
            PassiveSkill passiveSkill = _passiveSkillsWhenUpdate[i];
            passiveSkill.AttemptUseSkill();
        }
    }

    public void UseStatSkill()
    {
        PassiveSkill[] statSkills = FindPassiveSkillS(UsePassiveSkillType.Stat);
        foreach (var statSkill in statSkills)
        {
            statSkill.AttemptUseSkill();
        }
    }

    public PassiveSkill[] FindPassiveSkillS(UsePassiveSkillType type) =>
        GetComponents<PassiveSkill>().Where(x => x.whenUsePassiveSkillTypes.Contains(type))
            .ToArray();


    public T GetSkill<T>() where T : Skill
    {
        Type t = typeof(T);
        if (_skills.TryGetValue(t, out Skill target))
        {
            return target as T;
        }

        return null;
    }

    public Skill GetSkill(PlayerSkill skill)
    {
        Type type = Type.GetType($"{skill}Skill");
        if (type == null) return null;

        if (_skills.TryGetValue(type, out Skill target))
        {
            return target;
        }

        return null;
    }

    public Skill GetSkill(Type type)
    {
        if (type == null) return null;

        if (_skills.TryGetValue(type, out Skill target))
        {
            return target;
        }

        return null;
    }
}