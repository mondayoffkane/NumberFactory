using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public int Max_Count = 10;


    public Stack<Product> ProductStack = new Stack<Product>();
    public float Stack_Interval = 1f;

    [SerializeField] Transform PickPos;

    public float Pickup_Interval = 0.5f;

    [SerializeField] bool isReady = true;


    MachineTable _machineTable;

    public Material Rail_Mat;
    public float RailMat_Speed = 1f;
    private void Start()
    {
        PickPos = transform.GetChild(0);

        
        
        Rail_Mat.SetTextureOffset("_BaseMap", Vector2.zero);
        Rail_Mat.DOOffset(Vector2.down, RailMat_Speed).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MachineTable"))
        {
            _machineTable = other.GetComponent<MachineTable>();
        }


    }

    private void OnTriggerStay(Collider other)
    {

        switch (other.tag)
        {
            case "MachineTable":
                if (_machineTable.ProductStack.Count > 0 && ProductStack.Count < Max_Count && isReady)
                {
                    if (ProductStack.Count == 0 || ProductStack.Peek().GetComponent<Product>()._productType
                        == other.GetComponent<MachineTable>().ProductStack.Peek().GetComponent<Product>()._productType)
                    {
                        Product _product = other.GetComponent<MachineTable>().ProductStack.Pop();

                        ProductStack.Push(_product);
                        _product.transform.SetParent(transform);

                        isReady = false;

                        _product.transform.DOLocalJump(PickPos.localPosition + Vector3.up * ProductStack.Count * 0.2f, 1f, 1, Pickup_Interval)
                            //.Join(_product.transform.DOLocalRotate(new Vector3(0f, 90f, 0f), Pickup_Interval).SetEase(Ease.Linear))
                        .OnComplete(() =>
                        {
                            isReady = true;

                            switch (_product._productType)
                            {
                                case Product.ProductType.Number:
                                    _product.transform.localEulerAngles = new Vector3(-180f, 90f, -180f);
                                    break;

                                case Product.ProductType.Battery:
                            _product.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                                    break;
                            }

                        });

                    }

                }
                break;

            case "Generator":
                if (ProductStack.Count > 0 && isReady && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Number)
                {
                    Product _product = ProductStack.Pop().GetComponent<Product>();
                    isReady = false;
                    _product.transform.DOLocalJump(other.transform.position, 1f, 1, Pickup_Interval)
                        .OnComplete(() =>
                        {
                            isReady = true;
                            other.GetComponent<Generator>().AddNum(_product.Number);
                            Managers.Pool.Push(_product.GetComponent<Poolable>());
                        });

                }

                break;

            case "Machine":
                if (ProductStack.Count > 0 && isReady)
                {
                    isReady = false;
                    Product _productObj = ProductStack.Pop();
                    _productObj.transform.DOJump(other.transform.position + Vector3.left * 2f, 1f, 1, Pickup_Interval)
                                    .OnComplete(() =>
                                    {
                                        _productObj.transform.SetParent(other.transform);
                                        other.GetComponentInParent<Machine>().resource_Stack.Push(_productObj);
                                        isReady = true;
                                    });
                }
                break;

            case "ChargingTable":
                if (isReady && ProductStack.Count > 0 && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Battery)
                {
                    other.GetComponent<ChargingTable>().PushBattery(ProductStack.Pop(), Pickup_Interval);
                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);
                }

                break;

            case "ChargingMachine":
                if (isReady && ProductStack.Count > 0 && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Battery)
                {
                    other.GetComponent<ChargingMachine>().PushBattery(ProductStack.Pop(), Pickup_Interval);
                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);
                }
                break;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MachineTable"))
        {
            _machineTable = null;
        }
    }






}
