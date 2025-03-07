using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();

        if (!_player.IsOwner) return;
        Vector2 scale = Vector2.one;
        scale.x = -1;
        _player.RootCompo.localScale = scale;

        _inputReader.JumpEvent += HandleJumpEvent;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }


    private void HandleJumpEvent()
    {
        _stateMachine.ChangeState(PlayerStateEnum.WallJump);
    }

    public override void OpUpdate()
    {
        base.OpUpdate();
        if (!_player.IsOwner) return;

        float xInput = _inputReader.xInput;
        float yInput = _inputReader.yInput;
        float x = 0;
        if (Mathf.Abs(xInput + -_player.FacingDirection) < 0.5f)
        {
            x = -_player.FacingDirection * -3;
            // if (_player.IsGrounded())
            // {
            //     _stateMachine.ChangeState(PlayerStateEnum.Idle);
            // }
            // else
            // {
            //     _stateMachine.ChangeState(PlayerStateEnum.Fall);
            // }

        }

        if (yInput > 0)
        {
            _player.SetVelocity(x, _rigidbody.velocity.y, false);
        }
        else
        {
            _player.SetVelocity(x, _rigidbody.velocity.y * 0.7f, false);
        }


        if (_player.IsGrounded())
        {
            _stateMachine.ChangeState(PlayerStateEnum.Idle);
        }
        else if (!_player.IsWallDetected())
        {
            _stateMachine.ChangeState(PlayerStateEnum.Fall);
        }
    }

    public override void OpExit()
    {
        base.OpExit();
        if (!_player.IsOwner) return;

        Vector2 scale = Vector2.one;
        scale.x = 1;
        _player.RootCompo.localScale = scale;
        if (!_player.IsOwner) return;

        _inputReader.JumpEvent -= HandleJumpEvent;
    }
}