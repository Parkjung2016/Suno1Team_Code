using System.Collections;
using System.Collections.Generic;
using ObjectPooling;
using UnityEngine;

public class PoolList : ScriptableObject
{
    [SerializeField] private List<PoolingItemSO> _list;
    public List<PoolingItemSO> GetList() => _list;
}