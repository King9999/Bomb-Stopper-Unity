using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public enum Difficulty {Easy, Normal, Hard, Special}
    public Difficulty currentDifficulty;

    [Header("UI")]
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;
    public TextMeshProUGUI modeDetails;     //description of difficulty settings
    

    [Header("------")]
    public static TitleManager instance;

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
        //mouseOverColor = Color.red;
        //easyButton.image.color = mouseOverColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnEasyButtonClicked()
    {
        currentDifficulty = Difficulty.Easy;
        SceneManager.LoadScene("Game");
    }

    public void OnNormalButtonClicked()
    {
        currentDifficulty = Difficulty.Normal;
        SceneManager.LoadScene("Game");
    }

    public void OnHardButtonClicked()
    {
        currentDifficulty = Difficulty.Hard;
        SceneManager.LoadScene("Game");
    }



}
