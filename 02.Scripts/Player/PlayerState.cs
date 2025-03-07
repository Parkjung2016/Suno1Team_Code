using Unity.Netcode;
using UnityEngine;

public abstract class PlayerState
{
    public bool animationTrigger;

    protected PlayerStateMachine _stateMachine;
    protected Rigidbody2D _rigidbody;
    protected Player _player;
    protected ClientNetworkAnimator _animator;
    protected InputReader _inputReader;
    protected Camera _mainCamera;

    protected string _animBoolHash;
    protected bool _canChangeAnimBool = true;


    protected PlayerState(Player player, string animBoolHash, PlayerStateMachine stateMachine)
    {
        _player = player;
        _stateMachine = stateMachine;
        _animator = _player.AnimatorCompo;
        _inputReader = _player.InputReader;
        _mainCamera = Camera.main;
        _animBoolHash = animBoolHash;
        _rigidbody = _player.RigidBodyCompo;
    }

    public virtual void OpEnter()
    {
        if (!_player.IsOwner) return;

        if (_canChangeAnimBool)
            _animator.Animator.SetBool(_animBoolHash, true);
    }

    public virtual void OpUpdate()
    {
        if (!_player.IsOwner) return;

        foreach (AnimatorControllerParameter parameter in _player.AnimatorCompo.Animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name != _animBoolHash)
                _player.AnimatorCompo.Animator.SetBool(parameter.name, false);
        }
    }

    public virtual void OpExit()
    {
        if (!_player.IsOwner) return;

        animationTrigger = false;


        if (_canChangeAnimBool)
            _animator.Animator.SetBool(_animBoolHash, false);
    }
}