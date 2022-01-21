using UnityEngine;
using TMPro;

/* This class contains all of the special rules. If enabled, various code is executed from here instead of the game manager 
update loop. */
public class SpecialRules : MonoBehaviour
{
    public enum Rule {None, Reversed, ThreeStrikes, HiddenLetters, WordOverflow, ReducedTime, CaseSensitive}
    public Rule specialRule;
    int TotalRules {get;} = 6;
    public TextMeshProUGUI ruleName;
    string[] ruleNames;
    public GameObject ruleDataContainer;

    //variables for specific rules
    //Reversed
    public bool wordReversed;
    float reverseRate;          //chance that a target word is spelled backwards
    public string originalWord; //player must type this word to score.

    //Three Strikes
    public int strikes;
    public int MaxStrikes {get;} = 3;

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

        if (tm.specialToggle.isOn)
        {
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
            specialRule = Rule.Reversed;
            ruleName.text = ruleNames[randRule];
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
                }
                else
                {
                    wordReversed = false;
                }
                break;

            case Rule.ThreeStrikes:
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
}
