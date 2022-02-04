using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* This is the game over script. When time is up, the screen fades to white, then fades to black. The ordering of the images in 
the inpsector is important. */

public class GameOverScreen : MonoBehaviour
{
    public Image whiteScreen;
    public Image blackScreen;
    public TextMeshProUGUI gameOverUI;
    public Button returnButton;
    public GameObject gameOverHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
