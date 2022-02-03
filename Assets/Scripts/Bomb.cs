using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class controls the bomb's behaviour, including shrinking the fuse and growing the bomb as the time decreases.
public class Bomb : MonoBehaviour
{
    public GameObject bombObject;
    public Image body;
    public Image redBody;               //bomb turns red when it's about to explode
    public Image spark;
    public Transform[] sparkPoints;
    int currentPoint;                   //iterator for the sparkPoints array.
    Color redBodyColor;
    public Slider bombFuse;
    float initTime {get;} = 120;       //the usual game time. I use this instead of gameTimer.initTime because 
                                        //gameTimer.initTime can change.
    bool pulseBombCoroutineOn;
    bool flashBombCoroutineOn;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        bombFuse.value = 1;
        redBodyColor = redBody.color;
        redBodyColor.a = 0;
        redBody.color = new Color(redBodyColor.r, redBodyColor.g, redBodyColor.b, redBodyColor.a);    //transparent by default
        gm = GameManager.instance;  //this is here to ensure the instance isn't null at runtime.

        if (gm != null)
        {
            Debug.Log("Game Manager not null");
            StartCoroutine(ReduceFuse());
        }

        //spark set up
        spark.transform.position = sparkPoints[0].position;
        currentPoint = 0;
    }

    void Update()
    {
        if (!gm.gameTimer.TimeUp() && !gm.gameOver && gm.currentWordCount < gm.totalWordCount)    //only run while stage hasn't cleared
        {
            if (!pulseBombCoroutineOn)
            {
                pulseBombCoroutineOn = true;
                StartCoroutine(PulseBomb());
            }

            if (gm.gameTimer.time / initTime <= 0.5f)
            {
                if (!flashBombCoroutineOn)
                {
                    flashBombCoroutineOn = true;
                    StartCoroutine(FlashBomb());
                }
            }

            //move spark along spark points
            
            if(currentPoint + 1 < sparkPoints.Length)
            {
                float moveSpeed = 10;
                Vector3 direction = (sparkPoints[currentPoint + 1].position - sparkPoints[currentPoint].position).normalized;
                    spark.transform.position += direction * moveSpeed * Time.deltaTime;
                
                //if spark is close to the destination, want it to "snap" to the destination point so it doesn't overshoot
                float diffX = Mathf.Abs(sparkPoints[currentPoint + 1].position.x - spark.transform.position.x);
                float diffY = Mathf.Abs(sparkPoints[currentPoint + 1].position.y - spark.transform.position.y);
                //Debug.Log("DiffX: " + diffX + " DiffY: " + diffY);
                if (diffX >= 0 && diffX < 0.05f && diffY >= 0 && diffY < 0.05f)
                {
                    spark.transform.position = sparkPoints[currentPoint + 1].position;
                }

                if (spark.transform.position == sparkPoints[currentPoint + 1].position)
                {
                    currentPoint++;
                }

                /*if (spark.transform.position.x < sparkPoints[currentPoint + 1].position.x)
                {
                    spark.transform.position = new Vector3(spark.transform.position.x + moveSpeed, spark.transform.position.y, spark.transform.position.z);
                }

                else if (spark.transform.position.x > sparkPoints[currentPoint + 1].position.x)
                {
                    spark.transform.position = new Vector3(spark.transform.position.x - moveSpeed, spark.transform.position.y, spark.transform.position.z);
                }

                else if (spark.transform.position.y > sparkPoints[currentPoint + 1].position.y)
                {
                    spark.transform.position = new Vector3(spark.transform.position.x, spark.transform.position.y - moveSpeed, spark.transform.position.z);
                }

                else if (spark.transform.position.y < sparkPoints[currentPoint + 1].position.y)
                {
                    spark.transform.position = new Vector3(spark.transform.position.x, spark.transform.position.y + moveSpeed, spark.transform.position.z);
                }*/
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
        Vector3 targetScale = new Vector3 (bombObject.transform.localScale.x + scaleValue, bombObject.transform.localScale.y + scaleValue, 1);
        Vector3 originalScale = bombObject.transform.localScale;
        float deltaScale;

        //expand bomb
        while (bombObject.transform.localScale.x < targetScale.x)
        {
            deltaScale = Time.deltaTime / 4f;
            bombObject.transform.localScale = new Vector3(bombObject.transform.localScale.x + deltaScale, 
                bombObject.transform.localScale.y + deltaScale, 1);
            yield return null;
        }

        //shrink back to normal
        while (bombObject.transform.localScale.x > originalScale.x)
        {
            deltaScale = Time.deltaTime / 4f;
            bombObject.transform.localScale = new Vector3(bombObject.transform.localScale.x - deltaScale, 
                bombObject.transform.localScale.y - deltaScale, 1);
            yield return null;
        }

        //ensure scale is back to normal.
        bombObject.transform.localScale = originalScale;
        pulseBombCoroutineOn = false;

    }

    //when time is low, bomb will flash red. Flashing increases as time decreases.
    IEnumerator FlashBomb()
    {
        float changeRate = 1.8f;

        //flashing is quicker depending on time
        if (gm.gameTimer.time / initTime <= 0.1f)
            changeRate = 3f;
        else if (gm.gameTimer.time / initTime <= 0.3f)
            changeRate = 2.4f;

        //turn red
        while (redBodyColor.a < 1)
        {
            redBodyColor.a += changeRate * Time.deltaTime;
            Debug.Log(redBodyColor.a + " Change Rate: " + changeRate);
            redBody.color = new Color(redBodyColor.a, redBodyColor.g, redBodyColor.b, redBodyColor.a);
            yield return null;
        }

        //go transparent again
        while (redBodyColor.a > 0)
        {
            redBodyColor.a -= changeRate * Time.deltaTime;
            redBody.color = new Color(redBodyColor.a, redBodyColor.g, redBodyColor.b, redBodyColor.a);
            yield return null;
        }

        //reset
        redBodyColor.a = 0;
        redBody.color = new Color(redBodyColor.a, redBodyColor.g, redBodyColor.b, redBodyColor.a);
        flashBombCoroutineOn = false;

    }
}
