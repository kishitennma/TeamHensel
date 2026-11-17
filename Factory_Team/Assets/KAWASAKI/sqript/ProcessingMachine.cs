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

    //加工後のアイテムPrefab
    [SerializeField]
    private Item output_item_prefab;

    private Item waiting_output_item;

    private void Update()
    {
        // 加工中ならタイマーを進める
        if (is_processing)
        {
            processing_timer -= Time.deltaTime;

            if (processing_timer <= 0f)
            {
                is_processing = false;

                Debug.Log("加工完了！");

                if (output_item_prefab != null)
                {
                    waiting_output_item = Instantiate(
                        output_item_prefab,
                        transform.position,
                        Quaternion.identity
                    );

                    Debug.Log("完成品を生成しました：" + waiting_output_item.name);
                }
                else
                {
                    Debug.LogError("完成品Prefabが設定されていません");
                }
            }
        }

        // 完成品があれば、出口のレールへ渡す
        if (waiting_output_item != null)
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

        // 加工機の出口に隣接するマスを取得
        List<Vector2Int> check_cells = Output_Get_Check_Cells();

        foreach (Vector2Int cellpos in check_cells)
        {
            Machine_Base next_machine =
                gridsystem.GetComponentAt<Machine_Base>(cellpos);

            if (next_machine == null)
            {
                continue;
            }

            // 相手のInputDirと加工機のOutputDirが合っているか確認
            if ((int)next_machine.InputDir != ((int)OutputDir + 2) % 4)
            {
                continue;
            }

            Debug.Log("出口の機械を発見：" + next_machine.name);

            bool received = next_machine.Recive_Item(waiting_output_item);

            if (received)
            {
                Debug.Log("完成品を出口の機械に渡しました！");
                waiting_output_item = null;
                return;
            }

            Debug.Log("出口の機械が受け取れませんでした");
        }
    }
}


