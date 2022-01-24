using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* This class contains all of the special rules. If enabled, various code is executed from here instead of the game manager 
update loop. */
public class SpecialRules : MonoBehaviour
{
    public enum Rule {None, Reversed, ThreeStrikes, HiddenLetters, WordOverflow, ReducedTime, Invisible, CaseSensitive}
    public Rule specialRule;
    int TotalRules {get;} = 7;
    public TextMeshProUGUI ruleName;
    string[] ruleNames;
    

    //variables for specific rules
    [Header("'Reversed' variables")]
    public GameObject reversedRuleContainer;
    public bool wordReversed;
    float reverseRate;              //chance that a target word is spelled backwards
    float initRate {get;} = 0.3f;
    public string originalWord;     //player must type this word to score. This variable is also used with the Hidden Letter rule.
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

    [Header("'Hidden Letters' variables")]
    public int totalHiddenLetters;

    [Header("Word Overflow variables")]
    public int overflowAmount;          //amount of extra target words added. 50% of the initial target.
    [HideInInspector]public string startColor = "<color=#00F0FF>";  //light blue
    [HideInInspector]public string endColor = "</color>";

    [Header("Reduced time variables")]
    public GameObject reducedTimeRuleContainer;
    float addedTime;                    //how much time in seconds is added to timer. Formula is base added time + (number of letters * 0.1 * perfect modifier)
    float baseAddedTime {get;} = 1;
    float perfectMod {get;} = 2;
    public bool timeAdded;              //used to prevent time being added more than once per frame.
    public float elapsedTime;           //need this because the elapsed time in the normal game uses different calculations
    public TextMeshProUGUI addedTimeUI;
    bool animateAddedTimeCoroutineOn;

    [Header("Invisible variables")]
    //string maskedWord;                  //all text in input field is replaced with this
    [HideInInspector]public string originalTypedWord;
    public List<char> wordCopy;
    public int wordCopyIndex;
    public string alphabet {get;} = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";   //used to find what player types

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
            ruleNames[5] = "Invisible";
            ruleNames[6] = "CaSe SeNsItive";


            //get a random rule
            //int randRule = Random.Range(1, TotalRules + 1); //ignoring the "none" rule at index 0
            //int randRule = Random.Range(1, 6);
            //specialRule = (Rule)randRule;
            specialRule = Rule.Invisible;
            ruleName.text = ruleNames[(int)specialRule - 1];    //I subtract 1 because I don't have a 6th index in ruleNames

            if (specialRule == Rule.Reversed)
            {
                //I've changed this rule so that the reverse rate will increase whenever a word isn't reversed. The rate will reset once
                //a word is modified.
                reverseRate = initRate;
            }

            //enable appropriate assets for certain rules as required
            else if (specialRule == Rule.ThreeStrikes)
            {
                threeStrikesRuleContainer.SetActive(true);
                redLight.gameObject.SetActive(false);   //don't need this right away
                greenLight.gameObject.SetActive(false);
            }

            else if (specialRule == Rule.WordOverflow)
            {
                //reduce text size so it fits in the window
                ruleName.fontSize -= 6;
            }

            else if (specialRule == Rule.ReducedTime)
            {
                reducedTimeRuleContainer.SetActive(true);
                addedTimeUI.gameObject.SetActive(false);    //hidden by default
            }

            else if (specialRule == Rule.Invisible)
            {
                wordCopyIndex = 0;
                wordCopy = new List<char>();
                wordCopy.Add('_');  //must add a space in order to mimic the input field exactly. Any letters must be added in front of the space.
            }
        }
    }

    public void ExecuteSpecialRule(Rule rule)
    {
        switch(rule)
        {
            case Rule.Reversed:
                //write the target word backwards. Player must type word with the normal spelling
                //reverseRate = 0.3f;
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
                    reverseRate = initRate;
                }
                else
                {
                    wordReversed = false;
                    reversedRuleContainer.SetActive(false);
                    reverseRate += 0.02f;   //goes up 2%
                }
                break;

            case Rule.ThreeStrikes:
                //turn one of the lights red
                lightChanged = true;
                break;
            
            case Rule.HiddenLetters:
                //check how many letters are hidden
                originalWord = gm.ui.targetWordUI.text;
                totalHiddenLetters = originalWord.Length / 4;    //minimum should be 1.
                Debug.Log("Hidden Letters: " + totalHiddenLetters);

                //create the new word
                string newWord = "";
                float successRate = 0.5f;          //the odds that a letter is hidden.
                int lettersHidden = 0;

                int j = 0;
                while (j < originalWord.Length)
                {
                    //check each letter that isn't the first or the last one, and determine if it should be hidden
                    if (lettersHidden < totalHiddenLetters && j != 0 && j != originalWord.Length - 1)
                    {
                        float hideChance = Random.Range(0, 1f);
                        if (hideChance <= successRate)
                        {
                            //hide this letter
                            newWord += "?";
                            lettersHidden++;
                            successRate = 0.5f;     //I reset to try to spread out the hidden letters.
                        }
                        else
                        {
                            newWord += originalWord.Substring(j, 1);
                            successRate += 0.1f;    //increase likelhood of the next letter being hidden.
                        }
                    }
                    else
                    {
                        newWord += originalWord.Substring(j, 1);
                    }

                    j++;
                }

                //display the updated word
                gm.ui.targetWordUI.text = newWord;
                break;

            case Rule.WordOverflow:
                overflowAmount = gm.totalWordCount / 2;
                gm.totalWordCount += overflowAmount;

                //change colour of target amount
                string startColor = "<color=#00F0FF>";  //light blue
                string endColor = "</color>";
                gm.ui.wordCountValueUI.text = gm.currentWordCount + "/" + startColor + gm.totalWordCount + endColor;
                break;

            case Rule.ReducedTime:
                //add time based on the word & conditions
                float mod = gm.correctionWasMade ? 1 : perfectMod;
                addedTime = baseAddedTime + (gm.ui.targetWordUI.text.Length * 0.1f * mod);
                gm.gameTimer.time += addedTime;
                addedTimeUI.text = "+" + addedTime + " sec.";
                Debug.Log("Time added: " + addedTime);
                break;

            case Rule.Invisible:
                int k = 0;
                //originalTypedWord = gm.ui.inputField.text;
                string maskedWord = "";

                //mask the word
                while (k < gm.ui.inputField.text.Length)
                {
                    maskedWord += "*";
                    k++;
                }

                gm.ui.inputField.text = maskedWord;
                //Debug.Log("Original word: " + originalTypedWord);
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

        //Reduced Time rule
        if (specialRule == Rule.ReducedTime)
        {
            elapsedTime += Time.deltaTime;
        }

        if (timeAdded)
        {
            if (!animateAddedTimeCoroutineOn)
            {
                animateAddedTimeCoroutineOn = true;
                StartCoroutine(AnimateAddedTime());
            }
        }
    }

    //Used with Reduced Time rule only
    public string DisplayElapsedTime()
    {
        float minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = Mathf.FloorToInt(elapsedTime % 60);

        string timeText = string.Format("{0:0}:{1:00}", minutes, seconds);

        return timeText;
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

    //Used with Reduced Time rule. 
    IEnumerator AnimateAddedTime()
    {
        Vector3 originalPos = addedTimeUI.transform.position;
        float duration = 0.5f;
        float currentTime = Time.time;
        float distance = 20;
        addedTimeUI.gameObject.SetActive(true);

        while(Time.time < currentTime + duration)
        {
            addedTimeUI.transform.position = new Vector3(addedTimeUI.transform.position.x, addedTimeUI.transform.position.y - distance * Time.deltaTime,
             addedTimeUI.transform.position.z);
            yield return null;
        }

        //time fades gradually after it reaches its position.
        while(addedTimeUI.alpha > 0)
        {
            addedTimeUI.alpha -= 2 * Time.deltaTime;
            yield return null;
        }
        
        //reset
        addedTimeUI.alpha = 1;
        addedTimeUI.transform.position = originalPos;
        addedTimeUI.gameObject.SetActive(false);
        animateAddedTimeCoroutineOn = false;
    }
}
