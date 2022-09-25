using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public NotificationUI notificationHolder;
    public bool inSlot;
    public int slotNum;

    public Image notificationImage;

    public string notificationTitle;

    [TextArea(10,15)]
    public string notificationBody;

    public Transform currentSlot;
    public Transform targetSlot;

    public Button button;
}
