using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraShakeFeedback : Feedback
{
    private CinemachineImpulseSource _impulseSource;

    [SerializeField] private float _force = .1f;

    protected override void Awake()
    {
        base.Awake();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public override void CompleteFeedback()
    {
        if (_owner.IsOwner)
        {
            CameraShake();
        }
        else
        {
            CameraShakeClientRPC();
        }
    }

    [ClientRpc]
    private void CameraShakeClientRPC()
    {
        if (!_owner.IsOwner) return;
        CameraShake();
    }

    private void CameraShake()
    {
        _impulseSource.GenerateImpulse(_force);
    }
}