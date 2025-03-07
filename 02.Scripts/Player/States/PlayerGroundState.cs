using UnityEngine;

public abstract class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
        if (!_player.IsOwner) return;
        _inputReader.JumpEvent += HandleJumpEvent;
        _inputReader.AttackEvent += HandleAttackEvent;
        _player.jumpCount = 0;
    }

    private void HandleJumpEvent()
    {
        _stateMachine.ChangeState(PlayerStateEnum.Jump);
    }

    private void HandleAttackEvent()
    {
        _stateMachine.ChangeState(PlayerStateEnum.Attack);
    }

    public override void OpUpdate()
    {
        base.OpUpdate();
        if (!_player.IsOwner) return;

        if (_player.IsFalling())
        {
            _stateMachine.ChangeState(PlayerStateEnum.Fall);
        }
    }

    public override void OpExit()
    {
        base.OpExit();
        if (!_player.IsOwner) return;
        _inputReader.JumpEvent -= HandleJumpEvent;
        _inputReader.AttackEvent -= HandleAttackEvent;
    }
}