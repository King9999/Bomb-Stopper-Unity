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
    int comboCount;                 //A combo is formed when player types at least 2 words consecutively without error or correction.
    bool comboCounted;              //prevents combo counter from increasing more than once if a combo was already performed on current word.
    int score;
    int pointsPerLetter {get;} = 10;
    bool scoreAdded;                //prevents score from being added multiple times per frame.    

    int currentWordCount;

    //JSON variables
    int totalWordCount;                 //number of words to complete to finish the level
    string difficultyId;
    float time;

    [Header("UI")]
    public GameObject uiHandler;        //controls all UI. Mainly used to create a "shake" effect
    public UI ui = UI.instance;         //this variable must be public in order to access the instance
    string filePath;                    //contains location of high score table JSON file
    Color perfectWordColor;
    Color wrongWordColor;

    [Header("Timers")]
    public Timer gameTimer;
    public float penaltyDuration;
    float currentTime;              
    float basePenalty { get; } = 1;    //time in seconds. base penalty is getting the word correct but making a correction.
    float basePenaltyPerLetter { get; } = 0.3f;
    float difficultyMod;                //adjusts the penalty based on difficulty. 0.5 for easy, 1 for normal, 1.5 for hard, 
                                        //1.8 for special
    float penaltyPerLetter;
    float comboTimer;                   //duration before combo is broken. Length depends on the length of last word completed.
    float baseComboTimer {get;} = 2;    //time in seconds

    //coroutine checks & setup
    bool stunCoroutineOn;
    bool resultCoroutineOn;
    bool comboCountdownCoroutineOn;
    IEnumerator comboCountDown;

    // Start is called before the first frame update
    void Start()
    {
        //read from JSON file
        dictionary = JsonUtility.FromJson<WordLists>(wordFile.text);

        //places the cursor in input field so player can start typing immediately.
        UI.instance.inputField.ActivateInputField();

        currentDifficulty = Difficulty.Normal;
        difficultyMod = AdjustDifficultyMod(difficultyMod);
        penaltyPerLetter = basePenaltyPerLetter * difficultyMod;

        //bool setup
        stunCoroutineOn = false;
        resultCoroutineOn = false;
        targetWordSelected = false;
        correctionWasMade = false;

        //JSON data
        difficultyId = dictionary.wordList[(int)currentDifficulty].difficultyId;
        totalWordCount = dictionary.wordList[(int)currentDifficulty].wordCount;
        time = dictionary.wordList[(int)currentDifficulty].time;

        //UI setup
        ui.penaltyUI.text = "PPL: +" + penaltyPerLetter + " sec.";
        ui.difficultyUI.text = "Difficulty: " + difficultyId;
        ui.resultUI.text = "";
        //ui.scoreUI.text = "Score: " + score;
        ui.wordCountValueUI.text = currentWordCount + "/" + totalWordCount;
        ui.scoreValueUI.text = score.ToString();
        wrongWordColor = new Color(0.9f, 0.2f, 0.2f);       //red
        perfectWordColor = new Color(1, 0.84f, 0);           //gold

        ui.stunMeterHandler.gameObject.SetActive(false);   //hidden by default
        ui.comboHandler.gameObject.SetActive(false);        //this too

        //timer setup
        gameTimer.SetTimer(time);
        gameTimer.timerRunning = true;

        
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
            string previousWord = ui.targetWordUI.text;
            while (previousWord == ui.targetWordUI.text)
            {
                int randWord = Random.Range(0, dictionary.wordList[(int)currentDifficulty].words.Length);
                ui.targetWordUI.text = dictionary.wordList[(int)currentDifficulty].words[randWord].word;
            }
            targetWordSelected = true;
            comboCounted = false;   //new word, no combo occurred yet
            scoreAdded = false;     //new word, new opportunity to add to score
        }

        //check if backspace or delete is pressed. Player takes a small penalty in these cases
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
        {
            correctionWasMade = true;
        }

        //check to see if amount of letters in input field matches the target word's letter count
        if (ui.inputField.text.Length >= ui.targetWordUI.text.Length)
        {
            //prevent player from adding any more input.
            ui.inputField.DeactivateInputField();

            //compare the words and check if they match.
            if (WordsMatch(ui.inputField.text, ui.targetWordUI.text))
            {
                //show icon indicating a correct word. Show "Perfect!" if no corrections were made, "OK" otherwise
                if (!correctionWasMade)
                {

                    //add to combo count
                    if (!comboCounted && targetWordSelected)
                    {
                        comboCount++;
                        Debug.Log("Combo Count: " + comboCount);
                        comboCounted = true;

                        //if a combo is already in progress, must reset the coroutine
                        if (comboTimer > 0)
                        {
                            StopCoroutine(comboCountDown);
                            comboCountdownCoroutineOn = false;
                        }
                    }

                    //add points
                    if (!scoreAdded && targetWordSelected)
                    {
                        int bonus = pointsPerLetter * ui.inputField.text.Length * comboCount;
                        score += bonus + (bonus / 2);
                        Debug.Log("Bonus: " + (bonus + (bonus / 2)));
                        scoreAdded = true;
                    }


                    if (comboCount > 1)
                    {
                        //run coroutine. The timer duration is base combo timer + (number of letters * 0.1)
                        if (!comboCountdownCoroutineOn)
                        {
                            comboCountdownCoroutineOn = true;
                            comboTimer = baseComboTimer + (ui.inputField.text.Length * 0.1f);
                            comboCountDown = CountdownComboTimer(comboTimer);
                            StartCoroutine(comboCountDown);
                            Debug.Log("Combo duration: " + comboTimer);
                        }
                    }


                    if (!resultCoroutineOn)
                    {
                        resultCoroutineOn = true;
                        StartCoroutine(ShowResult("Perfect!", perfectWordColor));
                        currentWordCount++;
                    }
                    if (!stunCoroutineOn)
                    {
                        stunCoroutineOn = true;
                        StartCoroutine(Stun(0.5f, false)); //I have this here so player can confirm that they typed the correct word
                    }
                }
                else    //an OK match
                {
                    //add points
                    if (!scoreAdded && targetWordSelected)
                    {
                        score += pointsPerLetter * ui.inputField.text.Length;
                        scoreAdded = true;
                    }

                    //reset combo & coroutine
                    comboCount = 0;
                    if (comboCountdownCoroutineOn)
                    {
                        ui.comboHandler.gameObject.SetActive(false);
                        StopCoroutine(comboCountDown);
                        comboCountdownCoroutineOn = false;
                    }

                    //In the case of an "OK" match, player is slightly penalized.
                    if (!resultCoroutineOn)
                    {
                        resultCoroutineOn = true;
                        StartCoroutine(ShowResult("OK", Color.white, basePenalty));
                        currentWordCount++;
                    }
                    if (!stunCoroutineOn)
                    {
                        stunCoroutineOn = true;
                        StartCoroutine(Stun(basePenalty));
                    }
                    
                }

            }
            else    //incorrect word
            {
                 //reset combo & coroutine
                comboCount = 0;
                if (comboCountdownCoroutineOn)
                {
                    ui.comboHandler.gameObject.SetActive(false);
                    StopCoroutine(comboCountDown);
                    comboCountdownCoroutineOn = false;
                }

                //highlight all of the incorrect letters in the target word.
                //penalty is base penalty + (number of incorrect letters * 0.3 * difficulty)
                int errorCount = IncorrectLetterTotal(ui.inputField.text, ui.targetWordUI.text);
                penaltyDuration = basePenalty + (errorCount * penaltyPerLetter);
                Debug.Log("Penalty time is " + penaltyDuration);
                if (!resultCoroutineOn)
                {
                    resultCoroutineOn = true;
                    StartCoroutine(ShowResult("Incorrect", wrongWordColor, penaltyDuration));
                }
                if (!stunCoroutineOn)
                {
                    stunCoroutineOn = true;
                    StartCoroutine(Stun(penaltyDuration));
                }
                
            }
            
        }

        //UI update
        ui.scoreValueUI.text = score.ToString();
        ui.wordCountValueUI.text = currentWordCount + "/" + totalWordCount;
    }

    float AdjustDifficultyMod(float difficultyScale)
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

        return difficultyScale;
    }

    bool WordsMatch(string typedWord, string targetWord)
    {
        return typedWord.ToLower() == targetWord.ToLower();
    }

    //NOTE: This method was slowing down the game, probably because of too many string operations. I decided to only highlight the
    //incorrect letters in the target word, instead of highlighting both words.
    int IncorrectLetterTotal(string typedWord, string targetWord)
    {
        int errorTotal = 0;

        string result = "";
        string startColor = "<color=#E53C3C>";  //I can append this to a string to add colour to individual letters.
        string endColor = "</color>";

        for (int i = 0; i < targetWord.Length; i++)
        {
            if (typedWord.ToLower().Substring(i,1) != targetWord.ToLower().Substring(i,1))
            {
                errorTotal++;
                //change colour of letter
                result += startColor + targetWord.Substring(i,1) + endColor;
            }
            else
            {
                result += targetWord.Substring(i,1);
            }
        }

        //update the onscreen words to show incorrect letters
        ui.targetWordUI.text = result;

        return errorTotal;
    }

#region Coroutines
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

    //display a result. Can specify the amount of time to display message.
    IEnumerator ShowResult(string result, Color textColor, float duration = 0.5f)
    {
        ui.resultUI.text = result;
        ui.resultUI.color = textColor;

        //display the result for a second. Show a pulse effect
        ui.resultUI.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        while (ui.resultUI.transform.localScale.x > 1)
        {
            ui.resultUI.transform.localScale = new Vector3(ui.resultUI.transform.localScale.x - 5f * Time.deltaTime, 
                ui.resultUI.transform.localScale.y - 5f * Time.deltaTime, 1);
            yield return null;
        }

        //ensure scale is back to normal
        ui.resultUI.transform.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(duration);
        ui.resultUI.text = "";
        resultCoroutineOn = false;
    }

    IEnumerator Stun(float stunDuration, bool stunMeterOn = true)
    {
        //shake the screen
        //uiHandler.transform.position = new Vector3(uiHandler.transform.position.x + 100, 
            //uiHandler.transform.position.y, uiHandler.transform.position.z);
        
        //show stun meter if there was a correction/word is wrong
        if (stunMeterOn)
        {
            ui.stunMeterHandler.gameObject.SetActive(true);
            ui.stunMeter.value = ui.stunMeter.maxValue;
            float currentTime = Time.time;
            while (Time.time < currentTime + stunDuration)
            {
                //update stun meter. The meter starts full, then gradually goes down.
                ui.stunMeter.value = ui.stunMeter.maxValue - ((Time.time - currentTime) / stunDuration);
                yield return null;
            }

            ui.stunMeterHandler.gameObject.SetActive(false);

        }
        else //got a perfect word
            yield return new WaitForSeconds(stunDuration);
        

         //clear the field and select new word
        ui.inputField.text = "";
        targetWordSelected = false;
        correctionWasMade = false;
        stunCoroutineOn = false;
        ui.inputField.ActivateInputField();
    }

IEnumerator CountdownComboTimer(float duration)
{
    ui.comboHandler.gameObject.SetActive(true);
    ui.comboMeter.value = ui.comboMeter.maxValue;
    ui.comboValueUI.text = comboCount.ToString();

    float currentTime = Time.time;
    while (Time.time < currentTime + duration)
    {
        //update combo meter. The meter starts full, then gradually goes down.
        ui.comboMeter.value = ui.comboMeter.maxValue - ((Time.time - currentTime) / duration);
        yield return null;
    }

    //if we get here, combo has ended
    comboCount = 0;
    ui.comboHandler.gameObject.SetActive(false);
    comboCountdownCoroutineOn = false;
}
#endregion
    /*public void WriteToFile()
    {
        if (!File.Exists(filePath))
        {
           // File.Writenew
           
        }
    }*/
}
