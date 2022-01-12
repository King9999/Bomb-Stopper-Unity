using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    Image buttonImage;
    Color mouseOverColor;
    Color normalColor;
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
        Debug.Log("Hovering over button");
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        buttonImage.color = normalColor;
        Debug.Log("Moved off button");
    }
}
