using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class ApplyDamageFunc : IComparable<ApplyDamageFunc>
{
    public ApplyDamageFunc(int priority)
    {
        this.priority = priority;
    }

    public int priority = 0;
    public abstract int Sequence(int damage, Player player, int stack);

    public int CompareTo(ApplyDamageFunc other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return priority.CompareTo(other.priority);
    }


    public abstract int GetTypeID();
}

public struct CombatData
{
    public int damage;
}

public class Health : NetworkBehaviour
{
    public Action<int> ChangedHealthEvent;

    public Action<Player> ApplyDamageEvent;
    public Action<Ailment> ApplyDamageByDotDamageEvent;

    public AilmentStat ailmentStat;

    public NetworkVariable<int> currentHealth;

    private Player _player;
    public Player Player => _player;
    public UnityEvent<Ailment, Ailment> OnAilmentChanged;

    private SortedDictionary<ApplyDamageFunc, int> ApplyDamageSequence = new();
    public CombatData combatData;

    public void AddSequence(int id)
    {
        AddSequenceServerRpc(id); // ServerRpc 호출
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddSequenceServerRpc(int id)
    {
        ApplyDamageFunc applyDamageFunc = null;

        switch (id)
        {
            case 0:

                applyDamageFunc = new DamageGuard(100); // 기본 생성자
                break;
            case 1:
                applyDamageFunc = new DamageLate(9); // 기본 생성자
                break;
            case 2:
                applyDamageFunc = new DamageDown(1); // 기본 생성자
                break;

            default:
                Debug.LogError("Unknown ApplyDamageFunc type ID");
                return;
        }

        if (!ApplyDamageSequence.TryAdd(applyDamageFunc, 0))
        {
            ApplyDamageSequence[applyDamageFunc]++;
        }
    }

    public void RemoveSequence(int id)
    {
        RemoveSequenceServerRpc(id);
    }

    [ServerRpc]
    private void RemoveSequenceServerRpc(int id)
    {
        ApplyDamageSequence.Remove(ApplyDamageSequence.First(x => x.Key.GetTypeID() == id).Key);
    }

    public int MaxHealth => (int)_player.playerStat.GetStatByType(StatType.MaxHealth).GetValue();

    private void Awake()
    {
        currentHealth = new NetworkVariable<int>();
        _player = GetComponent<Player>();

        ailmentStat = new AilmentStat();
        ailmentStat.AilmentChangeEvent += HandAilmentChangeEvent;
        ailmentStat.DotDamageEvent += HandleDotDamageEvent;
    }


    private void OnDestroy()
    {
        ailmentStat.AilmentChangeEvent -= HandAilmentChangeEvent;
        ailmentStat.DotDamageEvent -= HandleDotDamageEvent;
    }

    private void HandleDotDamageEvent(Ailment ailmentType, int damage)
    {
        currentHealth.Value = Mathf.Clamp(currentHealth.Value - damage, 0, MaxHealth);

        ApplyDamageByDotDamageEvent?.Invoke(ailmentType);
    }

    private void HandAilmentChangeEvent(Ailment oldAilment, Ailment newAilment)
    {
        OnAilmentChanged?.Invoke(oldAilment, newAilment);
    }

    public void SetAilment(Ailment ailment, float duration, int damage)
    {
        ailmentStat.ApplyAilments(ailment, duration, damage);
    }

    protected void Update()
    {
        ailmentStat.UpdateAilment();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
            currentHealth.Value = MaxHealth;
        currentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, currentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= HandleHealthChanged;
    }

    public void ApplyDamage(int damage, Player player)
    {
        if (_player.isInvincible) return;
        foreach (var func in ApplyDamageSequence)
        {
            damage = func.Key.Sequence(damage, _player, func.Value);
        }

        currentHealth.Value = Mathf.Clamp(currentHealth.Value - damage, 0, MaxHealth);
        ApplyDamageEvent?.Invoke(player);
    }

    public void SetCombatData(int damage)
    {
        combatData.damage = damage;
    }

    public void ApplyHeal(int amount)
    {
        currentHealth.Value = Mathf.Min(currentHealth.Value + amount, MaxHealth);
        if (IsOwner)
            ChangedHealthEvent?.Invoke(currentHealth.Value);
    }

    private void HandleHealthChanged(int previousValue, int newValue)
    {
        if (IsOwner)
            ChangedHealthEvent?.Invoke(newValue);
    }
}