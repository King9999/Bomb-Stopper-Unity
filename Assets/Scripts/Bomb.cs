using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class controls the bomb's behaviour, including shrinking the fuse and growing the bomb as the time decreases.
public class Bomb : MonoBehaviour
{
    public Image bombBody;
    public Slider bombFuse;
    float initTime {get;} = 120;       //the usual game time. I use this instead of gameTimer.initTime because 
                                        //gameTimer.initTime can change.
    bool reduceFuseCoroutineOn;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        bombFuse.value = 1;
        gm = GameManager.instance;  //this is here in case the instance isn't initialized yet.

        if (gm != null)
        {
            Debug.Log("Gm not null");
            StartCoroutine(ReduceFuse());
        }
    }

    void Update()
    {
         /*if (gm != null)
         {
             if (!reduceFuseCoroutineOn)
             {
                reduceFuseCoroutineOn = true;
                StartCoroutine(ReduceFuse());
             }
         }*/
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
}
