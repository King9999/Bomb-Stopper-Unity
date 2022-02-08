using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ToggleHandler : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [TextArea]public string details;          //difficulty info. Time, word count, word length
    Color mouseOverColor;
    Color normalColor;
    public Image highlight;

    bool animateHighlightCoroutineOn;
    bool pointerEntered;                //this is used to trigger the coroutine in the update loop.

    public TitleManager tm = TitleManager.instance;

    // Start is called before the first frame update
    void Start()
    {
        mouseOverColor = Color.red;
        normalColor = Color.clear;
        highlight.color = Color.clear;
        animateHighlightCoroutineOn = false;
    }

    void Update()
    {
        if (pointerEntered)
        {
            if (!animateHighlightCoroutineOn)
            {
                animateHighlightCoroutineOn = true;
                StartCoroutine(AnimateHighlight());
            }
        }
    }


    public void OnPointerEnter(PointerEventData pointer)
    {
        highlight.color = mouseOverColor; 
        tm.modeDetails.text = details;
        tm.modeDetailsHandler.SetActive(true);

        //start coroutine
        pointerEntered = true; 
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        highlight.color = normalColor;

        //don't want coroutine to continue
        StopAllCoroutines();
        animateHighlightCoroutineOn = false;
        pointerEntered = false;

        tm.modeDetailsHandler.SetActive(false); 
    }

    //the highlight gradually disappears and re-appears
    IEnumerator AnimateHighlight()
    {

        float alpha = highlight.color.a;

        while(alpha > 0)
        {
            alpha -= 0.8f * Time.deltaTime;
            highlight.color = new Color(highlight.color.r, highlight.color.g, highlight.color.b, alpha);
            yield return null;
        }

        while(alpha < 1)
        {
            alpha += 0.8f * Time.deltaTime;
            highlight.color = new Color(highlight.color.r, highlight.color.g, highlight.color.b, alpha);
            yield return null;
        }

        animateHighlightCoroutineOn = false;
    }
}
