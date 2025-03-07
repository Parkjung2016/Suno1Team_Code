using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public partial class Player
{
    public delegate void HitWithWeaponEventHandler(Player target);

    public delegate void ApplyDamageEventHandler(Player target);

    public Action<bool> OnGuardEvent;
    private readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");

    [SerializeField] private InputReader _inputReader;
    public PlayerStat playerStat;

    #region component

    public PlayerEquipment EquipmentCompo { get; private set; }
    public Rigidbody2D RigidBodyCompo { get; private set; }
    public Transform VisualCompo { get; private set; }
    public ClientNetworkAnimator AnimatorCompo { get; private set; }
    public Transform RootCompo { get; private set; }
    public Collider2D ColliderCompo { get; private set; }
    public Health HealthCompo { get; private set; }
    public PlayerSpriteEffect SpriteEffectCompo { get; private set; }
    public PlayerSkill SkillCompo { get; private set; }
    public PlayerTargetFeedback TargetFeedbackCompo { get; private set; }


    public GuardCoolTimeUI GuardCoolTimeUI { get; private set; }

    #endregion


    #region detect ground

    private Transform _detectGroundTrm;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Vector2 _detectGroundSize;

    #endregion

    #region detect Wall

    [SerializeField] private float _detectWallDistance;
    [SerializeField] private float _detectCeilingDistance;

    private Transform _detectWallTrm;
    private Transform _detectCeilingTrm;

    #endregion


    public HitWithWeaponEventHandler HitWithWeaponEvent;
    public ApplyDamageEventHandler ApplyDamageEvent;
    public InputReader InputReader => _inputReader;

    public Vector2 wallJumpPower;
    public bool CanStateChangeable { get; private set; } = true;
    public bool IsMoving => _inputReader.xInput == 0;
    public PlayerStateMachine StateMachine { get; private set; }
    public float FacingDirection => VisualCompo.localScale.x;
    public ulong ownerID => NetworkObject.OwnerClientId;
    public Vector2[] airAttackMovement;
    [HideInInspector] public Vector2 airAttackKnockBackPower;
    [HideInInspector] public int currentAirAttackCount;

    [SerializeField] private int _maxAirAttackCount;
    private float _originScale;

    public Weapon weapon;
    public int jumpCount;
    public bool isInvincible;
    public bool canWallSlide = true;
    public bool canBouncing = true;
    private readonly RaycastHit2D[] _wallhits = new RaycastHit2D[1];
    [SerializeField] private string _ignoreWalltag;
    [SerializeField] private AudioClip[] _hitSounds;
}