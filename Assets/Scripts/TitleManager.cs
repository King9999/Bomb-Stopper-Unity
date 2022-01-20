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
    public Toggle specialToggle;
    

    [Header("------")]
    public static TitleManager instance;
    public GameObject modeDetailsHandler;

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
        //modeDetails.text = "";
        modeDetailsHandler.gameObject.SetActive(false);
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
