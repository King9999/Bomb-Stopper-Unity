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
    public TextMeshProUGUI readyText;
    public Image readyBackground;

    [Header("Sliders")]
    public Slider stunMeter;
    public Slider comboMeter;               //combo duration
    public Slider screenTransition;

    [Header("Handlers")]
    public GameObject stunMeterHandler; //need this to hide stun meter when necessary.
    public GameObject comboHandler;     //manages all UI pertaining to combos
    public GameObject scoreHandler;
    public GameObject wordCountHandler;   
    public GameObject startGameHandler;        

    //instances
    public static UI instance;
    GameManager gm;     

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);    //Only want one instance of UI
            return;
        }

        instance = this;
    }

    void Start()
    {
        gm = GameManager.instance; 
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

    //displays the ready background and text and begins stage when animation is finished.
    public IEnumerator BeginStage()
    {
        //shrink the ready text and fade it
        float duration = 1f;
        float currentTime = Time.time;

        while(Time.time < currentTime + duration)
        {
            readyText.alpha -= Time.deltaTime;
            float scaleRate = 0.2f * Time.deltaTime;
            readyText.transform.localScale = new Vector3(readyText.transform.localScale.x - scaleRate, readyText.transform.localScale.y - scaleRate, readyText.transform.localScale.z);
            yield return null;
        }

        //Display "GO!"
        readyText.alpha = 1;
        readyText.fontSize = 160;
        readyText.transform.localScale = new Vector3(1,1,1);
        readyText.text = "GO!";

        //expand briefly, then disappear
        currentTime = Time.time;
        duration = 0.5f;
        while (Time.time < currentTime + duration)
        {
            float scaleRate = 0.6f * Time.deltaTime;
            readyText.transform.localScale = new Vector3(readyText.transform.localScale.x + scaleRate, readyText.transform.localScale.y + scaleRate, readyText.transform.localScale.z);
            readyText.alpha -= 2 * Time.deltaTime;
            yield return null;
        }

        //start game
        startGameHandler.SetActive(false);
        inputField.ActivateInputField();
        gm.gameTimer.StartTimer();
        gm.gameStarted = true;
        
    }
}
