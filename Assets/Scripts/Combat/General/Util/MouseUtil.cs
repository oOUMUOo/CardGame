using UnityEngine;

public static class MouseUtil
{
    private static Camera cachedCamera;

    private static Camera GetActiveCamera()
    {
        if (cachedCamera != null)
        {
            return cachedCamera;
        }

        cachedCamera = Camera.main;
        if (cachedCamera == null)
        {
            cachedCamera = Object.FindFirstObjectByType<Camera>();
        }

        return cachedCamera;
    }

    public static Vector3 GetMousePositionInWorldSpace(float zValue = 0f)
    {
        Camera camera = GetActiveCamera();
        if (camera == null)
        {
            return Vector3.zero;
        }

        Plane dragPlane = new(camera.transform.forward, new Vector3(0f, 0f, zValue));
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}
