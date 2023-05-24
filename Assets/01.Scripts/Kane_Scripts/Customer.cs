using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    public float _minDist = 2f;

    public StageManager StageManager;

    public bool isArrive = false;

    public NavMeshAgent _agent;
    //public float Total_ChargingTime = 10f;
    public float Current_ChargingTime = 0f;

    public ChargingTable _chargingTable;

    public int OrderCount = 5;

    public Transform StackPos;

    // ==== values
    public float Stack_Interval = 0.2f;

    public float BaseUp_Interval = 0.5f;


    public Stack<Product> BatteryStack = new Stack<Product>();

    [SerializeField] Vector3 Init_StackPointPos;



    public enum State
    {
        Init,
        Wait,
        Order,
        Move,
        Charging,
        Exit
    }
    public State CustomerState;
    public Animator _animator;
    // ===============================
    private void Start()
    {
        Init_StackPointPos = StackPos.transform.localPosition;
    }


    public void SetInit(StageManager _stagemanager, int _setbatterycount = 5)
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        StageManager = _stagemanager;
        CustomerState = State.Init;
        OrderCount = _setbatterycount;

        if (_animator == null) _animator = GetComponent<Animator>();
        _animator.SetBool("Walk", true);
        _animator.SetBool("Pick", false);
    }

    public void SetDest(Vector3 _destiny)
    {
        _animator.SetBool("Walk", true);
        _agent.destination = _destiny;
        isArrive = false;
    }

    private void Update()
    {
        if ((_agent.remainingDistance < _minDist) && isArrive == false)
        {
            switch (CustomerState)
            {
                case State.Init:
                    isArrive = true;


                    _animator.SetBool("Walk", false);

                    //if (isFirstCustomer == true)
                    //{
                    //    Managers.Game._stagemanager.isHumanOrderReady = true;
                    //}
                    CustomerState = State.Wait;
                    break;

                case State.Wait:
                    isArrive = true;
                    _animator.SetBool("Walk", false);
                    break;

                case State.Order:
                    _animator.SetBool("Walk", false);
                    break;

                case State.Move:
                    int _count = StackPos.childCount;
                    for (int i = 0; i < _count; i++)
                    {
                        Transform _obj = StackPos.GetChild(0);
                        _obj.SetParent(_chargingTable.StackPoint);
                        _obj.localPosition = new Vector3(0f, _obj.localPosition.y, 0f);
                        _obj.localEulerAngles = new Vector3(0f, 45f, 0f);
                    }
                    _animator.SetBool("Walk", false);
                    _animator.SetBool("Pick", false);
                    _animator.SetBool("Charge", true);
                    CustomerState = State.Charging;
                    break;

                case State.Charging:
                    if (BatteryStack.Count > 0)
                    {
                        Current_ChargingTime += Time.deltaTime;
                        if (Current_ChargingTime >= /*Total_ChargingTime*/ 1f)
                        {
                            _chargingTable.SpawnMoney();
                            Current_ChargingTime = 0;
                            BatteryStack.Pop().gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        SetDest(StageManager.HumanMovePos[0].position);
                        _chargingTable.isEmpty = true;
                        _animator.SetBool("Walk", true);
                        _animator.SetBool("Charge", false);
                        CustomerState = State.Exit;
                    }
                    break;

                case State.Exit:
                    StackPos.localPosition = Init_StackPointPos;
                    StageManager.List_Humans.Remove(this);
                    Managers.Pool.Push(this.GetComponent<Poolable>());
                    break;
            }


        }
    }


    public void PushBattery(Product _product, float _interval = 0.5f)
    {
        _animator.SetBool("Pick", true);
        DOTween.Kill(_product);
        Stack_Interval = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        _product.transform.SetParent(StackPos);
        _product.transform.DOLocalJump(Vector3.up * (BatteryStack.Count * Stack_Interval), 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DOLocalRotate(new Vector3(-90f, 0f, 0f), _interval).SetEase(Ease.Linear))
                                         .OnComplete(() =>
                                         {
                                             BatteryStack.Push(_product);
                                             OrderCount--;
                                             _product.transform.localEulerAngles = Vector3.zero; // new Vector3(-90f, 0f, 0f);
                                             // if (OrderCount <= 0) CustomerState = State.Wait;

                                         });
    }




}
