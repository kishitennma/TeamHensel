using UnityEngine;

public class PlaceableFacility : MonoBehaviour
{
    [Header("Facility Settings")]
    [SerializeField] private Vector2Int size = Vector2Int.one;

    public Vector2Int Size => size;

    // 回転前のサイズは変更せず、回転状態だけを保持
    public int Rotation { get; private set; }

    public void SetRotation(int rotation)
    {
        Rotation = ((rotation % 4) + 4) % 4;
        transform.rotation = Quaternion.Euler(
            0f, 0f, Rotation * 90f);
    }

    public Vector2Int GetRotatedSize()
    {
        return Rotation % 2 == 0
            ? size
            : new Vector2Int(size.y, size.x);
    }
}