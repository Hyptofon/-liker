using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] TMP_Text countText;
    [SerializeField] TMP_Text incomeText;

    [Header("Offline UI Popup")]
    [SerializeField] GameObject offlinePanel; 
    [SerializeField] TMP_Text offlineEarningsText;

    private void Start()
    {
        if(offlinePanel != null) offlinePanel.SetActive(false);
        GameState.Instance.OnResourcesChanged += UpdateMainUI;
        GameState.Instance.OnOfflineEarningsCalculated += ShowOfflinePopup;
        
        GameState.Instance.InitializeGame();
    }

    private void OnDestroy()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.OnResourcesChanged -= UpdateMainUI;
            GameState.Instance.OnOfflineEarningsCalculated -= ShowOfflinePopup;
        }
    }
    
    void UpdateMainUI(float count, float income)
    {
        countText.text = Mathf.RoundToInt(count).ToString();
        incomeText.text = income.ToString("F1") + "/s"; 
    }
    
    void ShowOfflinePopup(float moneyEarned, double secondsGone)
    {
        offlinePanel.SetActive(true);
        
        int totalSeconds = Mathf.FloorToInt((float)secondsGone);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
    
        string timeText = minutes > 0 
            ? $"{minutes} min. {seconds} sec." 
            : $"{seconds} sec.";
        
        float percent = GameState.Instance.offlinePercent * 100;
        
        string finalMessage = $"Welcome back!\nGone: {timeText}\nPassive: {percent:F0}%\nEarned: <color=green>+{Mathf.RoundToInt(moneyEarned)}</color>";
        
        offlineEarningsText.text = finalMessage;
            
    }

    public void CloseOfflinePopup()
    {
        if(offlinePanel != null) 
        {
            offlinePanel.SetActive(false); 
        }
    }

    public void OnMainClick()
    {
        GameState.Instance.ClickAction();
        AudioManager.Instance.PlayCookieClick();
    }
}