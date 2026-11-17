using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class F_Machine : Machine_Base
{
    List<Item> input_items;
    Item output_item;

    float process_time;
    float timer;

    public override bool Recive_Item(Item item)
    {
        if(input_items.Count >= input)
        {
            return false;
        }

        input_items.Add(item);

        if(input_items.Count == input)
        {
            Recipe_Search(input_items,out output_item.itemID);
        }

        return true;
    }

    bool Recipe_Search(List<Item> items, out Item.ItemID result_id)
    {

        if (true)
            result_id = Item.ItemID.Braed;
        return true;
        if (false)
            result_id = Item.ItemID.Oreo;
        return false;
    }

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
            bool received = neighbor.Recive_Item(output_item);

            if (received)
            {
                Debug.Log("アイテムを渡しました：" + neighbor.name);

                // 受け渡し成功したので保持アイテムを解除
                output_item = null;
                return;
            }
        }
    }

}
