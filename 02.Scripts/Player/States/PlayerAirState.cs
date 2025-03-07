using UnityEngine;

public abstract class PlayerAirState : PlayerState
{
    protected bool _canAirAttack;


    protected PlayerAirState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
        if (!_player.IsOwner) return;

        _inputReader.AttackEvent += HandleAttackEvent;
    }

    private void HandleAttackEvent()
    {
        if (!_canAirAttack || !_player.CanAirAttack() || _player.IsWallDetected() || _player.IsGrounded()) return;
        _stateMachine.ChangeState(PlayerStateEnum.AirAttack);
    }


    public override void OpUpdate()
    {
        base.OpUpdate();
        if (!_player.IsOwner) return;

        float xInput = _inputReader.xInput;
        // if (Mathf.Abs(xInput) > 0.05f)
        // {
        _player.SetVelocity(_player.playerStat.GetStatByType(StatType.MoveSpeed).GetValue() * 0.7f * xInput,
            _player.RigidBodyCompo.velocity.y, Mathf.Abs(xInput) > 0.05f);
        // }
        if (_player.IsWallDetected())
        {
            if (_player.canWallSlide)
            {
                _stateMachine.ChangeState(PlayerStateEnum.WallSlide);
            }
        }
    }

    public override void OpExit()
    {
        base.OpExit();
        if (!_player.IsOwner) return;

        _inputReader.AttackEvent -= HandleAttackEvent;
    }
}