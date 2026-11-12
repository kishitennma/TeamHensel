using UnityEngine;

public class RailTest : MonoBehaviour
{
    [SerializeField] private RailSystem rail;
    [SerializeField] private Item test_item;

    void Start()
    {
        bool result = rail.Recive_Item(test_item);

        Debug.Log("アイテム受け取り結果：" + result);
    }
}
