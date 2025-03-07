using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    private bool _canCheckGround;
    private Coroutine _delayCoroutine;


    private float _canAirAttackTime;
    private float _currentCanAirAttackTime;

    public PlayerJumpState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
        if (!_player.IsOwner) return;

        _player.SetVelocity(_rigidbody.velocity.x, _player.playerStat.GetStatByType(StatType.JumpPower).GetValue(),
            false);
        _player.canWallSlide = false;
        _delayCoroutine = _player.StartDelayCallback(.1f, () =>
        {
            if (!_player.StateMachine.EqualState(PlayerStateEnum.Jump)) return;
            _canCheckGround = true;
            _player.canWallSlide = true;
        });
    }


    public override void OpUpdate()
    {
        base.OpUpdate();
        if (!_player.IsOwner) return;

        if (_player.IsFalling())
        {
            _stateMachine.ChangeState(PlayerStateEnum.Fall);
            return;
        }

        if (_player.IsGrounded() && _canCheckGround)
        {
            if (_inputReader.isMoving)
                _stateMachine.ChangeState(PlayerStateEnum.Move);
            else
                _stateMachine.ChangeState(PlayerStateEnum.Idle);
            return;
        }

        if (_currentCanAirAttackTime <= _canAirAttackTime)
        {
            _currentCanAirAttackTime += Time.deltaTime;
        }
        else
        {
            _currentCanAirAttackTime = 0;
            _canAirAttack = true;
        }
    }

    public override void OpExit()
    {
        base.OpExit();
        if (!_player.IsOwner) return;
        _currentCanAirAttackTime = 0;

        _canCheckGround = false;
        _player.StopCoroutine(_delayCoroutine);
    }
}