using System;
using Hellmade.Sound;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Player
{
    private void HandleGuardEvent()
    {
        GuardSkill guardSkill = SkillCompo.GetSkill<GuardSkill>();
        if (guardSkill.AttemptUseSkill())
        {
            HealthCompo.AddSequence(0);
            SpriteEffectCompo.CallSetHitEffect(Color.cyan, .15f);
            OnGuardEvent?.Invoke(true);

            float guardCoolTime = playerStat.GetStatByType(StatType.GuardTime).GetValue();

            StartDelayCallback(guardCoolTime,
                () =>
                {
                    OnGuardEvent?.Invoke(false);
                    SpriteEffectCompo.CallSetHitEffect(Color.cyan, 0);
                    HealthCompo.RemoveSequence(0);
                });
        }
    }

    private void HandleChangedHealthEvent(int health)
    {
        if (health <= 0)
        {
            Death();
        }
    }

    private void HandleApplyDamageByDotDamageEvent(Ailment ailment)
    {
        Color ailmentColor = GameConfig.GetAilmentColor(ailment);
        SpriteEffectCompo.CallSetHitEffect(ailmentColor, .15f);
        StartDelayCallback(playerStat.GetStatByType(StatType.GuardTime).GetValue(),
            () => { SpriteEffectCompo.CallSetHitEffect(Color.cyan, 0); });
    }

    private void HandleApplyDamageEvent(Player target)
    {
        if (target == null) return;

        float direction = Mathf.Sign(-target.VisualCompo.localScale.x);
        float powerX = 0;
        float powerY = 0;
        float duration = 0;
        if (target.airAttackKnockBackPower == Vector2.zero)
        {
            powerX = target.playerStat.GetStatByType(StatType.KnockBackPowerX).GetValue();
            powerY = target.playerStat.GetStatByType(StatType.KnockBackPowerY).GetValue();
            duration = target.playerStat.GetStatByType(StatType.KnockBackDuration).GetValue();
        }
        else
        {
            powerX = target.airAttackKnockBackPower.x;
            powerY = target.airAttackKnockBackPower.y;
            duration = .1f;
        }

        if (IsOwner)
        {
            KnockBack(direction * powerX, powerY, duration);
        }
        else
        {
            KnockBackClientRpc(direction * powerX, powerY, duration);
        }

        PlayHitSound();
        ApplyDamageEvent?.Invoke(target);
    }

    private void PlayHitSound()
    {
        PlayHitSoundClientRPC();
    }

    [ClientRpc]
    private void PlayHitSoundClientRPC()
    {
        EazySoundManager.PlaySound(_hitSounds[Random.Range(0, _hitSounds.Length)]);
    }

    public void KnockBack(float powerX, float powerY, float duration, Action CallBack = null)
    {
        SetVelocity(powerX, powerY, false);
        StartDelayCallback(duration, () =>
        {
            SetVelocity(0, 0, false);
            CallBack?.Invoke();
        });
    }

    [ClientRpc]
    private void KnockBackClientRpc(float powerX, float powerY, float duration)
    {
        KnockBack(powerX, powerY, duration);
    }
}