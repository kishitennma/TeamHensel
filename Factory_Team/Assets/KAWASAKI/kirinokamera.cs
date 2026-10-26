using UnityEngine;

public class kirinokamera : MonoBehaviour
{
    float speed = 0.01f;
    Vector2 pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos.x += speed;
        transform.position = pos;
    }
}
