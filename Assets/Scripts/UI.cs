using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//All UI is stored here.
public class UI : MonoBehaviour
{
    [Header("UI")]
    //public GameObject uiHandler;        //controls all UI. Mainly used to create a "shake" effect
    public TextMeshProUGUI scoreUI;
    string filePath;                    //contains location of high score table JSON file
    public TMP_InputField inputField;   //player types words in here
    //public TextMeshProUGUI timerUI;
    public TextMeshProUGUI difficultyUI;
    public TextMeshProUGUI targetWordUI;
    public TextMeshProUGUI resultUI;    //displays result of the typed word, either "Perfect" or "OK" or "Wrong"
    public TextMeshProUGUI comboUI;     //displays combo count, starting at 2 consecutive words
    public TextMeshProUGUI penaltyUI;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
