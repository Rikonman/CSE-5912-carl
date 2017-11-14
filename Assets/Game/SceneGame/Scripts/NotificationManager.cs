using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour {

    [SerializeField]
    private GameObject notificationItemPrefab;

    private static float notificationTime = 5f; // The amount of time (in seconds) to show the notification before making it disappear
    private static GameObject notificationItem;
    private static GameObject notificationPanel;

    private void Start()
    {
        notificationPanel = gameObject;
        notificationItem = notificationItemPrefab;
    }

    public static void NewNotification(string message)
    {
        if (notificationItem != null && notificationPanel != null)
        {
            GameObject newObj = Instantiate(notificationItem, notificationPanel.transform);
            newObj.GetComponent<NotificationItem>().Setup(message);
            Destroy(newObj, notificationTime);
        }
        else
        {
            if (notificationItem == null)
                Debug.LogError("Can't create notification: static notificationItem GameObject is NULL");
        }
    }

    public static string GetRedString(string msg)
    {
        return "<color=#FF0000>" + msg + "</color>";
    }

    public static string GetBlueString(string msg)
    {
        return "<color=#0099FF>" + msg + "</color>";
    }

    public static string GetColoredString(Color clr, string msg)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(clr)  + ">" + msg + "</color>";
    }
}
