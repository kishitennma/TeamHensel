using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
     // 素材の値段
    private Dictionary<Item.ItemID, int> sellPrices =
        new Dictionary<Item.ItemID, int>()
    {
        { Item.ItemID.wheat, 3 },     //小麦
        { Item.ItemID.Choco, 3 },     //チョコ
        { Item.ItemID.Cake, 20 },     //ケーキ
        { Item.ItemID.Cake, 20 },     //パン
        { Item.ItemID.Cookie, 20 },    //クッキー
        { Item.ItemID.Oreo, 20 },      //オレオ
        { Item.ItemID.dumpling, 30 },   //団子
        { Item.ItemID.Creampuff, 30 }, //シュークリーム
        { Item.ItemID.Macaron, 30 },   //マカロン
        { Item.ItemID.Pudding, 50 },   //プリン
    };

    // 売却価格を取得
    public int GetSellPrice(Item.ItemID id)
    {
        if (sellPrices.ContainsKey(id))
        {
            return sellPrices[id];
        }

        return 0;
    }
}