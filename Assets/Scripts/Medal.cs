using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This contains all the achievements the player can earn. They consist of bool values, and default to TRUE
//until game progression changes them.
[CreateAssetMenu(menuName = "Medal", fileName = "Medal_")]
public class Medal : ScriptableObject
{
    public Sprite medalSprite;         //can be bronze, silver, or gold
    public enum Rank {Bronze, Silver, Gold}
    public Rank rank;
    public bool medalAcquired = true;    //if true, player acquired this medal. Games assumes that all medals were earned until the game says otherwise.
    public string medalName;
    public string details;

}
