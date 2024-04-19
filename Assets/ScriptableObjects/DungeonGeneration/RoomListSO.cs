using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomListSO : ScriptableObject
{
    [SerializeField] public List<RoomSO> rooms;
    [SerializeField] public GameObject roomPrefab;
    [SerializeField] public GameObject exitPrefab;
    [SerializeField] public GameObject noexitPrefab;
    [SerializeField] public float exitRadius;
}
