using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;


public class ChargingTable : MonoBehaviour
{
    public string _objectName = "Table";
    public int Table_Num = 0;
    public bool isActive = false;


    //[ShowInInspector]
    //public Stack<Product> BatteryStack = new Stack<Product>();
    public Transform StackPoint;
    //public int Order_Count = 0;

    public bool isEmpty = true;

    public double BasePrice = 20d;
    public Stack<Transform> MoneyStack = new Stack<Transform>();
    public GameObject Money_Pref;
    public Transform MoneyStackPoint;
    public Transform customerPos;
    public InteractArea _interactArea;



    // ===== Values
    public float Stack_Interval = 0.2f;
    public float Update_Interval = 1f;
    public float BaseUp_Interval = 0.5f;
    public double Upgrade_Price = 100d;
    public double Current_Price = 100d;

    // ======= money 
    public int width = 5, height = 3;
    public float Left = 0.5f;
    public float back = 0.2f;
    public float horizontal_interval = 0.3f;
    public float vertical_inteval = 0.6f;
    public float height_interval = 0.1f;
    // =============================
    public Text PriceText;
    public bool isPlayerIn = false;

    Renderer _renderer;
    // ==================
    private void Start()
    {
        if (StackPoint == null) StackPoint = transform.GetChild(0);

        _interactArea = GetComponentInChildren<InteractArea>();
        _interactArea.SetTarget(this, InteractArea.TargetType.Table);
        PriceText = _interactArea.GetComponentInChildren<Text>();
        _renderer = GetComponent<Renderer>();
        _renderer.enabled = false;
        GetComponent<SphereCollider>().isTrigger = true;

        isActive = Managers.Data.GetBool(_objectName + Table_Num.ToString());


        Current_Price = Upgrade_Price;
        PriceText.text = $"{Current_Price:0}";
        CheckActive();

        StartCoroutine(Cor_Update());
    }


    IEnumerator Cor_Update()
    {

        while (true)
        {
            yield return null;
            if (isActive == false)
            {
                if (isPlayerIn)
                {
                    if (Managers.Game.Money >= Upgrade_Price * 0.5f * Time.deltaTime)
                    {
                        //Managers.Game.Money -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                        Managers.Game.UpdateMoney(-Upgrade_Price * 0.5f * Time.deltaTime);
                        Current_Price -= Upgrade_Price * 0.5f * Time.deltaTime;
                        if (Current_Price <= 0)
                        {
                            Current_Price = 0;
                            isPlayerIn = false;
                            isActive = true;
                            CheckActive();
                            Managers.Data.SetBool(_objectName + Table_Num.ToString(),true);
                            // add save data
                        }
                        PriceText.text = $"{Current_Price:0}";

                    }
                }
            }
            else
            {

                break;
            }


        }
    }


    void CheckActive()
    {
        _renderer.enabled = isActive;
        _interactArea.gameObject.SetActive(!isActive);
        GetComponent<SphereCollider>().isTrigger = !isActive;
    }



    //IEnumerator Cor_Update()
    //{
    //    WaitForSeconds _wait = new WaitForSeconds(Update_Interval);
    //    while (true)
    //    {
    //        yield return _wait;

    //        if (Order_Count > 0 && BatteryStack.Count > 0)
    //        {
    //            Order_Count--;
    //            Managers.Pool.Push(BatteryStack.Pop().GetComponent<Poolable>());
    //        }

    //    }
    //}


    //public void PushBattery(Product _product, float _interval)
    //{
    //    _product.transform.SetParent(transform);
    //    _product.transform.DOJump(StackPoint.position + Vector3.up * (BaseUp_Interval + BatteryStack.Count * Stack_Interval), 1, 1, _interval).SetEase(Ease.Linear)
    //                                     .Join(_product.transform.DORotate(Vector3.up * 45f, _interval).SetEase(Ease.Linear))
    //                                     .OnComplete(() => BatteryStack.Push(_product));
    //}

    [Button]
    public void SpawnMoney(int _popCount = 3)
    {
        for (int i = 0; i < _popCount; i++)
        {
            Transform _momey = Managers.Pool.Pop(Money_Pref).transform;
            MoneyStack.Push(_momey);
            int _count = MoneyStack.Count;
            _momey.transform.SetParent(MoneyStackPoint);
            _momey.eulerAngles = new Vector3(0f, 45f, 0f);
            _momey.transform.localPosition = new Vector3(
                -Left + horizontal_interval * ((_count - 1) % width)
                , height_interval * (_count / (width * height))
                , -back + vertical_inteval * (int)(((_count - 1) % (width * height)) / width));
        }
        //if (BatteryStack.Count > 0)
        //{
        //    Managers.Pool.Push(BatteryStack.Pop().GetComponent<Poolable>());

        //}
    }

    public Transform PopMoney()
    {
        if (MoneyStack.Count > 0)
        {
            return MoneyStack.Pop();
        }
        else { return null; }
    }


}
