using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ObjectSO : ScriptableObject
{
    [SerializeField] public Vector3 prefabOffset;
    [SerializeField] string name;
    [SerializeField] public GameObject prefab;
}
