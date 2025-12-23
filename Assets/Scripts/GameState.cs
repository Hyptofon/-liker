using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance; 

    [SerializeField] StoreUpgrade[] storeUpgrades;
    [SerializeField] int updatesPerSecond = 5;
    
    [Header("Settings")]
    public float offlinePercent = 0.2f;
    
    public event Action<float, float> OnResourcesChanged; 
    public event Action<float, double> OnOfflineEarningsCalculated; 
    
    private float _count = 0;
    private float _lastIncomeValue = 0;
    private float _nextTimeCheck = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadGame();
        
        float currentIncomePerSec = CalculateTotalIncome();
        
        CalculateOfflineEarnings(currentIncomePerSec);
        
        NotifyUI(); 
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        if (_nextTimeCheck < Time.timeSinceLevelLoad)
        {
            IdleCalculation();
            _nextTimeCheck = Time.timeSinceLevelLoad + (1f / updatesPerSecond);
        }
    }
    

    void IdleCalculation()
    {
        float sum = CalculateTotalIncome();
        _lastIncomeValue = sum;
        _count += sum / updatesPerSecond;
        
        NotifyUI();
    }

    public void ClickAction()
    {
        _count++;
        _count += _lastIncomeValue * 0.02f; 
        NotifyUI();
    }

    public bool TryPurchase(int cost)
    {
        if (_count >= cost)
        {
            _count -= cost;
            NotifyUI();
            SaveGame();
            return true;
        }
        return false;
    }
    
    public float GetCurrentCount() => _count; 

    private void NotifyUI()
    {
        OnResourcesChanged?.Invoke(_count, _lastIncomeValue);
    }

    float CalculateTotalIncome()
    {
        float sum = 0;
        foreach (var upgrade in storeUpgrades)
        {
            sum += upgrade.CalculateIncomePerSecond();
        }
        return sum;
    }
    

    void CalculateOfflineEarnings(float incomePerSec)
    {

        if (PlayerPrefs.HasKey("LastLoginTime"))
        {
            long binaryTime = Convert.ToInt64(PlayerPrefs.GetString("LastLoginTime"));
            DateTime lastLogin = DateTime.FromBinary(binaryTime);
            double secondsAway = (DateTime.Now - lastLogin).TotalSeconds;

            if (secondsAway > 86400) secondsAway = 86400;
            
            float earnedOffline = (float)secondsAway * incomePerSec * offlinePercent;
            
            if (earnedOffline > 0)
            {
                _count += earnedOffline;
                OnOfflineEarningsCalculated?.Invoke(earnedOffline, secondsAway);
            }

        }

    }
    
    private void OnApplicationQuit() => SaveGame();
    private void OnApplicationPause(bool pause) { if (pause) SaveGame(); }

    public void SaveGame()
    {
        PlayerPrefs.SetFloat("Save_CookiesCount", _count);
        PlayerPrefs.SetString("LastLoginTime", DateTime.Now.ToBinary().ToString());
        foreach (var upgrade in storeUpgrades) upgrade.SaveData();
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        _count = PlayerPrefs.GetFloat("Save_CookiesCount", 0);
        foreach (var upgrade in storeUpgrades) upgrade.LoadData();
    }
    
    public void RecalculateAndNotify()
    {
        _lastIncomeValue = CalculateTotalIncome();
        NotifyUI();
    }
    
    public void InitializeGame()
    {
        float currentIncomePerSec = CalculateTotalIncome();
    
        CalculateOfflineEarnings(currentIncomePerSec);
    
        NotifyUI();
    }
}