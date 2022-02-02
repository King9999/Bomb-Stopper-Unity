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

    //coroutine bools
    [HideInInspector]public bool resultCoroutineOn;
    [HideInInspector]public bool stunCoroutineOn;
    [HideInInspector]public bool comboCountdownCoroutineOn;
    [HideInInspector]public bool animatePointsCoroutineOn;

    //other
    [HideInInspector]public Color perfectWordColor;
    [HideInInspector]public Color wrongWordColor;

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

        wrongWordColor = new Color(0.9f, 0.2f, 0.2f);        //red
        perfectWordColor = new Color(1, 0.84f, 0);           //gold 
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

    //display a result. Can specify the amount of time to display message.
    public IEnumerator ShowResult(string result, Color textColor, float duration = 0.5f)
    {
        resultUI.text = result;
        resultUI.color = textColor;

        //display the result for a second. Show a pulse effect
        resultUI.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        while (resultUI.transform.localScale.x > 1)
        {
            resultUI.transform.localScale = new Vector3(resultUI.transform.localScale.x - 5f * Time.deltaTime, 
                resultUI.transform.localScale.y - 5f * Time.deltaTime, 1);
            yield return null;
        }

        //ensure scale is back to normal
        resultUI.transform.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(duration);
        resultUI.text = "";
        resultCoroutineOn = false;
    }

    public IEnumerator Stun(float stunDuration, bool stunMeterOn = true)
    {
        //shake the screen
        //uiHandler.transform.position = new Vector3(uiHandler.transform.position.x + 100, 
            //uiHandler.transform.position.y, uiHandler.transform.position.z);
        
        //show stun meter if there was a correction/word is wrong
        if (stunMeterOn)
        {
            stunMeterHandler.gameObject.SetActive(true);
            stunMeter.value = stunMeter.maxValue;
            float currentTime = Time.time;
            while (Time.time < currentTime + stunDuration)
            {
                //update stun meter. The meter starts full, then gradually goes down.
                stunMeter.value = stunMeter.maxValue - ((Time.time - currentTime) / stunDuration);
                yield return null;
            }

            stunMeterHandler.gameObject.SetActive(false);

        }
        else //got a perfect word
            yield return new WaitForSeconds(stunDuration);
        

         //clear the field and select new word
        inputField.text = "";
        gm.targetWordSelected = false;
        gm.correctionWasMade = false;
        stunCoroutineOn = false;
        inputField.ActivateInputField();
    }

    public IEnumerator CountdownComboTimer(float duration)
    {
        comboHandler.gameObject.SetActive(true);
        comboMeter.value = comboMeter.maxValue;
        comboValueUI.text = gm.comboCount.ToString();

        float currentTime = Time.time;
        while (Time.time < currentTime + duration)
        {
            //update combo meter. The meter starts full, then gradually goes down.
            comboMeter.value = comboMeter.maxValue - ((Time.time - currentTime) / duration);
            yield return null;
        }

        //if we get here, combo has ended
        gm.comboCount = 0;
        comboHandler.gameObject.SetActive(false);
        comboCountdownCoroutineOn = false;
    }

    public IEnumerator AnimatePoints(float pointValue)
    {
        Vector3 originalPos = pointValueUI.transform.position;
        float duration = 0.5f;
        float currentTime = Time.time;
        float distance = 20;
        pointValueUI.text = pointValue + " pts.";

        //colour of text depends on whether player got a perfect word
        if (!gm.correctionWasMade)
            pointValueUI.color = perfectWordColor;
        else
            pointValueUI.color = new Color(0.1f, 0.7f, 0.9f);   //light blue colour
        pointValueUI.gameObject.SetActive(true);

        while(Time.time < currentTime + duration)
        {
            pointValueUI.transform.position = new Vector3(pointValueUI.transform.position.x, pointValueUI.transform.position.y + distance * Time.deltaTime,
             pointValueUI.transform.position.z);
            yield return null;
        }

        //time fades gradually after it reaches its position.
        while(pointValueUI.alpha > 0)
        {
            pointValueUI.alpha -= 2 * Time.deltaTime;
            yield return null;
        }
        
        //reset
        pointValueUI.alpha = 1;
        pointValueUI.transform.position = originalPos;
        pointValueUI.gameObject.SetActive(false);
        animatePointsCoroutineOn = false;
    }
}
