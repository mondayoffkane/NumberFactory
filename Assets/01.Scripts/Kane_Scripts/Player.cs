using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{

    public int Speed_Level = 0;
    public int Capacity_Level = 0;
    public int Income_Level = 0;

    public int Max_Count = 10;

    public double baseMoneyPrice = 3f;
    public double AddMoneyPrice = 3f;


    public Stack<Product> ProductStack = new Stack<Product>();
    public float Stack_Interval = 1f;

    [SerializeField] Transform PickPos;

    public float Move_Interval = 0.5f;
    public float Pickup_Interval = 0.1f;

    [SerializeField] bool isReady = true;


    public Material Rail_Mat;
    public float RailMat_Speed = 1f;

    Animator _animator;

    // ==========================
    public void InitPlayer(int _speedLevel, int _capacityLevel, int _incomLevel)
    {
        Speed_Level = _speedLevel;
        Capacity_Level = _capacityLevel;
        Income_Level = _incomLevel;

    }


    private void Start()
    {
        PickPos = transform.GetChild(0);

        if (_animator == null) _animator = GetComponent<Animator>();

        Rail_Mat.SetTextureOffset("_BaseMap", Vector2.zero);
        Rail_Mat.DOOffset(Vector2.down, RailMat_Speed).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }



    private void OnTriggerStay(Collider other)
    {

        switch (other.tag)
        {
            case "MachineTable":
                _animator.SetBool("Pick", true);
                if (other.GetComponent<MachineTable>().ProductStack.Count > 0 && ProductStack.Count < Max_Count && isReady)
                {
                    if (ProductStack.Count == 0 || ProductStack.Peek().GetComponent<Product>()._productType
                        == other.GetComponent<MachineTable>().ProductStack.Peek().GetComponent<Product>()._productType)
                    {
                        Product _product = other.GetComponent<MachineTable>().ProductStack.Pop();
                        DOTween.Kill(_product);
                        ProductStack.Push(_product);
                        _product.transform.SetParent(transform);

                        Stack_Interval = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;

                        isReady = false;

                        _product.transform.DOLocalJump(PickPos.localPosition + Vector3.up * ProductStack.Count * Stack_Interval, 1f, 1, Move_Interval)
                        .OnComplete(() =>
                        {
                            //isReady = true;
                            switch (_product._productType)
                            {
                                case Product.ProductType.Number:
                                    _product.transform.localEulerAngles = new Vector3(-180f, 90f, -180f);
                                    break;

                                case Product.ProductType.Battery:
                                    //_product.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                                    _product.transform.localEulerAngles = Vector3.zero;
                                    break;
                            }
                        });
                        DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);

                    }
                }
                break;

            case "Generator":
                if (ProductStack.Count > 0 && isReady && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Number)
                {
                    Product _product = ProductStack.Pop().GetComponent<Product>();
                    isReady = false;
                    other.GetComponent<Generator>().PushNum(_product);
                    //DOTween.Kill(_product);
                    //_product.transform.DOLocalJump(other.transform.position, 1f, 1, Move_Interval)
                    //    .OnComplete(() =>
                    //    {
                    //        //isReady = true;
                    //        other.GetComponent<Generator>().AddNum(_product.Number);
                    //        Managers.Pool.Push(_product.GetComponent<Poolable>());
                    //    });
                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);
                }
                else if (ProductStack.Count <= 0) _animator.SetBool("Pick", false);

                break;



            case "Counter":
                if (isReady && ProductStack.Count > 0 && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Battery)
                {
                    other.GetComponent<Counter>().PushBattery(ProductStack.Pop(), Move_Interval);
                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);
                }
                else if (ProductStack.Count <= 0) _animator.SetBool("Pick", false);

                break;


            case "ChargingMachine":
                if (isReady && ProductStack.Count > 0 && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Battery)
                {
                    other.GetComponent<ChargingMachine>().PushBattery(ProductStack.Pop(), Move_Interval);
                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);
                }
                else if (ProductStack.Count <= 0) _animator.SetBool("Pick", false);

                ChargingMachine _chargingmachine = other.GetComponent<ChargingMachine>();
                int _count = _chargingmachine.MoneyStack.Count / 10;
                _count = _count < 1 ? 1 : _count;
                _count = _count > 10 ? 10 : _count;
                for (int i = 0; i < _count; i++)
                {

                    Transform _money = _chargingmachine.PopMoney();
                    if (_money != null)
                    {
                        _money.SetParent(transform);
                        DOTween.Kill(_money);
                        _money.DOLocalJump(Vector3.zero, 2, 1, Move_Interval * 0.5f).SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                Managers.Game.Money += baseMoneyPrice + Income_Level * AddMoneyPrice;
                                Managers.Pool.Push(_money.GetComponent<Poolable>());
                                _money.gameObject.SetActive(false);
                            });
                    }
                }


                break;
            case "ChargingTable":

                ChargingTable _chargingtable = other.GetComponent<ChargingTable>();
                int _count2 = _chargingtable.MoneyStack.Count / 10;
                _count2 = _count2 < 1 ? 1 : _count2;
                _count2 = _count2 > 10 ? 10 : _count2;
                for (int i = 0; i < _count2; i++)
                {

                    Transform _money = _chargingtable.PopMoney();
                    if (_money != null)
                    {
                        _money.SetParent(transform);
                        DOTween.Kill(_money);
                        _money.DOLocalJump(Vector3.zero, 2, 1, Move_Interval * 0.5f).SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                Managers.Game.Money += baseMoneyPrice + Income_Level * AddMoneyPrice;
                                Managers.Pool.Push(_money.GetComponent<Poolable>());
                                _money.gameObject.SetActive(false);
                            });
                    }
                }

                break;



        }

    }



}
