using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is used to display a bomb sprite in the background. Its colour will be random and will change colour again when it goes off screen.

public class BombImage : MonoBehaviour
{
    Vector3 screenPos;      //used to check if object is offscreen
    GameManager gm;
    SpriteRenderer sr;
     float redColor;
     float greenColor;
     float blueColor;
    float screenBoundaryX;
    float screenBoundaryY;

    //coroutine stuff
    bool rotateCoroutineOn;
    bool isRotating;
    float rotateCooldown;       //prevents constant checking to rotate image. Time in seconds.
    float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        screenPos = Camera.main.WorldToViewportPoint(gm.transform.position);
        sr = GetComponent<SpriteRenderer>();

        //set a random colour
        RandomizeColor();

        screenBoundaryX = (float)Screen.width / 100;       //100 = pixels per unit
        screenBoundaryY = (float)Screen.height / 100;

        rotateCooldown = 3;

        /*redColor = Random.Range(0f, 1f);
        greenColor = Random.Range(0f, 1f);
        blueColor = Random.Range(0f, 1f);
        sr.color = new Color(redColor, greenColor, blueColor, 0.4f);*/
    }

    // Update is called once per frame
    void Update()
    {
        //move the image. It moves up and left simultaneously.
        transform.position = new Vector3(transform.position.x - Time.deltaTime, transform.position.y + Time.deltaTime, transform.position.z);

        //check if this object is off screen and change its colour. The object will be moving up and diagonally left, so the boundary
        //check will be based on that movement.
        if (transform.position.x < screenPos.x * 2 * screenBoundaryX && transform.position.y > screenPos.y * 2 * screenBoundaryY)
        {
            //relocate the image and change its colour
            transform.position = new Vector3(transform.position.x * -1, transform.position.y * -1, transform.position.z);
            RandomizeColor();
        }

        //rotate check      
        if (!rotateCoroutineOn && Time.time > currentTime + rotateCooldown)
        {
            float chance = Random.Range(0f, 1f);
            if (chance <= 0.5f)
            {
                rotateCoroutineOn = true;
                Debug.Log("Coroutine started");
                StartCoroutine(Rotate());
            }
            else
            {
                currentTime = Time.time;
            }

        }
    }

    void RandomizeColor()
    {
        redColor = Random.Range(0f, 1f);
        greenColor = Random.Range(0f, 1f);
        blueColor = Random.Range(0f, 1f);
        sr.color = new Color(redColor, greenColor, blueColor, 0.4f);   
    }

    //rotate the image's Y axis
    IEnumerator Rotate()
    {
        while(transform.rotation.y >= 0)
        {
            float yValue = 90 * Time.deltaTime;
            transform.Rotate(0, yValue, 0);
            //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + yValue, transform.rotation.z, transform.rotation.w);
            yield return null;
        }

        //yield return new WaitForSeconds(1);

       /* while(transform.rotation.y < 360)
        {
            float yValue = 5 * Time.deltaTime;
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + yValue, transform.rotation.z, transform.rotation.w);
            yield return null;
        }*/
        //yield return null;
        Debug.Log("Coroutine ended");
        transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
        rotateCoroutineOn = false;
        currentTime = Time.time;
    }
}
