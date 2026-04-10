using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Сохранение прогресса карты в PlayerPrefs. 
/// </summary>
public class MapSaveSystem : MonoBehaviour
{
    private const string SaveKey = "MapSaveData";

    public static bool TryLoad(out MapSaveData data)
    {
        data = null;

        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return false;
        }

        string json = PlayerPrefs.GetString(SaveKey);
        data = JsonUtility.FromJson<MapSaveData>(json);
        return data != null;
    }

    public static void SaveMap(Map map, MapNode standingNode, MapNode pendingNode)
    {
        if (map == null)
        {
            return;
        }

        MapSaveData saveData = new MapSaveData
        {
            GenerationSeed = map.GenerationSeed,
            Orientation = map.Data.Orientation,
            CompletedNodes = new List<MapNodeSaveData>()
        };

        if (standingNode != null)
        {
            saveData.CurrentNodeLayer = GetNodeLayerIndex(map, standingNode);
            saveData.CurrentNodeIndex = GetNodeIndexInLayer(map, standingNode);
        }
        else
        {
            saveData.CurrentNodeLayer = -1;
            saveData.CurrentNodeIndex = -1;
        }

        if (pendingNode != null)
        {
            saveData.PendingEncounterLayer = GetNodeLayerIndex(map, pendingNode);
            saveData.PendingEncounterIndex = GetNodeIndexInLayer(map, pendingNode);
            saveData.PendingNodeType = pendingNode.NodeType;
        }
        else
        {
            saveData.PendingEncounterLayer = -1;
            saveData.PendingEncounterIndex = -1;
        }

        foreach (var layer in map.Layers)
        {
            foreach (var node in layer)
            {
                if (!node.IsVisited)
                {
                    continue;
                }

                MapNodeSaveData nodeSaveData = new MapNodeSaveData
                {
                    LayerIndex = GetNodeLayerIndex(map, node),
                    NodeIndex = GetNodeIndexInLayer(map, node),
                    NodeType = node.NodeType
                };
                saveData.CompletedNodes.Add(nodeSaveData);
            }
        }

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
    }

    public static void ClearSave()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Вызвать после успешного прохождения узла (из сцены боя/события перед загрузкой карты).
    /// </summary>
    public static void RecordEncounterCompleted()
    {
        if (!TryLoad(out MapSaveData data))
        {
            return;
        }

        if (data.PendingEncounterLayer < 0 || data.PendingEncounterIndex < 0)
        {
            return;
        }

        data.CompletedNodes ??= new List<MapNodeSaveData>();

        bool exists = false;
        foreach (var entry in data.CompletedNodes)
        {
            if (entry.LayerIndex == data.PendingEncounterLayer && entry.NodeIndex == data.PendingEncounterIndex)
            {
                exists = true;
                break;
            }
        }

        if (!exists)
        {
            data.CompletedNodes.Add(new MapNodeSaveData
            {
                LayerIndex = data.PendingEncounterLayer,
                NodeIndex = data.PendingEncounterIndex,
                NodeType = data.PendingNodeType
            });
        }

        data.CurrentNodeLayer = data.PendingEncounterLayer;
        data.CurrentNodeIndex = data.PendingEncounterIndex;
        data.PendingEncounterLayer = -1;
        data.PendingEncounterIndex = -1;

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    private static int GetNodeLayerIndex(Map map, MapNode node)
    {
        for (int i = 0; i < map.Layers.Count; i++)
        {
            if (map.Layers[i].Contains(node))
            {
                return i;
            }
        }

        return -1;
    }

    private static int GetNodeIndexInLayer(Map map, MapNode node)
    {
        int layerIndex = GetNodeLayerIndex(map, node);
        if (layerIndex >= 0)
        {
            return map.Layers[layerIndex].IndexOf(node);
        }

        return -1;
    }
}

[System.Serializable]
public class MapSaveData
{
    public int GenerationSeed;
    public MapOrientation Orientation;
    public int CurrentNodeLayer = -1;
    public int CurrentNodeIndex = -1;
    public int PendingEncounterLayer = -1;
    public int PendingEncounterIndex = -1;
    public MapNodeType PendingNodeType;
    public List<MapNodeSaveData> CompletedNodes;
}

[System.Serializable]
public class MapNodeSaveData
{
    public int LayerIndex;
    public int NodeIndex;
    public MapNodeType NodeType;
}
