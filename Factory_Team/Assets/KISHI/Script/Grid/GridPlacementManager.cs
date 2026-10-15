using System.Collections.Generic;
using UnityEngine;

public class GridPlacementManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private PlaceableFacility selectedPrefab;

    private readonly Dictionary<Vector2Int, PlaceableFacility>
        occupiedCells = new();

    private PlaceableFacility preview;
    private int rotation;

    private void Start()
    {
        if (selectedPrefab == null || grid == null)
            return;

        preview = Instantiate(selectedPrefab);
        preview.SetRotation(rotation);
    }

    private void Update()
    {
        if (preview == null)
            return;

        UpdatePreview();

        if (Input.GetKeyDown(KeyCode.R))
        {
            rotation = (rotation + 1) % 4;
            preview.SetRotation(rotation);
        }

        if (Input.GetMouseButtonDown(0))
            TryPlace();

        if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    private void UpdatePreview()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            Input.mousePosition);

        Vector3Int cell = grid.WorldToCell(mouseWorld);
        Vector2Int origin = new(cell.x, cell.y);

        preview.transform.position =
            grid.GetCellCenterWorld(cell);

        bool canPlace = CanPlace(preview, origin);

        // 仮実装：設置可能なら緑、不可なら赤
        foreach (var sr in preview.GetComponentsInChildren<SpriteRenderer>())
            sr.color = canPlace
                ? new Color(0.5f, 1f, 0.5f, 0.5f)
                : new Color(1f, 0.5f, 0.5f, 0.5f);
    }

    private bool CanPlace(
        PlaceableFacility facility,
        Vector2Int origin)
    {
        Vector2Int size = facility.GetRotatedSize();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cell = origin + new Vector2Int(x, y);

                if (occupiedCells.ContainsKey(cell))
                    return false;
            }
        }

        return true;
    }

    private void TryPlace()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            Input.mousePosition);

        Vector3Int cell = grid.WorldToCell(mouseWorld);
        Vector2Int origin = new(cell.x, cell.y);

        if (!CanPlace(preview, origin))
            return;

        Vector2Int size = preview.GetRotatedSize();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int occupiedCell =
                    origin + new Vector2Int(x, y);

                occupiedCells[occupiedCell] = preview;
            }
        }

        preview.transform.position =
            grid.GetCellCenterWorld(cell);

        foreach (var sr in preview.GetComponentsInChildren<SpriteRenderer>())
            sr.color = Color.white;

        // 設置した施設を確定し、次のプレビューを生成
        preview = Instantiate(selectedPrefab);
        preview.SetRotation(rotation);
    }

    private void CancelPlacement()
    {
        Destroy(preview.gameObject);
        preview = null;
    }
}