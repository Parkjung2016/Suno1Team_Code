using System;
using System.Linq;
using DG.Tweening;
using ObjectPooling;
using Unity.Netcode;
using UnityEngine;


public class Weapon : Equipment
{
    private WeaponCollider _collider;


    public override void Init(Player owner)
    {
        base.Init(owner);

        _collider = GetComponentInChildren<WeaponCollider>();
        _collider.Init(this);
        Physics2D.IgnoreCollision(owner.ColliderCompo, _collider.ColliderCompo);
    }

    public void ChangeColliderLength(float length)
    {
        _collider.ChangeColliderLength(length);
    }

    public void EnableCollider(bool enable)
    {
        _collider.EnableCollider(enable);
    }
}