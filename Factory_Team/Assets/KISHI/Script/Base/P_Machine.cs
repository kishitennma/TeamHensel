
using System.Collections.Generic;
using UnityEngine;

public class P_Machine: Machine_Base
{
    [Header("生産設定")]
    [SerializeField] private float produce_interval = 3f; // 生産間隔
    [SerializeField] private Item item_prefab;             // 生産するアイテムPrefab

    private float timer = 0f;

    private Item item_stock; // 生産したアイテムを保持

    private void Update()
    {
        // アイテムを保持していない場合のみ生産時間を進める
        if (item_stock == null)
        {
            timer += Time.deltaTime;

            if (timer >= produce_interval)
            {
                Produce_Item();
            }
        }

        // アイテムを保持していれば受け渡しを試みる
        if (item_stock != null)
        {
            Try_Transfer_Item();
        }
    }

    /// <summary>
    /// アイテムを生産する
    /// </summary>
    private void Produce_Item()
    {
        // Prefabが設定されていなければ終了
        if (item_prefab == null)
        {
            Debug.LogWarning("生産するアイテムPrefabが設定されていません");
            return;
        }

        // アイテムを生成して保持
        item_stock = Instantiate(
            item_prefab,
            transform.position,
            Quaternion.identity
        );

        timer = 0f;

        Debug.Log("アイテムを生産しました：" + item_stock.name);
    }

    /// <summary>
    /// 出力方向にある機械へアイテムを渡す
    /// </summary>
    private void Try_Transfer_Item()
    {
        if (gridsystem == null)
        {
            return;
        }

        // Output辺に隣接するセルを取得
        List<Vector2Int> cells = Output_Get_Check_Cells();

        foreach (Vector2Int cellpos in cells)
        {
            // 隣接するMachine_Baseを取得
            Machine_Base neighbor =
                gridsystem.GetComponentAt<Machine_Base>(cellpos);

            if (neighbor == null)
            {
                continue;
            }

            // 相手のInput方向と自分のOutput方向を確認
            if ((int)neighbor.InputDir != ((int)OutputDir + 2) % 4)
            {
                continue;
            }

            // アイテムを渡す
            bool received = neighbor.Recive_Item(item_stock);

            if (received)
            {
                Debug.Log("アイテムを渡しました：" + neighbor.name);

                // 受け渡し成功したので保持アイテムを解除
                item_stock = null;
                return;
            }
        }
    }
}
