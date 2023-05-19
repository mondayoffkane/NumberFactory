using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChargingMachine : MonoBehaviour
{

    public Stack<Product> BatteryStack = new Stack<Product>();


    public Transform StackPoint;
    [SerializeField] int _offsetCount;

    public double BasePrice = 20d;
    public Stack<Transform> MoneyStack = new Stack<Transform>();
    public GameObject Money_Pref;
    public Transform MoneyStackPoint;

    //========== Values====================
    public float Stack_Interval = 0.2f;
    public float Update_Interval = 1f;
    public float StartX = -0.375f, OffsetX = 0.2f;
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

        //StartCoroutine(Cor_Update());
    }

    //IEnumerator Cor_Update()
    //{
    //    WaitForSeconds _interval = new WaitForSeconds(Update_Interval);
    //    while (true)
    //    {
    //        yield return _interval;

    //        // add func
    //    }
    //}

    public void PushBattery(Product _product, float _interval = 0.5f)
    {
        BatteryStack.Push(_product);
        _offsetCount = BatteryStack.Count - 1;
        _product.transform.SetParent(StackPoint);
        DOTween.Kill(_product);
        _product.transform.DOLocalJump(
                new Vector3(0f, _offsetCount / 4 * Stack_Interval
                                     , (StartX + OffsetX * (_offsetCount % 4)))
            , 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DORotate(Vector3.up * 45f, _interval).SetEase(Ease.Linear));
        //.OnComplete(() => BatteryStack.Push(_product));
    }



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
