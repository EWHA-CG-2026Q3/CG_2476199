using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_VaseMesh : MonoBehaviour
{
    [Range(3, 32)] public int sides = 12;  

    public Vector2[] profile =
    {
        new Vector2(0.07f, 0.00f),   
        new Vector2(0.15f, 0.20f),  
        new Vector2(0.17f, 0.30f),  
        new Vector2(0.05f, 0.40f),   
        new Vector2(0.06f, 0.45f),  
    };

    List<Vector3> vertices;
    List<int> triangles;

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
        if (profile == null || profile.Length < 2) return;

        vertices = new List<Vector3>();
        triangles = new List<int>();
        int n = profile.Length;


        for (int k = 0; k < n - 1; k++)
        {
            float r0 = profile[k].x, y0 = profile[k].y;
            float r1 = profile[k + 1].x, y1 = profile[k + 1].y;

            for (int i = 0; i < sides; i++)
            {
                int j = (i + 1) % sides;  

                Vector3 b0 = Ring(r0, y0, i);
                Vector3 b1 = Ring(r0, y0, j);
                Vector3 t0 = Ring(r1, y1, i);
                Vector3 t1 = Ring(r1, y1, j);

                Vector3 outward = (b0 + b1) * 0.5f;
                outward.y = 0f;

                AddTri(b0, t0, t1, outward);
                AddTri(b0, t1, b1, outward);
            }
        }


        Vector3 bottomCenter = new Vector3(0f, profile[0].y, 0f);
        Vector3 topCenter = new Vector3(0f, profile[n - 1].y, 0f);
        for (int i = 0; i < sides; i++)
        {
            int j = (i + 1) % sides;
            AddTri(bottomCenter, Ring(profile[0].x, profile[0].y, i),
                   Ring(profile[0].x, profile[0].y, j), Vector3.down);
            AddTri(topCenter, Ring(profile[n - 1].x, profile[n - 1].y, i),
                   Ring(profile[n - 1].x, profile[n - 1].y, j), Vector3.up);
        }

        Mesh mesh = new Mesh();
        mesh.name = "Vase";
        mesh.hideFlags = HideFlags.DontSave;
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }


    Vector3 Ring(float r, float y, int i)
    {
        float angle = 2f * Mathf.PI * i / sides;
        return new Vector3(r * Mathf.Cos(angle), y, r * Mathf.Sin(angle));
    }


    void AddTri(Vector3 a, Vector3 b, Vector3 c, Vector3 outward)
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
        triangles.Add(start + 0);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
    }
}
