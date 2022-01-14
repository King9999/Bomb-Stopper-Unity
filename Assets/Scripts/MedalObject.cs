using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MedalObject : MonoBehaviour
{
    public Medal.Rank medalRank;
    public bool medalAcquired;    //if true, player acquired this medal. Games assumes that all medals were earned until the game says otherwise.
    public TextMeshProUGUI medalNameUI;
    public TextMeshProUGUI medalDetailsUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
