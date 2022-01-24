using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public TextMeshProUGUI scoreValueUI;
    public TextMeshProUGUI pointValueUI;    //amount of points for completed word
    public TextMeshProUGUI wordCountValueUI;
    public Button returnButton;         //sends player back to title screen.

    [Header("Sliders")]
    public Slider stunMeter;
    public Slider comboMeter;               //combo duration
    public Slider screenTransition;

    [Header("Handlers")]
    public GameObject stunMeterHandler; //need this to hide stun meter when necessary.
    public GameObject comboHandler;     //manages all UI pertaining to combos
    public GameObject scoreHandler;
    public GameObject wordCountHandler;            

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

    /*void Update()
    {
        int i = 0;
        string hiddenWord = "";
        while (i < inputField.text.Length)
        {
            hiddenWord += "*";
            i++;
        }

        inputField.text = hiddenWord;
    }*/

    IEnumerator ReduceStunMeter()
    {
        while (stunMeter.value > 0)
        {
            stunMeter.value -= Time.deltaTime;
            yield return null;
        }

        stunMeter.enabled = false;
    }

    public void OnReturnButtonClicked()
    {
        StartCoroutine(ChangeToScreen("Title"));
        //SceneManager.LoadScene("Title");
    }

//This screen transition uses a slider to wipe the screen in a radial pattern like a clock.
    IEnumerator ChangeToScreen(string newScene)
    {
        while (screenTransition.value < 1)
        {
            screenTransition.value += Time.deltaTime * 2;
            yield return null;
        }

        SceneManager.LoadScene(newScene);

    }
}
