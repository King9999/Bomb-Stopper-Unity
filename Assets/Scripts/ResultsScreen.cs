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
    GameManager gm;

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
        gm = GameManager.instance;

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
        //NOTE: CANNOT COPY TEXT THROUGH WEBGL. IF THE PLAYER WANTS TO POST THEIR RESULTS THEY NEED TO USE THE PC BUILD.
        string results = "I just defused a bomb in Bomb Stopper!\n\n";
        results += tm.currentDifficulty + "\n";
        results += "Score " + scoreUI.text + "\n";
        results += "Time " + elapsedTimeValueUI.text + "\n";
        results += "Highest Combo " + highestComboUI.text + "\n";
        results += "-\n";
        results += "Total Words Attempted " + totalWordsAttemptedUI.text + "\n";
        results += "Perfect Words " + perfectWordCountUI.text + "\n";
        results += "OK Words " + okWordCountUI.text + "\n";
        results += "Incorrect Words " + wrongWordCountUI.text + "\n\n";

        #region Medals
        //medal data
        //"\u2606" is the unicode for star outline

        //The player will always receive this medal in Normal or Hard
        if (tm.currentDifficulty == TitleManager.Difficulty.Normal)
        {
            results += "\u2606 Bomb Defused\n";
        }
        if (tm.currentDifficulty == TitleManager.Difficulty.Hard)
        {
            results += "\u2606 Expert Defuser\n";
        }

        //Two Thousand Club medal
        if (gm.score >= 2000)
        {
           results += "\u2606 Two Thousand Club\n";
        }

        //Ten Thosuand Club Medal
        if (gm.score >= 10000)
        {
            results += "\u2606 Ten Thousand Club\n";
        }

        //A Special Kind of Person
        if (tm.specialToggle.isOn)
        {
            results += "\u2606 A Special Kind of Person\n";
        }

        //Combo Rookie/Master
        if (gm.highestCombo >= 20)
        {
            results += "\u2606 Combo Master\n";
        }
        else if (gm.highestCombo >= 5)
        {
            results += "\u2606 Combo Rookie\n";
        }

        //Dramatic Finish
        if (gm.gameTimer.time <= 5)
        {
           results += "\u2606 Dramatic Finish\n";
        }

        //God Defuser
        if (tm.currentDifficulty == TitleManager.Difficulty.Hard && tm.specialToggle.isOn)
        {
            results += "\u2606 God Defuser\n";
        }

        //Perfect
        if (gm.okWordCount <= 0 && gm.wrongWordCount <= 0)
        {
            results += "\u2606 Perfect!\n";
        }

        //Speed Demon
        float elapsedTime = (gm.sr.specialRule == SpecialRules.Rule.ReducedTime) ? gm.sr.elapsedTime : gm.gameTimer.initTime - gm.gameTimer.time;
        if (elapsedTime <= 45)
        {
            results += "\u2606 Speed Demon\n";
        }
#endregion
        results += "\nhttps://king9999.itch.io/bomb-stopper-unity";

       

        GUIUtility.systemCopyBuffer = results;
    }

    public IEnumerator DisplayMedal(MedalObject medal, Vector3 position)
    {
        //I changed this method to be a coroutine that gradually increases the alpha of each medal UI element.
        //medal.gameObject.SetActive(true);
        Color medalColor = medal.medalSprite.color;
        medal.medalSprite.color = new Color(medalColor.r, medalColor.g, medalColor.b, 0);
        medal.medalNameUI.alpha = 0;
        medal.medalDetailsUI.alpha = 0;
        
        medal.medalSprite.transform.position = new Vector3(position.x - 290, position.y, position.z);
        medal.medalNameUI.transform.position = position;
        medal.medalDetailsUI.transform.position = new Vector3(position.x + 21, position.y - 16, position.z);

        //animate medal
        medal.gameObject.SetActive(true);
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            medal.medalSprite.color = new Color(medalColor.r, medalColor.g, medalColor.b, alpha);
            medal.medalNameUI.alpha = alpha;
            medal.medalDetailsUI.alpha = alpha;
            yield return null;
        }

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
