using UnityEngine;
using TMPro; 

public class PlantTimeDisplay : MonoBehaviour
{
    public TextMeshPro textMeshProComponent; 

    private PlantingSpot observedSpot;
    private float timeRemaining;

    void Awake()
    {
        if (textMeshProComponent == null)
        {
            textMeshProComponent = GetComponent<TextMeshPro>();
            if (textMeshProComponent == null)
            {
                Debug.LogError("TextMeshPro component not found on PlantTimeDisplay!", this);
                enabled = false; 
            }
        }
        Hide();
    }

    void Update()
    {
        if (observedSpot != null && observedSpot.isGrowing)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0)
            {
                timeRemaining = 0;
                Hide();
                return;
            }

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            textMeshProComponent.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
        else
        {
            Hide(); 
        }
    }

    public void Show(PlantingSpot spot, float initialTime)
    {
        observedSpot = spot;
        timeRemaining = initialTime;
        gameObject.SetActive(true);
        Update(); 
    }

    public void Hide()
    {
        observedSpot = null;
        gameObject.SetActive(false);
    }
}