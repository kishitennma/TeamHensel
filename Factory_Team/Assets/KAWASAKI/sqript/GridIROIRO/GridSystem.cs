using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;

    // ワールド座標からマス目の座標を取得する
    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        Vector3Int cell = grid.WorldToCell(worldPosition);

        return new Vector2Int(cell.x, cell.y);
    }

    // マス目の座標から、そのマスの中心位置を取得する
    public Vector3 CellToWorldCenter(Vector2Int cellPosition)
    {
        Vector3Int cell = new Vector3Int(
            cellPosition.x,
            cellPosition.y,
            0
        );

        return grid.GetCellCenterWorld(cell);
    }
}
