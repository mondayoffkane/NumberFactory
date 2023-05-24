using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

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

    [ShowInInspector]
    public List<StageData> _stageData = new List<StageData>();

    public void SaveData(StageData _tempdata, int _stagenum)
    {
        SetDouble("Money", Managers.Game.Money);
        _stageData[_stagenum] = _tempdata;
        ES3.Save<StageData>("StageData" + _stagenum.ToString(), _stageData[_stagenum]);
    }


    public StageData LoadData(int _stagenum)
    {
        if (_stageData.Count == 0)
        {
            _stageData.Add(new StageData());
        }

        Managers.Game.Money = GetDouble("Money");
        if (Managers.Game.Money < 500) Managers.Game.Money = 500;
        _stageData[_stagenum] = ES3.Load<StageData>("StageData" + _stagenum.ToString(), new StageData());

        return _stageData[_stagenum];

    }

}


public class StageData
{
    public int PlayerSpeed_Level = 0;
    public int PlayerCapacity_Level = 0;
    public int PlayerIncome_Level = 0;

    public int StaffSpeed_Level = 0;
    public int StaffCapacity_Level = 0;
    public int StaffHire_Level = 0;

    public int[] MachineLevels = new int[3] { 0, 0, 0 };

}





