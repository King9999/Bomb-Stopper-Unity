using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//All UI is stored here.
public class UI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreUI;
    public TMP_InputField inputField;   //player types words in here
    public TextMeshProUGUI difficultyUI;
    public TextMeshProUGUI targetWordUI;
    public TextMeshProUGUI resultUI;    //displays result of the typed word, either "Perfect" or "OK" or "Wrong"
    public TextMeshProUGUI comboTextUI;     
    public TextMeshProUGUI comboValueUI;    //displays combo count, starting at 2 consecutive words
    
    public TextMeshProUGUI penaltyUI;   //displays how much time is added to penalty per incorrect letter.
    public TextMeshProUGUI wordCountUI; //total number of words that need to be completed to finish the stage.

    [Header("Sliders")]
    public Slider stunMeter;
    public Slider comboMeter;               //combo duration

    [Header("Handlers")]
    public GameObject stunMeterHandler; //need this to hide stun meter when necessary.
    public GameObject comboHandler;     //manages all UI pertaining to combos            

    public static UI instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);    //Only want one instance of UI
            return;
        }

        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //stunMeter.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ReduceStunMeter()
    {
        while (stunMeter.value > 0)
        {
            stunMeter.value -= Time.deltaTime;
            yield return null;
        }

        stunMeter.enabled = false;
    }
}
