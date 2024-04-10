using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ObjectSO : ScriptableObject
{
    [SerializeField] private Vector3 offset;
    [SerializeField] string name;
    public GameObject prefab;
}
