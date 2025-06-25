using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public GameObject interactionCanvas;
    public TextMeshProUGUI interactionText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Show(string message, Vector3 position)
    {
        interactionCanvas.SetActive(true);
        interactionCanvas.transform.LookAt(Camera.main.transform);
        interactionText.text = message;

        interactionCanvas.transform.position = position;
    }

    public void Hide()
    {
        interactionCanvas.SetActive(false);
    }
}
