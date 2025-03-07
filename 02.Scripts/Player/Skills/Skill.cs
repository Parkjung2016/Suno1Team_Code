using Unity.Netcode;
using UnityEngine;

public delegate void CooldownInfoEvent(float current, float total);

public abstract class Skill : NetworkBehaviour
{
    public bool skillEnabled = false;

    protected Player _player;

    protected float _cooldownTimer;

    [SerializeField] protected float _cooldown;
    public bool IsCooldown => _cooldownTimer < _cooldown;
    public event CooldownInfoEvent OnCooldownEvent;
    public virtual void Init(Player owner)
    {
        _player = owner;
        _cooldownTimer = _cooldown;
    }
    public virtual void OnSkillEnable()
    {

    }

    protected virtual void Update()
    {
        if (_cooldownTimer < _cooldown)
        {
            _cooldownTimer += Time.deltaTime;

            if (_cooldownTimer >= _cooldown)
            {
                _cooldownTimer = _cooldown;
            }

            OnCooldownEvent?.Invoke(_cooldownTimer, _cooldown);
        }
    }

    public virtual bool AttemptUseSkill()
    {
        if (_cooldownTimer == _cooldown && skillEnabled)
        {
            _cooldownTimer = 0;
            UseSkill();
            return true;
        }

        //Debug.Log("Skill cooldown or locked");
        return false;
    }

    public virtual void UseSkill()
    {
    }
    
}