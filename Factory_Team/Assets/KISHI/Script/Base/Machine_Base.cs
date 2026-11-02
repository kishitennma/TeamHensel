using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class Machine_Base : MonoBehaviour
{
    [SerializeField] int output;//アウトプット数
    [SerializeField] int input;//インプット数

    [SerializeField] private Direction input_dir;//インプットが向いている方向
    [SerializeField] private Direction output_dir;//アウトプットが向いている方向
    [SerializeField] private Direction machine_dir = Direction.RIGHT;//機械自体の向いている方向

    public Direction InputDir => input_dir;
    public Direction OutputDir => output_dir;
    public Direction MachinetDir => machine_dir;

    [SerializeField]Vector2Int size;//機械のサイズ
    Vector2Int grid_pos;//グリッド上での機械のポジション



    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }

    void Rotate_Direction(Direction dir)
    {
        if (dir == Direction.LEFT)
            dir = Direction.UP;
        else
            dir++;
    }

    public void Machine_Rotate()
    {
        Rotate_Direction(machine_dir);
        Rotate_Direction(input_dir);
        Rotate_Direction(output_dir);
    }

}
