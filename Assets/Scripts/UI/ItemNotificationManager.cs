using UnityEngine;
using System.Collections.Generic;

public class ItemNotificationManager : MonoBehaviour
{
    public static ItemNotificationManager Instance { get; private set; } 

    [Header("UI References")]
    public GameObject itemNotificationPrefab; 
    public Transform notificationParent; 

    [Header("Settings")]
    public float notificationDisplayDuration = 3f; 
    public int maxNotificationsDisplayed = 3; 

    private Queue<NotificationData> notificationQueue = new Queue<NotificationData>();
    private List<ItemNotificationUI> activeNotifications = new List<ItemNotificationUI>();

    private class NotificationData
    {
        public Sprite icon;
        public string name;
        public int quantity;

        public NotificationData(Sprite icon, string name, int quantity)
        {
            this.icon = icon;
            this.name = name;
            this.quantity = quantity;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        while (notificationQueue.Count > 0 && activeNotifications.Count < maxNotificationsDisplayed)
        {
            ShowNextNotification();
        }

        activeNotifications.RemoveAll(item => item == null);
    }

    public void AddNotification(Sprite icon, string itemName, int quantity = 1)
    {
        NotificationData newNotification = new NotificationData(icon, itemName, quantity);
        notificationQueue.Enqueue(newNotification);
    }

    private void ShowNextNotification()
    {
        if (notificationQueue.Count > 0)
        {
            NotificationData data = notificationQueue.Dequeue();
            GameObject notificationObj = Instantiate(itemNotificationPrefab, notificationParent);
            ItemNotificationUI notificationUI = notificationObj.GetComponent<ItemNotificationUI>();

            if (notificationUI != null)
            {
                notificationUI.Setup(data.icon, data.name, notificationDisplayDuration);
                activeNotifications.Add(notificationUI);
            }
            else
            {
                Debug.LogError("Prefab da notificação de item não tem o script ItemNotificationUI!", itemNotificationPrefab);
                Destroy(notificationObj); 
            }
        }
    }
}