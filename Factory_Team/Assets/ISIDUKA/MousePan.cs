using UnityEngine;

public class MousePan : MonoBehaviour
{
    private Vector3 lastMousePosition;

    [Header("ˆÚ“®”ÍˆÍ")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    [Header("ˆÚ“®‘¬“x")]
    public float moveSpeed = 0.01f;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 delta = currentMousePosition - lastMousePosition;

            transform.position -= new Vector3(
                delta.x * moveSpeed,
                delta.y * moveSpeed,
                0
            );

            // ƒJƒƒ‰‚ÌˆÚ“®”ÍˆÍ‚ğ§ŒÀ
            Vector3 pos = transform.position;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            transform.position = pos;

            lastMousePosition = currentMousePosition;
        }
    }
}