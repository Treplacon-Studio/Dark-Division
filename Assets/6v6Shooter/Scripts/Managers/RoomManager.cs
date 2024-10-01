
using UnityEngine;
using TMPro;
public class RoomManager : MonoBehaviour
{
    public static string RoomCode { get; private set; }

    public static void SetRoomCode(string code)
    {
        RoomCode = code;
    }
}
