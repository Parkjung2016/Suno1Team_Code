using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    private Coroutine _delayCoroutine = null;

    public PlayerWallJumpState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
        if (!_player.IsOwner) return;
        _player.canWallSlide = true;

        _player.SetVelocity(
            _player.wallJumpPower.x * _player.FacingDirection,
            _player.wallJumpPower.y);

        _delayCoroutine = _player.StartDelayCallback(0.2f, () =>
        {
            if (_player.IsGrounded())
                _stateMachine.ChangeState(PlayerStateEnum.Idle);
            else
                _stateMachine.ChangeState(PlayerStateEnum.Fall);
        });
    }

    public override void OpExit()
    {
        base.OpExit();
        if (!_player.IsOwner) return;

        _player.StopCoroutine(_delayCoroutine);
    }
}