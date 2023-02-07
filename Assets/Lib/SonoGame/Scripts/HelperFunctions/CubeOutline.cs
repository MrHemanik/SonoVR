using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class CubeOutline : MonoBehaviour
{
    public CubeOutlineConfig config;
    private Material lineMaterial;
    private int layerMask;
    private bool enableLines; /// set with <see cref="OnBeginCameraRendering(ScriptableRenderContext, Camera)"/> 



    private void OnEnable()
    {
        CreateLineMaterial();

        //layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
        layerMask = LayerMask.GetMask("Indicators3D");
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    private void Start()
    {
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        enableLines = false;

#if UNITY_EDITOR
        if (config == null || config.lineColor == null || config.line == null)
        {
            return;
        }
#endif

        enableLines = (camera.cullingMask & layerMask) > 0;
    }

    private void OnRenderObject()
    {

        if (enableLines)
        {
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix * Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(transform.rotation), Vector3.one));  // Set transformation matrix for drawing to match our transform

            DrawLines(config.normStart, config.normEnd, step: 1, stepDistance: 0.0003f, layer: 0);

            GL.PopMatrix();
        }
    }

    private void DrawLines(float outerDist, float innerDist, float step, float stepDistance, float layer)
    {
        lineMaterial.SetPass(0);

        outerDist -= step * stepDistance;

        float offsetY;
        float offsetX;
        float offsetZ;

        // Begin Draw lines 
        GL.Begin(GL.LINES);

        if (config.line[0])
        {
            offsetY = outerDist; // +Z/+Y vorne oben
            GL.Color(config.lineColor[0]);
            GL.Vertex3(-outerDist, offsetY, outerDist); // P1 Line1 top from x: -outerDist to x: -innerDist
            GL.Vertex3(-innerDist, offsetY, outerDist); // P2
            GL.Vertex3(+innerDist, offsetY, outerDist); // P1 Line2 top from x: +outerDist to x: +innerDist
            GL.Vertex3(+outerDist, offsetY, outerDist); // P2 
        }

        if (config.line[1]) // +Z/+Y vorne unten
        {
            offsetY = -outerDist;
            GL.Color(config.lineColor[1]);
            GL.Vertex3(-outerDist, offsetY, outerDist); // P1 Line1 top from x: -outerDist to x: -innerDist
            GL.Vertex3(-innerDist, offsetY, outerDist); // P2
            GL.Vertex3(+innerDist, offsetY, outerDist); // P1 Line2 top from x: +outerDist to x: +innerDist
            GL.Vertex3(+outerDist, offsetY, outerDist); // P2 
        }

        if (config.line[2])
        {
            GL.Color(config.lineColor[2]);
            offsetY = outerDist; // +X/+Y
            GL.Vertex3(outerDist, offsetY, outerDist); // rechts oben (horizontal)
            GL.Vertex3(outerDist, offsetY, innerDist);
            GL.Vertex3(outerDist, offsetY, -outerDist);
            GL.Vertex3(outerDist, offsetY, -innerDist);
        }

        if (config.line[3])
        {
            GL.Color(config.lineColor[3]);
            offsetY = -outerDist; //+X/-Y
            GL.Vertex3(outerDist, offsetY, outerDist); // rechts unten
            GL.Vertex3(outerDist, offsetY, innerDist);
            GL.Vertex3(outerDist, offsetY, -outerDist);
            GL.Vertex3(outerDist, offsetY, -innerDist);
        }


        if (config.line[4])
        {
            offsetY = outerDist; // -X/+Y
            GL.Color(config.lineColor[4]);
            GL.Vertex3(-outerDist, offsetY, +outerDist); // links oben
            GL.Vertex3(-outerDist, offsetY, +innerDist);
            GL.Vertex3(-outerDist, offsetY, -innerDist);
            GL.Vertex3(-outerDist, offsetY, -outerDist);
        }

        if (config.line[5])
        {
            offsetY = -outerDist; // -X/-Y
            GL.Color(config.lineColor[5]);
            GL.Vertex3(-outerDist, offsetY, +outerDist); // links unten
            GL.Vertex3(-outerDist, offsetY, +innerDist);
            GL.Vertex3(-outerDist, offsetY, -innerDist);
            GL.Vertex3(-outerDist, offsetY, -outerDist);
        }


        if (config.line[6])
        {
            offsetY = outerDist; // -Z/+Y
            GL.Color(config.lineColor[6]);
            GL.Vertex3(-outerDist, offsetY, -outerDist); // hinten oben
            GL.Vertex3(-innerDist, offsetY, -outerDist);
            GL.Vertex3(+innerDist, offsetY, -outerDist);
            GL.Vertex3(+outerDist, offsetY, -outerDist);
        }

        if (config.line[7])
        {
            offsetY = -outerDist; // -Z/+Y
            GL.Color(config.lineColor[7]);
            GL.Vertex3(-outerDist, offsetY, -outerDist); // hinten unten
            GL.Vertex3(-innerDist, offsetY, -outerDist);
            GL.Vertex3(+innerDist, offsetY, -outerDist);
            GL.Vertex3(+outerDist, offsetY, -outerDist);
        }


        if (config.line[8])
        {
            offsetX = outerDist; // +Z/+X
            GL.Color(config.lineColor[8]);
            GL.Vertex3(offsetX, -outerDist, outerDist); // vertikal vorne rechts
            GL.Vertex3(offsetX, -innerDist, outerDist);
            GL.Vertex3(offsetX, +innerDist, outerDist);
            GL.Vertex3(offsetX, +outerDist, outerDist);
        }

        if (config.line[9])
        {
            offsetX = -outerDist;
            GL.Color(config.lineColor[9]); // +Z/-X
            GL.Vertex3(offsetX, -outerDist, outerDist); // vertikal vorne links
            GL.Vertex3(offsetX, -innerDist, outerDist);
            GL.Vertex3(offsetX, +innerDist, outerDist);
            GL.Vertex3(offsetX, +outerDist, outerDist);
        }


        if (config.line[10])
        {
            offsetX = outerDist; // -Z y/+X
            GL.Color(config.lineColor[10]);
            GL.Vertex3(offsetX, -outerDist, -outerDist); // vertikal hinten rechts 
            GL.Vertex3(offsetX, -innerDist, -outerDist);
            GL.Vertex3(offsetX, +innerDist, -outerDist);
            GL.Vertex3(offsetX, +outerDist, -outerDist);
        }

        if (config.line[11])
        {
            offsetX = -outerDist; // -Z/-X
            GL.Color(config.lineColor[11]);
            GL.Vertex3(offsetX, -outerDist, -outerDist); // vertikal hinten links
            GL.Vertex3(offsetX, -innerDist, -outerDist);
            GL.Vertex3(offsetX, +innerDist, -outerDist);
            GL.Vertex3(offsetX, +outerDist, -outerDist);
        }

        GL.End();


        // Begin Draw quads
        GL.Begin(GL.QUADS);

        if (config.quadBottom)
        {
            offsetY = -outerDist;
            GL.Color(config.floorColor);

            GL.Vertex3(-outerDist, offsetY, -outerDist); // -X/-Z
            GL.Vertex3(+outerDist, offsetY, -outerDist); // +X/-Z
            GL.Vertex3(+outerDist, offsetY, +outerDist); // +X/+Z
            GL.Vertex3(-outerDist, offsetY, +outerDist); // -X/+Z
        }

        if (config.quadBack)
        {
            offsetZ = outerDist;
            GL.Color(config.floorColor);

            GL.Vertex3(-outerDist, -outerDist, offsetZ); // -X/-Y
            GL.Vertex3(+outerDist, -outerDist, offsetZ); // +X/-Y
            GL.Vertex3(+outerDist, outerDist, offsetZ); // +X/+Y
            GL.Vertex3(-outerDist, outerDist, offsetZ); // -X/+Y
        }

        if (config.hint) // || config.quadFront
        {
            offsetZ = -outerDist;
            float hintSize = config.hintSize;
            float hintHoriz = config.hintLeft ? -1 : 1;
            float hintVert = config.hintTop ? 1 : -1;

            GL.Color(config.hintColor);

            GL.Vertex3(hintHoriz * outerDist + hintHoriz * -hintSize, hintVert * outerDist, offsetZ); // -X/+Y
            GL.Vertex3(hintHoriz * outerDist, hintVert * outerDist, offsetZ); // +X/+Y
            GL.Vertex3(hintHoriz * outerDist, hintVert * outerDist + hintVert * -hintSize, offsetZ); // +X/-Y
            GL.Vertex3(hintHoriz * outerDist + hintHoriz * -hintSize, hintVert * outerDist + hintVert * -hintSize, offsetZ); // -X/-Y
        }

        GL.End();
    }

    private void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            //lineMaterial.SetInt("_ZWrite", 0);

            lineMaterial.SetPass(0);
        }
    }


}