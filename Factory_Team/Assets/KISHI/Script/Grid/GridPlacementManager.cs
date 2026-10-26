
using System.Collections.Generic;
using UnityEngine;

public class GridPlacementManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private PlaceableFacility selectedPrefab;

    private readonly Dictionary<Vector2Int, PlaceableFacility>
        occupiedCells = new();

    private PlaceableFacility preview;
    private float rotation = 0f;

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            rotation = (rotation + 90f) % 360f;
            preview.SetRotation(rotation);
        }

        UpdatePreview();

        if (Input.GetMouseButtonDown(0))
            TryPlace();

        if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    private Vector2Int GetMouseCell()
    {
        Vector3 mouseWorld = GetMouseWorldPosition();
        Vector3Int cell = grid.WorldToCell(mouseWorld);

        return new Vector2Int(cell.x, cell.y);
    }

    // 左下のセルを基準に、施設全体の中心位置を計算
    private Vector3 GetFacilityCenter(
        Vector2Int origin,
        Vector2Int size)
    {
        Vector3 firstCellCenter = grid.GetCellCenterWorld(
            new Vector3Int(origin.x, origin.y, 0));

        Vector3 lastCellCenter = grid.GetCellCenterWorld(
            new Vector3Int(
                origin.x + size.x - 1,
                origin.y + size.y - 1,
                0));

        return (firstCellCenter + lastCellCenter) * 0.5f;
    }

    private void UpdatePreview()
    {
        Vector2Int origin = GetMouseCell();
        Vector2Int size = preview.GetRotatedSize();

        // 回転後のサイズを使って施設の中心を決める
        preview.transform.position =
            GetFacilityCenter(origin, size);

        bool canPlace = CanPlace(preview, origin);

        foreach (var sr in preview.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = canPlace
                ? new Color(0.5f, 1f, 0.5f, 0.5f)
                : new Color(1f, 0.5f, 0.5f, 0.5f);
        }
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
                Vector2Int cell =
                    origin + new Vector2Int(x, y);

                if (occupiedCells.ContainsKey(cell))
                    return false;
            }
        }

        return true;
    }

    private void TryPlace()
    {
        Vector2Int origin = GetMouseCell();
        Vector2Int size = preview.GetRotatedSize();

        if (!CanPlace(preview, origin))
            return;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cell = origin + new Vector2Int(x, y);
                occupiedCells[cell] = preview;
            }
        }

        preview.transform.position =
            GetFacilityCenter(origin, size);

        foreach (var sr in preview.GetComponentsInChildren<SpriteRenderer>())
            sr.color = Color.white;

        preview = Instantiate(selectedPrefab);
        preview.SetRotation(rotation);
    }

    private void CancelPlacement()
    {
        Destroy(preview.gameObject);
        preview = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return Vector3.zero;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }
}
