using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;


public class ChargingTable : MonoBehaviour
{

    public Stack<Product> BatteryStack = new Stack<Product>();
    Transform StackPoint;
    [SerializeField] int Order_Count = 0;


    // ===== Values
    public float Stack_Interval = 0.2f;
    public float Update_Interval = 1f;
    public float BaseUp_Interval = 0.5f;
    // =============================
    private void Start()
    {
        if (StackPoint == null) StackPoint = transform.GetChild(0);

        StartCoroutine(Cor_Update());
    }


    IEnumerator Cor_Update()
    {
        WaitForSeconds _wait = new WaitForSeconds(Update_Interval);
        while (true)
        {
            yield return _wait;

            if (Order_Count > 0 && BatteryStack.Count > 0)
            {
                Order_Count--;
                Managers.Pool.Push(BatteryStack.Pop().GetComponent<Poolable>());
            }

        }
    }


    public void PushBattery(Product _product, float _interval)
    {
        _product.transform.SetParent(transform);
        _product.transform.DOJump(StackPoint.position + Vector3.up * (BaseUp_Interval + BatteryStack.Count * Stack_Interval), 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DORotate(Vector3.up * 45f, _interval).SetEase(Ease.Linear))
                                         .OnComplete(() => BatteryStack.Push(_product));
    }



}
