using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Counter : MonoBehaviour
{
    //public bool isSell = true;
    public Transform StackPoint;
    public GameObject _staff;

    // ==== values
    public float Stack_Interval = 0.2f;
    public float Update_Interval = 1f;
    public float BaseUp_Interval = 0.5f;


    // =====================
    public InteractArea _interactArea;
    public Text PriceText;
    public int Upgrade_Level = 0;
    public double[] UpgradePrice = new double[4] { 100d, 200d, 500d, 1000d };
    public double CurrentPrice = 100d;

    public bool isPlayerIn = false;



    public Stack<Product> BatteryStack = new Stack<Product>();
    // ============================
    private void Start()
    {
        _interactArea = GetComponentInChildren<InteractArea>();
        _interactArea.SetTarget(this, InteractArea.TargetType.Counter);
        PriceText = _interactArea.GetComponentInChildren<Text>();
        _staff.SetActive(false);
        Upgrade_Level = Managers.Data.GetInt("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_counter");

        Setting();

        CurrentPrice = UpgradePrice[Upgrade_Level];
    }


    private void Update()
    {
        if (isPlayerIn)
        {
            if (Managers.Game.Money >= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime && Upgrade_Level < 1)
            {
                //Managers.Game.Money -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                Managers.Game.UpdateMoney(-UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime);
                CurrentPrice -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                if (CurrentPrice <= 0)
                {
                    CurrentPrice = 0;
                    isPlayerIn = false;
                    Upgrade();
                }
                PriceText.text = $"{CurrentPrice:0}";

            }
        }
    }

    [Button]
    public void Upgrade()
    {
        if (Upgrade_Level == 0)
        {
            _staff.SetActive(true);
        }
        Upgrade_Level++;
        _interactArea.gameObject.SetActive(false);
        Managers.Data.SetInt("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_counter", Upgrade_Level);
    }

    public void Setting()
    {
        if (Upgrade_Level > 0)
        {
            _staff.SetActive(true);
        }
        _interactArea.gameObject.SetActive(false);

    }


    public void PushBattery(Product _product, float _interval = 0.5f)
    {
        Stack_Interval = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        DOTween.Kill(_product);
        _product.transform.SetParent(transform);
        BatteryStack.Push(_product);
        _product.transform.DOJump(StackPoint.position + Vector3.up * (BaseUp_Interval + (BatteryStack.Count - 1) * Stack_Interval), 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DORotate(Vector3.up * 45f, _interval).SetEase(Ease.Linear));
        //.OnComplete(() => BatteryStack.Push(_product));
    }







}
