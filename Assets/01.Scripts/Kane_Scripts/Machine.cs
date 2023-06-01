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
    public int MaxUpgrade_Level = 5;
    //public int Food_Num;

    public Product.ProductType _productType;

    [SerializeField] MeshFilter Child_Machine;
    [SerializeField] MeshFilter Sample_Product;

    [SerializeField] MachineTable MachineTable;
    [SerializeField] Transform StartBelt, EndBelt;
    Transform _player;
    public GameObject Money_Pref;

    [ShowInInspector]
    public Stack<Product> resource_Stack = new Stack<Product>();
    //Product _product;
    //public bool Test_isLay = false;
    /// ////////////////////////////////////
    [Title("Values")]
    public Vector3 SampleRot = new Vector3(0f, 90f, -90f);
    public float SpawnInterval = 2f;
    public float Rail_Speed = 4f;
    public int Max_Count = 10;
    public int Current_Count = 0;
    public Vector3 RailonProduct_interval = Vector3.up * 0.5f;
    public float Product_Stack_Interval = 0.06f;
    public float BaseUpStack_Inteval = 1f;
    public int base_Value = 0;
    public int calc_Value = 1;



    public InteractArea _interactArea;
    //public Text PriceText;

    public string _objectName = "Machine";

    public enum MachineType
    {
        Plus,
        Multiple
    }
    public MachineType _machineType;

    public double[] UpgradePrice = new double[10] { 100d, 200d, 500d, 1000d, 2000d, 4000d, 8000d, 16000d, 32000, 64000d };
    public double CurrentPrice = 100d;

    public bool isPlayerIn = false;

    //public bool isProduce = false;
    public Machine PrevNode;
    public Machine NextNode;
    public bool isActive = false;
    //bool isnextinit = false;
    public GameObject MaxText;
    //public bool isVertical = false;
    public Vector3 StackOffset = new Vector3(0.5f, 45f, 0.5f);
    // =======================================

    public void SetObj()
    {
        Child_Machine = transform.Find("Machine").GetComponent<MeshFilter>();
        Sample_Product = Child_Machine.transform.Find("SampleProduct").GetComponent<MeshFilter>();
        MachineTable = Child_Machine.transform.Find("MachineTable").GetComponent<MachineTable>();
        StartBelt = Child_Machine.transform.Find("StartBelt");
        EndBelt = Child_Machine.transform.Find("EndBelt");

        _interactArea = GetComponentInChildren<InteractArea>();
        _interactArea.SetTarget(this, InteractArea.TargetType.Machine);
        //PriceText = _interactArea.GetComponentInChildren<Text>();
    }

    [Button]
    public void SetMachine()
    {
        SetObj();
        Child_Machine.sharedMesh = Machine_Meshs[Machine_Num];
        SetMesh();

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

    public void SetStart()
    {
        SetObj();
        Sample_Product.transform.rotation = Quaternion.Euler(SampleRot);
        Child_Machine.gameObject.SetActive(false);

        Upgrade_Level = Managers.Data.GetInt(_objectName + Machine_Num.ToString());
        if (Upgrade_Level >= MaxUpgrade_Level) _interactArea.gameObject.SetActive(false);



        if (Upgrade_Level != 0)
        {
            ActiveMachine();
            isActive = true;
        }

        SetMesh();
        StartCoroutine(Cor_Update());
        StartCoroutine(Cor_Spawn());

        CurrentPrice = UpgradePrice[Upgrade_Level];
        _interactArea.SetPrice(CurrentPrice);
        //PriceText.text = $"{CurrentPrice:0}";
        _player = GameObject.FindGameObjectWithTag("Player").transform;  //

        if (PrevNode != null && PrevNode.isActive == false)
        {
            _interactArea.gameObject.SetActive(false);
        }
        MaxText.SetActive(false);

        if (Money_Pref == null) Money_Pref = Resources.Load<GameObject>("Money_Pref");
    }

    public void CheckData()
    {
        Managers.Data.SetInt(_objectName + Machine_Num.ToString(), Upgrade_Level); // Set Upgrade Num

    }

    [Button]
    public void ActiveMachine()
    {
        GameObject _obj = Managers.Pool.Pop(Resources.Load<GameObject>("NewEffect"), transform).gameObject;
        _obj.transform.localPosition = Vector3.zero;
        _obj.GetComponent<ParticleSystem>().PlayAllParticle();
        DOTween.Sequence().AppendInterval(1f).OnComplete(() => { Managers.Pool.Push(_obj.GetComponent<Poolable>()); });

        Child_Machine.gameObject.SetActive(true);
        if (PrevNode != null)
        {
            int count = PrevNode.MachineTable.ProductStack.Count;
            for (int i = 0; i < count; i++)
            {
                Poolable _trans = PrevNode.MachineTable.ProductStack.Pop().GetComponent<Poolable>();
                _trans.gameObject.SetActive(false);
                Managers.Pool.Push(_trans);

            }
            PrevNode.MachineTable.gameObject.SetActive(false);
            PrevNode.MachineTable.isActive = false;
            PrevNode.MaxText.SetActive(false);
        }

        if (NextNode == null)
        {
            MachineTable.isActive = true;
        }
    }



    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return null;

            if (isPlayerIn)
            {
                if (Managers.Game.Money >= UpgradePrice[Upgrade_Level] * 1f * Time.deltaTime && Upgrade_Level < MaxUpgrade_Level)
                {
                    //Managers.Game.Money -= UpgradePrice[Upgrade_Level] * 0.5f * Time.deltaTime;
                    Managers.Game.UpdateMoney(-UpgradePrice[Upgrade_Level] * 1f * Time.deltaTime);
                    CurrentPrice -= UpgradePrice[Upgrade_Level] * 1f * Time.deltaTime;
                    Transform _momey = Managers.Pool.Pop(Money_Pref).transform;

                    _momey.SetParent(_player);
                    _momey.transform.localPosition = Vector3.zero;
                    _momey.DOJump(transform.position, 2f, 1, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        _momey.gameObject.SetActive(false);
                        Managers.Pool.Push(_momey.GetComponent<Poolable>());
                    });
                    if (CurrentPrice <= 0)
                    {
                        CurrentPrice = 0;
                        isPlayerIn = false;
                        UpgradeMachine();
                    }
                    _interactArea.UpdatePrice(CurrentPrice);
                    //PriceText.text = $"{CurrentPrice:0}";

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

                if (isActive)
                {
                    if (PrevNode == null)
                    {
                        _product = Managers.Pool.Pop(Resources.Load<GameObject>("Product"), transform).transform.GetComponent<Product>();
                        _product.transform.localScale = Vector3.one;
                        _product.transform.eulerAngles = new Vector3(0f, 90f, 0f);
                        _product.GetComponent<Product>().Setproduct(Sample_Product.sharedMesh, _productType, Upgrade_Level);
                    }
                    else if (resource_Stack.Count > 0)
                    {
                        _product = resource_Stack.Pop();

                        switch (_machineType)
                        {

                            case MachineType.Plus:
                                _product.SetNum(_product.Number += calc_Value);
                                break;

                            case MachineType.Multiple:
                                _product.SetNum(_product.Number *= calc_Value);

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
    }

    [Button]
    public void UpgradeMachine()
    {

        if (NextNode != null && NextNode.Upgrade_Level < NextNode.MaxUpgrade_Level)
        {
            NextNode._interactArea.gameObject.SetActive(true);
            if (NextNode.isActive == false)
                MachineTable.isActive = true;
        }
        else if (NextNode == null)
        {
            MachineTable.isActive = true;
        }
        if (Upgrade_Level < 1)
        {
            Managers.Sound.Play("NewObj");
            GameObject _obj = Managers.Pool.Pop(Resources.Load<GameObject>("NewEffect"), transform).gameObject;
            _obj.transform.localPosition = Vector3.zero;
            _obj.GetComponent<ParticleSystem>().PlayAllParticle();
            DOTween.Sequence().AppendInterval(1f).OnComplete(() => { Managers.Pool.Push(_obj.GetComponent<Poolable>()); });
        }
        else
        {
            Managers.Sound.Play("UpgradeObj");
            GameObject _obj = Managers.Pool.Pop(Resources.Load<GameObject>("UpgradeEffect"), transform).gameObject;
            _obj.transform.localPosition = Vector3.zero;
            _obj.GetComponent<ParticleSystem>().PlayAllParticle();
            DOTween.Sequence().AppendInterval(1f).OnComplete(() => { Managers.Pool.Push(_obj.GetComponent<Poolable>()); });
        }
        ActiveMachine();
        isActive = true;

        Upgrade_Level++;
        if (Upgrade_Level >= MaxUpgrade_Level)
        {
            Upgrade_Level = MaxUpgrade_Level;
            _interactArea.gameObject.SetActive(false);

        }
        else
        {
            CurrentPrice = UpgradePrice[Upgrade_Level];
            _interactArea.UpdatePrice(CurrentPrice);
            //PriceText.text = $"{CurrentPrice:0}";

        }
        Managers.Data.SetInt(_objectName + Machine_Num.ToString(), Upgrade_Level);
        SetMesh();

    }


    public void MoveNextNode(Product _product)
    {
        Product _productObj = _product;
        DOTween.Sequence(_productObj.transform.position = StartBelt.position + RailonProduct_interval)
            .AppendCallback(() => _productObj.transform.eulerAngles = SampleRot)
            .Append(_productObj.transform.DOMove(EndBelt.position + RailonProduct_interval, Rail_Speed))
            .OnComplete(() =>
            {
                if (NextNode != null && NextNode.isActive) // next node  true and isactive
                {
                    NextNode.Input_Resource(_productObj);
                }
                else // next node null
                {
                    if (MachineTable.ProductStack.Count < Max_Count)
                    {
                        //MachineTable.ProductStack.Push(_productObj);
                        int _count = MachineTable.ProductStack.Count;
                        if (_count % 2 == 0)
                        {
                            _productObj.transform.DOJump(MachineTable.transform.position + new Vector3(-StackOffset.x * Mathf.Sin(StackOffset.y), BaseUpStack_Inteval + (_count / 2) * Product_Stack_Interval, -StackOffset.z * Mathf.Sin(StackOffset.y) - 0.5f) // positon
                            , 0.5f, 1, 0.5f) // power,jump count,duration                            
                            .Join(_product.transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f).SetEase(Ease.Linear))
                            .OnComplete(() => MachineTable.ProductStack.Push(_productObj));

                        }
                        else
                        {
                            _productObj.transform.DOJump(MachineTable.transform.position + new Vector3(StackOffset.x * Mathf.Sin(StackOffset.y), BaseUpStack_Inteval + (_count / 2) * Product_Stack_Interval, +StackOffset.z * Mathf.Sin(StackOffset.y) - 0.5f) // positon
                          , 0.5f, 1, 0.5f) // power,jump count,duration                            
                            .Join(_product.transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f).SetEase(Ease.Linear))
                            .OnComplete(() => MachineTable.ProductStack.Push(_productObj));
                        }
                        MaxText.SetActive(false);
                    }
                    else
                    {
                        Managers.Pool.Push(_productObj.GetComponent<Poolable>());
                        MaxText.SetActive(true);
                    }
                }
            });
    }

    public void Input_Resource(Product _resource)
    {
        resource_Stack.Push(_resource);
        _resource.transform.position = StartBelt.position + RailonProduct_interval;
        _resource.transform.SetParent(transform);
    }



    [Button]
    public void InitData()
    {


        ES3.Save<int>(_objectName + Machine_Num.ToString(), 0);
    }

    public void SetMesh()
    {
        calc_Value = base_Value + Upgrade_Level;
        Sample_Product.sharedMesh = Product_Meshes[calc_Value];

    }



}
