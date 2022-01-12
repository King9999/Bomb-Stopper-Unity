using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour/*, IPointerEnterHandler, IPointerExitHandler*/
{
    public enum Difficulty {Easy, Normal, Hard, Special}
    public Difficulty currentDifficulty;

    [Header("UI")]
    public Button easyButton;
    

    [Header("------")]
    public static TitleManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //mouseOverColor = Color.red;
        //easyButton.image.color = mouseOverColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
