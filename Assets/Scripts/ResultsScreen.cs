using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultsScreen : MonoBehaviour
{
    public Slider screenTransition;
    public TextMeshProUGUI elapsedTimeValueUI;
    public TextMeshProUGUI totalWordsAttemptedUI;
    public TextMeshProUGUI perfectWordCountUI;
    public TextMeshProUGUI okWordCountUI;
    public TextMeshProUGUI wrongWordCountUI;
    public TextMeshProUGUI highestComboUI;
    public TextMeshProUGUI scoreUI;
    public Transform medalOrganizer;        //used to arrange the medals

    public TextMeshProUGUI[] uiElements;    //used to animate the textmeshes
    public Vector3[] uiPositions; 

    public static ResultsScreen instance;
    TitleManager tm = TitleManager.instance;
    GameManager gm = GameManager.instance;

    bool animateTextCoroutineOn;

    private void Awake()
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
        //store the initial positions so they can be animated later.
        uiPositions = new Vector3[uiElements.Length];

        for(int i = 0; i < uiElements.Length; i++)
        {
            uiPositions[i] = uiElements[i].transform.position;

            //change alpha to 0, and reposition UI
            uiElements[i].alpha = 0;

            //all the UI values aren't repositioned as far since they're small
            if (i % 2 == 0)
                uiElements[i].transform.position = new Vector3(uiElements[i].transform.position.x - 100, uiElements[i].transform.position.y, uiElements[i].transform.position.z);
            else
                uiElements[i].transform.position = new Vector3(uiElements[i].transform.position.x - 50, uiElements[i].transform.position.y, uiElements[i].transform.position.z);
        }

    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (!animateTextCoroutineOn)
            {
                animateTextCoroutineOn = true;
                StartCoroutine(AnimateText());
            }
        }
    }

    public void OnReturnButtonClicked()
    {
        StartCoroutine(ChangeToScreen("Title"));
    }

    public void OnShareButtonClicked()
    {
        //copy data to clipboard. This data first comes from game manager before it's copied
        string results = "I just defused a bomb in Bomb Stopper!\n\n";
        results += tm.currentDifficulty + "\n";
        results += "Score: " + scoreUI.text + "\n";
        results += "Time: " + elapsedTimeValueUI.text + "\n";
        results += "Highest Combo: " + highestComboUI.text + "\n";
        results += "---\n";
        results += "Total Words Attempted: " + totalWordsAttemptedUI.text + "\n";
        results += "Perfect Words: " + perfectWordCountUI.text + "\n";
        results += "OK Words: " + okWordCountUI.text + "\n";
        results += "Incorrect Words: " + wrongWordCountUI.text + "\n\n";
        results += "Check out the game at https://king9999.itch.io/bomb-stopper-unity?secret=lmNz7wEO7pvCDnVS2A6Vd05YdXM";

        //need to copy medal data

        GUIUtility.systemCopyBuffer = results;
    }

    public void DisplayMedal(MedalObject medal, Vector3 position)
    {
        medal.gameObject.SetActive(true);
        medal.medalSprite.transform.position = new Vector3(position.x - 290, position.y, position.z);
        medal.medalNameUI.transform.position = position;
        medal.medalDetailsUI.transform.position = new Vector3(position.x + 21, position.y - 16, position.z);
    }

    IEnumerator ChangeToScreen(string newScene)
    {
        while (screenTransition.value < 1)
        {
            screenTransition.value += Time.deltaTime * 2;
            yield return null;
        }

        SceneManager.LoadScene(newScene);

    }

    IEnumerator AnimateText()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            while (uiElements[i].transform.position.x < uiPositions[i].x)
            {
                float moveRate = 350 * Time.deltaTime;
                uiElements[i].transform.position = new Vector3(uiElements[i].transform.position.x + moveRate, uiElements[i].transform.position.y, uiElements[i].transform.position.z);
                uiElements[i].alpha += 2 * Time.deltaTime;
                yield return null;
            }

            uiElements[i].transform.position = uiPositions[i];
            uiElements[i].alpha = 1;
        }
        
        animateTextCoroutineOn = false;
    }
}
