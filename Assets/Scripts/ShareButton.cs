using UnityEngine;

//This script's purpose is to not display the share button if the game build is WebGL.
public class ShareButton : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_WEBGL
            gameObject.SetActive(false);
        #endif

        //if I don't have the following code, when I run the game in the editor while WebGL build is selected, the button won't appear.
        #if UNITY_EDITOR    
            gameObject.SetActive(true);
        #endif
    }

   
}
