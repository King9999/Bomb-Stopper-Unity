using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/* This is the game over script. When time is up, the screen fades to white, then fades to black. The ordering of the images in 
the inpsector is important. */

public class GameOverScreen : MonoBehaviour
{
    public Image whiteScreen;
    public Image blackScreen;
    public TextMeshProUGUI gameOverUI;
    public Button returnButton;
    public GameObject gameOverHandler;
    public Image screenTransition;         //fades to black before returning to title screen

    bool gameOverCoroutineOn;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        //alpha set up
        whiteScreen.color = new Color(1, 1, 1, 0);
        blackScreen.color = new Color(0, 0, 0, 0);
        screenTransition.color = new Color(0, 0, 0, 0);


        returnButton.gameObject.SetActive(false);

        gameOverUI.alpha = 0;
        gameOverUI.text = "The End";


        gm = GameManager.instance;
    }

    void Update()
    {
        if (gm.gameTimer.TimeUp() || gm.gameOver)
        {
            if (!gameOverCoroutineOn)
            {
                gameOverCoroutineOn = true;
                StartCoroutine(GameOver());
            }
        }
    }

    IEnumerator GameOver()
    {
        //fade screen to white
        while (whiteScreen.color.a < 1)
        {
            float screenAlpha = whiteScreen.color.a + 0.8f * Time.deltaTime;
            whiteScreen.color = new Color(1, 1, 1, screenAlpha);
            yield return null;
        }

        //fade screen to black
        while (blackScreen.color.a < 1)
        {
            float screenAlpha = blackScreen.color.a + 0.5f * Time.deltaTime;
            blackScreen.color = new Color(0, 0, 0, screenAlpha);
            yield return null;
        }

        //show the game over text
         while (gameOverUI.alpha < 1)
         {
            gameOverUI.alpha += 0.5f * Time.deltaTime;
            yield return null;
         }

        //show the return button
        returnButton.gameObject.SetActive(true);
    }

    public void OnReturnButtonClicked()
    {
        StartCoroutine(ChangeToScreen("Title"));
    }

    //This screen transition is a standard fade to black
    IEnumerator ChangeToScreen(string newScene)
    {
        while (screenTransition.color.a < 1)
        {
            float newAlpha = screenTransition.color.a + 0.8f * Time.deltaTime;
            screenTransition.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        SceneManager.LoadScene(newScene);

    }
}
