using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
    }

    public override void OpUpdate()
    {
        base.OpUpdate();
        if (!_player.IsOwner) return;
        if (!_inputReader.isMoving)
        {
            _stateMachine.ChangeState(PlayerStateEnum.Idle);
            return;
        }


        _player.SetVelocity(_inputReader.xInput * _player.playerStat.GetStatByType(StatType.MoveSpeed).GetValue(),
            _rigidbody.velocity.y);
    }
}