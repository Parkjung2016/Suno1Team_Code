using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
        if (_player.IsOwner)
            _inputReader.EnablePlayerInput(false);
    }
}