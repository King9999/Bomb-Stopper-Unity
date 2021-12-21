using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextAsset hiScoreFile;

    public HighScoreTable hiScoreTable;

    // Start is called before the first frame update
    void Start()
    {
        //read from JSON file
        hiScoreTable = JsonUtility.FromJson<HighScoreTable>(hiScoreFile.text);
        Debug.Log("Score: " + hiScoreTable.score);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
