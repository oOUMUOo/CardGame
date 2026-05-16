using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public static void Generate(Map map)
    {
        MapData data = map.Data;
        
        int preBossLayers = Random.Range(data.PreBossNodesMin, data.PreBossNodesMax + 1);
        int totalLayers = preBossLayers + 1;
        
        Debug.Log($"Generating map with {totalLayers} layers");
        
        // Шаг 1: Создаём все слои и узлы
        for (int layerIndex = 0; layerIndex < totalLayers; layerIndex++)
        {
            List<MapNode> layer = new();
            
            MapLayerData layerData;
            if (layerIndex < data.Layers.Count)
            {
                layerData = data.Layers[layerIndex];
            }
            else
            {
                layerData = data.Layers[data.Layers.Count - 1];
            }
            
            int nodeCount;
            if (layerIndex == 0)
            {
                nodeCount = Random.Range(data.StartingNodesMin, data.StartingNodesMax + 1);
            }
            else if (layerIndex == totalLayers - 1)
            {
                nodeCount = 1;
            }
            else
            {
                nodeCount = Random.Range(2, 5);
            }
            
            for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                NodeBlueprintData blueprint = GetNodeBlueprint(data, layerData, layerIndex);
                MapNode node = new MapNode(blueprint);
                node.SetLayerIndex(layerIndex);

                Vector3 position = CalculateNodePosition(map, layerIndex, nodeIndex, nodeCount, layerData);
                node.Position = position;
                
                layer.Add(node);
            }
            
            map.AddLayer(layer);
        }
        
        // Шаг 2: Соединяем слои (гарантируем двустороннюю связность)
        ConnectAllLayers(map);
        
        // Шаг 3: Удаляем пересекающиеся линии (безопасно)
        EliminateCrossNodes(map);
        
        // Шаг 4: Проверяем, что нет тупиков
        EnsureNoDeadEnds(map);
        
        // Шаг 5: Проверяем, что все узлы достижимы
        EnsureAllNodesReachable(map);
    }
    
    private static NodeBlueprintData GetNodeBlueprint(MapData data, MapLayerData layerData, int layerIndex)
    {
        if (layerIndex == data.Layers.Count - 1)
        {
            return GetBlueprintByType(data, MapNodeType.BOSS);
        }
        
        if (Random.value > layerData.RandomizedNodesProbability)
        {
            return GetBlueprintByType(data, layerData.NodeType);
        }
        
        List<NodeBlueprintData> availableBlueprints = new();
        foreach (var blueprint in data.NodeBlueprints)
        {
            if (blueprint.NodeType != MapNodeType.BOSS)
            {
                availableBlueprints.Add(blueprint);
            }
        }
        
        return availableBlueprints[Random.Range(0, availableBlueprints.Count)];
    }
    
    private static NodeBlueprintData GetBlueprintByType(MapData data, MapNodeType type)
    {
        foreach (var blueprint in data.NodeBlueprints)
        {
            if (blueprint.NodeType == type)
            {
                return blueprint;
            }
        }
        return data.NodeBlueprints[0];
    }
    
    private static Vector3 CalculateNodePosition(Map map, int layerIndex, int nodeIndex, int nodeCount, MapLayerData layerData)
    {
        MapData data = map.Data;
        float spacing = 2.8f;
        
        float xOffset = (nodeIndex - (nodeCount - 1) / 2f) * spacing;
        float yOffset = layerIndex * spacing;
        
        yOffset -= 8f;
        
        if (Random.value < data.PositionRandomization)
        {
            xOffset += Random.Range(-spacing * 0.3f, spacing * 0.3f);
        }
        
        return new Vector3(yOffset, xOffset, 0);
    }
    
    // ← ИСПРАВЛЕННАЯ ФУНКЦИЯ: Гарантирует связность в обе стороны
    private static void ConnectAllLayers(Map map)
    {
        for (int i = 0; i < map.LayerCount - 1; i++)
        {
            List<MapNode> currentLayer = map.Layers[i];
            List<MapNode> nextLayer = map.Layers[i + 1];
            
            // ПРИОРИТЕТ 1: Каждый узел следующего слоя должен иметь вход из текущего
            foreach (var nextNode in nextLayer)
            {
                MapNode closestPrevNode = GetClosestNode(nextNode, currentLayer);
                
                // Проверяем, есть ли уже соединение
                bool hasConnection = false;
                foreach (var connected in closestPrevNode.ConnectedNodes)
                {
                    if (connected == nextNode)
                    {
                        hasConnection = true;
                        break;
                    }
                }
                
                if (!hasConnection)
                {
                    closestPrevNode.AddConnection(nextNode);
                    nextNode.AddConnection(closestPrevNode);
                }
            }
            
            // ПРИОРИТЕТ 2: Каждый узел текущего слоя должен иметь выход в следующий
            foreach (var currentNode in currentLayer)
            {
                bool hasForwardConnection = false;
                foreach (var connected in currentNode.ConnectedNodes)
                {
                    if (nextLayer.Contains(connected))
                    {
                        hasForwardConnection = true;
                        break;
                    }
                }
                
                if (!hasForwardConnection)
                {
                    MapNode closestNextNode = GetClosestNode(currentNode, nextLayer);
                    currentNode.AddConnection(closestNextNode);
                    closestNextNode.AddConnection(currentNode);
                }
            }
            
            // ПРИОРИТЕТ 3: Добавляем дополнительные соединения для вариативности
            foreach (var currentNode in currentLayer)
            {
                int extraConnections = Random.Range(0, 2);
                List<MapNode> availableNodes = new List<MapNode>(nextLayer);
                
                // Удаляем уже соединённые
                foreach (var connected in currentNode.ConnectedNodes)
                {
                    if (nextLayer.Contains(connected))
                    {
                        availableNodes.Remove(connected);
                    }
                }
                
                for (int j = 0; j < extraConnections && availableNodes.Count > 0; j++)
                {
                    MapNode selectedNode = availableNodes[Random.Range(0, availableNodes.Count)];
                    currentNode.AddConnection(selectedNode);
                    selectedNode.AddConnection(currentNode);
                    availableNodes.Remove(selectedNode);
                }
            }
        }
    }
    
    private static MapNode GetClosestNode(MapNode node, List<MapNode> candidates)
    {
        MapNode closest = candidates[0];
        float minDistance = float.MaxValue;
        
        foreach (var candidate in candidates)
        {
            float distance = Vector3.Distance(node.Position, candidate.Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = candidate;
            }
        }
        
        return closest;
    }
    
    // ← ИСПРАВЛЕННАЯ ФУНКЦИЯ: Безопасное удаление пересечений
    private static void EliminateCrossNodes(Map map)
    {
        for (int i = 0; i < map.LayerCount - 1; i++)
        {
            List<MapNode> currentLayer = map.Layers[i];
            List<MapNode> nextLayer = map.Layers[i + 1];
            
            List<(MapNode, MapNode)> connectionsToRemove = new();
            
            foreach (var node1 in currentLayer)
            {
                foreach (var node2 in currentLayer)
                {
                    if (node1 == node2) continue;
                    
                    List<MapNode> node1ConnectionsCopy = new List<MapNode>(node1.ConnectedNodes);
                    
                    foreach (var conn1 in node1ConnectionsCopy)
                    {
                        if (!nextLayer.Contains(conn1)) continue;
                        
                        List<MapNode> node2ConnectionsCopy = new List<MapNode>(node2.ConnectedNodes);
                        
                        foreach (var conn2 in node2ConnectionsCopy)
                        {
                            if (conn2 == conn1) continue;
                            if (!nextLayer.Contains(conn2)) continue;
                            
                            if (LinesIntersect(node1.Position, conn1.Position, node2.Position, conn2.Position))
                            {
                                // ← ВАЖНО: Проверяем, не нарушим ли связность
                                bool canRemove = true;
                                
                                // Проверяем node1 - останется ли у него выход вперёд?
                                if (node1.ConnectedNodes.Count <= 1)
                                {
                                    canRemove = false;
                                }
                                
                                // Проверяем conn1 - останется ли у него вход сзади?
                                int incomingConnections = 0;
                                foreach (var prevNode in currentLayer)
                                {
                                    if (prevNode.ConnectedNodes.Contains(conn1))
                                    {
                                        incomingConnections++;
                                    }
                                }
                                if (incomingConnections <= 1)
                                {
                                    canRemove = false;
                                }
                                
                                if (canRemove)
                                {
                                    connectionsToRemove.Add((node1, conn1));
                                }
                            }
                        }
                    }
                }
            }
            
            foreach (var pair in connectionsToRemove)
            {
                pair.Item1.ConnectedNodes.Remove(pair.Item2);
                pair.Item2.ConnectedNodes.Remove(pair.Item1);
            }
        }
    }
    
    private static void EnsureNoDeadEnds(Map map)
    {
        for (int i = 0; i < map.LayerCount - 1; i++)
        {
            List<MapNode> currentLayer = map.Layers[i];
            List<MapNode> nextLayer = map.Layers[i + 1];
            
            foreach (var node in currentLayer)
            {
                bool hasForwardConnection = false;
                foreach (var connected in node.ConnectedNodes)
                {
                    if (nextLayer.Contains(connected))
                    {
                        hasForwardConnection = true;
                        break;
                    }
                }
                
                if (!hasForwardConnection)
                {
                    MapNode closestNode = GetClosestNode(node, nextLayer);
                    node.AddConnection(closestNode);
                    closestNode.AddConnection(node);
                }
            }
        }
    }
    
    // ← НОВАЯ ФУНКЦИЯ: Проверяет, что все узлы достижимы от старта
    private static void EnsureAllNodesReachable(Map map)
    {
        // Для каждого слоя кроме первого
        for (int i = 1; i < map.LayerCount; i++)
        {
            List<MapNode> currentLayer = map.Layers[i];
            List<MapNode> previousLayer = map.Layers[i - 1];
            
            foreach (var node in currentLayer)
            {
                // Проверяем, есть ли вход из предыдущего слоя
                bool hasIncomingConnection = false;
                foreach (var connected in node.ConnectedNodes)
                {
                    if (previousLayer.Contains(connected))
                    {
                        hasIncomingConnection = true;
                        break;
                    }
                }
                
                // Если нет - добавляем соединение с ближайшим узлом предыдущего слоя
                if (!hasIncomingConnection)
                {
                    MapNode closestPrevNode = GetClosestNode(node, previousLayer);
                    closestPrevNode.AddConnection(node);
                    node.AddConnection(closestPrevNode);
                    Debug.Log($"Added incoming connection for node at layer {i}");
                }
            }
        }
    }
    
    private static bool LinesIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float d = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
        if (d == 0) return false;
        
        float uA = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / d;
        float uB = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / d;
        
        return uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1;
    }
}