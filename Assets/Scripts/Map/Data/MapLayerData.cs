using System;
using UnityEngine;

[Serializable]
public class MapLayerData
{
    [SerializeField] public MapNodeType NodeType;
    [SerializeField] public float DistanceFromPreviousLayer;
    [SerializeField] public float NodesFarApartDistance;
    [SerializeField] public float RandomizedNodesProbability;
    [SerializeField] public bool RandomizeNodeType;
}