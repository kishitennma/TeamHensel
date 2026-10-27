using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GridMacineplacer : MonoBehaviour
{
    [Header("配置に必要な設定")]
    [SerializeField] private GridSystem gridSystem;

    //選択しているPrefab
    private GameObject selectedPrefab;

    [Header("配置できる機械")]
    [SerializeField]
    private List<GameObject> machinePrefabs = new List<GameObject>();


    // 配置済みのマスと機械を記録
    private Dictionary<Vector3Int, GameObject> placedMachines =
        new Dictionary<Vector3Int, GameObject>();

    // 配置モードか撤去モードか
    private bool isRemovalMode = false;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError(
                "MainCameraタグのカメラが見つかりません。"
            );
        }
    }

    private void Update()
    {
        if (mainCamera == null ||gridSystem == null )
        {
            return;
        }

        // 右クリックで選択解除
        if (Input.GetMouseButtonDown(1))
        {
            selectedPrefab = null;
            Debug.Log("選択解除");
            return;
        }


        // 左クリックしたとき
        if (Input.GetMouseButtonDown(0))
        {
            //Uiクリック時は配置しない
            if(EventSystem.current!=null&&EventSystem.current.IsPointerOverGameObject())
            {
                return;

            }

            if (isRemovalMode)
            {
                RemoveMachine();
            }
            else
            {
                PlaceMachine();
            }

        }
    }

    // 配置モード
    public void SetPlacementMode()
    {
        isRemovalMode = false;

        Debug.Log("配置モードに切り替えました。");
    }

    //撤去モード
    public void SetRemovalMode()
    {
        isRemovalMode = true;
        selectedPrefab = null;

        Debug.Log("撤去モードに切り替えました。");
    }

    public void SelectMachine(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= machinePrefabs.Count)
        {
            Debug.LogWarning("機械の番号が正しくありません。");
            return;
        }

        selectedPrefab = machinePrefabs[prefabIndex];

        //機械の選択の解除
        if (selectedPrefab == null)
        {
            return;
        }
        isRemovalMode = false;
        Debug.Log($"機械:{selectedPrefab.name}");
    }

    // 機械を撤去
    private void RemoveMachine()
    {
        // クリックしたマスを取得
        Vector3Int cellPosition = GetMouseCellPosition();

        // そのマスに機械がなければ終了
        if (!placedMachines.TryGetValue(
            cellPosition, out GameObject machine))
        {
            Debug.Log("このマスには機械がありません。");
            return;
        }

        // 機械を削除
        Destroy(machine);

        // 配置記録を削除
        placedMachines.Remove(cellPosition);

        Debug.Log($"機械を撤去しました: {cellPosition}");

    }


    private void PlaceMachine()
    {
        if(selectedPrefab==null)
        {
            return;
        }

        //マウスの画面座標を取得
        Vector3 mousePosition = Input.mousePosition;

        //カメラから平面までの距離
        mousePosition.z=-mainCamera.transform.position.z;

        //画面座標をワールド座標に変換
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        //クリックしたマスを取得
        Vector3Int cellPosition = gridSystem.WorldToCell(worldPosition);

        // すでに機械がある場合は配置しない
        if (placedMachines.ContainsKey(cellPosition))
        {
            Debug.Log("このマスには配置できません");
            return;
        }

        //マスの中心位置を取得
        Vector3 spawPosition = gridSystem.CellToWorldCenter(cellPosition);

        //選択中のPrefabを生成
        GameObject newMachine = Instantiate(
            selectedPrefab,
            spawPosition,
            Quaternion.identity
            );

        //使用済みのマスとして記録
        placedMachines.Add(cellPosition, newMachine);
        Debug.Log(
        $"{selectedPrefab.name}を配置しました: {cellPosition}");
    }

    // マウス位置からグリッドのマスを取得
    private Vector3Int GetMouseCellPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Z=0の平面までの距離
        mousePosition.z = -mainCamera.transform.position.z;

        // スクリーン座標をワールド座標に変換
        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        return gridSystem.WorldToCell(worldPosition);
    }

}
