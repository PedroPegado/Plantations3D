using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class ItemNotificationUI : MonoBehaviour
{
    public Image backgroundImage;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;

    private float _displayDuration = 3f; 
    private Vector3 _initialPosition; 
    private Vector3 _hiddenPosition;  
    private CanvasGroup _canvasGroup; 

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        _canvasGroup.alpha = 0f;
        _initialPosition = transform.localPosition;
    }

    public void Setup(Sprite icon, string name, float displayDuration)
    {
        itemIcon.sprite = icon;
        itemNameText.text = $"{name}";
        _displayDuration = displayDuration;
        _hiddenPosition = _initialPosition + new Vector3(0, 20, 0); 

        ShowNotification();
    }

    private void ShowNotification()
    {
        transform.DOKill(true);
        _canvasGroup.DOKill(true);

        _canvasGroup.alpha = 0f;
        transform.localPosition = _initialPosition;

        _canvasGroup.DOFade(1f, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => StartCoroutine(HideAfterDelay()));
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_displayDuration);

        _canvasGroup.DOFade(0f, 0.5f) 
            .SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(gameObject)); 

    }

    void OnDestroy()
    {
        transform.DOKill(true);
        _canvasGroup?.DOKill(true);
    }
}