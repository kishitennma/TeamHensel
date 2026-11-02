using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject main_panel;
    [SerializeField] GameObject rail_panel;
    [SerializeField] GameObject production_panel;
    [SerializeField] GameObject processing_panel;
    [SerializeField] GameObject synthesis_panel;


    public void Main_Panel()
    {
        main_panel.SetActive(!main_panel.activeSelf);
    }

    public void Rail_Panel()
    {
        rail_panel.SetActive(!rail_panel.activeSelf);
        production_panel.SetActive(!production_panel.activeSelf);
        processing_panel.SetActive(!processing_panel.activeSelf);
        synthesis_panel.SetActive(!synthesis_panel.activeSelf);
    }
    public void Production_Panel()
    {
        rail_panel.SetActive(!rail_panel.activeSelf);
        production_panel.SetActive(!production_panel.activeSelf);
        processing_panel.SetActive(!processing_panel.activeSelf);
        synthesis_panel.SetActive(!synthesis_panel.activeSelf);


    }
    public void Processing_Panel()
    {
        rail_panel.SetActive(!rail_panel.activeSelf);
        production_panel.SetActive(!production_panel.activeSelf);
        processing_panel.SetActive(!processing_panel.activeSelf);
        synthesis_panel.SetActive(!synthesis_panel.activeSelf);
    }
    public void Synthesis_Panel()
    {
        rail_panel.SetActive(!rail_panel.activeSelf);
        production_panel.SetActive(!production_panel.activeSelf);
        processing_panel.SetActive(!processing_panel.activeSelf);
        synthesis_panel.SetActive(!synthesis_panel.activeSelf);
    }
}
