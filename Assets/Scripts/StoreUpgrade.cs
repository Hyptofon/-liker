using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUpgrade : MonoBehaviour
{
    [Header("UI Components (Frontend)")]
    public TMP_Text priceText;
    public TMP_Text incomeInfoText;
    public Button button;
    public Image characterImage;
    public TMP_Text upgradeNameText;

    [Header("Logic Data (Backend)")]
    public string upgradeName;
    public int startPrice = 15;
    public float upgradePriceMultiplier = 1.15f;
    public float cookiesPerUpgrade = 0.1f;
    
    private int _level = 0; 

    private void Start()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.OnResourcesChanged += CheckAffordability;
        }
        UpdateVisuals();
    }
    
    private void OnDestroy()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.OnResourcesChanged -= CheckAffordability;
        }
    }

    public void ClickAction()
    {
        int price = CalculatePrice();
        bool purchaseSuccessful = GameState.Instance.TryPurchase(price);
    
        if (purchaseSuccessful)
        {
            _level++;
            UpdateVisuals(); 
            SaveData(); 

            GameState.Instance.RecalculateAndNotify();
            AudioManager.Instance.PlayUpgradeBuy();
        }
    }

    public int CalculatePrice()
    {
       return Mathf.RoundToInt(startPrice * Mathf.Pow(upgradePriceMultiplier, _level));
    }

    public float CalculateIncomePerSecond()
    {
        return cookiesPerUpgrade * _level;
    }

    private void CheckAffordability(float currentMoney, float income)
    {
        button.interactable = currentMoney >= CalculatePrice();
    }
    
    public void UpdateVisuals()
    {
       priceText.text = CalculatePrice().ToString();
       incomeInfoText.text = _level.ToString() + " x " + cookiesPerUpgrade +  "/s";
       
       bool isPurchased = _level > 0;
       characterImage.color = isPurchased ? Color.white : Color.black;
       upgradeNameText.text = isPurchased ? upgradeName : " ??? ";
    }
    
    public void SaveData() => PlayerPrefs.SetInt("Upgrade_" + upgradeName, _level);
    
    public void LoadData()
    {
        _level = PlayerPrefs.GetInt("Upgrade_" + upgradeName, 0);
        UpdateVisuals();
    }
}