
//word list consists of a difficulty ID and an array of words.
[System.Serializable]
public class WordList
{
    public string difficultyId;
    public Word[] words;
    public int time;            //time in seconds to complete stage
    public int wordCount;       //number of words needed to type correctly

}
