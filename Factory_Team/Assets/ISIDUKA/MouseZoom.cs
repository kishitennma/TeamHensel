using UnityEngine;

public class MouseZoom : MonoBehaviour
{
    [Header("ズーム設定")]
    [SerializeField] float zoomSpeed = 5f;

    [Header("ズーム制限")]
    [SerializeField] float minZoom = 3f;
    [SerializeField] float maxZoom = 15f;

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // マウスホイールの入力
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0) return;

        // ズーム前のマウス位置を取得
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -cam.transform.position.z;

        Vector3 beforeZoom = cam.ScreenToWorldPoint(mousePos);

        // ズーム変更
        float zoom = cam.orthographicSize - scroll * zoomSpeed;

        // ズーム制限
        cam.orthographicSize = Mathf.Clamp(zoom, minZoom, maxZoom);

        // ズーム後のマウス位置を取得
        Vector3 afterZoom = cam.ScreenToWorldPoint(mousePos);

        // カメラ位置を補正
        Vector3 difference = beforeZoom - afterZoom;

        transform.position += difference;
    }
}
