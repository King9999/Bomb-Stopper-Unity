using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
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

        //places the cursor in input field so player can start typing immediately.
        inputField.ActivateInputField();

        //get hiscore table data
        //string tableUrl = /*"http://mikemurraygames.rf.gd/hiscoretable.json"*/ "https://drive.google.com/file/d/11ERWGBUGuLbtt1WbJHM6PzBXYxJIPuNQ";
        //StartCoroutine(GetTableData(tableUrl));
       /* UnityWebRequest blah = UnityWebRequest.Get(tableUrl);

        if (blah.error == null)
        {
            Debug.Log("No error");
        }


        hiScoreTable = JsonUtility.FromJson<HighScoreTable>(blah.downloadHandler.text);
        Debug.Log("Score: " + hiScoreTable.score);
        scoreUI.text = "Score: " + hiScoreTable.score;
        filePath = Application.dataPath + "/Resources/hiscoretable.json";
        Debug.Log(filePath);*/
    }

    // Update is called once per frame
    void Update()
    {
        //When I click the mouse button, random word is displayed
        if (Input.GetMouseButtonDown(0)) //left button pressed
        {
            //int time = dictionary.wordList[0].time;
            //Debug.Log("time is " + dictionary.wordList[0].words[0].word);
            int randWord = Random.Range(0, dictionary.wordList[0].words.Length);
            targetWordUI.text = dictionary.wordList[0].words[randWord].word;
            /*hiScoreTable.score += 100;
            scoreGUI.text = "Score: " + hiScoreTable.score;
            //save the result to file
            string scoreStr = JsonUtility.ToJson(hiScoreTable);
            Debug.Log("New score data: " + scoreStr);
            File.WriteAllText(filePath, scoreStr);*/
        }

        //check to see if amount of letters in input field matches the target word's letter count
        if (inputField.text.Length >= targetWordUI.text.Length)
        {
            //compare the words and check if they match.
            if (inputField.text.ToLower() == targetWordUI.text.ToLower())
            {
                Debug.Log("It's a match");
            }
            else
            {
                Debug.Log("no match");
            }
            
        }
    }

    IEnumerator GetTableData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            //Debug.Log("No error");
            hiScoreTable = JsonUtility.FromJson<HighScoreTable>(request.downloadHandler.text);
            Debug.Log("Score: " + hiScoreTable.score);
        }
        else
        {
            Debug.Log(request.error);
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
