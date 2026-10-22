using System.Collections.Generic;
using UnityEngine;

public class GridMacineplacer : MonoBehaviour
{
    [Header("配置に必要な設定")]
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private GameObject machinePrefab;

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
        if (mainCamera == null ||
            gridSystem == null ||
            machinePrefab == null)
        {
            return;
        }

        // 左クリックしたとき
        if (Input.GetMouseButtonDown(0))
        {
            PlaceMachine();
        }
    }

    private void PlaceMachine()
    {
        // マウスの画面座標を取得
        Vector3 mousePosition = Input.mousePosition;

        // カメラからZ=0の平面までの距離を設定
        mousePosition.z = -mainCamera.transform.position.z;

        // 画面座標をワールド座標に変換
        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        // クリックした場所のマスを取得
        Vector3Int cellPosition =
            gridSystem.WorldToCell(worldPosition);

        // すでに機械がある場合は配置しない
        if (occupiedCells.Contains(cellPosition))
        {
            Debug.Log("このマスには配置できません。");
            return;
        }

        // マスの中心位置を取得
        Vector3 spawnPosition =
            gridSystem.CellToWorldCenter(cellPosition);

        // 機械を生成
        Instantiate(
            machinePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // 使用済みのマスとして記録
        occupiedCells.Add(cellPosition);

        Debug.Log($"機械を配置しました: {cellPosition}");
    }
}
