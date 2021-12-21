//This is a simple table that will grab data from a JSON file. Updated scores are written to the same file.
[System.Serializable]
public class HighScoreTable
{

    public int score;
    public string name;         //consist of 3-letter initials
    public string difficulty;   //this is the ID for each table. There are separate tables for each difficulty.

    //consts
    int NameLimit { get; } = 3;
}
