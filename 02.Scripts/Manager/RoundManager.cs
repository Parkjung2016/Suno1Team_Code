using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RoundManager : NetMonoSingleton<RoundManager>
{
    [SerializeField] private int _maxWinCount = 6;
    [SerializeField] private int _selectSkillWinCount = 2;
    private Dictionary<ulong, int> _playerWinCount = new();

    public Action<ulong> WinPlayerEvent;
    public Action<ulong> StartSelectSkillEvent;
    public Action EndSelectSkillEvent;

    private void Awake()
    {
        EndSelectSkillEvent += HandleEndSelectSkill;
    }

    private void HandleEndSelectSkill()
    {
        Time.timeScale = 1f;
    }

    public void AddPlayerWinCountDictionary(ulong ownerID)
    {
        _playerWinCount.Add(ownerID, 0);
    }


    public void WinPlayer(ulong ownerID)
    {
        int winCount = ++_playerWinCount[ownerID];
        if (winCount >= _maxWinCount)
        {
            WinPlayerClientRpc(ownerID);
        }

        else if (winCount % _selectSkillWinCount == 0)
        {
            SelectSkillClientRpc(ownerID);
        }

        GameUI.Instance.CallWinGame(ownerID);
    }

    [ClientRpc]
    public void WinPlayerClientRpc(ulong ownerID)
    {
        WinPlayerEvent?.Invoke(ownerID);
        Time.timeScale = 0;
    }

    [ClientRpc]
    public void SelectSkillClientRpc(ulong ownerID)
    {
        StartSelectSkillEvent?.Invoke(ownerID);
        Time.timeScale = 0;
    }
}