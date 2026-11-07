using UnityEngine;

public class Mid_LanternFloat : MonoBehaviour
{
    public float helixRadius = 0.5f; 
    public float riseHeight = 1.2f;   
    public float turns = 1.5f;          
    public float cycleTime = 10f;     
    public float spinSpeed = 30f;    
    public float swayAngle = 5f;      
    public float phase = 0f;          

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;  
    }

    void Update()
    {
        float t = Time.time + phase;


        float p = 0.5f - 0.5f * Mathf.Cos(2f * Mathf.PI * t / cycleTime);

        float height = riseHeight * p;     
        float angle = 360f * turns * p;     
        float spin = spinSpeed * t;
        float sway = swayAngle * Mathf.Sin(t * 1.5f);


        Matrix4x4 M = Translate(startPos)
                    * RotateY(angle)
                    * Translate(new Vector3(helixRadius, height, 0f))
                    * RotateY(spin)
                    * RotateZ(sway);


        transform.position = (Vector3)M.GetColumn(3);
        transform.rotation = Quaternion.LookRotation((Vector3)M.GetColumn(2), (Vector3)M.GetColumn(1));
    }

    static Matrix4x4 Translate(Vector3 t)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m.m03 = t.x;
        m.m13 = t.y;
        m.m23 = t.z;
        return m;
    }

    static Matrix4x4 RotateY(float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(r);
        float sin = Mathf.Sin(r);

        Matrix4x4 m = Matrix4x4.identity;
        m.m00 = cos;  m.m02 = sin;
        m.m20 = -sin; m.m22 = cos;
        return m;
    }


    static Matrix4x4 RotateZ(float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(r);
        float sin = Mathf.Sin(r);

        Matrix4x4 m = Matrix4x4.identity;
        m.m00 = cos; m.m01 = -sin;
        m.m10 = sin; m.m11 = cos;
        return m;
    }
}
