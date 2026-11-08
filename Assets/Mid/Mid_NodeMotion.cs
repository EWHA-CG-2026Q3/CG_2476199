using UnityEngine;


public class Mid_NodeMotion : MonoBehaviour
{
    public float spinY = 0f;        
    public float startAngle = 0f;  
    public float bobHeight = 0f;    
    public float bobSpeed = 1f;    

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float t = Time.time;
        transform.localRotation = Quaternion.Euler(0f, startAngle + spinY * t, 0f);
        transform.localPosition = startPosition + Vector3.up * (bobHeight * Mathf.Sin(t * bobSpeed));
    }
}
