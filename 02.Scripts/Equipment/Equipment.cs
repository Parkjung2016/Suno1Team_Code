using System;
using Unity.Netcode;
using UnityEngine;

public enum SocketType
{
    HandL,
    HandR
}


public abstract class Equipment : NetworkBehaviour
{
    public SocketType socketType;
    public byte id;
    public Player owner;
    [HideInInspector] public SpriteRenderer spriteRenderer;


    public virtual void Init(Player owner)
    {
        this.owner = owner;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}