using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ChargingMachine : MonoBehaviour
{

    public Stack<Product> BatteryStack = new Stack<Product>();


    public Transform StackPoint;
    [SerializeField] int _offsetCount;

    public double BasePrice = 20d;
    public Stack<Transform> MoneyStack = new Stack<Transform>();
    public GameObject Money_Pref;
    public Transform MoneyStackPoint;

    public bool isPlayerIn = false;
    public InteractArea _interactArea;
    //public Text PriceText;
    //public int Upgrade_Level = 0;
    public bool isActive = false;
    public double UpgradePrice = 100d;
    public double CurrentPrice = 100d;
    Transform _player;



    //========== Values====================
    public float Stack_Interval = 0.2f;
    public float Update_Interval = 1f;
    public float StartX = -0.375f, OffsetX = 0.2f;
    public float Side_Interval = 0.5f;
    // ======= money 
    public int width = 5, height = 3;
    public float Left = 0.5f;
    public float back = 0.2f;
    public float horizontal_interval = 0.3f;
    public float vertical_inteval = 0.6f;
    public float height_interval = 0.1f;

    // =============================
    // ====================================
    private void Start()
    {
        if (StackPoint == null) StackPoint = transform.GetChild(0);

        isActive = Managers.Data.GetBool("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_charginemachine");

        if (_interactArea == null)
            _interactArea = GetComponentInChildren<InteractArea>();
        _interactArea.SetTarget(this, InteractArea.TargetType.ChargingMachine);
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        Setting();
        StartCoroutine(Cor_Update());
    }

    IEnumerator Cor_Update()
    {

        while (true)
        {
            yield return null;
            
            if (isPlayerIn)
            {
            
                if (Managers.Game.Money >= UpgradePrice * 1f * Time.deltaTime)
                {
                    //Managers.Game.Money -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                    Managers.Game.UpdateMoney(-UpgradePrice * 1f * Time.deltaTime);
                    CurrentPrice -= UpgradePrice * 1f * Time.deltaTime;
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
                        isActive = true;
                        Upgrade();

                        break;
                    }
                    //PriceText.text = $"{CurrentPrice:0}";
                    _interactArea.UpdatePrice(CurrentPrice);

                }
            }


            // add func
        }
    }

    public void Upgrade()
    {
        Managers.Sound.Play("NewObj");
        Setting();

        Managers.Data.SetBool("Stage_" + Managers.Game._stagemanager.Stage_Num.ToString() + "_charginemachine", true);
    }
    public void Setting()
    {

        if (isActive)
        {
            _interactArea.gameObject.SetActive(false);
            GetComponent<Renderer>().enabled = true;
            Collider[] _cols = GetComponents<Collider>();
            foreach (Collider _col in _cols)
            {
                _col.enabled = true;
            }
            StackPoint.gameObject.SetActive(true);
        }
        else
        {
            _interactArea.gameObject.SetActive(true);
            GetComponent<Renderer>().enabled = false;
            Collider[] _cols = GetComponents<Collider>();
            foreach (Collider _col in _cols)
            {
                _col.enabled = false;
            }
            StackPoint.gameObject.SetActive(false);
        }
    }

    public void PushBattery(Product _product, float _interval = 0.5f)
    {
        Stack_Interval = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        BatteryStack.Push(_product);
        _offsetCount = BatteryStack.Count - 1;
        _product.transform.SetParent(StackPoint);
        DOTween.Kill(_product);


        Side_Interval = _product.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
        _product.transform.DOLocalJump(
                new Vector3(0f, _offsetCount / 4 * Stack_Interval
                                     , (StartX + Side_Interval * (_offsetCount % 4)))
            , 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DORotate(Vector3.up * 45f, _interval).SetEase(Ease.Linear));

    }



    [Button]
    public void SpawnMoney(int _popCount = 3)
    {
        BatteryStack.Pop().gameObject.SetActive(false);
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
