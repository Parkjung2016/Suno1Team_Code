using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : NetMonoSingleton<GameManager>
{
    [SerializeField] private Player[] _playerPrefabs;
    public EquipmentListSO equipmentList;
    [SerializeField] private Transform[] _playerSpawnTrms;
    [SerializeField] private Transform[] _playerRespawnTrms;

    private Dictionary<ulong, Transform> _playerSpawnDictionary = new();

    private Dictionary<ulong, Player> _players = new();

    private List<Player> _playerPrefabsList;

    private CinemachineVirtualCamera _cam;

    public Player GetPlayer(ulong playerId)
    {
        return _players[playerId];
    }

    public void AddPlayer(ulong id, Player player)
    {
        _players.Add(id, player);
    }

    public void RemovePlayer(ulong id)
    {
        _players.Remove(id);
    }

    private void Awake()
    {
        _cam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();

        _playerSpawnTrms = new Transform[2];
        for (int i = 1; i <= _playerSpawnTrms.Length; i++)
        {
            _playerSpawnTrms[i - 1] = GameObject.Find($"PlayerPos{i}").transform;
        }

        _playerPrefabsList = _playerPrefabs.ToList();
        VolumeManager.Instance = new VolumeManager();
    }

    private void Start()
    {
        if (IsServer)
            GameStart();
    }

    public void GameStart()
    {
        var users = HostSingleton.Instance.GameManager.NetServer.clientToAuthDictionary;
        int spawnTrmIdx = 0;
        foreach (var client in users.Keys)
        {
            SpawnPlayer(client, spawnTrmIdx++);
        }
    }

    public void SpawnPlayer(ulong clientID, int spawnTrmIdx)
    {
        Transform spawnTrm = _playerSpawnTrms[spawnTrmIdx];
        int rand = Random.Range(0, _playerPrefabsList.Count);
        Player player = Instantiate(_playerPrefabsList[rand], spawnTrm.position, Quaternion.identity);

        _playerSpawnDictionary.Add(clientID, spawnTrm);
        player.NetworkObject.SpawnAsPlayerObject(clientID);
        RoundManager.Instance.AddPlayerWinCountDictionary(clientID);
        GameUI.Instance.CallAddWinCountImagePlayer(clientID, NetworkObject.OwnerClientId == clientID);
        _playerPrefabsList.RemoveAt(rand);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnPlayerServerRpc(ulong ownerID)
    {
        Player player = GetPlayer(ownerID);
        player.HealthCompo.currentHealth.Value = player.HealthCompo.MaxHealth;

        if (player.IsOwner)
        {
            RespawnPlayer(player);
        }
        else
        {
            RespawnPlayerClientRpc(ownerID);
        }
    }

    [ClientRpc(RequireOwnership = false)]
    private void RespawnPlayerClientRpc(ulong ownerID)
    {
        Player player = FindObjectsOfType<Player>().First(x => x.ownerID == ownerID);

        if (!player.IsOwner) return;
        RespawnPlayer(player);
    }

    private void RespawnPlayer(Player player)
    {
        player.InputReader.EnablePlayerInput(true);
        player.RigidBodyCompo.constraints = RigidbodyConstraints2D.FreezeRotation;

        player.StateMachine.ChangeState(PlayerStateEnum.Idle);
        player.transform.position = GetRespawnPos();
    }

    private Vector3 GetRespawnPos()
    {
        Vector3 camPos = _cam.transform.position;
        float camHalfHeight = _cam.m_Lens.OrthographicSize;
        float camHalfWidth = _cam.m_Lens.Aspect * camHalfHeight;

        while (true)
        {
            int idx = Random.Range(0, _playerRespawnTrms.Length);
            Vector3 spawnPos = _playerRespawnTrms[idx].position;

            if (camPos.x - camHalfWidth > spawnPos.x || camPos.x + camHalfWidth < spawnPos.x ||
                camPos.y - camHalfHeight > spawnPos.y || camPos.y + camHalfHeight < spawnPos.y)
                continue;

            return spawnPos;
        }
    }
}