
using System.Collections.Generic;
using UnityEngine;

public class F_Machine : Machine_Base
{
    // 合成に使用する素材
    private List<Item> input_items = new List<Item>();

    //レシピ管理クラス
    [SerializeField] private MixerRecipe mixer_recipe;

    // 最大受け取り数
    [SerializeField] private int input_capacity = 2;

    // 合成時間
    [SerializeField] private float synthesis_time = 3f;

    // 合成タイマー
    private float synthesis_timer = 0f;

    // 合成中かどうか
    private bool is_synthesizing = false;

    // 完成品
    private Item waiting_output_item;

    // 完成品ID
    private Item.ItemID output_id;

    // 完成品IDとPrefabの対応データ
    [System.Serializable]
    private class OutputItemData
    {
        public Item.ItemID item_id;
        public Item prefab;
    }

    [SerializeField]
    private List<OutputItemData> output_item_list = new List<OutputItemData>();

    private void Update()
    {
        // 合成中ならタイマーを進める
        if (is_synthesizing)
        {
            synthesis_timer -= Time.deltaTime;

            if (synthesis_timer <= 0f)
            {
                Complete_Synthesis();
            }
        }

        // 完成品があれば出力を試みる
        if (waiting_output_item != null)
        {
            Try_Output_Item();
        }
    }

    public override bool Recive_Item(Item item)
    {
        if (item == null)
            return false;

        if (is_synthesizing || waiting_output_item != null)
            return false;

        if (input_items.Count >= input_capacity)
            return false;

        // 素材を受け取る
        input_items.Add(item);

        Debug.Log("素材を受け取りました：" + item.name);

        // 2個揃ったらレシピ検索
        if (input_items.Count == input_capacity)
        {
            Item.ItemID id1 = input_items[0].GetItemID();
            Item.ItemID id2 = input_items[1].GetItemID();

            bool recipe_found = Recipe_Search(
                id1,
                id2,
                out Item.ItemID result_id
            );

            // レシピの有無に関係なく素材を消費
            Consume_Input_Items();

            if (recipe_found)
            {
                output_id = result_id;
                Start_Synthesis();
            }
            else
            {
                Debug.Log("レシピがありません。素材を消費しました");
            }
        }

        return true;
    }

    private void Consume_Input_Items()
    {
        foreach (Item item in input_items)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        input_items.Clear();

        Debug.Log("素材をすべて消費しました");
    }

    // レシピ検索（仮実装）
    private bool Recipe_Search(
     Item.ItemID id1,
     Item.ItemID id2,
     out Item.ItemID result_id)
    {
        // Managerが設定されているか確認
        if (mixer_recipe == null)
        {
            Debug.LogError("MixerRecipeが設定されていません！");
            result_id = Item.ItemID.None;
            return false;
        }

        // レシピ検索をManagerに任せる
        if (mixer_recipe.TryGetRecipe(
            id1,
            id2,
            out CraftRecipe recipe))
        {
            result_id = recipe.Output;
            return true;
        }

        result_id = Item.ItemID.None;
        return false;
    }

    // 合成開始
    private void Start_Synthesis()
    {
        is_synthesizing = true;
        synthesis_timer = synthesis_time;

        Debug.Log("合成開始！");
    }

    // 合成完了
    private void Complete_Synthesis()
    {
        is_synthesizing = false;

        // 完成品IDに対応するPrefabを取得
        Item prefab = Get_Output_Prefab(output_id);

        if (prefab == null)
        {
            Debug.LogError("完成品Prefabが見つかりません：" + output_id);
            return;
        }

        // 完成品生成
        waiting_output_item = Instantiate(
            prefab,
            transform.position,
            Quaternion.identity
        );

        Debug.Log("合成完了：" + waiting_output_item.name);
    }

    // 完成品Prefab検索
    private Item Get_Output_Prefab(Item.ItemID id)
    {
        foreach (OutputItemData data in output_item_list)
        {
            if (data.item_id == id)
            {
                return data.prefab;
            }
        }

        return null;
    }

    // 完成品を出力
    private void Try_Output_Item()
    {
        if (gridsystem == null)
            return;

        List<Vector2Int> cells = Output_Get_Check_Cells();

        foreach (Vector2Int cellpos in cells)
        {
            Machine_Base neighbor =
                gridsystem.GetComponentAt<Machine_Base>(cellpos);

            if (neighbor == null)
                continue;

            // 入力方向と出力方向の確認
            if ((int)neighbor.InputDir != ((int)OutputDir + 2) % 4)
                continue;

            // アイテムを受け渡す
            bool received = neighbor.Recive_Item(waiting_output_item);

            if (received)
            {
                Debug.Log("合成品を渡しました：" + neighbor.name);

                waiting_output_item = null;
                return;
            }
        }
    }
}
