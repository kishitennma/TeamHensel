using UnityEngine;
using static Item;

public class Item : MonoBehaviour
{

          public enum ItemID
        {
            Wheat = 0,//小麦
            Choco = 1,//チョコ
            Cake = 2,//ケーキ
            Braed = 3,//パン
            Cookie = 4,//クッキー
            Oreo = 5,//オレオ
            dumpling = 6,//団子
            Creampuff = 7,//シュークリーム
            Macaron = 8,//マカロン
            Pudding = 9,//プリン
            flour = 10,//小麦粉
    }

    public ItemID itemID;

    //アイテムの種類を取得する
    public ItemID GetItemID()
    {
        return itemID;
    }

    public void SetItemID(ItemID id)
    {
        itemID = id;
    }
}
