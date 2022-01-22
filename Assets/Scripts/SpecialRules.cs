using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/* This class contains all of the special rules. If enabled, various code is executed from here instead of the game manager 
update loop. */
public class SpecialRules : MonoBehaviour
{
    public enum Rule {None, Reversed, ThreeStrikes, HiddenLetters, WordOverflow, ReducedTime, CaseSensitive}
    public Rule specialRule;
    int TotalRules {get;} = 6;
    public TextMeshProUGUI ruleName;
    string[] ruleNames;
    

    //variables for specific rules
    [Header("'Reversed' variables")]
    public GameObject reversedRuleContainer;
    public bool wordReversed;
    float reverseRate;          //chance that a target word is spelled backwards
    public string originalWord; //player must type this word to score.
    public TextMeshProUGUI reverseIndicator;    //alerts player that a word is backwards
    public Image reverseArrow;      //shows the direction a word must be read and typed
    bool animateArrowCoroutineOn;

    //Three Strikes
    [Header("'3 Strikes' variables")]
    public GameObject threeStrikesRuleContainer;

    public int lightIndex;      //this is the iterator for the arrays below
    public int MaxStrikes {get;} = 3;
    public Image[] strikeLights;
    bool lightChanged;
    bool changeStrikeLightCoroutineOn;
    public Image redLight;
    public Image greenLight;

    //instances
    public GameManager gm = GameManager.instance;
    TitleManager tm = TitleManager.instance;
    public static SpecialRules instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        instance = this;
    }

    void Start()
    {
        specialRule = Rule.None;    //default rule
        gameObject.SetActive(false);    //default state

        if (tm.specialToggle.isOn)
        {
            gameObject.SetActive(true);

            ruleNames = new string[TotalRules];
            ruleNames[0] = "Reversed";
            ruleNames[1] = "3 Strikes";
            ruleNames[2] = "Hidden Letters";
            ruleNames[3] = "Words Overflowing";
            ruleNames[4] = "Reduced Time";
            ruleNames[5] = "CaSe SeNsItive";


            //get a random rule
            int randRule = Random.Range(1, TotalRules + 1); //ignoring the "none" rule at index 0
            //specialRule = (Rule)randRule;
            specialRule = Rule.ThreeStrikes;
            ruleName.text = ruleNames[(int)specialRule - 1];    //I subtract 1 because I don't have a 6th index in ruleNames

            //enable appropriate assets for certain rules as required
            if (specialRule == Rule.ThreeStrikes)
            {
                threeStrikesRuleContainer.SetActive(true);
                redLight.gameObject.SetActive(false);   //don't need this right away
                greenLight.gameObject.SetActive(false);
            }
        }
    }

    public void ExecuteSpecialRule(Rule rule)
    {
        switch(rule)
        {
            case Rule.Reversed:
                //write the target word backwards. Player must type word with the normal spelling
                reverseRate = 0.3f;
                float chance = Random.Range(0, 1f);
                if (chance <= reverseRate)
                {
                    wordReversed = true;

                    string reversedWord = "";
                    originalWord = gm.ui.targetWordUI.text;
                    int i = gm.ui.targetWordUI.text.Length;
                    while (i > 0)
                    {
                        reversedWord += gm.ui.targetWordUI.text.Substring(i - 1, 1);
                        i--;
                    }

                    gm.ui.targetWordUI.text = reversedWord;
                    Debug.Log(reversedWord);

                    //alert player of reversed word
                    reversedRuleContainer.SetActive(true);
                }
                else
                {
                    wordReversed = false;
                    reversedRuleContainer.SetActive(false);
                }
                break;

            case Rule.ThreeStrikes:
                //turn one of the lights red
                lightChanged = true;
                //strikeLights[lightIndex].sprite = redLight.sprite;
                //lightIndex++;

                /*if (lightIndex >= MaxStrikes)
                {
                    //the end
                    gm.gameOver = true;
                }*/
                break;
            
            case Rule.HiddenLetters:
                break;

            case Rule.WordOverflow:
                break;

            case Rule.ReducedTime:
                break;

            case Rule.CaseSensitive:
                break;

            default:
                break;
        }
    }

    void Update()
    {
        /*************COROUTINE CHECKS FOR VARIOUS RULES*************/
        //'Reversed' rule
        if (wordReversed)
        {
            if (!animateArrowCoroutineOn)
            {
                animateArrowCoroutineOn = true;
                StartCoroutine(AnimateReverseArrow());
            }
        }

        //Three Strikes rule
        if (lightChanged)
        {
            if (!changeStrikeLightCoroutineOn)
            {
                changeStrikeLightCoroutineOn = true;
                StartCoroutine(ChangeStrikeLight());
            }
        }
    }

    //moves the reverse arrow to alert player
    IEnumerator AnimateReverseArrow()
    {
        Vector3 originalPos = reverseArrow.transform.position;
        float duration = 0.5f;
        float currentTime = Time.time;
        float travelRate = 40;

        while(Time.time < currentTime + duration)
        {
            reverseArrow.transform.position = new Vector3(reverseArrow.transform.position.x - travelRate * Time.deltaTime, reverseArrow.transform.position.y, 
                reverseArrow.transform.position.z);
            yield return null;
        }
        
        reverseArrow.transform.position = originalPos;
        animateArrowCoroutineOn = false;
    }

    //light flashes green and red for a duration and then stays red.
    IEnumerator ChangeStrikeLight()
    {
        float duration = 0.5f;
        float currentTime = Time.time;

        while (Time.time < currentTime + duration)
        {
            if(strikeLights[lightIndex].sprite != redLight.sprite)
                strikeLights[lightIndex].sprite = redLight.sprite;
            else
                strikeLights[lightIndex].sprite = greenLight.sprite;
            yield return new WaitForSeconds(0.05f);
        }

        //turn light red & check for game over
        strikeLights[lightIndex].sprite = redLight.sprite;
        lightIndex++;

        if (lightIndex >= MaxStrikes)
        {
            //the end
            gm.gameOver = true;
        }

        lightChanged = false;
        changeStrikeLightCoroutineOn = false;
    }
}
