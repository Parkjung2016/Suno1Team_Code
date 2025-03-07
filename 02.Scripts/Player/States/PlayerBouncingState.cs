public class PlayerBouncingState : PlayerState
{
    public PlayerBouncingState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
        _player.isInvincible = true;
    }

    public override void OpUpdate()
    {
        base.OpUpdate();
        if (!_player.IsOwner) return;
        if (_player.IsWallDetected())
            _stateMachine.ChangeState(PlayerStateEnum.WallSlide);
        else if (_player.IsGrounded() || _player.IsCeilingDetected())
            _stateMachine.ChangeState(PlayerStateEnum.Idle);

        if (animationTrigger)
        {
            _stateMachine.ChangeState(PlayerStateEnum.Idle);
        }

        // _player.SetVelocity(_inputReader.xInput * _player.playerStat.GetStatByType(StatType.MoveSpeed).GetValue() * .8f,
        //     _rigidbody.velocity.y);
    }

    public override void OpExit()
    {
        base.OpExit();
        _player.isInvincible = false;
    }
}