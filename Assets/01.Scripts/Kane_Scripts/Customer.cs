using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    public StageManager StageManager;

    public bool isArrive = false;

    public NavMeshAgent _agent;
    //public float Total_ChargingTime = 10f;
    public float Current_ChargingTime = 0f;

    public ChargingTable _chargingTable;

    public int OrderCount = 5;

    public Transform StackPoint;

    // ==== values
    public float Stack_Interval = 0.2f;
   
    public float BaseUp_Interval = 0.5f;


    public Stack<Product> BatteryStack = new Stack<Product>();

    [SerializeField] Vector3 Init_StackPointPos;

    public enum State
    {
        Init,
        Order,
        Wait,
        Charging,
        Exit
    }
    public State CustomerState;

    private void Start()
    {
        Init_StackPointPos = StackPoint.transform.localPosition;
    }


    public void SetInit(StageManager _stagemanager, int _setbatterycount = 5)
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        StageManager = _stagemanager;
        CustomerState = State.Init;
        OrderCount = _setbatterycount;


    }

    public void SetDest(Vector3 _destiny)
    {
        _agent.destination = _destiny;
        isArrive = false;
    }

    private void Update()
    {
        if ((Vector3.Distance(transform.position, _agent.destination)) < 1.5f && isArrive == false)
        {
            switch (CustomerState)
            {
                case State.Init:
                    isArrive = true;
                    CustomerState = State.Order;
                    break;

                case State.Order:

                    break;

                case State.Wait:
                    int _count = StackPoint.childCount;
                    for (int i = 0; i < _count; i++)
                    {
                        Transform _obj = StackPoint.GetChild(0);
                        _obj.SetParent(_chargingTable.StackPoint);
                        _obj.localPosition = new Vector3(0f, _obj.localPosition.y, 0f);
                        _obj.localEulerAngles = new Vector3(0f, 45f, 0f);
                    }
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
                        CustomerState = State.Exit;
                    }
                    break;

                case State.Exit:
                    StackPoint.localPosition = Init_StackPointPos;
                    StageManager.List_Humans.Remove(this);
                    Managers.Pool.Push(this.GetComponent<Poolable>());
                    break;
            }


        }
    }


    public void PushBattery(Product _product, float _interval = 0.5f)
    {
        DOTween.Kill(_product);

        _product.transform.SetParent(StackPoint);
        _product.transform.DOLocalJump(Vector3.up * (BaseUp_Interval + BatteryStack.Count * Stack_Interval), 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DOLocalRotate(new Vector3(-90f, 0f, 0f), _interval).SetEase(Ease.Linear))
                                         .OnComplete(() =>
                                         {
                                             BatteryStack.Push(_product);
                                             OrderCount--;
                                             _product.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                                             // if (OrderCount <= 0) CustomerState = State.Wait;

                                         });
    }




}
