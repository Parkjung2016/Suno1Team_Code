using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStateMachine : NetworkBehaviour
{
    private Player _player;

    private Dictionary<PlayerStateEnum, PlayerState> _stateDictionary = new();
    public PlayerState CurrentState { get; set; }

    public void AddState(PlayerStateEnum stateEnum, PlayerState state)
    {
        _stateDictionary.Add(stateEnum, state);
    }

    public void ChangeState(PlayerStateEnum state)
    {
        if (!IsOwner) return;

        if (!_player.CanStateChangeable || !_player.enabled) return;
        if (CurrentState != null) CurrentState.OpExit();
        CurrentState = _stateDictionary[state];
        if (CurrentState != null) CurrentState.OpEnter();
        // ChangeStateServerRpc(state);
    }

    public void ClearState()
    {
        CurrentState.OpExit();
        CurrentState = null;
    }

    public bool EqualState(PlayerStateEnum state)
    {
        return CurrentState == _stateDictionary[state];
    }

    // [ServerRpc]
    // private void ChangeStateServerRpc(PlayerStateEnum state)
    // {
    //     ChangeStateClientRpc(state);
    // }
    //
    // [ClientRpc]
    // private void ChangeStateClientRpc(PlayerStateEnum state)
    // {
    //     Debug.Log(34);
    //     if (!_player.CanStateChangeable || !_player.enabled) return;
    //     if (CurrentState != null) CurrentState.OpExit();
    //     CurrentState = _stateDictionary[state];
    //     if (CurrentState != null) CurrentState.OpEnter();
    // }

    public virtual void Init(PlayerStateEnum startState, Player player)
    {
        _player = player;


        ChangeState(startState);
    }

    public void DoStateOpUpdate()
    {
        if (CurrentState == null) return;
        CurrentState.OpUpdate();
    }
}