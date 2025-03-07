using Unity.Netcode;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public BoxCollider2D ColliderCompo { get; private set; }
    private Transform _pivot;

    private Weapon _weapon;

    public void Init(Weapon weapon)
    {
        ColliderCompo = GetComponent<BoxCollider2D>();
        _weapon = weapon;
        _pivot = transform.parent;
        EnableCollider(false);
    }

    public void ChangeColliderLength(float length)
    {
        _pivot.localScale = new Vector3(1, length, 1);
    }

    public void EnableCollider(bool enable)
    {
        ColliderCompo.enabled = enable;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_weapon.owner.IsServer) return;
        if (other.TryGetComponent(out Health health))
        {
            if (health.currentHealth.Value <= 0) return;
            if (_weapon.owner.IsOwner)
                _weapon.owner.TargetFeedbackCompo.CallPlayTargetHitFeedback(health.GetComponent<NetworkObject>());
            else
            {
                health.GetComponent<PlayerTargetFeedback>()
                    .CallPlayTargetHitFeedback(health.GetComponent<NetworkObject>());
            }

            int damage = (int)_weapon.owner.playerStat.GetStatByType(StatType.Damage).GetValue();
            health.SetCombatData(damage);
            _weapon.owner.HitWithWeaponEvent?.Invoke(health.Player);
            _weapon.EnableCollider(false);
            health.ApplyDamage((int)_weapon.owner.playerStat.GetStatByType(StatType.Damage).GetValue(), _weapon.owner);
        }
    }
}