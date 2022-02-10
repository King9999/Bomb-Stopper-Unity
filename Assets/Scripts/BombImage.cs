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
    }

    void RandomizeColor()
    {
        redColor = Random.Range(0f, 1f);
        greenColor = Random.Range(0f, 1f);
        blueColor = Random.Range(0f, 1f);
        sr.color = new Color(redColor, greenColor, blueColor, 0.4f);   
    }
}
