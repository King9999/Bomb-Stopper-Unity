using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

//NOTE: Unable to add high score table as there's no reliable way to have persistent data on WebGL without using a server.
public class GameManager : MonoBehaviour
{
    #region Variables
    public TextAsset hiScoreFile;
    public TextAsset wordFile;

    HighScoreTable hiScoreTable;
    WordLists dictionary;

     [HideInInspector]public bool targetWordSelected;
    [HideInInspector]public bool correctionWasMade;         //if true, player pressed delete or backspace
     [HideInInspector]public int comboCount;                 //A combo is formed when player types at least 2 words consecutively without error or correction.
    bool comboCounted;              //prevents combo counter from increasing more than once if a combo was already performed on current word.
    [HideInInspector]public float score;
    float pointsPerLetter {get;} = 10;
    bool scoreAdded;                //prevents score from being added multiple times per frame.
    float specialMod;               //more points are awarded if special rules are enabled.
    float pointsAdded;                    //amount of points for a complete word    

    [HideInInspector]public int currentWordCount;

    //Used Words variables
    string[] usedWords;             //holds the last words used. Any words in this list won't appear as a target word.
    int maxUsedWords {get;} = 20;
    int usedWordIndex;              //always points to last open space in the usedWords array.

    //JSON variables
    [HideInInspector]public int totalWordCount;                 //number of words to complete to finish the level
     [HideInInspector]public string difficultyId;
     [HideInInspector]public float time;

    //variables for winning a stage
    int totalWordsAttempted;        //includes correct and incorrect words
    [HideInInspector]public int highestCombo;
    int perfectWordCount;
    [HideInInspector] public int okWordCount;
    [HideInInspector] public int wrongWordCount;

    //Medal variables
    public MedalObject[] medalObjects;  //array of earned medals
    public MedalObject medalPrefab;

    [Header("UI")]
    public GameObject uiHandler;        //controls all UI. Mainly used to create a "shake" effect
    public GameObject resultsScreenHandler;
    string filePath;                    //contains location of high score table JSON file
    
    //Color perfectWordColor;
    //Color wrongWordColor;

    [Header("Timers")]
    public Timer gameTimer;
    Timer countdownTimer;               //no other action can occur while this is not 0
    public float penaltyDuration;
    float currentTime;              
    float basePenalty { get; } = 1;    //time in seconds. base penalty is getting the word correct but making a correction.
    float basePenaltyPerLetter { get; } = 0.3f;
    float difficultyMod;                //adjusts the penalty based on difficulty. 0.5 for easy, 1 for normal, 1.5 for hard, 
                                        //1.8 for special
    float penaltyPerLetter;
    float comboTimer;                   //duration before combo is broken. Length depends on the length of last word completed.
    float baseComboTimer {get;} = 2;    //time in seconds


    [Header("Audio")]
    [HideInInspector] public AudioSource soundSource;
    public AudioSource musicSource;
    public AudioClip typeSound;
    public AudioClip perfectWordSound;
    public AudioClip okWordSound;
    public AudioClip incorrectWordSound;


    //instances
    public UI ui = UI.instance;         //this variable must be public in order to access the instance
    TitleManager tm = TitleManager.instance;
    MedalManager mm = MedalManager.instance;
    public ResultsScreen rs = ResultsScreen.instance;
    public static GameManager instance;
    public SpecialRules sr = SpecialRules.instance;

    //coroutine checks & setup
    /*bool stunCoroutineOn;
    bool resultCoroutineOn;
    bool comboCountdownCoroutineOn;
    bool animatePointsCoroutineOn;*/
    IEnumerator comboCountDown;

    //other variables
    [HideInInspector] public bool gameOver;           //reserved for the Three Strikes rule
    [HideInInspector]public bool gameStarted;         //becomes true after countdown to begin game ends.
    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        //Random.InitState((int)System.DateTime.Now.Ticks);   //set a new seed. Unsure if Unity keeps the same seed until game is closed.

        //read from JSON file
        dictionary = JsonUtility.FromJson<WordLists>(wordFile.text);

        //places the cursor in input field so player can start typing immediately.
        //ui.inputField.ActivateInputField();

        //tm.currentDifficulty = TitleManager.Difficulty.Normal;
        difficultyMod = AdjustDifficultyMod(difficultyMod);
        penaltyPerLetter = basePenaltyPerLetter * difficultyMod;

        //bool setup
        //stunCoroutineOn = false;
        //resultCoroutineOn = false;
        //targetWordSelected = false;
        //correctionWasMade = false;

        //JSON data
        difficultyId = dictionary.wordList[(int)tm.currentDifficulty].difficultyId;
        totalWordCount = dictionary.wordList[(int)tm.currentDifficulty].wordCount;
        time = dictionary.wordList[(int)tm.currentDifficulty].time;

        //UI setup
        ui.penaltyUI.text = "+" + penaltyPerLetter + " sec.";
        ui.difficultyUI.text = difficultyId;
        ui.resultUI.text = "";
        //ui.scoreUI.text = "Score: " + score;
        ui.wordCountValueUI.text = currentWordCount + "/" + totalWordCount;
        ui.scoreValueUI.text = score.ToString();
        //wrongWordColor = new Color(0.9f, 0.2f, 0.2f);       //red
        //perfectWordColor = new Color(1, 0.84f, 0);           //gold
        ui.screenTransition.value = 0;
        ui.targetWordUI.text = "";
        ui.inputField.DeactivateInputField();               //deactivated by default until countdown to begin stage

        ui.stunMeterHandler.gameObject.SetActive(false);   //hidden by default
        ui.comboHandler.gameObject.SetActive(false);        //this too
        //ui.returnButton.gameObject.SetActive(false);
        resultsScreenHandler.SetActive(false);
        ui.pointValueUI.gameObject.SetActive(false);

        //timer setup
        //If Reduced Time rule is set, player starts with 10 seconds
        if (sr.specialRule != SpecialRules.Rule.ReducedTime)
            //gameTimer.SetTimer(10);
        //else
            gameTimer.SetTimer(time);

        //gameTimer.StartTimer();

        //sound setup
        soundSource = GetComponent<AudioSource>();     //gets the attached audiosource component


        usedWords = new string[maxUsedWords];

        //medal setup
        medalObjects = new MedalObject[mm.medals.Length];

        for (int i = 0; i < medalObjects.Length; i++)
        {
        
            medalObjects[i] = Instantiate(medalPrefab);

            medalObjects[i].medalNameUI.text = mm.medals[i].medalName;
            medalObjects[i].medalDetailsUI.text = mm.medals[i].details;
            medalObjects[i].medalRank = mm.medals[i].rank;
            medalObjects[i].medalAcquired = mm.medals[i].medalAcquired;
            medalObjects[i].medalSprite.sprite = mm.medals[i].medalSprite;
            medalObjects[i].transform.SetParent(rs.medalOrganizer); //objects must go into canvas so that it's hidden behind the screen transistion.

            //medal objects are hidden by default
            medalObjects[i].gameObject.SetActive(false);

        }


        totalWordsAttempted = 0; 
        highestCombo = 0;
        perfectWordCount = 0;
        okWordCount = 0;
        wrongWordCount = 0;

        //special rule check
        if (tm.specialToggle.isOn)
        {
            specialMod = 1.5f;
        }
        else
        {
            specialMod = 1;
        }

        if (sr.specialRule == SpecialRules.Rule.WordOverflow)
        {
            sr.ExecuteSpecialRule(sr.specialRule);
        }

        //begin countdown to start game
        StartCoroutine(ui.BeginStage());

    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            if (!gameTimer.TimeUp() && !gameOver && currentWordCount < totalWordCount)
            {
                if (!targetWordSelected)
                {
                    //'Invisible rule check
                    if (sr.specialRule == SpecialRules.Rule.Invisible)
                    {
                        //we're done with the previous word, so we clear it.
                        sr.originalTypedWord = "";
                        sr.wordCopy.Clear();
                        sr.wordCopy.Add('_');
                        sr.wordCopyIndex = 0;
                    }

                    totalWordsAttempted++;          
                    //change the target word, taking care to make sure the same word isn't selected.
                    string previousWord = ui.targetWordUI.text;
                    bool usedWordFound = false;
                    while (previousWord == ui.targetWordUI.text || usedWordFound)
                    {
                        int randWord = Random.Range(0, dictionary.wordList[(int)tm.currentDifficulty].words.Length);
                        string newWord = dictionary.wordList[(int)tm.currentDifficulty].words[randWord].word;

                        //check if word was already used
                        int i = 0;
                        usedWordFound = false;
                        while(!usedWordFound && i < usedWords.Length)
                        {
                            if (usedWords[i] != null && newWord.ToLower() == usedWords[i].ToLower())
                            {
                                //don't use this word, find another
                                usedWordFound = true;
                            }
                            else
                            {
                                i++;
                            }
                        }

                        if (!usedWordFound)
                        {
                            //we can use this word
                            ui.targetWordUI.text = newWord;

                            //add to used word list
                            usedWords[usedWordIndex] = newWord;
                            usedWordIndex++;
                            

                            if (usedWordIndex >= usedWords.Length)
                                //oldest used word will be removed next time
                                usedWordIndex = 0;
                            
                            //Check if we need to reverse the letters or hide letters
                            if (sr.specialRule == SpecialRules.Rule.Reversed || sr.specialRule == SpecialRules.Rule.HiddenLetters || 
                                sr.specialRule == SpecialRules.Rule.CaseSensitive)
                            {
                                //run this set of rules
                                sr.ExecuteSpecialRule(sr.specialRule);
                            }
                        }
                    }

                    //resetting various bools so that certain code executes again.
                    targetWordSelected = true;
                    comboCounted = false;   
                    scoreAdded = false;     
                    if (sr.specialRule == SpecialRules.Rule.ReducedTime)
                    {
                        sr.timeAdded = false;
                    }
                }

                //if any key is pressed, play a sound
                if (Input.anyKeyDown)
                {
                    soundSource.PlayOneShot(typeSound);
                }

                //check if backspace, delete, or directional arrows are pressed. Player takes a small penalty in these cases
                if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    //Under the Three Strikes rule, any attempt at a correction counts as a strike
                    if (sr.specialRule == SpecialRules.Rule.ThreeStrikes)
                        sr.ExecuteSpecialRule(sr.specialRule);

                    if (sr.specialRule == SpecialRules.Rule.Invisible)
                    {
                        //lots of stuff happens here
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (sr.wordCopyIndex - 1 >= 0)
                                sr.wordCopyIndex--;
                            else
                                Debug.Log("At beginning of word");
                        }
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (sr.wordCopyIndex + 1 < sr.wordCopy.Count)
                                    sr.wordCopyIndex++;
                                else
                                    Debug.Log("At end of word");
                            
                        }

                        Debug.Log("Current Letter: " + sr.wordCopy[sr.wordCopyIndex]);

                        if (Input.GetKeyDown(KeyCode.Delete) && sr.wordCopy[sr.wordCopyIndex] != '_')
                        {
                            sr.wordCopy.RemoveAt(sr.wordCopyIndex);

                            //overwrite originalTypedWord
                            sr.originalTypedWord = "";
                            foreach(char letter in sr.wordCopy)
                            {
                                if (letter != '_')
                                    sr.originalTypedWord += letter;
                            }
                            Debug.Log("Updated word: " + sr.originalTypedWord);

                        }

                        if (Input.GetKeyDown(KeyCode.Backspace) && sr.wordCopyIndex - 1 >= 0 && sr.wordCopy.Count > 1)  //can't delete the space
                        {
                            sr.wordCopy.RemoveAt(sr.wordCopyIndex - 1);
                            sr.wordCopyIndex--;

                            //overwrite originalTypedWord
                            sr.originalTypedWord = "";
                            foreach(char letter in sr.wordCopy)
                            {
                                if (letter != '_')
                                    sr.originalTypedWord += letter;
                                
                            }
                            Debug.Log("Updated word: " + sr.originalTypedWord);

                        }
                    
                    }

                    correctionWasMade = true;
                }

                //Check for Invisible rule
                if (sr.specialRule == SpecialRules.Rule.Invisible && Input.anyKeyDown)
                {
                    //get what the player typed and add it to original word string. We only want to check the alphabet, and ignore anything else.
                    int i = 0;
                    bool keyFound = false;
                    string lowerAlphabet = sr.alphabet.ToLower();
                    char[] alphabetArray = lowerAlphabet.ToCharArray();
        

                    while (!keyFound && i < alphabetArray.Length)
                    {
                        if (Input.GetKeyDown(alphabetArray[i].ToString()))
                        {
                            sr.originalTypedWord += sr.alphabet.Substring(i,1).ToLower();

                            //new letters are always inserted before the space
                            sr.wordCopy.Add('_');
                            sr.wordCopy[sr.wordCopyIndex] = alphabetArray[i];
                            sr.wordCopyIndex++;
                            Debug.Log("last letter typed: " + sr.wordCopy[sr.wordCopyIndex - 1]);
                            keyFound = true;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    sr.ExecuteSpecialRule(sr.specialRule);
                }

            
                //check to see if amount of letters in input field matches the target word's letter count
                if (ui.inputField.text.Length >= ui.targetWordUI.text.Length)
                {
                    //prevent player from adding any more input.
                    ui.inputField.DeactivateInputField();

                    //show the typed word if this rule is active.
                    if (sr.specialRule == SpecialRules.Rule.Invisible)
                        ui.inputField.text = sr.originalTypedWord;

                    //compare the words and check if they match.
                    if (WordsMatch(ui.inputField.text, ui.targetWordUI.text))
                    {
                        //additional check if reverse rule is active. Display the target word with correct spelling
                        if ((sr.specialRule == SpecialRules.Rule.Reversed && sr.wordReversed) || sr.specialRule == SpecialRules.Rule.HiddenLetters)
                            ui.targetWordUI.text = sr.originalWord;
                    

                        //show icon indicating a correct word. Show "Perfect!" if no corrections were made, "OK" otherwise
                        if (!correctionWasMade)
                        {

                            //add to combo count
                            if (!comboCounted && targetWordSelected)
                            {
                                comboCount++;
                                //is this the highest combo so far?
                                if (highestCombo < comboCount)
                                    highestCombo = comboCount;

                                Debug.Log("Combo Count: " + comboCount);
                                comboCounted = true;

                                //if a combo is already in progress, must reset the coroutine
                                if (comboTimer > 0)
                                {
                                    StopCoroutine(comboCountDown);
                                    ui.comboCountdownCoroutineOn = false;
                                }
                                
                                //display combo counter
                                if (comboCount > 1)
                                {
                                    if (!ui.animateComboValueCoroutineOn)
                                    {
                                        ui.animateComboValueCoroutineOn = true;
                                        StartCoroutine(ui.AnimateComboValue());
                                    }
                                    //run coroutine. The timer duration is base combo timer + (number of letters * 0.1)
                                    if (!ui.comboCountdownCoroutineOn)
                                    {
                                        ui.comboCountdownCoroutineOn = true;
                                        comboTimer = baseComboTimer + (ui.inputField.text.Length * 0.1f);
                                        comboCountDown = ui.CountdownComboTimer(comboTimer);
                                        StartCoroutine(comboCountDown);
                                        Debug.Log("Combo duration: " + comboTimer);
                                    }
                                }

                                //add points
                                if (!scoreAdded)
                                {
                                    pointsAdded = pointsPerLetter * ui.inputField.text.Length * (1 + comboCount * 0.25f);
                                    score += Mathf.Round(pointsAdded * specialMod);
                                    Debug.Log("Pts Added: " + Mathf.Round(pointsAdded * specialMod));
                                    scoreAdded = true;

                                    if(!ui.animatePointsCoroutineOn)
                                    {
                                        ui.animatePointsCoroutineOn = true;
                                        StartCoroutine(ui.AnimatePoints(Mathf.Round(pointsAdded * specialMod)));
                                    }
                                }

                        
                            }


                            //add time if Reduced Time rule is enabled
                            if (sr.specialRule == SpecialRules.Rule.ReducedTime && !sr.timeAdded)
                            {
                                sr.ExecuteSpecialRule(sr.specialRule);
                                sr.timeAdded = true;
                            }

                            //show perfect result
                            if (!ui.resultCoroutineOn)
                            {
                                ui.resultCoroutineOn = true;
                                soundSource.PlayOneShot(perfectWordSound, 0.4f);
                                StartCoroutine(ui.ShowResult("Perfect!", ui.perfectWordColor));
                                currentWordCount++;
                                perfectWordCount++;
                            }
                            if (!ui.stunCoroutineOn)
                            {
                                ui.stunCoroutineOn = true;
                                StartCoroutine(ui.Stun(0.5f, false)); //I have this here so player can confirm that they typed the correct word
                            }
                        }
                        else    //an OK match
                        {
                            //add points
                            if (!scoreAdded && targetWordSelected)
                            {
                                pointsAdded = Mathf.Round(pointsPerLetter * ui.inputField.text.Length * specialMod);
                                score += pointsAdded;
                                scoreAdded = true;

                                if(!ui.animatePointsCoroutineOn)
                                {
                                    ui.animatePointsCoroutineOn = true;
                                    StartCoroutine(ui.AnimatePoints(pointsAdded));
                                }
                            }

                            //add time if Reduced Time rule is enabled
                            if (sr.specialRule == SpecialRules.Rule.ReducedTime && !sr.timeAdded)
                            {
                                sr.ExecuteSpecialRule(sr.specialRule);
                                sr.timeAdded = true;
                            }

                            //reset combo & coroutine
                            comboCount = 0;
                            if (ui.comboCountdownCoroutineOn)
                            {
                                ui.comboHandler.gameObject.SetActive(false);
                                StopCoroutine(comboCountDown);
                                ui.comboCountdownCoroutineOn = false;
                            }

                            //In the case of an "OK" match, player is slightly penalized.
                            if (!ui.resultCoroutineOn)
                            {
                                ui.resultCoroutineOn = true;
                                soundSource.PlayOneShot(okWordSound, 0.3f);
                                StartCoroutine(ui.ShowResult("OK", Color.white, basePenalty));
                                currentWordCount++;
                                okWordCount++;
                            }
                            if (!ui.stunCoroutineOn)
                            {
                                ui.stunCoroutineOn = true;
                                StartCoroutine(ui.Stun(basePenalty));
                            }
                            
                        }

                    }
                    else    //incorrect word
                    {
                        //reset combo & coroutine
                        comboCount = 0;
                        if (ui.comboCountdownCoroutineOn)
                        {
                            ui.comboHandler.gameObject.SetActive(false);
                            StopCoroutine(comboCountDown);
                            ui.comboCountdownCoroutineOn = false;
                        }

                        //highlight all of the incorrect letters in the target word.
                        //penalty is base penalty + (number of incorrect letters * 0.3 * difficulty)
                        int errorCount;
                        if ((sr.specialRule == SpecialRules.Rule.Reversed && sr.wordReversed) || sr.specialRule == SpecialRules.Rule.HiddenLetters)
                            errorCount = IncorrectLetterTotal(ui.inputField.text, sr.originalWord);
                        else if (sr.specialRule == SpecialRules.Rule.Invisible)
                            errorCount = IncorrectLetterTotal(sr.originalTypedWord, ui.targetWordUI.text);
                        else
                            errorCount = IncorrectLetterTotal(ui.inputField.text, ui.targetWordUI.text);

                        penaltyDuration = basePenalty + (errorCount * penaltyPerLetter);
                        Debug.Log("Penalty time is " + penaltyDuration);
                        if (!ui.resultCoroutineOn)
                        {
                            ui.resultCoroutineOn = true;
                            soundSource.PlayOneShot(incorrectWordSound, 0.3f);
                            StartCoroutine(ui.ShowResult("Incorrect", ui.wrongWordColor, penaltyDuration));
                            wrongWordCount++;
                        }
                        if (!ui.stunCoroutineOn)
                        {
                            ui.stunCoroutineOn = true;
                            StartCoroutine(ui.Stun(penaltyDuration));
                        }

                        //'Three Strikes' rule check
                        if (sr.specialRule == SpecialRules.Rule.ThreeStrikes)
                        {
                            sr.ExecuteSpecialRule(sr.specialRule);
                        }

                        
                    }

            
                
                }

                //UI update
                ui.scoreValueUI.text = score.ToString();
                if (sr.specialRule == SpecialRules.Rule.WordOverflow)
                    ui.wordCountValueUI.text = currentWordCount + "/" + sr.startColor + totalWordCount + sr.endColor;
                else
                    ui.wordCountValueUI.text = currentWordCount + "/" + totalWordCount;
            }
            else //time is up or stage is complete
            {
                if (gameTimer.TimeUp() || gameOver)
                {
                    gameTimer.StopTimer();
                    //show animation of screen exploding


                    //ui.returnButton.gameObject.SetActive(true); //return to title
                    ui.inputField.DeactivateInputField();
                }
                else
                {
                    //stage is complete. Check for any medals and display results
                    gameTimer.StopTimer();
                    uiHandler.SetActive(false);
                    GetStageCompletionResults();
                }
            }
        }
    }

    float AdjustDifficultyMod(float difficultyScale)
    {
        switch(tm.currentDifficulty)
        {
            case TitleManager.Difficulty.Easy:
                difficultyScale = 0.5f;
                break;

            case TitleManager.Difficulty.Normal:
                difficultyScale = 1f;
                break;
            
            case TitleManager.Difficulty.Hard:
                difficultyScale = 1.5f;
                break;
            
            case TitleManager.Difficulty.Special:
                difficultyScale = 1.8f;
                break;
            
            default:
                break;
        }

        return difficultyScale;
    }

    public bool WordsMatch(string typedWord, string targetWord)
    {
        if ((sr.specialRule == SpecialRules.Rule.Reversed && sr.wordReversed) || sr.specialRule == SpecialRules.Rule.HiddenLetters)
            return typedWord.ToLower() == sr.originalWord.ToLower();
        else if (sr.specialRule == SpecialRules.Rule.Invisible)
            return sr.originalTypedWord.ToLower() == targetWord.ToLower();
        else if (sr.specialRule == SpecialRules.Rule.CaseSensitive)
            return typedWord == targetWord;    //words must match exactly, including case
        else
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

         //check if Case Sensitive rule is in place
        if (sr.specialRule == SpecialRules.Rule.CaseSensitive)
        {
            for (int i = 0; i < targetWord.Length; i++)
            {
            
                if (typedWord.Substring(i,1) != targetWord.Substring(i,1))
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
        }
        else //regular check
        {
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
        }

        //update the onscreen words to show incorrect letters
        ui.targetWordUI.text = result;

        return errorTotal;
    }

    void GetStageCompletionResults()
    {
        resultsScreenHandler.SetActive(true);

        //remove all special rules UI
        if (tm.specialToggle.isOn)
        {
            sr.gameObject.SetActive(false);
        }

        //collect data
        if (sr.specialRule == SpecialRules.Rule.ReducedTime)
            rs.elapsedTimeValueUI.text = sr.DisplayElapsedTime();
        else
            rs.elapsedTimeValueUI.text = gameTimer.DisplayElapsedTime();

        rs.totalWordsAttemptedUI.text = totalWordsAttempted.ToString();
        rs.scoreUI.text = score.ToString();
        rs.perfectWordCountUI.text = perfectWordCount.ToString();
        rs.okWordCountUI.text = okWordCount.ToString();
        rs.wrongWordCountUI.text = wrongWordCount.ToString();
        rs.highestComboUI.text = highestCombo.ToString();
    
        //check & display medals. each time a medal is awarded, the next medal data's position must change.
        Vector3 medalPos = rs.medalOrganizer.transform.position;
        float yOffset = 50;

    #region Medals
        //The player will always receive this medal in Normal or Hard
        if (tm.currentDifficulty == TitleManager.Difficulty.Normal)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.BombDefused], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }
        if (tm.currentDifficulty == TitleManager.Difficulty.Hard)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.ExpertDefuser], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }

        //Two Thousand Club medal
        if (score >= 2000)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.OneThousandClub], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }

        //Ten Thosuand Club Medal
        if (score >= 10000)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.TenThousandClub], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }

        //A Special Kind of Person
        if (tm.specialToggle.isOn)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.ASpecialKindOfPerson], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }

        //Combo Rookie/Master
        if (highestCombo >= 20)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.ComboMaster], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }
        else if (highestCombo >= 5)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.ComboRookie], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }

        //Dramatic Finish
        if (gameTimer.time <= 5)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.DramaticFinish], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }

        //God Defuser
        if (tm.currentDifficulty == TitleManager.Difficulty.Hard && tm.specialToggle.isOn)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.GodDefuser], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }

        //Perfect
        if (okWordCount <= 0 && wrongWordCount <= 0)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.Perfect], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }

        //Speed Demon
        float elapsedTime = (sr.specialRule == SpecialRules.Rule.ReducedTime) ? sr.elapsedTime : gameTimer.initTime - gameTimer.time;
        if (elapsedTime <= 45)
        {
            StartCoroutine(rs.DisplayMedal(medalObjects[(int)MedalManager.MedalName.SpeedDemon], medalPos));
            medalPos = new Vector3(medalPos.x, medalPos.y - yOffset, medalPos.z);
        }
#endregion

    }
   
}
