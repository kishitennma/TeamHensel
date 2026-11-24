using System.Collections.Generic;
using UnityEngine;

public class ProcessingMachine : Machine_Base
{
    //加工機が保管しているアイテム
    private List<Item> input_items = new List<Item>();

    //加工レシピ管理クラス
    [SerializeField]
    private MaterialProcessing material_processing;

    //加工時間
    [SerializeField]
    private float processing_time = 3f;

    //加工中かどうか
    private bool is_processing = false;

    //アイテムの最大保管数
    [SerializeField]
    private int input_capacity = 2;

    //加工残り時間
    private float processing_timer = 0f;

    // 加工完了時に使用する完成品Prefab
    private Item processing_output_prefab;

    // 加工完了時に生産する個数
    private int processing_output_amount;

    // 搬出待ちの完成品
    private Queue<Item> waiting_output_items = new Queue<Item>();
    private void Update()
    {
        // 加工中のタイマー処理
        if (is_processing)
        {
            processing_timer -= Time.deltaTime;

            // 加工時間が終了した
            if (processing_timer <= 0f)
            {
                is_processing = false;

                Debug.Log("加工完了");

                // 完成品を必要数生成する
                if (processing_output_prefab != null)
                {
                    for (int i = 0; i < processing_output_amount; i++)
                    {
                        Item new_item = Instantiate(
                            processing_output_prefab,
                            transform.position,
                            Quaternion.identity);

                        waiting_output_items.Enqueue(new_item);
                    }
                    Debug.Log("完成品を生成しました：" + processing_output_amount + "個");
                }
                else
                {
                    Debug.LogError("完成品Prefabが取得できていません");
                }
            }
        }

        // 搬出待ちの完成品がある場合
        if (waiting_output_items.Count > 0)
        {
            Try_Output_Item();
        }
    }
    private void Awake()
    {
        material_processing =
            GetComponentInChildren<MaterialProcessing>();

        if (material_processing == null)
        {
            Debug.LogError(
                "MaterialProcessingがPrefab内にありません",
                this
            );
        }
        else
        {
            Debug.Log("MaterialProcessingを取得しました！");
        }
    }

    public override bool Recive_Item(Item item)
    {
        //アイテムが空なら受け取らない
        if(item==null)
        {
            return false;
        }

        //保管数がいっぱいになると受け取らない
        if(input_items.Count>=input_capacity)
        {
            Debug.Log("保管数がいっぱいです");
            return false;
        }

        input_items.Add(item);

        Debug.Log("アイテムを受け取りました:"+item.name);
        Debug.Log("保管数:"+input_items.Count);

        Check_Recipe();

        return true;
    }

    private void Check_Recipe()
    {
        if (material_processing == null)
        {
            Debug.LogWarning("MaterialProcessingが設定されていません");
            return;
        }

        if (input_items.Count == 0) return;

        //最初のアイテムの素材IDを取得
        string materialId = input_items[0].GetItemID().ToString();

        //同じ素材の個数をそろえる
        int material_count = 0;

        foreach (Item item in input_items)
        {
            if(item!=null&&item.GetItemID().ToString()==materialId)
            {
                material_count++;
            }
        }

        if (!material_processing.TryGetRecipe(materialId,out MaterialRecipe recipe))
        {
            Debug.Log("レシピが見つかりません：" + materialId);
            return;
        }
        Debug.Log("素材：" + materialId +" / 現在数：" + material_count +" / 必要数：" + recipe.InputAmount);




        // 必要数が揃ったか確認
        //そろった場合加工を開始する
        if (material_count >= recipe.InputAmount)
        {
            if (is_processing) return;
            // CSVの出力IDをItemIDに変換
            if (!System.Enum.TryParse(
                recipe.OutputMaterialID,
                out Item.ItemID outputID))
            {
                Debug.LogError("出力IDがItemIDに存在しません：" + recipe.OutputMaterialID);
                return;
            }

            // 出力IDからPrefabを取得
            if (!material_processing.TryGetItemPrefab(
                outputID, out Item prefab))
            {
                Debug.LogError("完成品Prefabを取得できません：" + outputID);
                return;
            }

            // 加工完了時に使用する情報を保存
            processing_output_prefab = prefab;
            processing_output_amount = recipe.OutputAmount;

            Debug.Log("今回の完成品：" + outputID);
            Debug.Log("今回の生産数：" + processing_output_amount);


            Debug.Log("レシピの必要素材が揃いました！");

            //必要な素材を消費する
            int consumed_count = 0;

            for (int i = input_items.Count - 1; i >= 0; i--)
            {
                Item item = input_items[i];

                if (item != null &&
                    item.GetItemID().ToString() == materialId)
                {
                    input_items.RemoveAt(i);

                    Destroy(item.gameObject);

                    consumed_count++;

                    if (consumed_count >= recipe.InputAmount)
                    {
                        break;
                    }
                }
            }

            Debug.Log("素材を消費しました：" + consumed_count + "個");

            //加工開始
            is_processing = true;
            processing_timer = processing_time;

            Debug.Log("加工開始！");
        }
    }


    private void Try_Output_Item()
    {
        if (gridsystem == null)
        {
            Debug.LogError("GridMacineplacerが設定されていません");
            return;
        }

        if (waiting_output_items.Count == 0)
        {
            return;
        }

        // Queueの先頭アイテムを取得（まだ削除しない）
        Item item = waiting_output_items.Peek();

        List<Vector2Int> check_cells = Output_Get_Check_Cells();

        foreach (Vector2Int cellpos in check_cells)
        {
            Machine_Base next_machine =
                gridsystem.GetComponentAt<Machine_Base>(cellpos);

            if (next_machine == null)
            {
                continue;
            }

            if ((int)next_machine.InputDir != ((int)OutputDir + 2) % 4)
            {
                continue;
            }

            Debug.Log("出口の機械を発見：" + next_machine.name);

            bool received = next_machine.Recive_Item(item);

            if (received)
            {
                waiting_output_items.Dequeue();

                Debug.Log("完成品を出口に渡しました！");
                Debug.Log("残りの完成品：" + waiting_output_items.Count + "個");

                return;
            }
        }
    }
}


