using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

//This script must be attached to a Button UI object.
public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image buttonImage;                        //using this to change button colour when mouse hovers over it
    [TextArea]public string details;          //difficulty info. Time, word count, word length
    Color mouseOverColor;
    Color normalColor;

    bool animateButtonCoroutineOn;
    bool pointerEntered;                //this is used to trigger the coroutine in the update loop.

    public TitleManager tm = TitleManager.instance;

    // Start is called before the first frame update
    void Start()
    {
        mouseOverColor = Color.red;
        normalColor = Color.clear;
        buttonImage = GetComponent<Image>();
    }

    void Update()
    {
        if (pointerEntered)
        {
            if (!animateButtonCoroutineOn)
            {
                animateButtonCoroutineOn = true;
                StartCoroutine(AnimateButton());
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointer)
    {
        buttonImage.color = mouseOverColor; 
        tm.modeDetails.text = details;
        tm.modeDetailsHandler.SetActive(true);

         //start coroutine
        pointerEntered = true;       
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        buttonImage.color = normalColor;

        //don't want coroutine to continue
        StopAllCoroutines();
        animateButtonCoroutineOn = false;
        pointerEntered = false;

        tm.modeDetailsHandler.SetActive(false); 
    }

    IEnumerator AnimateButton()
    {
        float alpha = buttonImage.color.a;

        while(alpha > 0)
        {
            alpha -= 0.8f * Time.deltaTime;
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, alpha);
            yield return null;
        }

        while(alpha < 1)
        {
            alpha += 0.8f * Time.deltaTime;
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, alpha);
            yield return null;
        }

        animateButtonCoroutineOn = false;
    }
}
