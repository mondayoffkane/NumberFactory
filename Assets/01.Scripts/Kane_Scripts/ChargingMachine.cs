using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChargingMachine : MonoBehaviour
{

    public Stack<Product> BatteryStack = new Stack<Product>();


    public Transform StackPoint;
    [SerializeField] int _offsetCount;
    //========== Values====================
    public float Stack_Interval = 0.2f;
    public float Update_Interval = 1f;
    public float StartX = -0.375f, OffsetX = 0.2f;

    // ====================================
    private void Start()
    {
        if (StackPoint == null) StackPoint = transform.GetChild(0);

        StartCoroutine(Cor_Update());
    }

    IEnumerator Cor_Update()
    {
        WaitForSeconds _interval = new WaitForSeconds(Update_Interval);
        while (true)
        {
            yield return _interval;

            // add func
        }
    }

    public void PushBattery(Product _product, float _interval)
    {
        _offsetCount = BatteryStack.Count;
        _product.transform.SetParent(StackPoint);
        
        _product.transform.DOLocalJump(
                new Vector3(0f, _offsetCount / 4 * Stack_Interval
                                     , (StartX + OffsetX * (_offsetCount % 4)))
            , 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DORotate(Vector3.up * 45f, _interval).SetEase(Ease.Linear))
                                         .OnComplete(() => BatteryStack.Push(_product));
    }


}
