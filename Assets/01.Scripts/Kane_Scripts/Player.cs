using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float base_Speed = 5f;
    public int base_Capacity = 4;
    public double base_MoneyPrice = 3f;

    public float add_Speed = 2f;
    public int add_Capacity = 4;
    public double add_MoneyPrice = 3f;

    public double total_MoneyPrice;

    public int Speed_Level = 0;
    public int Capacity_Level = 0;
    public int Income_Level = 0;

    public int Max_Count = 10;



    public Stack<Product> ProductStack = new Stack<Product>();
    public float Stack_Interval = 1f;
    public float Stack_num_interval = 0.05f;

    [SerializeField] Transform PickPos;

    public float Move_Interval = 0.5f;
    public float Pickup_Interval = 0.1f;

    [SerializeField] bool isReady = true;


    public Material Rail_Mat;
    public float RailMat_Speed = 1f;

    Animator _animator;

    public AnimationCurve _curveLine;
    public JoyStickController _joystick;
    [SerializeField] double tempGetMoney = 0;

    [SerializeField] GameObject _FloatingText;
    public GameObject MaxText;
    // ==========================

    public void UpdateStat(int _speedLevel, int _capacityLevel, int _incomeLevel)
    {
        Speed_Level = _speedLevel;
        Capacity_Level = _capacityLevel;
        Income_Level = _incomeLevel;

        _joystick.Speed = base_Speed + (add_Speed * Speed_Level);
        Max_Count = base_Capacity + (add_Capacity * Capacity_Level);
        total_MoneyPrice = base_MoneyPrice + (add_MoneyPrice * Income_Level);
    }


    private void Start()
    {
        PickPos = transform.GetChild(0);

        if (_animator == null) _animator = GetComponent<Animator>();

        Rail_Mat.SetTextureOffset("_BaseMap", Vector2.zero);
        Rail_Mat.DOOffset(Vector2.down, RailMat_Speed).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        _FloatingText = Resources.Load<GameObject>("Floating");

        //MaxText.transform.SetParent(null);
        MaxText.GetComponent<FollowObj_NonRot>().Start();
        MaxText.SetActive(false);

        if (Managers.Game._stagemanager._stageData.isFirst)
        {
            transform.position = new Vector3(27f, 0f, -22f);
            transform.Find("Look").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Look").gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //StackCurve();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Managers.Game.Money += 10000d;
        }
    }


    public void StackCurve()
    {
        if (ProductStack.Count > 0)
        {
            for (int i = 0; i < ProductStack.Count; i++)
            {
                Transform _trans = PickPos.GetChild(i);
                _trans.localPosition = Vector3.Lerp(_trans.localPosition, _trans.localPosition + new Vector3(0f, 0f, -1f * _curveLine.Evaluate((float)i / (float)Max_Count)), Time.deltaTime);
            }


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "PlayerHR":
                Managers.GameUI.PlayerHR_Panel.SetActive(true);
                if (Managers.Game._stagemanager._tutorial._tutorial_num == 8)
                {
                    Managers.Game._stagemanager._tutorial._arrow.position = Vector3.one * 1000;
                    transform.Find("Look").gameObject.SetActive(false);
                }
                break;

            case "StaffHR":
                Managers.GameUI.StaffHR_Panel.SetActive(true);
                if (Managers.Game._stagemanager._tutorial._tutorial_num == 8)
                {
                    Managers.Game._stagemanager._tutorial._arrow.position = Vector3.one * 1000;
                    transform.Find("Look").gameObject.SetActive(false);
                }
                break;

            case "ChargingTable":
                GetMoney2(other.GetComponent<ChargingTable>().MoneyStack);

                break;

            case "ChargingMachine":
                GetMoney2(other.GetComponent<ChargingMachine>().MoneyStack);

                break;

            case "StartMoney":
                GetMoney2(other.GetComponent<StartMoney>().MoneyStack);
                Managers.Game._stagemanager._tutorial.LockOff();
                other.gameObject.SetActive(false);
                break;

            case "Counter":
                Managers.Game._stagemanager.isPlayerinCounter = true;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "PlayerHR":
                Managers.GameUI.PlayerHR_Panel.SetActive(false);
                break;

            case "StaffHR":
                Managers.GameUI.StaffHR_Panel.SetActive(false);
                break;
            case "Counter":
                Managers.Game._stagemanager.isPlayerinCounter = false;
                break;

        }
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
                        Managers.Sound.Play("Stack");
                        Product _product = other.GetComponent<MachineTable>().ProductStack.Pop();
                        DOTween.Kill(_product);
                        ProductStack.Push(_product);
                        _product.transform.SetParent(PickPos);

                        Stack_Interval = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y + Stack_num_interval;

                        isReady = false;

                        switch (_product._productType)
                        {
                            case Product.ProductType.Number:
                                _product.transform.DOLocalJump(new Vector3(0f, 0f, 1f) + Vector3.up * (ProductStack.Count - 1) * Stack_Interval, 1f, 1, Move_Interval).OnComplete(() => _product.transform.localEulerAngles = new Vector3(-180f, 90f, -180f));

                                if (Managers.Game._stagemanager._tutorial._tutorial_num == 2)
                                {
                                    Managers.Game._stagemanager._tutorial._arrow.position =
                                        Managers.Game._stagemanager._generator.transform.position + Vector3.left * 4.6f;
                                }
                                break;

                            case Product.ProductType.Battery:
                                _product.transform.DOLocalJump(Vector3.up * (ProductStack.Count - 1) * Stack_Interval, 1f, 1, Move_Interval).OnComplete(() => _product.transform.localEulerAngles = Vector3.zero);

                                if (Managers.Game._stagemanager._tutorial._tutorial_num == 4)
                                {
                                    Managers.Game._stagemanager._tutorial._arrow.position =
                                        Managers.Game._stagemanager._chargingMachine.StackPoint.transform.position;
                                }

                                if (Managers.Game._stagemanager._tutorial._tutorial_num == 6)
                                {
                                    Managers.Game._stagemanager._tutorial._arrow.position =
                                        Managers.Game._stagemanager._counter.StackPoint.transform.position;
                                }


                                break;
                        }


                        DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() =>
                        {
                            isReady = true;

                            //if (ProductStack.Count >= Max_Count) MaxText.SetActive(true);
                        });

                    }
                }
                break;

            case "Generator":
                if (ProductStack.Count > 0 && isReady && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Number)
                {
                    Managers.Sound.Play("Stack");
                    Product _product = ProductStack.Pop().GetComponent<Product>();
                    isReady = false;
                    other.GetComponent<Generator>().PushNum(_product);

                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);
                    if (Managers.Game._stagemanager._tutorial._tutorial_num == 2)
                    {
                        Managers.Game._stagemanager._tutorial.LockOff();
                    }

                }
                else if (ProductStack.Count <= 0) _animator.SetBool("Pick", false);

                break;



            case "Counter":
                if (isReady && ProductStack.Count > 0 && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Battery)
                {
                    Managers.Sound.Play("Stack");
                    other.GetComponent<Counter>().PushBattery(ProductStack.Pop(), Move_Interval);
                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);

                    if (Managers.Game._stagemanager._tutorial._tutorial_num == 6)
                    {
                        Managers.Game._stagemanager._tutorial._arrow.position =
                            Managers.Game._stagemanager._counter._interactArea.transform.position;
                    }
                }
                else if (ProductStack.Count <= 0) _animator.SetBool("Pick", false);

                break;


            case "ChargingMachine":
                if (isReady && ProductStack.Count > 0 && ProductStack.Peek().GetComponent<Product>()._productType == Product.ProductType.Battery)
                {
                    Managers.Sound.Play("Stack");
                    other.GetComponent<ChargingMachine>().PushBattery(ProductStack.Pop(), Move_Interval);
                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);

                    if (Managers.Game._stagemanager._tutorial._tutorial_num == 4)
                    {
                        Managers.Game._stagemanager._tutorial._arrow.position =
                            Managers.Game._stagemanager._chargingMachine.StackPoint.transform.position;
                        Managers.Game._stagemanager._tutorial.LockOff();
                    }
                }
                else if (ProductStack.Count <= 0) _animator.SetBool("Pick", false);

                //ChargingMachine _chargingmachine = other.GetComponent<ChargingMachine>();
                //int _count = _chargingmachine.MoneyStack.Count / 10;
                //_count = _count < 1 ? 1 : _count;
                //_count = _count > 10 ? 10 : _count;
                //for (int i = 0; i < _count; i++)
                //{
                //    GetMoney(_chargingmachine.PopMoney());
                //}


                break;
            case "ChargingTable":

                //ChargingTable _chargingtable = other.GetComponent<ChargingTable>();
                //int _count2 = _chargingtable.MoneyStack.Count / 10;
                //_count2 = _count2 < 1 ? 1 : _count2;
                //_count2 = _count2 > 10 ? 10 : _count2;
                //for (int i = 0; i < _count2; i++)
                //{
                //    GetMoney(_chargingtable.PopMoney());
                //}

                break;

            case "GarbageCan":
                if (isReady && ProductStack.Count > 0)
                {
                    Managers.Sound.Play("Stack");
                    Transform _trans = ProductStack.Pop().transform;
                    _trans.SetParent(other.transform);
                    _trans.DOLocalJump(Vector3.zero, 2f, 1, Move_Interval)
                        .Join(_trans.DOScale(Vector3.zero, Move_Interval))
                        .Join(_trans.DOLocalRotate(Vector3.zero, Move_Interval))
                        .OnComplete(() => Managers.Pool.Push(_trans.GetComponent<Poolable>()));
                    DOTween.Sequence(isReady = false).AppendInterval(Pickup_Interval).OnComplete(() => isReady = true);
                }
                else if (ProductStack.Count <= 0) _animator.SetBool("Pick", false);

                break;


        }

        if (ProductStack.Count >= Max_Count) MaxText.SetActive(true);

        bool isactive = ProductStack.Count >= Max_Count ? true : false;
        MaxText.SetActive(isactive);


    }

    public void GetMoney(Transform _money)
    {
        if (_money != null)
        {
            _money.SetParent(transform);
            DOTween.Kill(_money);
            _money.DOLocalJump(Vector3.zero, 2, 1, Move_Interval * 0.5f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Managers.Game.Money += total_MoneyPrice;
                    Managers.Pool.Push(_money.GetComponent<Poolable>());
                    _money.gameObject.SetActive(false);

                    Managers.GameUI.Money_Text.text = $"{Managers.ToCurrencyString(Managers.Game.Money)}";


                });
        }
    }

    public void GetMoney2(Stack<Transform> _tempmoneyStack)
    {

        Stack<Transform> _moneyStack = _tempmoneyStack;
        int totalcount = _moneyStack.Count;
        int _count = _moneyStack.Count / 10 < 1 ? _moneyStack.Count : (_moneyStack.Count / 10) + 1;

        if (_count != 0)
        {
            StartCoroutine(Cor_GetMoney());
            Managers.Sound.Play("Money");
        }
        IEnumerator Cor_GetMoney()
        {
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < _count; j++)
                {
                    try
                    {
                        if (_moneyStack.Count < 1) break;
                        Transform _money = _moneyStack.Pop();
                        _money.SetParent(transform);
                        _money.DOLocalJump(Vector3.zero, 2, 1, Move_Interval * 0.5f).SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                Managers.Pool.Push(_money.GetComponent<Poolable>());
                                _money.gameObject.SetActive(false);
                            });
                    }
                    catch { }
                }
                yield return new WaitForSeconds(0.05f);
            }

            tempGetMoney = total_MoneyPrice * totalcount;
            //Managers.Game.Money += tempGetMoney;
            Managers.Game.UpdateMoney(tempGetMoney);
            PopText(Managers.ToCurrencyString(tempGetMoney));

        }
    }



    public void PopText(string _str)
    {
        Transform _floatingText = Managers.Pool.Pop(_FloatingText, transform).GetComponent<Transform>();
        _floatingText.localPosition = new Vector3(0f, 3f, 0f);
        _floatingText.GetComponentInChildren<Text>().text = $"{_str}";
        _floatingText.DOMoveY(4f, 1f).SetEase(Ease.Linear)
            .OnComplete(() => Managers.Pool.Push(_floatingText.GetComponent<Poolable>()));
    }


    [Button]
    public void InitData()
    {
        PlayerPrefs.DeleteAll();
    }

}

