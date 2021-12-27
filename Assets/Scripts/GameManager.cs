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

    bool targetWordSelected;
    bool correctionWasMade;         //if true, player pressed delete or backspace
    enum Difficulty {Easy, Normal, Hard, Special}
    Difficulty currentDifficulty;

    [Header("UI")]
    public TextMeshProUGUI scoreUI;
    string filePath;                    //contains location of high score table JSON file
    public TMP_InputField inputField;   //player types words in here
    //public TextMeshProUGUI timerUI;
    public TextMeshProUGUI difficultyUI;
    public TextMeshProUGUI targetWordUI;

    [Header("Timers")]
    public float penaltyDuration;
    float currentTime;              
    float basePenalty { get; } = 1;    //time in seconds. base penalty is getting the word correct but making a correction.
    float basePenaltyPerLetter { get; } = 0.3f;
    float difficultyMod;                //adjusts the penalty based on difficulty. 0.5 for easy, 1 for normal, 1.5 for hard, 1.8 for special
    float penaltyPerLetter;

    // Start is called before the first frame update
    void Start()
    {
        //read from JSON file
        dictionary = JsonUtility.FromJson<WordLists>(wordFile.text);

        //places the cursor in input field so player can start typing immediately.
        inputField.ActivateInputField();

        targetWordSelected = false;
        correctionWasMade = false;

        currentDifficulty = Difficulty.Easy;
        AdjustDifficultyMod(difficultyMod);
        penaltyPerLetter = basePenaltyPerLetter * difficultyMod;

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
        //if (Input.GetMouseButtonDown(0)) //left button pressed
        //{
            //int time = dictionary.wordList[0].time;
            //Debug.Log("time is " + dictionary.wordList[0].words[0].word);
            //int randWord = Random.Range(0, dictionary.wordList[0].words.Length);
            //targetWordUI.text = dictionary.wordList[0].words[randWord].word;
            /*hiScoreTable.score += 100;
            scoreGUI.text = "Score: " + hiScoreTable.score;
            //save the result to file
            string scoreStr = JsonUtility.ToJson(hiScoreTable);
            Debug.Log("New score data: " + scoreStr);
            File.WriteAllText(filePath, scoreStr);*/
        //}

        if (!targetWordSelected)
        {
           
            //change the target word, taking care to make sure the same word isn't selected.
            string previousWord = targetWordUI.text;
            while (previousWord == targetWordUI.text)
            {
                int randWord = Random.Range(0, dictionary.wordList[(int)currentDifficulty].words.Length);
                targetWordUI.text = dictionary.wordList[(int)currentDifficulty].words[randWord].word;                
            }
            targetWordSelected = true;
        }

        //check if backspace or delete is pressed. Player takes a small penalty in these cases
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
        {
            correctionWasMade = true;
        }

        //check to see if amount of letters in input field matches the target word's letter count
        if (inputField.text.Length >= targetWordUI.text.Length)
        {
            //compare the words and check if they match.
            if (inputField.text.ToLower() == targetWordUI.text.ToLower())
            {
                //show icon indicating a correct word. Show "Perfect!" if no corrections were made, "OK" otherwise
                //In the case of an "OK" match, player is slightly penalized.
                Debug.Log("It's a match");
            }
            else
            {
                //highlight all of the incorrect letters in both the typed word and the target word.
                //penalty is base penalty + (number of incorrect letters * 0.3)
                Debug.Log("no match");
            }
            
            //clear the field and select new word
            inputField.text = "";
            targetWordSelected = false;
        }
    }

    void AdjustDifficultyMod(float difficultyScale)
    {
        switch(currentDifficulty)
        {
            case Difficulty.Easy:
                difficultyScale = 0.5f;
                break;

            case Difficulty.Normal:
                difficultyScale = 1f;
                break;
            
            case Difficulty.Hard:
                difficultyScale = 1.5f;
                break;
            
            case Difficulty.Special:
                difficultyScale = 1.8f;
                break;
            
            default:
                break;
        }
    }

    //I'm using this coroutine to grab the high score table from the web, but currently I'm unsuccessful in 
    //retrieving the data.
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
