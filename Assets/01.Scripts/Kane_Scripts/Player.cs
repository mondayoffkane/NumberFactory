using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public int Max_Count = 10;


    public Stack<Transform> ProductStack = new Stack<Transform>();


    [SerializeField] Transform PickPos;

    public float Pickup_Interval = 0.5f;
    [SerializeField] bool canPick = true;

    // ////////////////////////////////////////////////////////////////////////////////

    MachineTable _machineTable;

    private void Start()
    {
        PickPos = transform.GetChild(0);
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
        if (other.CompareTag("MachineTable"))
        {
            if (_machineTable.ProductStack.Count > 0 && ProductStack.Count < Max_Count && canPick)
            {
                Transform _product = other.GetComponent<MachineTable>().ProductStack.Pop();
                ProductStack.Push(_product);
                _product.SetParent(transform);
                canPick = false;
                _product.DOLocalJump(PickPos.localPosition + Vector3.up * ProductStack.Count * 0.5f, 1f, 1, Pickup_Interval)
                    .OnComplete(() => canPick = true);

            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MachineTable"))
        {
            _machineTable = null;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Reposit"))
        {
            collision.transform.GetComponent<Repository>().PushProduct(ProductStack.Pop());
        }
    }


}
