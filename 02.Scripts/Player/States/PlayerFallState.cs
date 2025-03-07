using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
        _canAirAttack = true;
    }

    public override void OpEnter()
    {
        base.OpEnter();
        if (!_player.IsOwner) return;
        _inputReader.JumpEvent += HandleJumpEvent;

        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void HandleJumpEvent()
    {
        if (_player.jumpCount < 1)
        {
            _stateMachine.ChangeState(PlayerStateEnum.Jump);
            _player.jumpCount++;
        }
    }

    public override void OpUpdate()
    {
        base.OpUpdate();
        if (!_player.IsOwner) return;

        if (_player.IsGrounded())
        {
            if (_inputReader.isMoving)
                _stateMachine.ChangeState(PlayerStateEnum.Move);
            else
            {
                _stateMachine.ChangeState(PlayerStateEnum.Idle);
            }

            _player.currentAirAttackCount = 0;
        }
    }

    public override void OpExit()
    {
        if (!_player.IsOwner) return;

        _inputReader.JumpEvent -= HandleJumpEvent;
    }
}