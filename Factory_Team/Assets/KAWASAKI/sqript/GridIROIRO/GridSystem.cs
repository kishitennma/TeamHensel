using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;

    // ワールド座標からマス目の座標を取得する
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    // マス目の座標から、そのマスの中心位置を取得する
    public Vector3 CellToWorldCenter(Vector3Int cellPosition)
    {
        return grid.GetCellCenterWorld(cellPosition);
    }
}
