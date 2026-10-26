using UnityEngine;

public abstract class Machine_Base : MonoBehaviour
{
    int output;
    int input;

    Direction input_dir;
    Direction output_dir;
    Direction machine_dir;



    enum Direction
    {
        Up,
        Right,
        Down,
        Left,
    }

}
