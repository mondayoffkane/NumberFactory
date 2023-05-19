using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Staff : MonoBehaviour
{
    public Transform StackPos;
    public NavMeshAgent _agent;

    public int Speed_Level = 0;
    public int Capacity_Level = 0;
    //public int Count_Level = 0;


    public float base_Speed = 2f;
    public float base_Capacity = 4;


    public float currentSpeed;
    public float currentCapacity;



    public float PickUp_Interval = 0.5f;

    public StageManager _stageManager;

    public Stack<Product> _productStack = new Stack<Product>();


    public enum StaffState
    {
        Idle,
        Move1,
        PickUp,
        Move2,
        PickDowm
    }
    public StaffState _staffState;

    float currentTime = 0f;

    public Object _targetPlace;
    [SerializeField] int _targetNum = 0;
    // =========================================


    public void SetStaffLevel(int _speed_Level, int _capacity_Level)
    {
        Speed_Level = _speed_Level;
        Capacity_Level = _capacity_Level;

        currentSpeed = base_Speed * (Speed_Level + 1);
        currentCapacity = base_Capacity * (Capacity_Level + 1);
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        FindWork();
    }


    private void Update()
    {
        if (Vector3.Distance(transform.position, _agent.destination) < 1f)
        {
            switch (_staffState)
            {
                case StaffState.Move1:
                    _staffState = StaffState.PickUp;

                    break;

                case StaffState.PickUp:

                    currentTime += Time.deltaTime;
                    if (currentTime >= 1f)
                    {
                        currentTime = 0f;
                        MachineTable _table = (MachineTable)_targetPlace;
                        if (_table.ProductStack.Count > 0)
                        {
                            PushProduct(_table.ProductStack.Pop());
                        }

                        if (_productStack.Count >= currentCapacity)
                        {
                            switch (_targetNum)
                            {
                                case 0:
                                    _agent.destination = _stageManager._counter.transform.position;
                                    _targetPlace = _stageManager._counter;
                                    break;

                                case 1:
                                    _agent.destination = _stageManager._chargingMachine.transform.position;
                                    _targetPlace = _stageManager._chargingMachine;

                                    break;

                                case 2:
                                    _agent.destination = _stageManager._generator.transform.position;
                                    _targetPlace = _stageManager._generator;
                                    break;
                            }
                            _staffState = StaffState.Move2;
                        }

                    }
                    break;

                case StaffState.Move2:
                    _staffState = StaffState.PickDowm;
                    break;

                case StaffState.PickDowm:
                    currentTime += Time.deltaTime;
                    if (currentTime >= 0.2f)
                    {
                        currentTime = 0f;

                        switch (_targetNum)
                        {
                            case 0:
                                Counter _counter = (Counter)_targetPlace;
                                _counter.PushBattery(_productStack.Pop());
                                break;

                            case 1:
                                ChargingMachine _chargingmachine = (ChargingMachine)_targetPlace;
                                _chargingmachine.PushBattery(_productStack.Pop());
                                break;

                            case 2:
                                Generator _generator = (Generator)_targetPlace;
                                _generator.PushNum(_productStack.Pop());
                                break;
                        }
                        if (_productStack.Count <= 0)
                        {
                            _staffState = StaffState.Idle;
                            FindWork();
                        }

                        //MachineTable _table = (MachineTable)_targetPlace;
                        //if (_table.ProductStack.Count > 0)
                        //{
                        //    _productStack.Push(_table.ProductStack.Pop());
                        //}

                        //if (_productStack.Count >= currentCapacity)
                        //{
                        //    _staffState = StaffState.Move2;
                        //}

                    }
                    break;
            }
        }
    }

    public void FindWork()
    {
        if (_stageManager._counter.BatteryStack.Count < 10)
        {
            _agent.destination = _stageManager._generator.StackPoint.transform.position;
            _targetPlace = (MachineTable)(_stageManager._generator.StackPoint);
            _staffState = StaffState.Move1;
            _targetNum = 0;
        }
        else if (_stageManager._chargingMachine.BatteryStack.Count < 10)
        {
            _agent.destination = _stageManager._generator.StackPoint.transform.position;
            _targetPlace = _stageManager._generator.StackPoint;
            _staffState = StaffState.Move1;
            _targetNum = 1;
        }
        else
        {
            for (int i = 0; i < _stageManager._numberTables.Length; i++)
            {
                if (_stageManager._numberTables[i].ProductStack.Count > 0)
                {
                    _agent.destination = _stageManager._numberTables[i].transform.position;
                    _targetPlace = _stageManager._numberTables[i];
                    _staffState = StaffState.Move1;
                    _targetNum = 2;
                    break;
                }
            }
        }
    }


    public void PushProduct(Product _product)
    {
        DOTween.Kill(_product);
        _productStack.Push(_product);
        _product.transform.SetParent(transform);
        _product.transform.DOLocalJump(StackPos.localPosition + Vector3.up * _productStack.Count * 0.2f, 1f, 1, 0.5f)
            .OnComplete(() =>
            {
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
