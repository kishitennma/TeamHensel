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

    // すでに機械を配置したマスを記録する
    private HashSet<Vector3Int> occupiedCells =
        new HashSet<Vector3Int>();

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

            PlaceMachine();

        }
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

        Debug.Log($"機械:{selectedPrefab.name}");
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

        //現時点では１マス占有
        if(occupiedCells.Contains(cellPosition))
        {
            Debug.Log("このマスには配置できません");
        }

        //マスの中心位置を取得
        Vector3 spawPosition = gridSystem.CellToWorldCenter(cellPosition);

        //選択中のPrefabを生成
        Instantiate(
            selectedPrefab,
            spawPosition,
            Quaternion.identity
            );

        //使用済みのマスとして記録
        occupiedCells.Add(cellPosition);

        Debug.Log(
        $"{selectedPrefab.name}を配置しました: {cellPosition}");
    }
}
