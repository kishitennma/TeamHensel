using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class RailPlace : MonoBehaviour
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


    private Camera main_camera;


    // 移動中の機械
    private GameObject moving_machine;

    // 移動前のマス
    private Vector2Int original_cell;

    // ドラッグ中かどうか
    private bool isDragging = false;

    // 機械ごとの左下の配置マスを記録
    private Dictionary<GameObject, Vector2Int> machine_start_cells =
        new Dictionary<GameObject, Vector2Int>();

    // ドラッグ開始時のクリック位置と機械の左下の差
    private Vector2Int drag_cell_offset;

    //MachineBaseを取得
    private MachineBaseTest GetMachineBase(GameObject machine)
    {
        MachineBaseTest machineBase = machine.GetComponent<MachineBaseTest>();

        if (machineBase == null)
        {
            Debug.LogError(
                $"{machine.name} に MachineBase がありません。"
            );
        }

        return machineBase;
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

        // 右クリックで選択解除
        if (Input.GetMouseButtonDown(1))
        {
            selected_prefab = null;
            Debug.Log("選択解除");
            return;
        }


        // 左クリックしたとき
        if (Input.GetMouseButtonDown(0))
        {
            //Uiクリック時は配置しない
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;

            }
            else
            {
                Vector2Int cell = GetMouseCellPosition();

                // 既存機械があればドラッグ
                if (placed_machines.ContainsKey(cell))
                {
                    StartDragging();
                }
                // 空いていれば選択中の機械を配置
                else if (selected_prefab != null)
                {
                    PlaceMachine();
                }
            }
        }
        // ドラッグ中
        if (isDragging)
        {
            UpdateDragging();

            // 左クリックを離した
            if (Input.GetMouseButtonUp(0))
            {
                EndDragging();
            }
        }
    }

    public void SelectMachine(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= machine_prefabs.Count)
        {
            Debug.LogWarning("機械の番号が正しくありません。");
            return;
        }

        selected_prefab = machine_prefabs[prefabIndex];

        //機械の選択の解除
        if (selected_prefab == null)
        {
            return;
        }
        Debug.Log($"機械:{selected_prefab.name}");
    }


    private void PlaceMachine()
    {
        if (selected_prefab == null)
        {
            return;
        }

        // MachineBaseTestからサイズを取得
        MachineBaseTest machineBase = GetMachineBase(selected_prefab);

        if (machineBase == null)
        {
            return;
        }

        int width = machineBase.Width;
        int height = machineBase.Height;

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

        // 占有マスと配置位置を登録
        RegisterMachine(newMachine, cellPosition);
        Debug.Log(
            $"{selected_prefab.name}を配置しました: {cellPosition}"
        );

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
    //ドラッグ開始距離
    private void StartDragging()
    {
        Vector2Int cell = GetMouseCellPosition();

        if (!placed_machines.TryGetValue(
            cell, out GameObject machine))
        {
            return;
        }

        if (!machine_start_cells.TryGetValue(
               machine, out Vector2Int startCell))
        {
            Debug.LogError("機械の配置位置が見つかりません");
            return;
        }

        moving_machine = machine;
        original_cell = startCell;

        // クリックしたマスと機械の左下の差を保存
        drag_cell_offset = cell - startCell;

        // 元の占有マスをすべて解除
        UnregisterMachine(machine);

        isDragging = true;

        Debug.Log("機械を移動します");
    }
    //ドラッグ中の処理
    private void UpdateDragging()
    {
        Vector2Int mouseCell = GetMouseCellPosition();

        Vector2Int newStartCell = mouseCell - drag_cell_offset;

        MachineBaseTest machineBase = GetMachineBase(moving_machine);

        if (machineBase == null)
        {
            return;
        }

        Vector3 worldPosition =
            grid_system.CellToWorldCenter(newStartCell);

        worldPosition += new Vector3(
            (machineBase.Width - 1) * 0.5f,
            (machineBase.Height - 1) * 0.5f,
            0
        );

        moving_machine.transform.position = worldPosition;
    }
    //ドラッグ終了時の処理


    private void EndDragging()
    {
        Vector2Int mouseCell = GetMouseCellPosition();

        // 機械の左下のマス
        Vector2Int newStartCell = mouseCell - drag_cell_offset;

        MachineBaseTest machineBase = GetMachineBase(moving_machine);

        if (machineBase == null)
        {
            return;
        }

        int width = machineBase.Width;
        int height = machineBase.Height;

        bool canMove = true;

        // 移動先の全マスを確認
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int cell = newStartCell
                    + new Vector2Int(x, y);

                if (placed_machines.ContainsKey(cell))
                {
                    canMove = false;
                    break;
                }
            }

            if (!canMove)
            {
                break;
            }
        }

        if (canMove)
        {
            // 新しい位置に移動
            Vector3 worldPosition =
                grid_system.CellToWorldCenter(newStartCell);

            worldPosition += new Vector3(
                (width - 1) * 0.5f,
                (height - 1) * 0.5f,
                0
            );

            moving_machine.transform.position = worldPosition;

            // 新しい占有マスを登録
            RegisterMachine(moving_machine, newStartCell);

            Debug.Log("機械を移動しました");
        }
        else
        {
            // 移動できない場合は元の位置に戻す
            Vector3 worldPosition =
                grid_system.CellToWorldCenter(original_cell);

            worldPosition += new Vector3(
                (width - 1) * 0.5f,
                (height - 1) * 0.5f,
                0
            );

            moving_machine.transform.position = worldPosition;

            RegisterMachine(moving_machine, original_cell);

            Debug.Log("移動先に機械があります");
        }

        moving_machine = null;
        isDragging = false;
    }



    private void RegisterMachine(
        GameObject machine, Vector2Int startCell)
    {
        MachineBaseTest machineBase = GetMachineBase(machine);

        if (machineBase == null)
        {
            return;
        }

        machine_start_cells[machine] = startCell;

        for (int x = 0; x < machineBase.Width; x++)
        {
            for (int y = 0; y < machineBase.Height; y++)
            {
                Vector2Int cell = startCell
                    + new Vector2Int(x, y);

                placed_machines[cell] = machine;
            }
        }
    }

    private void UnregisterMachine(GameObject machine)
    {
        if (!machine_start_cells.TryGetValue(
            machine, out Vector2Int startCell))
        {
            return;
        }

        MachineBaseTest machineBase = GetMachineBase(machine);

        if (machineBase == null)
        {
            return;
        }

        for (int x = 0; x < machineBase.Width; x++)
        {
            for (int y = 0; y < machineBase.Height; y++)
            {
                Vector2Int cell = startCell
                    + new Vector2Int(x, y);

                // この機械自身の登録だけを解除
                if (placed_machines.TryGetValue(
                    cell, out GameObject registeredMachine)
                    && registeredMachine == machine)
                {
                    placed_machines.Remove(cell);
                }
            }
        }

        machine_start_cells.Remove(machine);
    }
}