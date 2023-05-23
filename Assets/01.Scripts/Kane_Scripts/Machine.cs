using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;



public class Machine : MonoBehaviour
{
    [Title("Modeling,Transform")]
    public Mesh[] Machine_Meshs;
    public Mesh[] Product_Meshes;

    public int Machine_Num = 0;
    public int Upgrade_Level = 0;
    //public int Food_Num;

    public Product.ProductType _productType;

    [SerializeField] MeshFilter Child_Machine;
    [SerializeField] MeshFilter Sample_Product;

    [SerializeField] MachineTable MachineTable;
    [SerializeField] Transform StartBelt, EndBelt;


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

    public Transform InteractArea;
    public Text PriceText;

    public string _objectName = "Machine";

    public enum MachineType
    {
        Plus,
        Multiple
    }
    public MachineType _machineType;

    public double[] UpgradePrice = new double[4] { 100d, 200d, 500d, 1000d };
    public double CurrentPrice = 100d;

    public bool isPlayerIn = false;

    // =======================================

    public void SetObj()
    {
        Child_Machine = transform.Find("Machine").GetComponent<MeshFilter>();
        Sample_Product = Child_Machine.transform.Find("SampleProduct").GetComponent<MeshFilter>();
        MachineTable = Child_Machine.transform.Find("MachineTable").GetComponent<MachineTable>();
        StartBelt = Child_Machine.transform.Find("StartBelt");
        EndBelt = Child_Machine.transform.Find("EndBelt");
        InteractArea = transform.Find("InteractArea");
        PriceText = InteractArea.GetComponentInChildren<Text>();
    }

    [Button]
    public void SetMachine()
    {
        SetObj();
        Child_Machine.sharedMesh = Machine_Meshs[Machine_Num];
        Sample_Product.sharedMesh = Product_Meshes[(int)_productType];

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

    }

    private void Start()
    {
        SetObj();

        Sample_Product.transform.rotation = Quaternion.Euler(SampleRot);
        Child_Machine.gameObject.SetActive(false);

        Upgrade_Level = Managers.Data.GetInt(_objectName + Machine_Num.ToString());
        if (Upgrade_Level >= 3) InteractArea.gameObject.SetActive(false);
        StartCoroutine(Cor_Update());
        StartCoroutine(Cor_Spawn());

        CurrentPrice = UpgradePrice[Upgrade_Level];
        PriceText.text = $"{CurrentPrice:0}";

        if (Upgrade_Level != 0)
        {
            ActiveMachine();
        }


    }

    public void CheckData()
    {
        Managers.Data.SetInt(_objectName + Machine_Num.ToString(), Upgrade_Level); // Set Upgrade Num

    }

    [Button]
    public void ActiveMachine()
    {
        Child_Machine.gameObject.SetActive(true);

    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return null;

            if (isPlayerIn)
            {
                if (Managers.Game.Money >= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime && Upgrade_Level < 3)
                {
                    Managers.Game.Money -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                    CurrentPrice -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                    if (CurrentPrice <= 0)
                    {
                        CurrentPrice = 0;
                        isPlayerIn = false;
                        UpgradeMachine();
                    }
                    PriceText.text = $"{CurrentPrice:0}";

                }
            }


        }
    }


    IEnumerator Cor_Spawn()
    {

        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);
            if (Upgrade_Level > 0)
            {
                Product _product;
                if (isProduce)
                {
                    _product = Managers.Pool.Pop(Resources.Load<GameObject>("Product"), transform).transform.GetComponent<Product>();
                    _product.transform.localScale = Vector3.one;
                    _product.transform.eulerAngles = new Vector3(0f, 90f, 0f);
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
            }
        }
    }

    [Button]
    public void UpgradeMachine()
    {
        Upgrade_Level++;
        if (Upgrade_Level >= 3)
        {
            Upgrade_Level = 3;
            InteractArea.gameObject.SetActive(false);
        }
        else
        {
            CurrentPrice = UpgradePrice[Upgrade_Level];
            PriceText.text = $"{CurrentPrice:0}";

        }
        SpawnInterval = 1f - 0.1f * Upgrade_Level;
        Max_Count = 20 + 10 * Upgrade_Level;



        if (NextNode != null)
        {
            NextNode.UpgradeMachine();
        }

        Managers.Data.SetInt(_objectName + Machine_Num.ToString(), Upgrade_Level);
        if (Upgrade_Level < 2)
        {
            ActiveMachine();
        }
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
                        MachineTable.ProductStack.Push(_productObj);
                        int _count = MachineTable.ProductStack.Count - 1;
                        if (_count % 2 == 0)
                        {
                            _productObj.transform.DOJump(MachineTable.transform.position + new Vector3(-0.3f * Mathf.Sin(45), BaseUpStack_Inteval + (_count / 2) * Product_Stack_Interval, -0.3f * Mathf.Sin(45)) // positon
                            , 0.5f, 1, 0.5f) // power,jump count,duration                            
                            .Join(_product.transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f).SetEase(Ease.Linear));
                            //.OnComplete(() => MachineTable.ProductStack.Push(_productObj));

                        }
                        else
                        {

                            _productObj.transform.DOJump(MachineTable.transform.position + new Vector3(0.3f * Mathf.Sin(45), BaseUpStack_Inteval + (_count / 2) * Product_Stack_Interval, +0.3f * Mathf.Sin(45)) // positon
                          , 0.5f, 1, 0.5f) // power,jump count,duration                            
                            .Join(_product.transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f).SetEase(Ease.Linear));
                            //.OnComplete(() => MachineTable.ProductStack.Push(_productObj));

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

    [Button]
    public void InitData()
    {
        //Managers.Data.SetInt(_objectName + Machine_Num.ToString(), 0);

        ES3.Save<int>(_objectName + Machine_Num.ToString(), 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }

}
