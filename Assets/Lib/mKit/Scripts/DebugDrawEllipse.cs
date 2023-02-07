using UnityEngine;

/// <summary>
/// Ellipse Gizmo.
/// Source: https://forum.unity.com/threads/solved-debug-drawline-circle-ellipse-and-rotate-locally-with-offset.331397/
/// Added z dimension
/// </summary>
public class DebugDrawEllipse : MonoBehaviour
{

    public Vector3 ellipsePlane;

    public int Segments = 32;
    public Color Color = Color.blue;
    public float radiusHoriz = 2;
    public float radiusVertical = 1;

    private void Start()
    {

    }

    private void OnDrawGizmosSelected()
    {
        if (enabled)
        {
            Quaternion q = Quaternion.Euler(ellipsePlane);

            DrawEllipse(transform.position, q * transform.forward, q * transform.up, radiusHoriz, radiusVertical, Segments, Color);
        }

    }

    private static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusH, float radiusV, int segments, Color color, float duration = 0)
    {
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusH;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusV;

            if (i > 0)
            {
                Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }
}
