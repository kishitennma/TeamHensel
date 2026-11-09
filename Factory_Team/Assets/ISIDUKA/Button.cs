using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject main_panel;
    [SerializeField] GameObject rail_panel;
    [SerializeField] GameObject production_panel;
    [SerializeField] GameObject processing_panel;
    [SerializeField] GameObject synthesis_panel;
    [SerializeField] GameObject buy_panel;
    [SerializeField] GameObject all_panel;
    [SerializeField] GameObject release_button;


    public void Main_Panel()
    {
        main_panel.SetActive(!main_panel.activeSelf);
    }

    public void Rail_Panel()
    {
        rail_panel.SetActive(true);

        production_panel.SetActive(false);
        processing_panel.SetActive(false);
        synthesis_panel.SetActive(false);
    }
    public void Production_Panel()
    {
        production_panel.SetActive(true);

        rail_panel.SetActive(false);
        processing_panel.SetActive(false);
        synthesis_panel.SetActive(false);
    }
    public void Processing_Panel()
    {
        processing_panel.SetActive(true);

        rail_panel.SetActive(false);
        production_panel.SetActive(false);
        synthesis_panel.SetActive(false);
    }
    public void Synthesis_Panel()
    {
        synthesis_panel.SetActive(true);

        rail_panel.SetActive(false);
        production_panel.SetActive(false);
        processing_panel.SetActive(false);
    }
    public void Buy_Panel()
    {
        buy_panel.SetActive(!buy_panel.activeSelf);
    }

    public void Modeswitch()
    {
        all_panel.SetActive(false);
        release_button.SetActive(true);
    }
    public void Release()
    {
        all_panel.SetActive(true);
        release_button.SetActive(false);
    }
}
