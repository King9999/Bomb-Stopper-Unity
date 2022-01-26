using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class controls the bomb's behaviour, including shrinking the fuse and growing the bomb as the time decreases.
public class Bomb : MonoBehaviour
{
    public GameObject bombBody;
    public Slider bombFuse;
    float initTime {get;} = 120;       //the usual game time. I use this instead of gameTimer.initTime because 
                                        //gameTimer.initTime can change.
    bool pulseBombCoroutineOn;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        bombFuse.value = 1;
        gm = GameManager.instance;  //this is here to ensure the instance isn't null at runtime.

        if (gm != null)
        {
            Debug.Log("Game Manager not null");
            StartCoroutine(ReduceFuse());
        }
    }

    void Update()
    {
        if (gm.currentWordCount < gm.totalWordCount)
        {
            if (!pulseBombCoroutineOn)
            {
                pulseBombCoroutineOn = true;
                StartCoroutine(PulseBomb());
            }
        }
        else //stage completed
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }

    //reduce fuse based on the timer.
    IEnumerator ReduceFuse()
    {
        while(!gm.gameTimer.TimeUp())
        {
            bombFuse.value = gm.gameTimer.time / initTime;
            yield return null;
        }
        
    }

    IEnumerator PulseBomb()
    {
        float scaleValue = 0.2f;
        Vector3 targetScale = new Vector3 (bombBody.transform.localScale.x + scaleValue, bombBody.transform.localScale.y + scaleValue, 1);
        Vector3 originalScale = bombBody.transform.localScale;
        float deltaScale;

        while (bombBody.transform.localScale.x < targetScale.x)
        {
            deltaScale = Time.deltaTime / 4f;
            bombBody.transform.localScale = new Vector3(bombBody.transform.localScale.x + deltaScale, 
                bombBody.transform.localScale.y + deltaScale, 1);
            yield return null;
        }

        //shrink back to normal
        while (bombBody.transform.localScale.x > originalScale.x)
        {
            deltaScale = Time.deltaTime / 4f;
            bombBody.transform.localScale = new Vector3(bombBody.transform.localScale.x - deltaScale, 
                bombBody.transform.localScale.y - deltaScale, 1);
            yield return null;
        }

        //ensure scale is back to normal.
        bombBody.transform.localScale = originalScale;
        pulseBombCoroutineOn = false;

    }
}
