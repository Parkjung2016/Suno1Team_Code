using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : NetMonoSingleton<GameUI>
{
    private Player _player;
    [SerializeField] private Image[] _leftWinCountImages;   
    [SerializeField] private Image[] _rightWinCountImages;

    private Dictionary<ulong, Image[]> _winCountImages = new();

    private void Awake()
    {
        for (int i = 0; i < _leftWinCountImages.Length; i++)
        {
            _leftWinCountImages[i].fillAmount = 0;
        }

        for (int i = 0; i < _rightWinCountImages.Length; i++)
        {
            _rightWinCountImages[i].fillAmount = 0;
        }
    }

    public void Init(Player player)
    {
        _player = player;
    }

    public void CallWinGame(ulong ownerID)
    {
        WinGame(ownerID);
        WinGameClientRpc(ownerID);
    }

    [ClientRpc]
    private void WinGameClientRpc(ulong ownerID)
    {
        if (IsServer) return;
        WinGame(ownerID);
    }

    private void WinGame(ulong ownerID)
    {
        for (int i = 0; i < _winCountImages[ownerID].Length; i++)
        {
            if (_winCountImages[ownerID][i].fillAmount != 1)
            {
                _winCountImages[ownerID][i].fillAmount += .5f;
                break;
            }
        }
    }

    public void CallAddWinCountImagePlayer(ulong ownerID, bool isHost)
    {
        AddWinCountImagePlayer(ownerID, isHost);
        AddWinCountImagePlayerClientRpc(ownerID, isHost);
    }

    [ClientRpc(RequireOwnership = false)]
    private void AddWinCountImagePlayerClientRpc(ulong ownerID, bool isHost)
    {
        if (IsServer) return;

        AddWinCountImagePlayer(ownerID, isHost);
    }

    private void AddWinCountImagePlayer(ulong ownerID, bool isHost)
    {
        _winCountImages.Add(ownerID, isHost ? _rightWinCountImages : _leftWinCountImages);
    }
}