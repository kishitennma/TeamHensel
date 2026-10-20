using UnityEngine;

public class MaterialConvertTest : MonoBehaviour
{
    [SerializeField] private MaterialProcessing processing;

    private void Start()
    {
        if (processing.TryConvert(
            "Wheat",
            3,
            out string outputId,
            out int outputCount,
            out int remainder))
        {
            Debug.Log($"•ÏŠ·Œ‹‰Ê: {outputId} x {outputCount}");
            Debug.Log($"c‚Á‚½‘fŞ: Wheat x {remainder}");
        }
        else
        {
            Debug.Log("•ÏŠ·‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½");
        }
    }
}

