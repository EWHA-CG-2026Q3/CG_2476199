using UnityEngine;


[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class Mid_GemChain : MonoBehaviour
{
    [SerializeField] Transform[] chain;        

    [SerializeField, HideInInspector] Mesh sourceMesh;  

    Mesh mesh;                 
    Vector3[] baseVertices;   

    void LateUpdate()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (mesh == null || meshFilter.sharedMesh != mesh)
        {
            if (!Init(meshFilter)) return;
        }
        if (chain == null) return;

        Matrix4x4 M = Matrix4x4.identity;
        foreach (Transform node in chain)
        {
            if (node == null) continue;
            M = M * Matrix4x4.TRS(node.localPosition, node.localRotation, node.localScale);
        }

        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
            verts[i] = M.MultiplyPoint(baseVertices[i]);

        mesh.vertices = verts;
        mesh.RecalculateNormals();      
        mesh.RecalculateBounds();       
    }

    bool Init(MeshFilter meshFilter)
    {
        if (meshFilter.sharedMesh != null) sourceMesh = meshFilter.sharedMesh;
        if (sourceMesh == null) return false;

        mesh = Instantiate(sourceMesh);
        mesh.name = sourceMesh.name + "_Chain";
        mesh.hideFlags = HideFlags.DontSave;
        baseVertices = mesh.vertices;
        meshFilter.sharedMesh = mesh;
        return true;
    }

    void OnDisable()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && sourceMesh != null && meshFilter.sharedMesh == mesh)
            meshFilter.sharedMesh = sourceMesh;
        mesh = null;
    }
}
