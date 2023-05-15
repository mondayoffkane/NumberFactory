using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;



public class Machine : MonoBehaviour
{
    [Title("Modeling,Transform")]
    public Mesh[] Machine_Meshs;
    public Mesh[] Product_Meshes;

    public int Machine_Num;
    //public int Food_Num;

    public Product.ProductType _productType;

    [SerializeField] MeshFilter Child_Machine;
    [SerializeField] MeshFilter Sample_Product;

    [SerializeField] MachineTable MachineTable;
    [SerializeField] Transform StartBelt, EndBelt;

    /// ////////////////////////////////////
    [Title("Values")]
    public float SpawnInterval = 2f;
    public int Max_Count = 20;
    public int Current_Count = 0;
    public Vector3 RailonProduct_interval = Vector3.up * 0.5f;
    public float Food_Stack_Interval = 0.5f;


    //public Stack<Transform> Machine_ProductStack;

    public void SetObj()
    {
        Child_Machine = transform.Find("Machine").GetComponent<MeshFilter>();
        Sample_Product = transform.Find("SampleProduct").GetComponent<MeshFilter>();
        MachineTable = transform.Find("MachineTable").GetComponent<MachineTable>();
        StartBelt = transform.Find("StartBelt");
        EndBelt = transform.Find("EndBelt");
    }

    [Button]
    public void SetMachine()
    {
        SetObj();
        Child_Machine.sharedMesh = Machine_Meshs[Machine_Num];
        Sample_Product.sharedMesh = Product_Meshes[(int)_productType];

    }

    private void Start()
    {
        SetObj();
        StartCoroutine(Cor_Update());

        //Machine_ProductStack = new Stack<Transform>();
    }


    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);


            Transform _product = Managers.Pool.Pop(Resources.Load<GameObject>("Product"), MachineTable.transform).transform;
            _product.GetComponent<Product>().Setproduct(Sample_Product.sharedMesh, _productType);
            //Machine_ProductStack.Push(_food);
            //int tempNum = Current_Count;
            //Current_Count++;

            DOTween.Sequence(_product.position = StartBelt.position + RailonProduct_interval)
                .Append(_product.DOMove(EndBelt.position + RailonProduct_interval, SpawnInterval * 2f))
                .OnComplete(() =>
                {
                    if (MachineTable.ProductStack.Count < MachineTable.Max_Count)
                    {
                        if (MachineTable.ProductStack.Count % 2 == 0)
                        {
                            _product.DOJump(MachineTable.transform.position + new Vector3(-0.25f * Mathf.Sin(45), 0.15f + (MachineTable.ProductStack.Count / 2) * Food_Stack_Interval, -0.25f * Mathf.Sin(45)) // positon
                                , 0.5f, 1, 0.5f) // power,jump count,duration
                            .OnComplete(() => MachineTable.ProductStack.Push(_product));
                        }
                        else
                        {
                            _product.DOJump(MachineTable.transform.position + new Vector3(0.25f * Mathf.Sin(45), 0.15f + (MachineTable.ProductStack.Count / 2) * Food_Stack_Interval, +0.25f * Mathf.Sin(45)) // positon
                                , 0.5f, 1, 0.5f) // power,jump count,duration
                            .OnComplete(() => MachineTable.ProductStack.Push(_product));
                        }
                    }
                    else
                    {
                        Managers.Pool.Push(_product.GetComponent<Poolable>());
                    }
                });


        }
    }

    [Button]
    public void UpgradeMachine()
    {
        SpawnInterval *= 0.5f;
        MachineTable.SetChange(MachineTable.Max_Count + 6);

    }

}
