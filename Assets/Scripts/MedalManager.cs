using UnityEngine;

public class MedalManager : MonoBehaviour
{
    public Medal[] medals;          //11 total

    public enum MedalName {ASpecialKindOfPerson, BombDefused, ComboMaster, ComboRookie, DramaticFinish, ExpertDefuser, GodDefuser, 
                            FiveThousandClub, Perfect, SpeedDemon, OneThousandClub}
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
