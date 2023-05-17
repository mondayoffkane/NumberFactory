using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public bool isSell = true;
    public Transform StackPoint;

    // ==== values
    public float Stack_Interval = 0.2f;
    public float Update_Interval = 1f;
    public float BaseUp_Interval = 0.5f;


    public Stack<Product> BatteryStack = new Stack<Product>();
    // ============================

    public void PushBattery(Product _product, float _interval)
    {
        DOTween.Kill(_product);
        _product.transform.SetParent(transform);
        BatteryStack.Push(_product);
        _product.transform.DOJump(StackPoint.position + Vector3.up * (BaseUp_Interval + (BatteryStack.Count - 1) * Stack_Interval), 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DORotate(Vector3.up * 45f, _interval).SetEase(Ease.Linear));
        //.OnComplete(() => BatteryStack.Push(_product));
    }







}
