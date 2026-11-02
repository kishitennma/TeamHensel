using UnityEngine;

public class MachineBaseTest : MonoBehaviour
{
    [Header("機械の占有サイズ")]
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;

    public int Width => width;
    public int Height => height;
}
