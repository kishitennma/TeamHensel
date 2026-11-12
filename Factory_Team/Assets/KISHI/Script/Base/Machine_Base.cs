using System.Collections.Generic;
using UnityEngine;

public abstract class Machine_Base : MonoBehaviour
{
    [SerializeField] int output;//アウトプット数
    [SerializeField] int input;//インプット数

    protected GridMacineplacer gridsystem;//グリッド管理クラス

    Machine_Base machine;

    [SerializeField] Item.ItemID o_id;
    [SerializeField] Item.ItemID i_id;

    [SerializeField] private Direction input_dir;//インプットが向いている方向
    [SerializeField] private Direction output_dir;//アウトプットが向いている方向
    [SerializeField] private Direction machine_dir = Direction.RIGHT;//機械自体の向いている方向

    public Direction InputDir => input_dir;
    public Direction OutputDir => output_dir;
    public Direction MachinetDir => machine_dir;

    [SerializeField] Vector2Int size;//機械のサイズ
    Vector2Int grid_pos;//グリッド上での機械のポジションgrid
    public Vector2Int GridPos => grid_pos;


    public enum Direction //方向
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }

    public void Set_GridPos(Vector2Int pos)
    {
        grid_pos = pos;
    }

    /// <summary>
    /// 渡されたDirection型を時計回りで90度ずつ回転
    /// </summary>
    /// <param name="dir">方向</param>
    /// <returns>時計回りに90度回転</returns>
    Direction Rotate_Direction(Direction dir)
    {
            return (Direction)(((int)dir + 1) % 4);
    }

    public void Machine_Rotate()//機械とinput辺/output辺の回転
    {
        machine_dir = Rotate_Direction(machine_dir);
        input_dir = Rotate_Direction(input_dir);
        output_dir = Rotate_Direction(output_dir);
    }

    public Vector2Int Get_Rotate_Size()//回転後の機械のサイズ取得
    {
        if (machine_dir == Direction.UP
         || machine_dir == Direction.DOWN)
        {
            return new Vector2Int(size.y, size.x);
        }
        return size;
    }

    List<Vector2Int> Output_Get_Check_Cells()//Output辺に隣接する座標
    {
        Vector2Int size = Get_Rotate_Size();
        List<Vector2Int> check_sells = new List<Vector2Int>();
        switch (output_dir)
        {
            case Direction.UP:
                for (int i = 0; i < size.x; i++)
                {
                    check_sells.Add(new Vector2Int(
                        grid_pos.x + i,
                        grid_pos.y + size.y));
                }
                break;
            case Direction.RIGHT:
                for (int i = 0; i < size.y; i++)
                {
                    check_sells.Add(new Vector2Int(
                        grid_pos.x + size.x,
                        grid_pos.y + i));
                }
                break;
            case Direction.DOWN:
                for (int i = 0; i < size.x; i++)
                {
                    check_sells.Add(new Vector2Int(
                        grid_pos.x + i,
                        grid_pos.y - 1));
                }
                break;
            case Direction.LEFT:
                for (int i = 0; i < size.y; i++)
                {
                    check_sells.Add(new Vector2Int(
                        grid_pos.x - 1,
                        grid_pos.y + i));
                }
                break;
        }

        return check_sells;
    }

    List<Vector2Int> Input_Get_Check_Cells()//Output辺に隣接する座標
    {
        Vector2Int size = Get_Rotate_Size();
        List<Vector2Int> check_sells = new List<Vector2Int>();
        switch (input_dir)
        {
            case Direction.UP:
                for (int i = 0; i < size.x; i++)
                {
                    check_sells.Add(new Vector2Int(
                        grid_pos.x + i,
                        grid_pos.y + size.y));
                }
                break;
            case Direction.RIGHT:
                for (int i = 0; i < size.y; i++)
                {
                    check_sells.Add(new Vector2Int(
                        grid_pos.x + size.x,
                        grid_pos.y + i));
                }
                break;
            case Direction.DOWN:
                for (int i = 0; i < size.x; i++)
                {
                    check_sells.Add(new Vector2Int(
                        grid_pos.x + i,
                        grid_pos.y - 1));
                }
                break;
            case Direction.LEFT:
                for (int i = 0; i < size.y; i++)
                {
                    check_sells.Add(new Vector2Int(
                        grid_pos.x - 1,
                        grid_pos.y + i));
                }
                break;
        }

        return check_sells;
    }
    /// <summary>
    /// output辺に隣接しているセルを検索
    /// </summary>
    public void Output_Check()
    {
        List<Vector2Int> output_check_cells = Output_Get_Check_Cells();

        foreach (Vector2Int cellpos in output_check_cells)
        {
            machine = gridsystem.GetComponentAt<Machine_Base>(cellpos);

            if (machine == null)
            {
                Debug.Log("x:" + cellpos.x + ",y:" + cellpos.y + "が空");
                continue;
            }

            if ((int)machine.InputDir == ((int)OutputDir + 2) % 4)
            {

            }


        }
    }

    public virtual bool Recive_Item(Item item)
    {
        return false;
    }
    ///// <summary>
    ///// input辺に隣接しているセルを検索
    ///// </summary>
    //public void Input_Check()
    //{
    //    List<Vector2Int> input_check_cells = Input_Get_Check_Cells();

    //    foreach(Vector2Int cellpos in input_check_cells)
    //    {
    //        machine = gridsystem.GetComponentAt<Machine_Base>(cellpos);
    //        if ((int)machine.OutputDir == ((int)InputDir + 2) % 4)
    //        {

    //        }
    //    }
    //}

}
