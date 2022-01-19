using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MedalObject : MonoBehaviour
{
    public Medal.Rank medalRank;
    public bool medalAcquired;    //if true, player acquired this medal. Games assumes that all medals were earned until the game says otherwise.
    public TextMeshProUGUI medalNameUI;
    public TextMeshProUGUI medalDetailsUI;
    public Image medalSprite;
   
}
