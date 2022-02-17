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
    }

   
}
