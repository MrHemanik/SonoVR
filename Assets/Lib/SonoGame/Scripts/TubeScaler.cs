using UnityEngine;

[ExecuteAlways]
public class TubeScaler : MonoBehaviour
{
    public Mesh templateMesh;
    public GameObject tubeGo;

    /// <summary>
    /// Normalized maximum distance (0.5f for circle)
    /// </summary>
    /// [Range(0,1)]
    public float vertexMaxDistance = 0.5f;

    /// <summary>
    /// Normalized edge width: factor applied to <see cref="vertexMaxDistance"/>
    /// </summary>
    [Range(0, 1)]
    public float edgeWidthNorm = 0.1f;

    /// <summary>
    /// Vertices below this threshold are modified [mesh specific]
    /// </summary>
    private readonly float vertexDistanceThreshold = 0.3f;
    private Mesh tubeMesh;

    private void Awake()
    {
        tubeMesh = Instantiate(templateMesh);
        SetEdgeWidthNorm(edgeWidthNorm);
        UpdateMesh();
    }

    /// <summary>
    /// Set normalized edge width 
    /// </summary>
    /// <param name="width">width normalized to tube width</param>
    public void SetEdgeWidthNorm(float width)
    {
        if (tubeMesh == null)
        {
            return;
        }

        float innerEdgeDistance = Mathf.Clamp01(1.0f - width);

        var vert = tubeMesh.vertices;
        for (int i = 0; i < vert.Length; i++)
        {
            Vector2 vXZ = new Vector2(templateMesh.vertices[i].x, templateMesh.vertices[i].z);
            float len = vXZ.magnitude;

            if (len < vertexDistanceThreshold)
            {
                Vector2 vNorm = vXZ * (1.0f / len);
                Vector2 inner = vNorm * vertexMaxDistance * innerEdgeDistance;
                vert[i].x = inner.x;
                vert[i].z = inner.y;
                //Debug.Log(inner.magnitude+" / "+vert[i].ToString("F3"));
            }

            tubeMesh.vertices = vert;
        }

        tubeMesh.RecalculateBounds();
        tubeMesh.RecalculateNormals();

        edgeWidthNorm = width;
    }

    public void UpdateMesh()
    {
        if (tubeMesh != null)
        {
            tubeGo.GetComponent<MeshFilter>().mesh = tubeMesh;
        }
    }

    private void OnDrawGizmos()
    {
        UpdateMesh();
    }

    private void OnValidate()
    {
        SetEdgeWidthNorm(edgeWidthNorm);
    }
}
