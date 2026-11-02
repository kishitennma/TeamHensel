using UnityEngine;

public class GridLine : MonoBehaviour
{
    [SerializeField] private Grid grid;

    [Header("•\¦”ÍˆÍ")]
    [SerializeField] private int startX = -10;
    [SerializeField] private int startY = -10;
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;

    [Header("ü‚Ìİ’è")]
    [SerializeField] private Color line_color = Color.gray;

    private void Start()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        GameObject gridLines = new GameObject("GridLines");
        gridLines.transform.SetParent(transform);

        // cü
        for (int x = startX; x <= startX + width; x++)
        {
            Vector3 start = grid.CellToWorld(
                new Vector3Int(x, startY, 0));

            Vector3 end = grid.CellToWorld(
                new Vector3Int(x, startY + height, 0));

            CreateLine(gridLines.transform, start, end);
        }

        // ‰¡ü
        for (int y = startY; y <= startY + height; y++)
        {
            Vector3 start = grid.CellToWorld(
                new Vector3Int(startX, y, 0));

            Vector3 end = grid.CellToWorld(
                new Vector3Int(startX + width, y, 0));

            CreateLine(gridLines.transform, start, end);
        }
    }

    private void CreateLine(
        Transform parent, Vector3 start, Vector3 end)
    {
        GameObject lineObject = new GameObject("GridLine");
        lineObject.transform.SetParent(parent);

        LineRenderer line =
            lineObject.AddComponent<LineRenderer>();

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startWidth = 0.025f;
        line.endWidth = 0.025f;

        line.material =
            new Material(Shader.Find("Sprites/Default"));

        line.startColor = line_color;
        line.endColor = line_color;
        line.sortingOrder = 10;
    }
}
