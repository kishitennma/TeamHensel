using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;
    [SerializeField] private float lineWidth = 0.02f;
    [SerializeField]
    private Color lineColor =
        new Color(1f, 1f, 1f, 0.25f);

    private void Start()
    {
        if (grid == null)
            return;

        int halfWidth = width / 2;
        int halfHeight = height / 2;

        // ècê¸
        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            Vector3 start = grid.CellToWorld(
                new Vector3Int(x, -halfHeight, 0));

            Vector3 end = grid.CellToWorld(
                new Vector3Int(x, halfHeight, 0));

            CreateLine($"Vertical_{x}", start, end);
        }

        // â°ê¸
        for (int y = -halfHeight; y <= halfHeight; y++)
        {
            Vector3 start = grid.CellToWorld(
                new Vector3Int(-halfWidth, y, 0));

            Vector3 end = grid.CellToWorld(
                new Vector3Int(halfWidth, y, 0));

            CreateLine($"Horizontal_{y}", start, end);
        }
    }

    private void CreateLine(
        string lineName,
        Vector3 start,
        Vector3 end)
    {
        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(transform, false);

        LineRenderer line =
            lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.startColor = lineColor;
        line.endColor = lineColor;

        line.numCapVertices = 0;
        line.sortingOrder = 100;
    }
}