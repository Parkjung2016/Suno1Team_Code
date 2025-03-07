using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAirAttackState : PlayerState
{
    private readonly float _comboWindow = .8f;

    private int _comboCounter;
    private float _lastTimeAttacked;
    private Coroutine _delayCoroutine;
    private bool _checkAnimation;

    public PlayerAirAttackState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
        if (!_player.IsOwner) return;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        _player.SetVelocity(0, 0, false);
        if (_comboCounter > 0 || Time.time >= _lastTimeAttacked + _comboWindow)
        {
            _comboCounter = 0;
            _player.currentAirAttackCount++;
        }

        PlayAirAttackAnimation();


        float attackDirection = -_player.FacingDirection;
        float xInput = _inputReader.xInput;

        if (Mathf.Abs(xInput) > 0.05f)
        {
            attackDirection = Mathf.Sign(xInput);
        }

        Vector2 movement = _player.airAttackMovement[_comboCounter];
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _player.airAttackKnockBackPower = movement;
        _player.SetVelocity(movement.x * attackDirection, movement.y);


        _delayCoroutine = _player.StartDelayCallback(0.1f, () =>
        {
            _rigidbody.constraints =
                RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            _player.StopImmediate(false);
        });
    }

    public override void OpUpdate()
    {
        base.OpUpdate();
        if (!_player.IsOwner) return;
        if (!_player.AnimatorCompo.Animator.GetCurrentAnimatorStateInfo(0).IsName($"airAttack_{_comboCounter}") &&
            !_checkAnimation)
        {
            PlayAirAttackAnimation();
        }

        if (animationTrigger)
        {
            if (_player.IsWallDetected())
                _stateMachine.ChangeState(PlayerStateEnum.WallSlide);
            else
                _stateMachine.ChangeState(PlayerStateEnum.Fall);
        }
    }

    async void PlayAirAttackAnimation()
    {
        _checkAnimation = true;
        _player.AnimatorCompo.SetTrigger($"airAttack_{_comboCounter}");
        await Task.Delay(800);
        _checkAnimation = false;
    }

    public override void OpExit()
    {
        base.OpExit();
        if (!_player.IsOwner) return;

        ++_comboCounter;
        _lastTimeAttacked = Time.time;
        _player.AnimatorCompo.Animator.speed = 1;
        _player.airAttackKnockBackPower = Vector2.zero;

        _player.StopCoroutine(_delayCoroutine);
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}