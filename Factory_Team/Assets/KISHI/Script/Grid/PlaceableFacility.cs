using UnityEngine;

public class PlaceableFacility : MonoBehaviour
{
    [Header("Facility Settings")]
    [SerializeField] private Vector2Int size = Vector2Int.one;

    public Vector2Int Size => size;

    // 回転前のサイズは変更せず、回転状態だけを保持
    public float Rotation { get; private set; }


    public void SetRotation(float angle)
    {
        Rotation = (angle % 360f + 360f) % 360f;

        transform.rotation =
            Quaternion.Euler(0f, 0f, Rotation);

        Debug.Log(
        $"Rotation Z = {transform.eulerAngles.z}");
    }

    public Vector2Int GetRotatedSize()
    {
        int quarterTurns = Mathf.RoundToInt(Rotation / 90f) % 4;

        if (quarterTurns % 2 == 1)
        {
            return new Vector2Int(Size.y, Size.x);
        }

        return Size;
    }
}