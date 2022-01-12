using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleHandler : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [TextArea]public string details;          //difficulty info. Time, word count, word length
    Color mouseOverColor;
    Color normalColor;
    public Image highlight;

    public TitleManager tm = TitleManager.instance;

    // Start is called before the first frame update
    void Start()
    {
        mouseOverColor = Color.red;
        normalColor = Color.clear;
        highlight.color = Color.clear;
    }


    public void OnPointerEnter(PointerEventData pointer)
    {
        highlight.color = mouseOverColor; 
        tm.modeDetails.text = details;      
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        highlight.color = normalColor;
        tm.modeDetails.text = "";
    }
}
