using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }


    public override void OpEnter()
    {
        base.OpEnter();
        if (!_player.IsOwner) return;
        _player.StopImmediate();
    }


    public override void OpUpdate()
    {
        if (!_player.IsOwner) return;
        float xInput = _inputReader.xInput;
        if (Mathf.Abs(_player.FacingDirection + xInput) > 1.2f && _player.IsWallDetected() && !_player.IsGrounded())
            return;
        if (_inputReader.isMoving)
        {
            _stateMachine.ChangeState(PlayerStateEnum.Move);
            return;
        }

        base.OpUpdate();
    }
}