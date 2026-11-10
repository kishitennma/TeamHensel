using UnityEngine;

public class RailSystem : Machine_Base
{
    [SerializeField] private float move_speed = 2f;//アイテムを運ぶ速度

    private Item item_carry;//運んでいるアイテムの保存

    private bool is_moving = false;//移動中か

    //アイテムを受け取る処理
    public bool Recive_Item(Item item)
    {
        if(item_carry!=null)
        {
            return false;
        }

        item_carry = item;
        is_moving = true;

        return true;

    }

    // Update is called once per frame
    void Update()
    {
        if(!is_moving||item_carry==null)
        {
            return;
        }

        Vector2Int direction = Get_Move_Direction();

        Vector3 moveDirection = new Vector3(
            direction.x,
            direction.y,
            0
         );

        item_carry.transform.position += moveDirection * move_speed * Time.deltaTime;

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
