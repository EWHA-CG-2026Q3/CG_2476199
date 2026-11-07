using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Mid_SpiralStairMesh : MonoBehaviour
{
    [Header("계단")]
    public float totalHeight = 3.19f;     
    public int stepCount = 18;             
    public float totalAngle = 450f;       
    public float innerRadius = 0.135f;     
    public float outerRadius = 1.0f;    
    public float stepThickness = 0.06f;    

    [Header("기둥")]
    [Range(3, 32)] public int columnSides = 12;
    public float columnExtra = 0.4f;   
    public float capHeight = 0.25f;    
    public float capRadius = 0.35f;       
    [Range(3, 32)] public int capSides = 4; 
    public float capRotation = 45f;  

    [Header("발판")]
    public float bridgeLength = 0.8f;      
    public float bridgeWidth = 0.8f;      
    public float bridgeThickness = 0.08f;  
    public float bridgeAngleOffset = 0f; 
    public float bridgeRaise = 0f;      

    List<Vector3> vertices;
    List<int> woodTris;   
    List<int> frameTris;
    List<int> roofTris;    

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
        woodTris = new List<int>();
        frameTris = new List<int>();
        roofTris = new List<int>();

        int steps = Mathf.Max(1, stepCount);
        float rise = totalHeight / steps;           
        float anglePerStep = totalAngle / steps;   


        float columnTop = totalHeight + columnExtra;
        AddPrism(0f, columnTop, innerRadius, columnSides, frameTris);
        AddCone(columnTop, columnTop + capHeight, capRadius, capSides, roofTris, capRotation);

        for (int i = 0; i < steps; i++)
        {
            float a0 = anglePerStep * i;
            float a1 = anglePerStep * (i + 1);
            float top = rise * (i + 1);
            AddSectorBlock(a0, a1, innerRadius, outerRadius, top - stepThickness, top, woodTris);
        }


        if (bridgeLength > 0f)
        {
            float bridgeAngle = totalAngle - anglePerStep * 0.5f + bridgeAngleOffset;
            Vector3 forward = Polar(1f, 0f, bridgeAngle);      
            Vector3 side = Polar(1f, 0f, bridgeAngle + 90f);    
            Vector3 origin = forward * outerRadius;           

            float top = totalHeight + bridgeRaise;
            AddBox(origin, forward, side, 0f, bridgeLength, bridgeWidth * 0.5f,
                   top - bridgeThickness, top, woodTris);
        }

        Mesh mesh = new Mesh();
        mesh.name = "SpiralStair";
        mesh.hideFlags = HideFlags.DontSave;
        mesh.SetVertices(vertices);
        mesh.subMeshCount = 3;
        mesh.SetTriangles(woodTris, 0);
        mesh.SetTriangles(frameTris, 1);
        mesh.SetTriangles(roofTris, 2);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;


        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }


    Vector3 Polar(float r, float y, float degrees)
    {
        float a = degrees * Mathf.Deg2Rad;
        return new Vector3(r * Mathf.Cos(a), y, r * Mathf.Sin(a));
    }



    void AddSectorBlock(float a0, float a1, float r0, float r1, float y0, float y1, List<int> tris)
    {
        Vector3 in0b = Polar(r0, y0, a0), in1b = Polar(r0, y0, a1);
        Vector3 out0b = Polar(r1, y0, a0), out1b = Polar(r1, y0, a1);
        Vector3 in0t = Polar(r0, y1, a0), in1t = Polar(r0, y1, a1);
        Vector3 out0t = Polar(r1, y1, a0), out1t = Polar(r1, y1, a1);

        Vector3 radial = Polar(1f, 0f, (a0 + a1) * 0.5f);   
        Vector3 tangent0 = Polar(1f, 0f, a0 + 90f);         
        Vector3 tangent1 = Polar(1f, 0f, a1 + 90f);

        AddQuad(in0t, out0t, out1t, in1t, Vector3.up, tris);      
        AddQuad(in0b, out0b, out1b, in1b, Vector3.down, tris); 
        AddQuad(out0b, out1b, out1t, out0t, radial, tris);       
        AddQuad(in0b, in1b, in1t, in0t, -radial, tris);         
        AddQuad(in0b, out0b, out0t, in0t, -tangent0, tris);      
        AddQuad(in1b, out1b, out1t, in1t, tangent1, tris);      
    }


    void AddPrism(float y0, float y1, float r, int sides, List<int> tris)
    {
        for (int i = 0; i < sides; i++)
        {
            float a = 360f / sides * i;
            float b = 360f / sides * (i + 1);
            Vector3 b0 = Polar(r, y0, a), b1 = Polar(r, y0, b);
            Vector3 t0 = Polar(r, y1, a), t1 = Polar(r, y1, b);
            Vector3 outward = Polar(1f, 0f, (a + b) * 0.5f);

            AddQuad(b0, b1, t1, t0, outward, tris);
            AddTri(new Vector3(0f, y1, 0f), t0, t1, Vector3.up, tris);
            AddTri(new Vector3(0f, y0, 0f), b0, b1, Vector3.down, tris);
        }
    }


    void AddCone(float yBase, float yApex, float r, int sides, List<int> tris, float angleOffset = 0f)
    {
        Vector3 apex = new Vector3(0f, yApex, 0f);
        Vector3 baseCenter = new Vector3(0f, yBase, 0f);
        for (int i = 0; i < sides; i++)
        {
            float a = 360f / sides * i + angleOffset;
            float b = 360f / sides * (i + 1) + angleOffset;
            Vector3 b0 = Polar(r, yBase, a), b1 = Polar(r, yBase, b);
            Vector3 outward = Polar(1f, 0f, (a + b) * 0.5f);

            AddTri(apex, b0, b1, outward, tris);
            AddTri(baseCenter, b0, b1, Vector3.down, tris);
        }
    }

    void AddBox(Vector3 origin, Vector3 dir, Vector3 side, float start, float end, float halfWidth, float y0, float y1, List<int> tris)
    {
        Vector3 up0 = new Vector3(0f, y0, 0f);
        Vector3 up1 = new Vector3(0f, y1, 0f);

        Vector3 sL = origin + dir * start - side * halfWidth, sR = origin + dir * start + side * halfWidth;
        Vector3 eL = origin + dir * end - side * halfWidth,   eR = origin + dir * end + side * halfWidth;

        AddQuad(sL + up1, sR + up1, eR + up1, eL + up1, Vector3.up, tris);   
        AddQuad(sL + up0, sR + up0, eR + up0, eL + up0, Vector3.down, tris); 
        AddQuad(sR + up0, eR + up0, eR + up1, sR + up1, side, tris);          
        AddQuad(sL + up0, eL + up0, eL + up1, sL + up1, -side, tris);      
        AddQuad(eL + up0, eR + up0, eR + up1, eL + up1, dir, tris);          
        AddQuad(sL + up0, sR + up0, sR + up1, sL + up1, -dir, tris);        
    }

    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 outward, List<int> tris)
    {
        AddTri(a, b, c, outward, tris);
        AddTri(a, c, d, outward, tris);
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
