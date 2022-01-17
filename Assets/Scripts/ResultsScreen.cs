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
