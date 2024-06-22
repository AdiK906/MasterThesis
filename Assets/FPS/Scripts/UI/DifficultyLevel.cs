using UnityEngine;

public class DifficultyLevel : MonoBehaviour
{
    public bool EasyMode;
    public bool MediumMode;
    public bool HardMode;
    public bool AdjustmentMode;
    
    public void SetEm()
    {
        EasyMode = true;
        SetMode();
    }
    public void SetMm()
    {
        MediumMode = true;
        SetMode();
    }
    public void SetHm()
    {
        HardMode = true;
        SetMode();
    }
    
    public void SetAdjustment()
    {
        AdjustmentMode = true;
        SetMode();
    }

    public void SetMode()
    {
        PlayerPrefs.SetInt("EasyMode", EasyMode ? 1 : 0);
        PlayerPrefs.SetInt("MediumMode", MediumMode ? 1 : 0);
        PlayerPrefs.SetInt("HardMode", HardMode ? 1 : 0);
        PlayerPrefs.SetInt("AdjustmentMode", AdjustmentMode ? 1 : 0);
        PlayerPrefs.Save();
    }
}
