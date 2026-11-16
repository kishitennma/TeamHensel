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
        if (material_processing != null)
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

        if(!material_processing.TryGetRecipe(materialId,out MaterialRecipe recipe))
        {
            Debug.Log("レシピが見つかりません：" + materialId);
            return;
        }
        Debug.Log("素材：" + materialId +" / 現在数：" + material_count +" / 必要数：" + recipe.InputAmount);

        // 必要数が揃ったか確認
        if (material_count >= recipe.InputAmount)
        {
            Debug.Log("レシピの必要素材が揃いました！");
        }

    }
}


