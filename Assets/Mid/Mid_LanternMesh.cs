using System.Collections.Generic;
using UnityEngine;

// 등롱(Lantern) 메시
// - 받침: 얇은 n각기둥 (반복문)
// - 몸체: n각기둥 (반복문)
// - 지붕: 원뿔 (반복문)
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_LanternMesh : MonoBehaviour
{
    [Range(3, 32)] public int sides = 8;   
    public float radius = 0.12f;           
    public float height = 0.25f;          
    public float baseThickness = 0.02f;    
    public float roofRadius = 0.2f;       
    public float roofHeight = 0.12f;      

    List<Vector3> vertices;
    List<int> paperTris;  
    List<int> frameTris;  

    bool dirty;

    void OnEnable()
    {
        Build();
    }


    void OnValidate()
    {
        dirty = true;
    }

    void Update()
    {
        if (dirty)
        {
            dirty = false;
            Build();
        }
    }

    void Build()
    {
        vertices = new List<Vector3>();
        paperTris = new List<int>();
        frameTris = new List<int>();

        AddPrism(-baseThickness, 0f, radius * 1.1f, frameTris);

        AddPrism(0f, height, radius, paperTris);

        AddCone(height, height + roofHeight, roofRadius, frameTris);

        Mesh mesh = new Mesh();
        mesh.name = "Lantern";
        mesh.hideFlags = HideFlags.DontSave;
        mesh.SetVertices(vertices);
        mesh.subMeshCount = 2;
        mesh.SetTriangles(paperTris, 0);
        mesh.SetTriangles(frameTris, 1);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }


    Vector3 Ring(float r, float y, int i)
    {
        float angle = 2f * Mathf.PI * i / sides;
        return new Vector3(r * Mathf.Cos(angle), y, r * Mathf.Sin(angle));
    }

    void AddPrism(float y0, float y1, float r, List<int> tris)
    {
        Vector3 bottomCenter = new Vector3(0f, y0, 0f);
        Vector3 topCenter = new Vector3(0f, y1, 0f);

        for (int i = 0; i < sides; i++)
        {
            int j = (i + 1) % sides;   

            Vector3 b0 = Ring(r, y0, i);
            Vector3 b1 = Ring(r, y0, j);
            Vector3 t0 = Ring(r, y1, i);
            Vector3 t1 = Ring(r, y1, j);

            Vector3 outward = (b0 + b1) * 0.5f;
            outward.y = 0f;

            AddTri(b0, t0, t1, outward, tris);
            AddTri(b0, t1, b1, outward, tris);

            AddTri(topCenter, t0, t1, Vector3.up, tris);       
            AddTri(bottomCenter, b0, b1, Vector3.down, tris);  
        }
    }

    void AddCone(float yBase, float yApex, float r, List<int> tris)
    {
        Vector3 apex = new Vector3(0f, yApex, 0f);
        Vector3 baseCenter = new Vector3(0f, yBase, 0f);

        for (int i = 0; i < sides; i++)
        {
            int j = (i + 1) % sides;

            Vector3 b0 = Ring(r, yBase, i);
            Vector3 b1 = Ring(r, yBase, j);

            Vector3 outward = (b0 + b1) * 0.5f;
            outward.y = 0f;

            AddTri(apex, b0, b1, outward, tris);               
            AddTri(baseCenter, b0, b1, Vector3.down, tris);     
        }
    }


    void AddTri(Vector3 a, Vector3 b, Vector3 c, Vector3 outward, List<int> tris)
    {
        Vector3 normal = Vector3.Cross(b - a, c - a);
        if (Vector3.Dot(normal, outward) < 0f)
        {
            Vector3 t = b; b = c; c = t;
        }

        int start = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        tris.Add(start + 0);
        tris.Add(start + 1);
        tris.Add(start + 2);
    }
}
