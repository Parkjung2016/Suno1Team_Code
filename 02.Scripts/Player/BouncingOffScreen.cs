using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class BouncingOffScreen : NetworkBehaviour
{
    [SerializeField] private int _bouncingDamage;
    [SerializeField] private float _bouncingPower = 40f;
    private Player _player;
    private Camera _mainCamera;


    private void Awake()
    {
        _player = GetComponent<Player>();
        _mainCamera = Camera.main;
    }

    private void ChangeBouncingState(float directionX, float directionY)
    {
        if (!_player.canBouncing) return;
        ApplyDamageServerRpc();
        _player.StateMachine.ChangeState(PlayerStateEnum.Bouncing);
        TriggerCanBouncing();
        _player.KnockBack(directionX * _bouncingPower, directionY * _bouncingPower, .2f, () =>
        {
            if (_player.StateMachine.EqualState(PlayerStateEnum.Bouncing))
                _player.StateMachine.CurrentState.animationTrigger = true;
        });
    }

    [ServerRpc]
    private void ApplyDamageServerRpc()
    {
        _player.HealthCompo.ApplyDamage(_bouncingDamage, null);
    }

    private async void TriggerCanBouncing()
    {
        _player.canBouncing = false;
        await Task.Delay(100);
        _player.canBouncing = true;
    }

    void Update()
    {
        if (!IsOwner) return;
        Vector3 viewPos = _mainCamera.WorldToViewportPoint(transform.position);


        if (viewPos.y < 0)
        {
            ChangeBouncingState(0, 1);
        }
        else if (viewPos.y > 1)
        {
            ChangeBouncingState(0, -1);
        }
        else if (viewPos.x < 0)
        {
            ChangeBouncingState(1, 0);
        }
        else if (viewPos.x > 1)
        {
            ChangeBouncingState(-1, 0);
        }
    }
}