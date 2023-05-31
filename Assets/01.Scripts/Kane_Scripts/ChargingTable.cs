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

    Transform _player;



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
    //public Text PriceText;
    public bool isPlayerIn = false;
    public MeshFilter _chargeObj;
    public Mesh[] _chargeobjMeshes;

    Renderer _renderer;
    // ==================
    private void Start()
    {
        if (StackPoint == null) StackPoint = transform.GetChild(0);

        _interactArea = GetComponentInChildren<InteractArea>();
        _interactArea.SetTarget(this, InteractArea.TargetType.Table);
        //PriceText = _interactArea.GetComponentInChildren<Text>();
        _renderer = GetComponent<Renderer>();
        _renderer.enabled = false;
        GetComponent<SphereCollider>().isTrigger = true;
        _player = GameObject.FindGameObjectWithTag("Player").transform;  //
        isActive = Managers.Data.GetBool(_objectName + Table_Num.ToString());


        Current_Price = Upgrade_Price;
        _interactArea.SetPrice(Current_Price);
        //PriceText.text = $"{Current_Price:0}";
        CheckActive(true);

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
                    if (Managers.Game.Money >= Upgrade_Price * 1f * Time.deltaTime)
                    {
                        //Managers.Game.Money -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                        Managers.Game.UpdateMoney(-Upgrade_Price * 1f * Time.deltaTime);
                        Current_Price -= Upgrade_Price * 1f * Time.deltaTime;
                        Transform _momey = Managers.Pool.Pop(Money_Pref).transform;

                        _momey.SetParent(_player);
                        _momey.transform.localPosition = Vector3.zero;
                        _momey.DOJump(transform.position, 2f, 1, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            _momey.gameObject.SetActive(false);
                            Managers.Pool.Push(_momey.GetComponent<Poolable>());
                        });
                        if (Current_Price <= 0)
                        {
                            Current_Price = 0;
                            isPlayerIn = false;
                            isActive = true;
                            CheckActive();
                            Managers.Data.SetBool(_objectName + Table_Num.ToString(), true);
                            // add save data
                        }
                        //PriceText.text = $"{Current_Price:0}";
                        _interactArea.UpdatePrice(Current_Price);

                    }
                }
            }
            else
            {

                break;
            }


        }
    }


    void CheckActive(bool isInit = false)
    {
        _renderer.enabled = isActive;
        _interactArea.gameObject.SetActive(!isActive);
        GetComponent<SphereCollider>().isTrigger = !isActive;

        if (isActive)
        {
            if (isInit == false)
            {
                Managers.Sound.Play("NewObj");
                GameObject _obj = Managers.Pool.Pop(Resources.Load<GameObject>("NewEffect"), transform).gameObject;
                _obj.transform.localPosition = Vector3.zero;
                _obj.GetComponent<ParticleSystem>().PlayAllParticle();
                DOTween.Sequence().AppendInterval(1f).OnComplete(() => { Managers.Pool.Push(_obj.GetComponent<Poolable>()); });
            }
        }
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


    public void ChargeobjectOnOff(bool isOn)
    {

        if (isOn)
        {
            //_chargeObj.gameObject.SetActive(true);
            _chargeObj.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutCubic);
            _chargeObj.sharedMesh = _chargeobjMeshes[Random.Range(0, _chargeobjMeshes.Length)];
        }
        else
        {
            _chargeObj.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutCubic);
            //_chargeObj.gameObject.SetActive(false);
        }


    }

}
