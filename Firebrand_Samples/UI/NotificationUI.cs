using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    public GameObject notificationHolder;

    [Header("Notification Icons")]
    public GameObject notificationExpanded;
    public Text titleExpanded;
    public Text descExpanded;


    public GameObject notificationPrefab;

    List<Notification> notificationQueue;
    List<Notification> activeNotifications;

    public GameObject overflowCounter;
    public Text overflowText;

    public Transform[] notificationSlots;
    public Transform hiddenSlot;

    [Header("Notification Icons")]
    public Sprite exclamation;
    public Sprite question;
    public Sprite elipsis;

    public void Awake()
    {
        notificationQueue = new List<Notification>();
        activeNotifications = new List<Notification>();
        notificationExpanded.SetActive(false);
        overflowCounter.SetActive(false);
    }

    public void AddNotification(string title, string description, int image)  //, Sprite image
    {
        GameObject obj = Instantiate(notificationPrefab, hiddenSlot, false);
        Notification newNotification = obj.GetComponent<Notification>();

        newNotification.notificationTitle = title;
        newNotification.notificationBody = description;
        newNotification.inSlot = false;

        switch (image)
        {
            case 0:
                newNotification.notificationImage.sprite = exclamation;
                break;
            case 1:
                newNotification.notificationImage.sprite = question;
                break;
            case 2:
                newNotification.notificationImage.sprite = elipsis;
                break;

            default:
                newNotification.notificationImage.sprite = exclamation;
                break;
        }


        newNotification.notificationHolder = this;

        notificationQueue.Add(newNotification);
    }

    public void PopulateNotifications()
    {
        if(activeNotifications.Count == notificationSlots.Length || notificationQueue.Count == 0)
        {
            if(notificationQueue.Count > 0)
            {
                int overflowCount = notificationQueue.Count;
                overflowText.text = "+" + overflowCount.ToString();
                overflowCounter.SetActive(true);
            }
            return;
        }
        
        int slots = notificationSlots.Length;
        int notifLength = notificationQueue.Count;
        Debug.Log("Populating " + notifLength +" Notifications in " + slots + " slots");



        int freeSlots = (slots - activeNotifications.Count);
        for(int x = slots-freeSlots; x < slots; x++)
        {
            if(notificationQueue.Count > 0)
            {
                Debug.Log("Moving Notification " + x + " to slot " + x);
                Notification not = notificationQueue.First();

                Debug.Log(not.notificationTitle);

                not.gameObject.transform.SetParent(notificationSlots[x], false);
                not.gameObject.transform.position = notificationSlots[x].position;
                not.button.onClick.AddListener(() => ExpandNotification(not));

                not.inSlot = true;
                not.slotNum = x;

                activeNotifications.Add(not);
                notificationQueue.Remove(not);
            }
        }

        if(notificationQueue.Count > 0)
        {
            int overflowCount = notificationQueue.Count;
            overflowText.text = "+" + overflowCount.ToString();
            overflowCounter.SetActive(true);
        }
    }

    public void UpdateNotifications(int slotNum)
    {
        //TODO: Refactor this so that it interates over the slots, not the notifications

        for(int x = slotNum; x < notificationSlots.Length; x++)
        {
            Notification moveNotif;
            int moveIndex = x + 1;
            if (moveIndex >= notificationSlots.Length)
            {
                if(notificationQueue.Count > 0)
                {
                    moveNotif = notificationQueue.First();
                    moveNotif.button.onClick.AddListener(() => ExpandNotification(moveNotif));
                    activeNotifications.Add(moveNotif);
                    notificationQueue.Remove(moveNotif);

                    if (notificationQueue.Count > 0)
                    {
                        overflowText.text = "+" + notificationQueue.Count;
                    }
                    else
                    {
                        overflowCounter.SetActive(false);
                    }
                } else
                {
                    continue;
                }
            }
            else
            {
                //moveNotif = activeNotifications.Find(n => n.slotNum == moveIndex);
                moveNotif = notificationSlots[moveIndex].GetComponentInChildren<Notification>();
            }

            moveNotif.gameObject.transform.SetParent(notificationSlots[x], false);
            moveNotif.gameObject.transform.position = notificationSlots[x].transform.position;
            moveNotif.slotNum = x;
        }

        /*
        for(int x = slotNum; x < activeNotifications.Count; x++)
        {
            Debug.Log(x);
            int moveIndex = x + 1;
            if(moveIndex > notificationSlots.Length)
            {

            }
            else
            {
                Notification notifToMove = notificationSlots[moveIndex].GetComponentInChildren<Notification>();
                notifToMove.gameObject.transform.SetParent(notificationSlots[x], false);
                notifToMove.slotNum = x;
            }
        }

        if(notificationQueue.Count > 0)
        {
            Notification newNotif = notificationQueue.First();
            newNotif.gameObject.transform.SetParent(notificationSlots[notificationSlots.Length - 1], false);
            newNotif.gameObject.transform.position = notificationSlots[notificationSlots.Length - 1].position;
            newNotif.button.onClick.AddListener(() => ExpandNotification(newNotif));

            notificationQueue.Remove(newNotif);
            if(notificationQueue.Count > 0)
            {
                overflowText.text = "+" + notificationQueue.Count;
            } else
            {
                overflowCounter.SetActive(false);
            }
        }
        */
    }

    public void ExpandNotification(Notification notification)
    {
        int slotNum = notification.slotNum;

        titleExpanded.text = notification.notificationTitle;
        descExpanded.text = notification.notificationBody;

        notificationExpanded.SetActive(true);

        activeNotifications.Remove(notification);
        Destroy(notification.gameObject);

        Debug.Log("Updating at " + slotNum);
        Debug.Log("Active Notifications " + activeNotifications.Count);
        UpdateNotifications(slotNum);
    }

    public void DismissExpandedNotification()
    {
        notificationExpanded.SetActive(false);
    }
}
