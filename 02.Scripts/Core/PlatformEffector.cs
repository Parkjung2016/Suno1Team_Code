using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlatformEffector : NetworkBehaviour
{
    private Collider2D _col2D;
    private Player _player;

    private void Awake()
    {
        _col2D = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.TryGetComponent(out Player player))
        {
            if (player.IsOwner)
                _player = player;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.TryGetComponent(out Player player))
        {
            if (player.IsOwner)
                _player = player;
        }
    }

    private void Update()
    {
        if (!_player) return;
        if (_player.InputReader.yInput < 0 && _col2D.enabled)
        {
            TriggerCollider();
        }
    }

    private async void TriggerCollider()
    {
        _col2D.enabled = false;
        await Task.Delay(500);
        _col2D.enabled = true;
    }
}