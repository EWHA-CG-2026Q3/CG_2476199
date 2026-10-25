using System.Collections.Generic;
using UnityEngine;

// S12_StillLife용: 정점과 삼각형 배열로 메시를 직접 만듦 (S03–S04 방식)
// 빈 오브젝트에 붙이고 Shape를 고르면 됨. Edit 모드에서도 보임
// Shape를 바꾼 뒤에는 컴포넌트 체크박스를 껐다 켜면 다시 만들어짐
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S12_CustomMesh : MonoBehaviour
{
    public enum Shape { Pyramid, Prism, Octahedron }
    public Shape shape = Shape.Pyramid;

    readonly List<Vector3> tris = new List<Vector3>();   // 정점 3개씩이 삼각형 하나

    void OnEnable()
    {
        Build();
    }

    void Build()
    {
        tris.Clear();

        switch (shape)
        {
            case Shape.Pyramid:   // 사각뿔: 밑면 1×1, 높이 1
            {
                Vector3 a = new Vector3(-0.5f, 0, -0.5f), b = new Vector3(0.5f, 0, -0.5f);
                Vector3 c = new Vector3(0.5f, 0, 0.5f),   d = new Vector3(-0.5f, 0, 0.5f);
                Vector3 top = new Vector3(0, 1, 0);
                AddQuad(a, b, c, d);                       // 밑면
                AddTri(a, b, top); AddTri(b, c, top);      // 옆면 4개
                AddTri(c, d, top); AddTri(d, a, top);
                break;
            }
            case Shape.Prism:     // 삼각기둥: 정삼각형 단면, 길이 1
            {
                float h = Mathf.Sqrt(3f) * 0.5f;
                Vector3 a0 = new Vector3(-0.5f, 0, -0.5f), b0 = new Vector3(0.5f, 0, -0.5f), c0 = new Vector3(0, h, -0.5f);
                Vector3 a1 = new Vector3(-0.5f, 0, 0.5f),  b1 = new Vector3(0.5f, 0, 0.5f),  c1 = new Vector3(0, h, 0.5f);
                AddTri(a0, b0, c0); AddTri(a1, b1, c1);    // 앞뒤 삼각형
                AddQuad(a0, b0, b1, a1);                   // 바닥
                AddQuad(b0, c0, c1, b1);                   // 오른쪽 경사면
                AddQuad(c0, a0, a1, c1);                   // 왼쪽 경사면
                break;
            }
            case Shape.Octahedron: // 정팔면체(다이아몬드): 꼭짓점 6개, 면 8개
            {
                Vector3 up = Vector3.up * 0.7f, down = Vector3.down * 0.7f;
                Vector3[] ring = { new Vector3(0.5f, 0, 0), new Vector3(0, 0, 0.5f),
                                   new Vector3(-0.5f, 0, 0), new Vector3(0, 0, -0.5f) };
                for (int i = 0; i < 4; i++)
                {
                    Vector3 p = ring[i], q = ring[(i + 1) % 4];
                    AddTri(p, q, up);
                    AddTri(p, q, down);
                }
                break;
            }
        }

        // 정점 배열과 삼각형(인덱스) 배열
        Vector3[] verts = tris.ToArray();
        int[] indices = new int[verts.Length];
        for (int i = 0; i < indices.Length; i++) indices[i] = i;

        // Winding order 맞추기: Unity는 바깥에서 볼 때 시계 방향이 앞면
        // Cross(b - a, c - a)가 바깥(가운데 → 면)을 향하지 않으면 b와 c를 바꿈
        Vector3 center = Vector3.zero;
        foreach (Vector3 v in verts) center += v;
        center /= verts.Length;

        for (int i = 0; i < verts.Length; i += 3)
        {
            Vector3 a = verts[i], b = verts[i + 1], c = verts[i + 2];
            Vector3 n = Vector3.Cross(b - a, c - a);
            if (Vector3.Dot(n, (a + b + c) / 3f - center) < 0f)
            {
                verts[i + 1] = c;
                verts[i + 2] = b;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = shape.ToString();
        mesh.hideFlags = HideFlags.DontSave;               // 씬 파일에 저장되지 않게
        mesh.vertices = verts;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;      // VirtualCamera가 읽는 sharedMesh
    }

    // 면마다 정점을 따로 둠 (모서리가 각지게 보이도록)
    void AddTri(Vector3 a, Vector3 b, Vector3 c)
    {
        tris.Add(a); tris.Add(b); tris.Add(c);
    }

    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        AddTri(a, b, c);
        AddTri(a, c, d);
    }
}
