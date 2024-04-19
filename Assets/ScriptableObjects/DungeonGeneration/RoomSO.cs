using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomSO : ScriptableObject
{
    [SerializeField] public Vector3 prefabOffset;
    [SerializeField] public GameObject roomPrefab;
    [SerializeField] public bool startingRoom;
    [SerializeField] public bool treasureRoom;
    [SerializeField] public List<Exits> roomExits;
}

public enum Exits
{
    NORTH, SOUTH, WEST, EAST
}
