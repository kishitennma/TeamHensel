using UnityEngine;

public class EXshop : Machine_Base
{
    private Item item_carry; // 運んでいるアイテム
    private bool is_moving = false;

    private Vector3 target_position;

    [SerializeField] private Item item_prefab;

    // 売却価格を管理するBase
    [SerializeField] private Base baseManager;

    // アイテムを受け取る処理
    public override bool Recive_Item(Item item)
    {
        if (item_carry != null)
        {
            return false;
        }

        item_carry = item;
        is_moving = true;

        return true;
    }

    void Update()
    {
        if (!is_moving || item_carry == null)
        {
            return;
        }

        // アイテムが換金所のマスに到着したか確認
        Vector2Int itemCell = new Vector2Int(
            Mathf.RoundToInt(item_carry.transform.position.x),
            Mathf.RoundToInt(item_carry.transform.position.y)
        );

        Vector2Int shopCell = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        if (itemCell == shopCell)
        {
            SellItem();
        }
    }

    // 換金処理
    void SellItem()
    {
        if (item_carry == null)
        {
            return;
        }

        // アイテムのIDから売却価格を取得
        int price = baseManager.GetSellPrice(item_carry.itemID);

        // 所持金を増やす
        // ここはBase側の所持金変数・加算処理に合わせて追加

        Debug.Log("売却金額：" + price);

        // アイテムを削除
        Destroy(item_carry.gameObject);

        // 初期化
        item_carry = null;
        is_moving = false;
    }

}
