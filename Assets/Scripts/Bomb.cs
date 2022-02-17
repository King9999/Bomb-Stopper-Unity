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
    float sparkMoveSpeed;               //varies by difficulty
    public Transform[] sparkPoints;
    int currentPoint;                   //iterator for the sparkPoints array.
    Color redBodyColor;
    public Slider bombFuse;
    float initTime;                 //used to measure the length of fuse
    float totalDistance;            //the total distance from the first spark point to the last.

    //coroutine bools
    bool pulseBombCoroutineOn;
    bool flashBombCoroutineOn;
    bool animateSparkCoroutineOn;

    GameManager gm;
    TitleManager tm;

    // Start is called before the first frame update
    void Start()
    {
        bombFuse.value = 1;
        redBodyColor = redBody.color;
        redBodyColor.a = 0;
        redBody.color = new Color(redBodyColor.r, redBodyColor.g, redBodyColor.b, redBodyColor.a);    //transparent by default
        gm = GameManager.instance;  //this is here to ensure the instance isn't null at runtime.
        tm = TitleManager.instance;

        initTime = gm.time;         //need this so the fuse length is constant
        StartCoroutine(ReduceFuse());
        StartCoroutine(AnimateSpark());

       

        //spark set up. Its position changes if a certain rule is enabled
        currentPoint = gm.sr.specialRule == SpecialRules.Rule.ReducedTime ? sparkPoints.Length - 2 : 0;
        spark.transform.position = sparkPoints[currentPoint].position;
        if (tm.currentDifficulty == TitleManager.Difficulty.Easy)
            sparkMoveSpeed = 2f;
        else if (tm.currentDifficulty == TitleManager.Difficulty.Normal)
            sparkMoveSpeed = 2.5f;
        else
            sparkMoveSpeed = 1.5f;

        totalDistance = Vector3.Distance(sparkPoints[0].position, sparkPoints[sparkPoints.Length - 1].position);
        //Debug.Log("Total Distance " + totalDistance);
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
            
            /*if(gm.gameStarted && currentPoint + 1 < sparkPoints.Length)
            {
                float moveSpeed = 1f;
                //float moveSpeed = (gm.gameTimer.time / initTime) * totalDistance;
                Vector3 direction = (sparkPoints[currentPoint + 1].position - sparkPoints[currentPoint].position).normalized;
                spark.transform.position += direction * moveSpeed * Time.deltaTime;
                
                //if spark is close to the destination, want it to "snap" to the destination point so it doesn't overshoot
                float diffX = Mathf.Abs(sparkPoints[currentPoint + 1].position.x - spark.transform.position.x);
                float diffY = Mathf.Abs(sparkPoints[currentPoint + 1].position.y - spark.transform.position.y);
                if (diffX >= 0 && diffX < 0.05f && diffY >= 0 && diffY < 0.05f)
                {
                    spark.transform.position = sparkPoints[currentPoint + 1].position;
                }

                if (spark.transform.position == sparkPoints[currentPoint + 1].position)
                {
                    currentPoint++;
                }

            }*/
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

            //move spark along spark points
            
            if(gm.gameStarted && currentPoint + 1 < sparkPoints.Length)
            {
                //float moveSpeed = 2f;
                //float moveSpeed = (gm.gameTimer.time / initTime) * totalDistance;
                Vector3 direction = (sparkPoints[currentPoint + 1].position - sparkPoints[currentPoint].position).normalized;
                spark.transform.position += direction * sparkMoveSpeed * Time.deltaTime;
                
                //if spark is close to the destination, want it to "snap" to the destination point so it doesn't overshoot
                float diffX = Mathf.Abs(sparkPoints[currentPoint + 1].position.x - spark.transform.position.x);
                float diffY = Mathf.Abs(sparkPoints[currentPoint + 1].position.y - spark.transform.position.y);
                if (diffX >= 0 && diffX < 0.05f && diffY >= 0 && diffY < 0.05f)
                {
                    spark.transform.position = sparkPoints[currentPoint + 1].position;
                }

                if (spark.transform.position == sparkPoints[currentPoint + 1].position)
                {
                    currentPoint++;
                }

            }
            yield return null;
        }
        
    }

    //spark quickly grows and reverts to normal. Tried to use particle effects but it's more trouble than it's worth.
    IEnumerator AnimateSpark()
    {
        while(!gm.gameTimer.TimeUp())
        {
            float initScale = spark.transform.localScale.x;
            float maxScale = spark.transform.localScale.x + 1;

            while (spark.transform.localScale.x < maxScale)
            {
                float scaleValue = 4f * Time.deltaTime;
                spark.transform.localScale = new Vector3(spark.transform.localScale.x + scaleValue, spark.transform.localScale.y + scaleValue, 1);
                yield return null;
            }

            //return to normal.
            spark.transform.localScale = new Vector3(initScale, initScale, 1);

            
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
            //Debug.Log(redBodyColor.a + " Change Rate: " + changeRate);
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
