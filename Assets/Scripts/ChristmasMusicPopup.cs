using UnityEngine;
using TMPro;
using DG.Tweening;
using Plugins.MusicNotificationSystem.Runtime; 

public class ChristmasMusicPopup : BaseMusicPopup
{
    [Header("UI References")]
    [SerializeField] private TMP_Text trackNameText;
    [SerializeField] private TMP_Text artistText;
    [SerializeField] private RectTransform panelRect;

    [Header("Settings")]
    [SerializeField] private float showDuration = 3f;
    [SerializeField] private Vector2 hiddenPosition = new Vector2(-500, 400); 
    [SerializeField] private Vector2 visiblePosition = new Vector2(20, 400);
    
    private void OnEnable()
    {
        if (MusicController.Instance != null)
        {
            MusicController.Instance.OnTrackChanged += Show;
        }
    }
    private void OnDisable()
    {
        if (MusicController.Instance != null)
        {
            MusicController.Instance.OnTrackChanged -= Show;
        }
    }

    private void Start()
    {
        panelRect.anchoredPosition = hiddenPosition;
    }

    public override void Show(MusicTrack track)
    {
        trackNameText.text = track.trackName;
        artistText.text = track.artistName;
        panelRect.DOKill(); 
        panelRect.DOAnchorPos(visiblePosition, 0.5f).SetEase(Ease.OutBack);
        DOVirtual.DelayedCall(showDuration, Hide);
    }

    public override void Hide()
    {
        panelRect.DOAnchorPos(hiddenPosition, 0.5f).SetEase(Ease.InBack);
    }
}