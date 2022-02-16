using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script will manage the bomb backgrounds, including movement and colour randomization.
public class BombBackgroundManager : MonoBehaviour
{
    public BombImage[] bombImages;
    GameManager gm;
    float screenBoundaryX;
    float screenBoundaryY;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Width " + Screen.width);
        Debug.Log("Height " + Screen.height);
        //randomize the colour and position of each bomb
        SpriteRenderer sr;
        screenBoundaryX = (float)Screen.width / 100;       //100 = pixels per unit
        screenBoundaryY = (float)Screen.height / 100;

        gm = GameManager.instance;
        Vector3 screenPos = Camera.main.WorldToViewportPoint(gm.transform.position);
        List<Vector3> usedPosList = new List<Vector3>();    //used to spread the images out so they don't overlap

        foreach(BombImage bomb in bombImages)
        {            
            sr = bomb.GetComponent<SpriteRenderer>();
            float xPos = Random.Range(-screenPos.x * 2 * screenBoundaryX, screenPos.x * 2 * screenBoundaryX);
            float yPos = Random.Range(-screenPos.y * 2 * screenBoundaryY, screenPos.y * 2 * screenBoundaryY);
            Debug.Log("xPos " + xPos);
            Debug.Log("yPos " + yPos);

            //when generating random locations, I don't want the images to be too close together, so we check each random value against
            //a list of already used values.
            for (int i = 0; i < usedPosList.Count; i++)
            {
                //while ((xPos >= usedPosList[i].x && xPos <= usedPosList[i].x + 7) && 
                //(yPos >= usedPosList[i].y && yPos <= usedPosList[i].y + 7))
                while(Vector3.Distance(new Vector3(xPos, yPos), usedPosList[i]) <= 1)
                {
                
                    //xPos = Random.Range(-screenPos.x * 2 * screenBoundaryX, screenPos.x * 2 * screenBoundaryX);
                    //yPos = Random.Range(-screenPos.y * 2 * screenBoundaryY, screenPos.y * 2 * screenBoundaryY);
                    xPos = Random.Range(usedPosList[i].x - 3, usedPosList[i].x + 3);
                    yPos = Random.Range(usedPosList[i].y - 3, usedPosList[i].y + 3);
                    Debug.Log("Distance: " + Vector3.Distance(new Vector3(xPos, yPos), usedPosList[i]));
                }

                /*while (yPos >= usedPosList[i].y && yPos <= usedPosList[i].y + 5)
                {
                    yPos = Random.Range(-screenPos.y * 2 * screenBoundaryY, screenPos.y * 2 * screenBoundaryY);
                    Debug.Log("YPos Changed");
                }*/
            }
            
            //once we get here, we added the new values to used list
            usedPosList.Add(new Vector3(xPos, yPos));

            //bomb.color = new Color(redColor, greenColor, blueColor, 0.5f);
            bomb.transform.position = new Vector3(xPos, yPos, 0);

           
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
