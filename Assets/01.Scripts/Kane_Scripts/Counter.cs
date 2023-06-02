using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
//using static UnityEditor.Experimental.GraphView.GraphView;

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
    //public int Upgrade_Level = 0;
    //public bool isActive = false;
    public int UpgradeLevel = 0;
    public double[] UpgradePrice = new double[] { 100d, 200d };
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
        //isActive = Managers.Data.GetBool("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_counter");
        UpgradeLevel = Managers.Data.GetInt("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_counter");
        Setting();

        if (UpgradeLevel < 2)
        {
            CurrentPrice = UpgradePrice[UpgradeLevel];
            _interactArea.SetPrice(CurrentPrice);
        }
        if (Money_Pref == null) Money_Pref = Resources.Load<GameObject>("Money_Pref");

    }

    private void OnEnable()
    {
        StartCoroutine(Cor_Update());

    }


    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return null;
            if (isPlayerIn)
            {
                if (Managers.Game.Money >= UpgradePrice[UpgradeLevel] * 1f * Time.deltaTime)
                {
                    //Managers.Game.Money -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                    Managers.Game.UpdateMoney(-UpgradePrice[UpgradeLevel] * 1f * Time.deltaTime);
                    CurrentPrice -= UpgradePrice[UpgradeLevel] * 1f * Time.deltaTime;
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
                        //isActive = true;
                        UpgradeLevel++;
                        Upgrade();

                        if (UpgradeLevel >= 2) break;
                    }
                    //PriceText.text = $"{CurrentPrice:0}";
                    _interactArea.UpdatePrice(CurrentPrice);

                }
                else
                {
                    Debug.Log("not upgrade");
                }
            }
        }
    }

    [Button]
    public void Upgrade()
    {
        Managers.Sound.Play("NewObj");
        Setting();

        GameObject _obj = Managers.Pool.Pop(Resources.Load<GameObject>("NewEffect"), transform).gameObject;
        _obj.transform.localPosition = Vector3.zero;
        _obj.GetComponent<ParticleSystem>().PlayAllParticle();
        DOTween.Sequence().AppendInterval(1f).OnComplete(() => { Managers.Pool.Push(_obj.GetComponent<Poolable>()); });

        if (UpgradeLevel == 1)
        {
            Managers.Game._stagemanager._tutorial.LockOff();
        }
        else if (UpgradeLevel == 2)
        {
            Managers.Game._stagemanager._tutorial.LockOff();
        }

        if (UpgradeLevel < 2)
        {
            CurrentPrice = UpgradePrice[UpgradeLevel];
            _interactArea.SetPrice(CurrentPrice);
        }
        else
        {
            _interactArea.gameObject.SetActive(false);

        }
        //Managers.Data.SetBool("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_counter", isActive);
        if (Managers.Game._stagemanager._stageData.isFirst == false)
        {
            SaveData();
            
        }
    }

    public void Setting()
    {

        switch (UpgradeLevel)
        {
            case 0:
                _staff.SetActive(false);
                _interactArea.gameObject.SetActive(true);
                GetComponent<Renderer>().enabled = false;
                StackPoint.gameObject.SetActive(false);

                break;

            case 1:
                _staff.SetActive(false);
                _interactArea.gameObject.SetActive(true);
                GetComponent<Renderer>().enabled = true;
                StackPoint.gameObject.SetActive(true);
                break;

            case 2:
                _staff.SetActive(true);
                _interactArea.gameObject.SetActive(false);
                StackPoint.gameObject.SetActive(true);
                break;

            default:

                break;
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

    public void SaveData()
    {
        Managers.Data.SetInt("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_counter", UpgradeLevel);
    }





}
