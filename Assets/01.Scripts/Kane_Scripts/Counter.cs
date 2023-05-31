using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    //public Text PriceText;
    public int Upgrade_Level = 0;
    public double[] UpgradePrice = new double[4] { 100d, 200d, 500d, 1000d };
    public double CurrentPrice = 100d;

    public bool isPlayerIn = false;

    Transform _player;
    public GameObject Money_Pref;
    public Stack<Product> BatteryStack = new Stack<Product>();
    // ============================
    private void Start()
    {
        _interactArea = GetComponentInChildren<InteractArea>();
        _interactArea.SetTarget(this, InteractArea.TargetType.Counter);
        //PriceText = _interactArea.GetComponentInChildren<Text>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;  //Managers.Game._stagemanager._player.transform;
        Upgrade_Level = Managers.Data.GetInt("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_counter");

        Setting();

        _interactArea.SetPrice(CurrentPrice);
        CurrentPrice = UpgradePrice[Upgrade_Level];
        if (Money_Pref == null) Money_Pref = Resources.Load<GameObject>("Money_Pref");
    }


    private void Update()
    {
        if (isPlayerIn)
        {
            if (Managers.Game.Money >= UpgradePrice[Upgrade_Level] * 1f * Time.deltaTime && Upgrade_Level < 1)
            {
                //Managers.Game.Money -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                Managers.Game.UpdateMoney(-UpgradePrice[Upgrade_Level] * 1f * Time.deltaTime);
                CurrentPrice -= UpgradePrice[Upgrade_Level] * 1f * Time.deltaTime;
                Transform _momey = Managers.Pool.Pop(Money_Pref).transform;

                _momey.SetParent(_player);
                _momey.transform.localPosition = Vector3.zero;
                _momey.DOJump(transform.position, 2f, 1, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _momey.gameObject.SetActive(false);
                    Managers.Pool.Push(_momey.GetComponent<Poolable>());
                });
                if (CurrentPrice <= 0)
                {
                    CurrentPrice = 0;
                    isPlayerIn = false;
                    Upgrade();
                }
                //PriceText.text = $"{CurrentPrice:0}";
                _interactArea.UpdatePrice(CurrentPrice);

            }
        }
    }

    [Button]
    public void Upgrade()
    {
        if (Upgrade_Level == 0)
        {
            _staff.SetActive(true);
            Managers.Sound.Play("NewObj");
        }
        else
        {
            Managers.Sound.Play("UpgradeObj");
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
            _interactArea.gameObject.SetActive(false);
        }
        else
        {
            _staff.SetActive(false);
            _interactArea.gameObject.SetActive(true);
        }


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
