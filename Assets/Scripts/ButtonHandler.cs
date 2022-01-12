using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//This script must be attached to a Button UI object.
public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image buttonImage;              //using this to change button colour when mouse hovers over it
    [TextArea]public string details;          //difficulty info. Time, word count, word length
    Color mouseOverColor;
    Color normalColor;

    public TitleManager tm = TitleManager.instance;

    // Start is called before the first frame update
    void Start()
    {
        mouseOverColor = Color.red;
        normalColor = new Color(1, 1, 1, 0);
        buttonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData pointer)
    {
        buttonImage.color = mouseOverColor; 
        tm.modeDetails.text = details;      
        Debug.Log("Hovering over easy button");       
        //Debug.Log("Hovering over button");
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        buttonImage.color = normalColor;
        Debug.Log("Moved off button");
    }
}
