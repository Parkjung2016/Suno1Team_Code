public class TestStatSkill : PassiveSkill
{
    public override void UseSkill()
    {
        if (_player.StateMachine == null || _player.StateMachine.CurrentState == null) return;
        if (_player.StateMachine.EqualState(PlayerStateEnum.Idle))
        {
        }
    }
}