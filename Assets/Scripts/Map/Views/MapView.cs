using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MapView : MonoBehaviour
{
    [Header("Node Settings")]
    [SerializeField] private GameObject nodePrefab;

    [Header("Line Settings")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] [Range(1, 20)] private int linePointsCount = 10;
    [SerializeField] private float offsetFromNodes = 0.5f;

    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private float dragThreshold = 5f;
    [SerializeField] private float edgePadding = 2f;

    private Map currentMap;
    private readonly List<MapNodeView> nodeViews = new();
    private readonly List<MapLineView> lineViews = new();
    private Vector3 currentPosition;
    private Camera mapCamera;

    private bool isDragging;
    private Vector2 startMousePosition;
    private Vector3 startDragPosition;
    private bool hasDragged;

    private float mapMinX = float.MaxValue;
    private float mapMaxX = float.MinValue;

    private void Awake()
    {
        if (mapCamera == null)
        {
            mapCamera = Camera.main;
        }

        currentPosition = Vector3.zero;
    }

    private void Update()
    {
        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            hasDragged = false;
            startMousePosition = Input.mousePosition;
            startDragPosition = transform.localPosition;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            float distance = Vector2.Distance(startMousePosition, currentMousePosition);

            if (distance > dragThreshold)
            {
                hasDragged = true;

                Vector3 mouseDelta = currentMousePosition - startMousePosition;

                if (mapCamera != null)
                {
                    float orthoSize = mapCamera.orthographicSize;
                    float worldHeight = orthoSize * 2f;
                    float worldWidth = worldHeight * mapCamera.aspect;

                    Vector3 moveDelta = new Vector3(
                        mouseDelta.x * worldWidth / Screen.width,
                        0,
                        0
                    ) * scrollSpeed;

                    Vector3 targetPosition = startDragPosition + moveDelta;
                    targetPosition.x = ClampMapX(targetPosition.x);

                    transform.localPosition = targetPosition;
                    currentPosition = targetPosition;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!hasDragged)
            {
                CheckNodeClick(Input.mousePosition);
            }

            isDragging = false;
            hasDragged = false;
        }

        float horizontal = Input.GetAxis("Horizontal");

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            Vector3 moveDelta = new Vector3(-horizontal, 0, 0) * scrollSpeed * 0.5f;
            MovePosition(moveDelta);
        }
    }

    private float ClampMapX(float x)
    {
        if (mapCamera == null)
        {
            return x;
        }

        float camHalfWidth = mapCamera.orthographicSize * mapCamera.aspect;
        float screenLeft = -camHalfWidth + edgePadding;
        float screenRight = camHalfWidth - edgePadding;

        float limitFromLeft = screenLeft - mapMinX;
        float limitFromRight = screenRight - mapMaxX;

        float minAllowed = Mathf.Min(limitFromLeft, limitFromRight);
        float maxAllowed = Mathf.Max(limitFromLeft, limitFromRight);

        return Mathf.Clamp(x, minAllowed, maxAllowed);
    }

    private void CheckNodeClick(Vector3 screenPosition)
    {
        if (mapCamera == null)
        {
            return;
        }

        Ray ray = mapCamera.ScreenPointToRay(screenPosition);
        MapNodeView nodeView = null;

        // 2D: BoxCollider2D / CircleCollider2D (Physics.Raycast их не видит)
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
        if (hit2D.collider != null)
        {
            nodeView = hit2D.collider.GetComponent<MapNodeView>()
                       ?? hit2D.collider.GetComponentInParent<MapNodeView>();
        }

        // 3D: BoxCollider и т.д.
        if (nodeView == null && Physics.Raycast(ray, out RaycastHit hit3D, 500f))
        {
            nodeView = hit3D.collider.GetComponent<MapNodeView>()
                       ?? hit3D.collider.GetComponentInParent<MapNodeView>();
        }

        if (nodeView == null || ActionSystem.Instance == null)
        {
            return;
        }

        if (nodeView.MapNode != null && nodeView.MapNode.IsAvailable)
        {
            EnterNodeGA enterNodeGA = new(nodeView.MapNode);
            ActionSystem.Instance.Perform(enterNodeGA);
        }
    }

    public void Setup(Map map)
    {
        currentMap = map;
        ClearViews();

        CreateNodes();
        CreateLines();

        CalculateMapBounds();
        AlignToLeftEdge();
    }

    private void CalculateMapBounds()
    {
        mapMinX = float.MaxValue;
        mapMaxX = float.MinValue;

        foreach (var nodeView in nodeViews)
        {
            float x = nodeView.transform.localPosition.x;
            if (x < mapMinX)
            {
                mapMinX = x;
            }

            if (x > mapMaxX)
            {
                mapMaxX = x;
            }
        }
    }

    private void CreateNodes()
    {
        foreach (var layer in currentMap.Layers)
        {
            foreach (var node in layer)
            {
                GameObject nodeGO = Instantiate(nodePrefab, transform);
                MapNodeView nodeView = nodeGO.GetComponent<MapNodeView>();

                if (nodeView == null)
                {
                    nodeView = nodeGO.AddComponent<MapNodeView>();
                }

                nodeGO.transform.localPosition = node.Position;
                nodeView.Setup(node);
                nodeViews.Add(nodeView);
            }
        }
    }

    private void CreateLines()
    {
        HashSet<(MapNode, MapNode)> createdConnections = new();

        foreach (var layer in currentMap.Layers)
        {
            foreach (var node in layer)
            {
                foreach (var connectedNode in node.ConnectedNodes)
                {
                    var connection = (node, connectedNode);
                    var reverseConnection = (connectedNode, node);

                    if (!createdConnections.Contains(connection) && !createdConnections.Contains(reverseConnection))
                    {
                        if (linePrefab != null)
                        {
                            GameObject lineGO = Instantiate(linePrefab, transform);
                            MapLineView lineView = lineGO.GetComponent<MapLineView>();

                            if (lineView != null)
                            {
                                lineView.Setup(node.Position, connectedNode.Position, linePointsCount, offsetFromNodes);
                                lineViews.Add(lineView);
                            }
                        }

                        createdConnections.Add(connection);
                    }
                }
            }
        }
    }

    private void ClearViews()
    {
        foreach (var nodeView in nodeViews)
        {
            if (nodeView != null)
            {
                Destroy(nodeView.gameObject);
            }
        }

        nodeViews.Clear();

        foreach (var lineView in lineViews)
        {
            if (lineView != null)
            {
                Destroy(lineView.gameObject);
            }
        }

        lineViews.Clear();
    }

    public void UpdateNodeAvailability(List<MapNode> availableNodes)
    {
        foreach (var nodeView in nodeViews)
        {
            if (nodeView != null && nodeView.MapNode != null)
            {
                bool isAvailable = availableNodes.Contains(nodeView.MapNode);
                nodeView.SetAvailable(isAvailable);
            }
        }
    }

    public void HighlightCurrentNode(MapNode node)
    {
        foreach (var nodeView in nodeViews)
        {
            if (nodeView != null && nodeView.MapNode != null)
            {
                nodeView.SetCurrent(nodeView.MapNode == node);
            }
        }
    }

    public void RefreshAllNodeVisuals(MapNode standingNode)
    {
        foreach (var nodeView in nodeViews)
        {
            if (nodeView == null || nodeView.MapNode == null)
            {
                continue;
            }

            nodeView.SetVisited(nodeView.MapNode.IsVisited);
            nodeView.SetCurrent(nodeView.MapNode == standingNode);
        }
    }

    public void MovePosition(Vector3 delta)
    {
        Vector3 targetPos = currentPosition + new Vector3(delta.x, 0, 0);
        targetPos.x = ClampMapX(targetPos.x);

        currentPosition = targetPos;
        transform.localPosition = currentPosition;
    }

    public void SetPosition(Vector3 position)
    {
        position.x = ClampMapX(position.x);
        currentPosition = position;
        transform.localPosition = currentPosition;
    }

    public Vector3 GetPosition()
    {
        return currentPosition;
    }

    private void AlignToLeftEdge()
    {
        if (currentMap == null || currentMap.LayerCount == 0 || nodeViews.Count == 0)
        {
            return;
        }

        float camHalfWidth = mapCamera.orthographicSize * mapCamera.aspect;
        float screenLeft = -camHalfWidth + edgePadding;

        float targetX = screenLeft - mapMinX;

        currentPosition = new Vector3(ClampMapX(targetX), 0, 0);
        transform.localPosition = currentPosition;
    }

    public void FitToScreen()
    {
        if (currentMap == null || nodeViews.Count == 0)
        {
            return;
        }

        Bounds bounds = new Bounds(nodeViews[0].transform.position, Vector3.zero);

        foreach (var nodeView in nodeViews)
        {
            bounds.Encapsulate(nodeView.transform.position);
        }

        if (mapCamera == null)
        {
            mapCamera = Camera.main;
        }

        if (mapCamera != null)
        {
            float padding = 2f;
            float boundsWidth = bounds.size.x + padding * 2;
            float boundsHeight = bounds.size.y + padding * 2;

            float orthoSizeHorizontal = boundsWidth / (2f * mapCamera.aspect);
            float orthoSizeVertical = boundsHeight / 2f;

            float targetOrthoSize = Mathf.Max(orthoSizeHorizontal, orthoSizeVertical);

            mapCamera.DOOrthoSize(targetOrthoSize, 0.5f);
        }

        CalculateMapBounds();
        AlignToLeftEdge();
    }
}
