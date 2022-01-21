using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class contains all of the special rules. If enabled, various code is executed from here instead of the game manager 
update loop. */
public class SpecialRules : MonoBehaviour
{
    public enum Rule {Reversed, ThreeStrikes, HiddenLetters, WordOverflow, ReducedTime, CaseSensitive}
    public Rule specialRule;

    //instances
    public GameManager gm = GameManager.instance;
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

    public void ExecuteSpecialRule(Rule rule)
    {
        switch(rule)
        {
            case Rule.Reversed:
                //write the target word backwards
                string newWord = "";
                int i = gm.ui.targetWordUI.text.Length;
                while (i > 0)
                {
                    newWord += gm.ui.targetWordUI.text.Substring(i - 1, 1);
                    i--;
                }

                Debug.Log(newWord);
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
