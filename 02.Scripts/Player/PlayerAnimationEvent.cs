using System;
using Hellmade.Sound;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Player _player;

    [SerializeField] private AudioClip[] _footStepSounds;
    [SerializeField] private AudioClip[] _swordSwipeSounds;
    [SerializeField] private AudioClip[] _jumpSounds;

    private void Awake()
    {
        _player = transform.root.GetComponent<Player>();
    }

    private void AnimationTrigger()
    {
        if (!_player.IsOwner) return;
        _player.StateMachine.CurrentState.animationTrigger = true;
    }

    private void EnableWeaponColliderL(int enable)
    {
        bool enableCollider = Convert.ToBoolean(enable);
        _player.EquipmentCompo.GetEquipment<Weapon>(SocketType.HandL)?.EnableCollider(enableCollider);
    }

    private void EnableWeaponColliderR(int enable)
    {
        bool enableCollider = Convert.ToBoolean(enable);
        _player.EquipmentCompo.GetEquipment<Weapon>(SocketType.HandR)?.EnableCollider(enableCollider);
    }

    private void PlayFootstepSound()
    {
        PlayFootstepSoundClientRPC();
    }

    [ClientRpc]
    private void PlayFootstepSoundClientRPC()
    {
        EazySoundManager.PlaySound(_footStepSounds[Random.Range(0, _footStepSounds.Length)], .5f);
    }

    private void PlaySwordSwipesSound()
    {
        PlaySwordSwipesSoundClientRPC();
    }

    [ClientRpc]
    private void PlaySwordSwipesSoundClientRPC()
    {
        EazySoundManager.PlaySound(_swordSwipeSounds[Random.Range(0, _swordSwipeSounds.Length)], .5f);
    }

    private void PlayJumpSound()
    {
        PlayJumpSoundClientRPC();
    }

    [ClientRpc]
    private void PlayJumpSoundClientRPC()
    {
        EazySoundManager.PlaySound(_jumpSounds[Random.Range(0, _jumpSounds.Length)]);
    }
}