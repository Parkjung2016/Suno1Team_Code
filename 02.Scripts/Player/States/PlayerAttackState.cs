using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private readonly float _comboWindow = .8f;

    private int _comboCounter;
    private float _lastTimeAttacked;
    private Coroutine _delayCoroutine;
    private readonly string MoveHash = "Move";
    private bool _checkAnimation;

    public PlayerAttackState(Player player, string animBoolHash, PlayerStateMachine stateMachine) : base(player,
        animBoolHash, stateMachine)
    {
    }

    public override void OpEnter()
    {
        base.OpEnter();
        if (!_player.IsOwner) return;
        if (_comboCounter > 1 || Time.time >= _lastTimeAttacked + _comboWindow)
            _comboCounter = 0;
        PlayAttackAnimation();
        _inputReader.JumpEvent += HandleJumpEvent;
    }

    private void HandleJumpEvent()
    {
        if (!_player.IsGrounded()) return;
        _player.SetVelocity(_rigidbody.velocity.x, _player.playerStat.GetStatByType(StatType.JumpPower).GetValue(),
            false);
    }

    public override void OpUpdate()
    {
        foreach (AnimatorControllerParameter parameter in _player.AnimatorCompo.Animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name != _animBoolHash &&
                parameter.name != MoveHash)
                _player.AnimatorCompo.Animator.SetBool(parameter.name, false);
        }

        if (!_player.IsOwner) return;
        if (!_player.AnimatorCompo.Animator.GetCurrentAnimatorStateInfo(1).IsName($"attack_{_comboCounter}") &&
            !_checkAnimation)
        {
            PlayAttackAnimation();
        }

        if (animationTrigger)
        {
            _stateMachine.ChangeState(PlayerStateEnum.Idle);
            return;
        }

        _animator.Animator.SetBool(MoveHash, _inputReader.isMoving);
        bool canRot = Mathf.Abs(_inputReader.xInput) > 0;
        _player.SetVelocity(_inputReader.xInput * _player.playerStat.GetStatByType(StatType.MoveSpeed).GetValue(),
            _rigidbody.velocity.y, canRot);
    }

    async void PlayAttackAnimation()
    {
        _checkAnimation = true;
        _player.AnimatorCompo.SetTrigger($"attack_{_comboCounter}");
        await Task.Delay(800);
        _checkAnimation = false;
    }

    public override void OpExit()
    {
        base.OpExit();
        if (!_player.IsOwner) return;

        ++_comboCounter;
        _lastTimeAttacked = Time.time;
        _player.AnimatorCompo.Animator.speed = 1;
        _animator.Animator.SetBool(MoveHash, false);
        _inputReader.JumpEvent -= HandleJumpEvent;


        // _player.StopCoroutine(_delayCoroutine);
    }
}