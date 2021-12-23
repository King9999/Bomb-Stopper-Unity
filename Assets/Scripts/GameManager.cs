using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

//NOTE: Unable to add high score table as there's no reliable way to have persistent data on WebGL without using a server.
public class GameManager : MonoBehaviour
{
    public TextAsset hiScoreFile;
    public TextAsset wordFile;

    HighScoreTable hiScoreTable;
    WordLists dictionary;

    [Header("UI")]
    public TextMeshProUGUI scoreUI;
    string filePath;                    //contains location of high score table JSON file
    public TMP_InputField inputField;   //player types words in here
    //public TextMeshProUGUI timerUI;
    public TextMeshProUGUI difficultyUI;
    public TextMeshProUGUI targetWordUI;

    // Start is called before the first frame update
    void Start()
    {
        //read from JSON file
        dictionary = JsonUtility.FromJson<WordLists>(wordFile.text);

        hiScoreTable = JsonUtility.FromJson<HighScoreTable>(hiScoreFile.text);
        Debug.Log("Score: " + hiScoreTable.score);
        scoreUI.text = "Score: " + hiScoreTable.score;
        filePath = Application.dataPath + "/Resources/hiscoretable.json";
        Debug.Log(filePath);
    }

    // Update is called once per frame
    void Update()
    {
        //When I click the mouse button, random word is displayed
        if (Input.GetMouseButtonDown(0)) //left button pressed
        {
            int time = dictionary.wordList[0].time;
            Debug.Log("time is " + dictionary.wordList[0].words[0].word);
            int randWord = Random.Range(0, dictionary.wordList[0].words.Length);
            Debug.Log("Random value " + dictionary.wordList[0].words[randWord]);
            targetWordUI.text = dictionary.wordList[0].words[randWord].word;
            /*hiScoreTable.score += 100;
            scoreGUI.text = "Score: " + hiScoreTable.score;
            //save the result to file
            string scoreStr = JsonUtility.ToJson(hiScoreTable);
            Debug.Log("New score data: " + scoreStr);
            File.WriteAllText(filePath, scoreStr);*/
        }
    }

    /*public void WriteToFile()
    {
        if (!File.Exists(filePath))
        {
           // File.Writenew
           
        }
    }*/
}
