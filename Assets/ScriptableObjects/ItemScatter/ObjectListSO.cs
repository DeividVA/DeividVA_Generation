using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ObjectListSO : ScriptableObject
{
    [SerializeField] public List<ObjectSO> objects;
}
