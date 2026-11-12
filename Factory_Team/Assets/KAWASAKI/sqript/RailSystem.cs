using UnityEngine;

public class RailSystem : Machine_Base
{
    [SerializeField] private float move_speed = 2f;//アイテムを運ぶ速度

    private Item item_carry;//運んでいるアイテムの保存

    private bool is_moving = false;//移動中か

    private Vector3 target_position;

    [SerializeField] private Item item_prefab;

    //アイテムを受け取る処理
    public override bool Recive_Item(Item item)
    {
        if (item_carry != null)
        {
            return false;
        }

        item_carry = item;
        is_moving = true;

        Vector2Int direction = Get_Move_Direction();

        Vector3 moveDirection = new Vector3(
            direction.x,
            direction.y,
            0
        );

        // 次のマスの中心を移動先にする
        target_position = item.transform.position + moveDirection;

        return true;
    }
    // Update is called once per frame
    void Update()
    {
        if(!is_moving||item_carry==null)
        {
            return;
        }

        // 移動先まで移動
        item_carry.transform.position = Vector3.MoveTowards(
            item_carry.transform.position,
            target_position,
            move_speed * Time.deltaTime
        );

        // 移動先に到着したか確認
        if (Vector3.Distance(
            item_carry.transform.position,
            target_position) < 0.001f)
        {
            item_carry.transform.position = target_position;

            Try_Transfer_Item();
        }
    }

    [ContextMenu("アイテム生成テスト")]
    private void SpawnItem()
    {
        // すでにアイテムが乗っていたら生成しない
        if (item_carry != null)
        {
            Debug.Log("このレールには既にアイテムがあります。");
            return;
        }

        // Prefabが設定されていなければ終了
        if (item_prefab == null)
        {
            Debug.LogWarning("アイテムPrefabが設定されていません。");
            return;
        }

        // レールの中央にアイテムを生成
        Item new_item = Instantiate(
            item_prefab,
            transform.position,
            Quaternion.identity
        );

        Debug.Log("アイテムを生成しました：" + new_item.name);


        // レールにアイテムを渡す
        Recive_Item(new_item);
    }


    private void Try_Transfer_Item()
    {
        Debug.Log("受け渡し処理開始");

        Vector2Int direction = Get_Move_Direction();

        // 出力方向の隣のマス
        Vector2Int nextCell = GridPos + direction;
        Debug.Log(
    $"検索先GridMacineplacer：{gridsystem.GetInstanceID()}"
);

        Debug.Log("自分のGridPos：" + GridPos);
        Debug.Log("移動方向：" + direction);
        Debug.Log("探しているマス：" + nextCell);

        Debug.Log("次のマス：" + nextCell);

        // 隣のマスにあるレールを取得
        RailSystem nextRail =
            gridsystem.GetComponentAt<RailSystem>(nextCell);




        if (nextRail == null)
        {
            Debug.Log("次のマスにレールがありません");
            return;
        }

        Debug.Log("次のレールを発見");

        // 入力方向を確認
        Debug.Log("相手のInputDir：" + nextRail.InputDir);
        Debug.Log("こちらのOutputDir：" + OutputDir);

        if ((int)nextRail.InputDir != ((int)OutputDir + 2) % 4)
        {
            Debug.Log("レールの向きが合っていません");
            return;
        }

        bool received = nextRail.Recive_Item(item_carry);

        if (received)
        {
            Debug.Log("次のレールにアイテムを渡しました！");
            item_carry = null;
            is_moving = false;
        }
        else
        {
            Debug.Log("次のレールがアイテムを受け取れませんでした");
        }
    }


    //OutputDirを確認して移動方向を返す処理
    private Vector2Int Get_Move_Direction()
    {
        switch (OutputDir)
        {
            case Direction.UP:
                return Vector2Int.up;

            case Direction.RIGHT:
                return Vector2Int.right;

            case Direction.DOWN:
                return Vector2Int.down;

            case Direction.LEFT:
                return Vector2Int.left;
        }

        return Vector2Int.zero;
    }

}
