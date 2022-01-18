using System.Collections;
using System.Collections.Generic;
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

    public static ResultsScreen instance;
    TitleManager tm = TitleManager.instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        instance = this;
    }

    public void OnReturnButtonClicked()
    {
        StartCoroutine(ChangeToScreen("Title"));
    }

    public void OnShareButtonClicked()
    {
        //copy data to clipboard. This data first comes from game manager before it's copied
        string results = "I just defused a bomb in Bomb Stopper!\n\n";
        results += "Difficulty: " + tm.currentDifficulty + "\n";
        results += "Score: " + scoreUI.text + "\n";
        results += "Elapsed Time: " + elapsedTimeValueUI.text + "\n";
        GUIUtility.systemCopyBuffer = results;
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
}
