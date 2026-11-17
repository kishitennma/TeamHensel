using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class EXshopGrid : MonoBehaviour
{
    [Header("配置に必要な設定")]
    [SerializeField] private GridSystem grid_system;

    //選択しているPrefab
    private GameObject selected_prefab;

    [Header("配置できる機械")]
    [SerializeField]
    private List<GameObject> machine_prefabs = new List<GameObject>();


    // 配置済みのマスと機械を記録
    private Dictionary<Vector2Int, GameObject> placed_machines =
        new Dictionary<Vector2Int, GameObject>();

    // 配置モードか撤去モードか
    private bool isRemovalMode = false;

    private Camera main_camera;

    // 移動中の機械
    private GameObject moving_machine;

    // 移動前のマス
    private Vector2Int original_cell;

    // ドラッグ中かどうか
    private bool isDragging = false;

    // 設置プレビュー用
    private GameObject preview_object;
    private SpriteRenderer[] preview_renderers;

    // 機械ごとの左下の配置マスを記録
    private Dictionary<GameObject, Vector2Int> machine_start_cells =
        new Dictionary<GameObject, Vector2Int>();

    // ドラッグ開始時のクリック位置と機械の左下の差
    private Vector2Int drag_cell_offset;

    //MachineBaseを取得
    private Machine_Base GetMachineBase(GameObject machine)
    {
        Machine_Base machine_base = machine.GetComponent<Machine_Base>();

        if (machine_base == null)
        {
            Debug.LogError(
                $"{machine.name} に MachineBase がありません。"
            );
        }

        return machine_base;
    }

    private void Awake()
    {
        main_camera = Camera.main;

        if (main_camera == null)
        {
            Debug.LogError(
                "MainCameraタグのカメラが見つかりません。"
            );
        }
    }

    private void Update()
    {
        if (main_camera == null || grid_system == null)
        {
            return;
        }
     
    }


    private void PlaceMachine()
    {
        if (selected_prefab == null)
        {
            return;
        }

        // MachineBaseTestからサイズを取得
        Machine_Base machine_base = GetMachineBase(selected_prefab);

        if (machine_base == null)
        {
            return;
        }

        int width = machine_base.Width;
        int height = machine_base.Height;

        // クリックしたマスを取得
        Vector2Int cellPosition = GetMouseCellPosition();

        // 配置予定の全マスを確認
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int checkCell = cellPosition
                    + new Vector2Int(x, y);

                if (placed_machines.ContainsKey(checkCell))
                {
                    Debug.Log("この範囲には配置できません");
                    return;
                }
            }
        }

        // 機械の中心位置を計算
        Vector3 spawnPosition =
            grid_system.CellToWorldCenter(cellPosition);

        spawnPosition += new Vector3(
            (width - 1) * 0.5f,
            (height - 1) * 0.5f,
            0
        );

        // 機械を生成
        GameObject newMachine = Instantiate(
            selected_prefab,
            spawnPosition,
            Quaternion.identity
        );
        Machine_Base new_machine_base = GetMachineBase(newMachine);

        new_machine_base.Set_GridPos(cellPosition);
        //new_machine_base.SetGridSystem(this);
        // 占有マスと配置位置を登録
        RegisterMachine(newMachine, cellPosition);
        Debug.Log($"{selected_prefab.name}を配置しました: {cellPosition}");

    }

    // マウス位置からグリッドのマスを取得
    private Vector2Int GetMouseCellPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Z=0の平面までの距離
        mousePosition.z = -main_camera.transform.position.z;

        // スクリーン座標をワールド座標に変換
        Vector3 worldPosition =
            main_camera.ScreenToWorldPoint(mousePosition);

        return grid_system.WorldToCell(worldPosition);
    }
 
    //機械をグリッドに登録する
    private void RegisterMachine(
        GameObject machine, Vector2Int startCell)
    {
        Machine_Base machineBase = GetMachineBase(machine);

        if (machineBase == null)
        {
            return;
        }
        Debug.Log("{machine.name} / Width={machineBase.Width} / Height={machineBase.Height}");
        machine_start_cells[machine] = startCell;

        for (int x = 0; x < machineBase.Width; x++)
        {
            for (int y = 0; y < machineBase.Height; y++)
            {
                Vector2Int cell = startCell
                    + new Vector2Int(x, y);

                placed_machines[cell] = machine;

                Debug.Log($"機械登録：{machine.name} / 座標：{cell}");
            }
        }
    }


    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //ほかのスクリプトでの呼び出し
    //Vector3Int bottomLeft = placer.GetMachineBottomLeftCell(targetMachine);


    //左下のマスを取得
    public Vector2Int GetMachineBottomLeftCell(GameObject machine)
    {
        if (machine_start_cells.TryGetValue(machine, out Vector2Int cell))
        {
            return cell;
        }

        Debug.LogError("機械の配置座標が見つかりません");
        return Vector2Int.zero;
    }


 
}

