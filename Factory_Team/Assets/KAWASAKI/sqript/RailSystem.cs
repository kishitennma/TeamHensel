using UnityEngine;

public class RailSystem : Machine_Base
{
    [SerializeField] private float move_speed = 2f;//アイテムを運ぶ速度

    private Item item_carry;//運んでいるアイテムの保存

    private bool is_moving = false;//移動中か

    private Vector3 target_position;

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

    private void Try_Transfer_Item()
    {
        Vector2Int direction = Get_Move_Direction();

        // 出力方向の隣のマス
        Vector2Int nextCell = GridPos + direction;

        // 隣のマスにあるレールを取得
        RailSystem nextRail =
            gridsystem.GetComponentAt<RailSystem>(nextCell);

        if (nextRail == null)
        {
            return;
        }

        // 相手の入力方向がこちらの出力方向と反対か確認
        if ((int)nextRail.InputDir != ((int)OutputDir + 2) % 4)
        {
            return;
        }

        // 相手が受け取れるか確認
        bool received = nextRail.Recive_Item(item_carry);

        if (received)
        {
            // 受け渡し
            item_carry = null;
            is_moving = false;
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
