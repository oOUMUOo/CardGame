using UnityEngine;

public class MapLineView : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    private void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    public void Setup(Vector3 startPosition, Vector3 endPosition, int pointsCount, float offsetFromNodes)
    {
        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.positionCount = pointsCount;

        Vector3 direction = (endPosition - startPosition).normalized;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

        for (int i = 0; i < pointsCount; i++)
        {
            float t = i / (float)(pointsCount - 1);
            Vector3 point = Vector3.Lerp(startPosition, endPosition, t);

            float curveAmount = Mathf.Sin(t * Mathf.PI) * offsetFromNodes;
            point += perpendicular * curveAmount;

            lineRenderer.SetPosition(i, point);
        }
    }

    public void SetVisited(bool visited)
    {
        // Можно добавить изменение цвета линии при необходимости.
    }
}
