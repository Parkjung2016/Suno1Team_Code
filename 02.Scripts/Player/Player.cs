using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public enum PlayerStateEnum
{
    Idle,
    Move,
    Jump,
    Fall,
    Attack,
    AirAttack,
    WallSlide,
    WallJump,
    Death,
    Bouncing
}

public class DamageGuard : ApplyDamageFunc
{
    public DamageGuard(int priority) : base(priority)
    {
    }

    public override int Sequence(int damage, Player player, int stack)
    {
        return 0;
    }

    public override int GetTypeID()
    {
        return 0;
    }
}

public partial class Player : NetworkBehaviour
{
    private void Awake()
    {
        _inputReader.AttackEvent = null;
        _inputReader.JumpEvent = null;
        // _inputReader.GuardEvent = null;
        _inputReader.AttackHoldEvent = null;
        _inputReader.ESCEvent = null;
        playerStat = Instantiate(playerStat);
        VisualCompo = transform.Find("Visual");
        _detectGroundTrm = transform.Find("DetectGround");
        _detectWallTrm = transform.Find("DetectWall");
        _detectCeilingTrm = transform.Find("DetectCeiling");
        RootCompo = VisualCompo.Find("Root");
        EquipmentCompo = GetComponent<PlayerEquipment>();
        AnimatorCompo = VisualCompo.GetComponent<ClientNetworkAnimator>();
        RigidBodyCompo = GetComponent<Rigidbody2D>();
        _originScale = VisualCompo.localScale.x;
        ColliderCompo = GetComponent<Collider2D>();
        HealthCompo = GetComponent<Health>();
        SpriteEffectCompo = GetComponent<PlayerSpriteEffect>();
        SkillCompo = GetComponent<PlayerSkill>();
        TargetFeedbackCompo = GetComponent<PlayerTargetFeedback>();
        GuardCoolTimeUI = transform.Find("GuardCoolTimeCanvas").GetComponent<GuardCoolTimeUI>();
        EquipmentCompo.Init(this);
        HealthCompo.ApplyDamageByDotDamageEvent += HandleApplyDamageByDotDamageEvent;
        HealthCompo.ApplyDamageEvent += HandleApplyDamageEvent;
    }

    private void SubscribeHitEvent()
    {
        PassiveSkill[] whenHitSkills = SkillCompo.FindPassiveSkillS(UsePassiveSkillType.WhenHit);

        foreach (var whenHitSkill in whenHitSkills)
        {
            HitWithWeaponEvent += whenHitSkill.TryUsePassiveSkill;
        }
    }

    private void SubscribeApplyDamageEvent()
    {
        PassiveSkill[] whenApplyDamageSkills = SkillCompo.FindPassiveSkillS(UsePassiveSkillType.WhenApplyDamage);
        foreach (var whenApplyDamageSkill in whenApplyDamageSkills)
        {
            ApplyDamageEvent += whenApplyDamageSkill.TryUsePassiveSkill;
        }
    }

    private void OnDisable()
    {
        if (IsOwner)
            _inputReader.GuardEvent -= HandleGuardEvent;
    }


    private void SetStateDictionary()
    {
        StateMachine = GetComponent<PlayerStateMachine>();
        foreach (PlayerStateEnum stateEnum in Enum.GetValues(typeof(PlayerStateEnum)))
        {
            string typeName = stateEnum.ToString();
            try
            {
                string tName = $"{GetType().Name}{typeName}State";
                Type t = Type.GetType(tName);
                var state = Activator.CreateInstance(t, this, typeName, StateMachine) as PlayerState;
                StateMachine.AddState(stateEnum, state);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{typeName} is loading error!");
                Debug.LogError(ex.Message);
            }
        }

        StateMachine.Init(PlayerStateEnum.Idle, this);
    }

    private void Update()
    {
        StateMachine.DoStateOpUpdate();

        UpdateAttackSpeed();
    }

    private void UpdateAttackSpeed()
    {
        float attackSpeed = playerStat.GetStatByType(StatType.AttackSpeed).GetValue();
        AnimatorCompo.Animator.SetFloat(AttackSpeedHash, attackSpeed);
    }

    #region velocity

    public void SetVelocity(float x, float y, bool canChangeRotation = true)
    {
        Vector3 velocity = new Vector3(x, y);
        RigidBodyCompo.velocity = velocity;

        if (canChangeRotation && velocity.sqrMagnitude > 0)
        {
            Vector3 scale = Vector2.one * _originScale;
            scale.x = _originScale * -Mathf.Sign(x);
            VisualCompo.localScale = scale;
        }
    }

    public void StopImmediate(bool applyGravity = true)
    {
        RigidBodyCompo.velocity = new Vector2(0, applyGravity ? RigidBodyCompo.velocity.y : 0);
    }

    #endregion

    public Coroutine StartDelayCallback(float time, Action callBack)
    {
        return StartCoroutine(DelayCallBack(time, callBack));
    }

    IEnumerator DelayCallBack(float time, Action callBack)
    {
        yield return YieldCache.WaitForSeconds(time);
        callBack?.Invoke();
    }

    private void SetTrigger(string name)
    {
        AnimatorCompo.SetTrigger(name);
    }

    private async void Death()
    {
        RigidBodyCompo.gravityScale = 0;
        SpriteEffectCompo.CallFade(1);
        EnableColliderServerRpc(false);
        WinPlayerServerRpc();
        StateMachine.ChangeState(PlayerStateEnum.Death);
        await Task.Delay(4000);
        RigidBodyCompo.gravityScale = 3;
        EnableColliderServerRpc(true);
        GameManager.Instance.RespawnPlayerServerRpc(ownerID);
        SpriteEffectCompo.CallFade(0);
    }

    [ServerRpc]
    private void WinPlayerServerRpc()
    {
        RoundManager.Instance.WinPlayer(ownerID);
    }

    [ServerRpc]
    private void EnableColliderServerRpc(bool enabled)
    {
        EnableColliderClientRpc(enabled);
    }

    [ClientRpc]
    private void EnableColliderClientRpc(bool enabled)
    {
        ColliderCompo.enabled = enabled;
    }

    public bool CanAirAttack() => currentAirAttackCount < _maxAirAttackCount;

    public override void OnDestroy()
    {
        _inputReader.AttackEvent = null;
        _inputReader.JumpEvent = null;
        _inputReader.GuardEvent = null;
        _inputReader.AttackHoldEvent = null;
        _inputReader.ESCEvent = null;
        _inputReader.EnablePlayerInput(true);
    }
}