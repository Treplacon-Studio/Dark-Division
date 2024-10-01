using UnityEngine;
using TMPro;

public class PracticeRangeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomCodeText;

    void Start()
    {
        roomCodeText.text = RoomManager.RoomCode;
    }
}
