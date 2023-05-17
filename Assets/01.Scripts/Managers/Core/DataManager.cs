using UnityEngine;

[System.Serializable]
public class DataManager
{
    ///<summary>Manager생산할때 만들어짐</summary>
    public void Init()
    {
        GetData();
    }

    public bool UseHaptic
    {
        get => _useHaptic;
        set
        {
            _useHaptic = value;
            ES3.Save<bool>("Haptic", value);
        }
    }
    [SerializeField]
    private bool _useHaptic;

    public bool UseSound
    {
        get => _useSound;
        set
        {
            _useSound = value;
            ES3.Save<bool>("Sound", value);
            Managers.Sound.BgmOnOff(value);
        }
    }
    [SerializeField]
    private bool _useSound;


    public void SaveData()
    {
    }
    public void GetData()
    {
        UseHaptic = ES3.Load<bool>("Haptic", true);
        UseSound = ES3.Load<bool>("Sound", true);
    }

    // =========================

    //public StageData[] StageDatas;

    // ========== Get
    public int GetInt(string _str)
    {
        return ES3.Load<int>(_str, 0);
    }
    public float GetFloat(string _str)
    {
        return ES3.Load<float>(_str, 0f);
    }
    public double GetDouble(string _str)
    {
        return ES3.Load<double>(_str, 0d);
    }

    public bool GetBool(string _str)
    {
        return ES3.Load<bool>(_str, false);
    }

    // ========== Set
    public void SetInt(string _str, int _default = 0)
    {
        ES3.Save<int>(_str, _default);
    }
    public void SetFloat(string _str, float _default = 0)
    {
        ES3.Save<float>(_str, _default);
    }
    public void SetDouble(string _str, double _default = 0)
    {
        ES3.Save<double>(_str, _default);
    }

    public void SetBool(string _str, bool _default = false)
    {
        ES3.Save<bool>(_str, _default);
    }


}


//public class StageData
//{
//    public int PlayerSpeed_Level;
//    public int PlayerCapacity_Level;
//    public int PlayerIncome_Level;

//    public int StaffSpeed_Level;
//    public int StaffCapacity_Level;
//    public int StaffCount_Level;

//}
