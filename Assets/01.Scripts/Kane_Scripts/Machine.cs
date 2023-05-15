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

<<<<<<< HEAD

    public bool isProduce = false;
    public Machine NextNode;
    [ShowInInspector]
    public Stack<Product> resource_Stack = new Stack<Product>();
    //Product _product;
    //public bool Test_isLay = false;
    /// ////////////////////////////////////
    [Title("Values")]
    public Vector3 SampleRot = new Vector3(0f, 90f, -90f);
    public float SpawnInterval = 2f;
    public float Rail_Speed = 4f;
    public int Max_Count = 20;
    public int Current_Count = 0;
    public Vector3 RailonProduct_interval = Vector3.up * 0.5f;
    public float Product_Stack_Interval = 0.06f;
    public float BaseUpStack_Inteval = 1f;
    public int Base_Value = 1;




    public enum MachineType
    {
        Plus,
        Multiple
    }
    public MachineType _machineType;


=======
    /// ////////////////////////////////////
    [Title("Values")]
    public float SpawnInterval = 2f;
    public int Max_Count = 20;
    public int Current_Count = 0;
    public Vector3 RailonProduct_interval = Vector3.up * 0.5f;
    public float Food_Stack_Interval = 0.5f;


    //public Stack<Transform> Machine_ProductStack;
>>>>>>> c4d44bd848e11b411eb88f4cca107d57fada4928

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

<<<<<<< HEAD
        switch (_machineType)
        {
            case MachineType.Plus:

                break;

            case MachineType.Multiple:

                break;

            default:

                break;
        }
        Sample_Product.transform.rotation = Quaternion.Euler(SampleRot);

=======
>>>>>>> c4d44bd848e11b411eb88f4cca107d57fada4928
    }

    private void Start()
    {
        SetObj();
<<<<<<< HEAD


        StartCoroutine(Cor_Update());
        Sample_Product.transform.rotation = Quaternion.Euler(SampleRot);


=======
        StartCoroutine(Cor_Update());

        //Machine_ProductStack = new Stack<Transform>();
>>>>>>> c4d44bd848e11b411eb88f4cca107d57fada4928
    }


    IEnumerator Cor_Update()
    {
<<<<<<< HEAD

        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);
            Product _product;
            if (isProduce)
            {
                _product = Managers.Pool.Pop(Resources.Load<GameObject>("Product")).transform.GetComponent<Product>();
                _product.GetComponent<Product>().Setproduct(Sample_Product.sharedMesh, _productType, Base_Value);
            }
            else if (resource_Stack.Count > 0)
            {
                _product = resource_Stack.Pop();

                switch (_machineType)
                {

                    case MachineType.Plus:
                        _product.SetNum(_product.Number += Base_Value);

                        break;

                    case MachineType.Multiple:
                        _product.SetNum(_product.Number *= Base_Value);

                        break;

                    default:

                        break;
                }

            }
            else _product = null;

            if (_product != null)
                MoveNextNode(_product);
=======
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


>>>>>>> c4d44bd848e11b411eb88f4cca107d57fada4928
        }
    }

    [Button]
    public void UpgradeMachine()
    {
        SpawnInterval *= 0.5f;
<<<<<<< HEAD
        Max_Count += 20;
        //MachineTable.SetChange(MachineTable.Max_Count + 6);

    }


    public void MoveNextNode(Product _product)
    {
        Product _productObj = _product;
        DOTween.Sequence(_productObj.transform.position = StartBelt.position + RailonProduct_interval)
            .AppendCallback(() => _productObj.transform.eulerAngles = SampleRot)
            .Append(_productObj.transform.DOMove(EndBelt.position + RailonProduct_interval, Rail_Speed))
            .OnComplete(() =>
            {
                if (NextNode == null)
                {
                    if (MachineTable.ProductStack.Count < Max_Count)
                    {
                        if (MachineTable.ProductStack.Count % 2 == 0)
                        {
                            //if (Test_isLay)
                            //{
                            _productObj.transform.DOJump(MachineTable.transform.position + new Vector3(-0.3f * Mathf.Sin(45), BaseUpStack_Inteval + (MachineTable.ProductStack.Count / 2) * Product_Stack_Interval, -0.3f * Mathf.Sin(45)) // positon
                            , 0.5f, 1, 0.5f) // power,jump count,duration                            
                            .Join(_product.transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f).SetEase(Ease.Linear))
                        .OnComplete(() => MachineTable.ProductStack.Push(_productObj));
                            //}
                            //else
                            //{
                            //    _productObj.transform.DOJump(MachineTable.transform.position + new Vector3(-0.25f * Mathf.Sin(45), 0.15f + (MachineTable.ProductStack.Count / 2) * Food_Stack_Interval, -0.25f * Mathf.Sin(45)) // positon
                            //        , 0.5f, 1, 0.5f) // power,jump count,duration                            
                            //    .OnComplete(() => MachineTable.ProductStack.Push(_productObj));

                            //}
                        }
                        else
                        {
                            //if (Test_isLay)
                            //{
                            _productObj.transform.DOJump(MachineTable.transform.position + new Vector3(0.3f * Mathf.Sin(45), BaseUpStack_Inteval + (MachineTable.ProductStack.Count / 2) * Product_Stack_Interval, +0.3f * Mathf.Sin(45)) // positon
                          , 0.5f, 1, 0.5f) // power,jump count,duration                            
                            .Join(_product.transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f).SetEase(Ease.Linear))
                      .OnComplete(() => MachineTable.ProductStack.Push(_productObj));
                            //}
                            //else
                            //{
                            //    _productObj.transform.DOJump(MachineTable.transform.position + new Vector3(0.25f * Mathf.Sin(45), 0.15f + (MachineTable.ProductStack.Count / 2) * Food_Stack_Interval, +0.25f * Mathf.Sin(45)) // positon
                            //    , 0.5f, 1, 0.5f) // power,jump count,duration
                            //.OnComplete(() => MachineTable.ProductStack.Push(_productObj));

                            //}
                        }
                    }
                    else
                    {
                        Managers.Pool.Push(_productObj.GetComponent<Poolable>());
                    }

                }
                else // NextNode == true
                {
                    //NextNode.MoveNextNode(_product);
                    NextNode.Input_Resource(_productObj);
                }
            });

        //_productObj = null;
    }

    public void Input_Resource(Product _resource)
    {
        resource_Stack.Push(_resource);
        _resource.transform.position = StartBelt.position + RailonProduct_interval;
        _resource.transform.SetParent(transform);
    }

    public void SetNextNode(Machine _nextNode)
    {
        NextNode = _nextNode;

        int count = MachineTable.ProductStack.Count;

        for (int i = 0; i < count; i++)
        {
            NextNode.resource_Stack.Push(MachineTable.ProductStack.Pop());
        }
    }

=======
        MachineTable.SetChange(MachineTable.Max_Count + 6);

    }

>>>>>>> c4d44bd848e11b411eb88f4cca107d57fada4928
}
