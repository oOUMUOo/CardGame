using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSystem : Singleton<MapSystem>
{
    [SerializeField] private MapView mapView;
    [SerializeField] private MapSceneRoutesData sceneRoutes;

    private Map currentMap;
    private MapNode currentNode;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<GenerateMapGA>(GenerateMapPerformer);
        ActionSystem.AttachPerformer<EnterNodeGA>(EnterNodePerformer);
        ActionSystem.AttachPerformer<SetNodeAvailabilityGA>(SetNodeAvailabilityPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<GenerateMapGA>();
        ActionSystem.DetachPerformer<EnterNodeGA>();
        ActionSystem.DetachPerformer<SetNodeAvailabilityGA>();
    }

    public Map CurrentMap => currentMap;
    public MapNode CurrentNode => currentNode;

    private IEnumerator GenerateMapPerformer(GenerateMapGA generateMapGA)
    {
        Random.InitState(generateMapGA.GenerationSeed);
        currentMap = new Map(generateMapGA.MapData, generateMapGA.GenerationSeed);
        MapGenerator.Generate(currentMap);
        mapView.Setup(currentMap);

        if (generateMapGA.RestoreFromSave != null)
        {
            ApplySaveData(generateMapGA.RestoreFromSave);
        }
        else
        {
            currentNode = null;
            PushAvailabilityToView();
        }

        MapSaveSystem.SaveMap(currentMap, currentNode, null);
        yield return null;
    }

    private IEnumerator EnterNodePerformer(EnterNodeGA enterNodeGA)
    {
        MapNode targetNode = enterNodeGA.MapNode;
        if (targetNode == null || currentMap == null)
        {
            yield break;
        }

        List<MapNode> allowed = GetAvailableNodes();
        if (!allowed.Contains(targetNode))
        {
            yield break;
        }

        MapSaveSystem.SaveMap(currentMap, currentNode, targetNode);
        LoadSceneForNode(targetNode);
        yield return null;
    }

    private IEnumerator SetNodeAvailabilityPerformer(SetNodeAvailabilityGA setNodeAvailabilityGA)
    {
        mapView.UpdateNodeAvailability(setNodeAvailabilityGA.AvailableNodes);
        yield return null;
    }

    public void ApplySaveData(MapSaveData saveData)
    {
        if (currentMap == null || saveData == null)
        {
            return;
        }

        if (saveData.CompletedNodes != null)
        {
            foreach (var entry in saveData.CompletedNodes)
            {
                MapNode node = currentMap.GetNode(entry.LayerIndex, entry.NodeIndex);
                node?.SetVisited(true);
            }
        }

        if (saveData.CurrentNodeLayer >= 0)
        {
            currentNode = currentMap.GetNode(saveData.CurrentNodeLayer, saveData.CurrentNodeIndex);
        }
        else
        {
            currentNode = null;
        }

        PushAvailabilityToView();
    }

    public List<MapNode> GetAvailableNodes()
    {
        List<MapNode> availableNodes = new List<MapNode>();

        if (currentMap == null || currentMap.Layers.Count == 0)
        {
            return availableNodes;
        }

        if (currentNode == null)
        {
            foreach (var node in currentMap.Layers[0])
            {
                if (!node.IsVisited)
                {
                    availableNodes.Add(node);
                }
            }

            return availableNodes;
        }

        int nextLayerIndex = currentNode.LayerIndex + 1;
        if (nextLayerIndex >= currentMap.Layers.Count)
        {
            return availableNodes;
        }

        foreach (var connected in currentNode.ConnectedNodes)
        {
            if (connected.LayerIndex != nextLayerIndex)
            {
                continue;
            }

            if (connected.IsVisited)
            {
                continue;
            }

            availableNodes.Add(connected);
        }

        return availableNodes;
    }

    private void PushAvailabilityToView()
    {
        List<MapNode> availableNodes = GetAvailableNodes();
        mapView.UpdateNodeAvailability(availableNodes);
        mapView.RefreshAllNodeVisuals(currentNode);
    }

    private void LoadSceneForNode(MapNode mapNode)
    {
        if (sceneRoutes == null)
        {
            Debug.LogError("MapSystem: assign MapSceneRoutesData.");
            return;
        }

        string sceneName = mapNode.NodeType switch
        {
            MapNodeType.MINOR_ENEMY => sceneRoutes.CombatSceneName,
            MapNodeType.ELITE_ENEMY => sceneRoutes.EliteCombatSceneName,
            MapNodeType.BOSS => sceneRoutes.BossCombatSceneName,
            MapNodeType.REST_SITE => sceneRoutes.RestSceneName,
            MapNodeType.TREASURE => sceneRoutes.TreasureSceneName,
            MapNodeType.MYSTERY => sceneRoutes.EventSceneName,
            MapNodeType.MERCHANT => sceneRoutes.MerchantSceneName,
            _ => sceneRoutes.CombatSceneName
        };

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"MapSystem: scene name is empty for node type {mapNode.NodeType}.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
