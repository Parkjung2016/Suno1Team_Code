using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct EquipmentSocket
{
    public SocketType socketType;
    public Transform socketTransform;
}

public class PlayerEquipment : NetworkBehaviour
{
    private Dictionary<SocketType, Equipment> _currentEquipments = new();


    [SerializeField] private List<EquipmentSocket> _socketDictionary;

    private Player _owner;

    public void Init(Player owner)
    {
        _owner = owner;
    }

    public void AddItemToInventory(int id)
    {
        if (!IsOwner) return;
        AddItemToEquipmentServerRpc(id);
    }

    [ServerRpc]
    private void AddItemToEquipmentServerRpc(int id)
    {
        Equipment item = GameManager.Instance.equipmentList.equipmentList.Find(x => x.id == id);
        Transform socket = _socketDictionary.Find(x => x.socketType == item.socketType).socketTransform;
        Equipment equipment = Instantiate(item, socket);
        equipment.Init(_owner);
        _currentEquipments.Add(item.socketType, equipment);

        AddItemToEquipmentClientRpc(id);
    }

    [ClientRpc]
    private void AddItemToEquipmentClientRpc(int id)
    {
        if (IsServer) return;
        Equipment item = GameManager.Instance.equipmentList.equipmentList.Find(x => x.id == id);
        Transform socket = _socketDictionary.Find(x => x.socketType == item.socketType).socketTransform;
        Equipment equipment = Instantiate(item, socket);
        equipment.Init(_owner);
        _currentEquipments.Add(item.socketType, equipment);
    }

    public T GetEquipment<T>(SocketType socketType) where T : Equipment
    {
        Type t = typeof(T);
        if (_currentEquipments.TryGetValue(socketType, out Equipment equipment))
        {
            return equipment as T;
        }

        return null;
    }
}