using UnityEngine;

public class MedalManager : MonoBehaviour
{
    public Medal[] medals;          //12 total
    public static MedalManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}
