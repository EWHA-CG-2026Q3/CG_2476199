using UnityEngine;

// 정사면체(Tetrahedron) 메시를 코드로 생성
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_TetrahedronMesh : MonoBehaviour
{
    public float size = 1f;

    void OnEnable()
    {
        Build();
    }

    void Build()
    {
        float s = size * 0.5f;

        // 정사면체의 꼭짓점 4개 (중심이 원점)
        Vector3[] p =
        {
            new Vector3( 1,  1,  1) * s,
            new Vector3(-1, -1,  1) * s,
            new Vector3(-1,  1, -1) * s,
            new Vector3( 1, -1, -1) * s,
        };

        // 면 4개 (삼각형)
        int[,] faces =
        {
            { 0, 1, 2 },
            { 0, 3, 1 },
            { 0, 2, 3 },
            { 1, 3, 2 },
        };

        // 면마다 꼭짓점을 따로 만들어야 모서리가 각지게(flat shading) 보임
        Vector3[] vertices = new Vector3[12];
        int[] triangles = new int[12];

        for (int f = 0; f < 4; f++)
        {
            Vector3 a = p[faces[f, 0]];
            Vector3 b = p[faces[f, 1]];
            Vector3 c = p[faces[f, 2]];

            Vector3 normal = Vector3.Cross(b - a, c - a);
            Vector3 center = (a + b + c) / 3f;
            if (Vector3.Dot(normal, center) < 0f)
            {
                Vector3 t = b; b = c; c = t;
            }

            vertices[f * 3 + 0] = a;
            vertices[f * 3 + 1] = b;
            vertices[f * 3 + 2] = c;
            triangles[f * 3 + 0] = f * 3 + 0;
            triangles[f * 3 + 1] = f * 3 + 1;
            triangles[f * 3 + 2] = f * 3 + 2;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Tetrahedron";
        mesh.hideFlags = HideFlags.DontSave;   
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
