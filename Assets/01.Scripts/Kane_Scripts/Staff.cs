using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;

public class Staff : MonoBehaviour
{
    public float _minDist = 2f;
    public Transform StackPos;
    public NavMeshAgent _agent;

    public int Speed_Level = 0;
    public int Capacity_Level = 0;
    //public int Count_Level = 0;


    public float base_Speed = 2f;
    public int base_Capacity = 4;

    public float add_Speed = 3.5f;
    public int add_Capacity = 2;

    //public float currentSpeed;
    public float max_Capacity;



    public float PickUp_Interval = 0.5f;

    public StageManager _stageManager;

    public Stack<Product> _productStack = new Stack<Product>();

    Animator _animator;

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

    [SerializeField] int _rand;

    public List<MachineTable> _list;
    float Stack_Interval = 0.2f;
    // =========================================


    public void SetStaffLevel(int _speed_Level, int _capacity_Level)
    {
       if(_agent==null) _agent = GetComponent<NavMeshAgent>();
        if (_animator == null) _animator = GetComponent<Animator>();


        UpdateStat(_speed_Level, _capacity_Level);
    }

    public void UpdateStat(int _speedLevel , int _capacityLevel)
    {
        Speed_Level = _speedLevel;
        Capacity_Level = _capacityLevel;

        _agent.speed = base_Speed +  (add_Speed* Speed_Level);
        max_Capacity = base_Capacity + (add_Capacity* Capacity_Level);

    }



    private void Start()
    {
        FindWork();

    }


    private void Update()
    {
        //if (Vector3.Distance(transform.position, _agent.destination) < 1f)
        if (_agent.remainingDistance < _minDist)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            switch (_staffState)
            {
                case StaffState.Move1:
                    _animator.SetBool("Walk", false);
                    _animator.SetBool("Pick", true);
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

                        if (_productStack.Count >= max_Capacity)
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
                            _agent.isStopped = false;
                            _animator.SetBool("Walk", true);
                            _animator.SetBool("Pick", true);
                            _staffState = StaffState.Move2;
                        }

                    }
                    break;

                case StaffState.Move2:
                    _animator.SetBool("Walk", false);
                    _animator.SetBool("Pick", true);
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
                            _agent.isStopped = false;
                            _animator.SetBool("Walk", false);
                            _animator.SetBool("Pick", false);
                            _staffState = StaffState.Idle;
                            FindWork();
                        }


                    }
                    break;
            }
        }
    }

    public void FindWork()
    {
        _rand = Random.Range(0, 3);

        switch (_rand)
        {
            case 0:
                if (_stageManager._chargingMachine.BatteryStack.Count < 5)
                {
                    _agent.destination = _stageManager._generator.StackPoint.transform.position;
                    _targetPlace = _stageManager._generator.StackPoint;
                    _animator.SetBool("Walk", true);
                    _animator.SetBool("Pick", false);
                    _staffState = StaffState.Move1;
                    _targetNum = 1;
                }
                else
                {
                    _agent.destination = _stageManager._generator.StackPoint.transform.position;
                    _targetPlace = (MachineTable)(_stageManager._generator.StackPoint);
                    _animator.SetBool("Walk", true);
                    _animator.SetBool("Pick", false);
                    _staffState = StaffState.Move1;
                    _targetNum = 0;
                }

                break;

            case 1:
                if (_stageManager._counter.BatteryStack.Count < 5)
                {
                    _agent.destination = _stageManager._generator.StackPoint.transform.position;
                    _targetPlace = (MachineTable)(_stageManager._generator.StackPoint);
                    _animator.SetBool("Walk", true);
                    _animator.SetBool("Pick", false);
                    _staffState = StaffState.Move1;
                    _targetNum = 0;
                }
                else
                {
                    _agent.destination = _stageManager._generator.StackPoint.transform.position;
                    _targetPlace = _stageManager._generator.StackPoint;
                    _animator.SetBool("Walk", true);
                    _animator.SetBool("Pick", false);
                    _staffState = StaffState.Move1;
                    _targetNum = 1;
                }

                break;

            case 2:
                /*List<MachineTable>*/
                _list = _stageManager._numberTables.OrderBy(x => Random.value).ToList();

                for (int i = 0; i < _list.Count; i++)
                {
                    if (_list[i].ProductStack.Count > 0)
                    {
                        _agent.destination = _list[i].transform.position;
                        _targetPlace = _list[i];
                        _animator.SetBool("Walk", true);
                        _animator.SetBool("Pick", false);
                        _staffState = StaffState.Move1;
                        _targetNum = 2;
                        //break;
                        return;
                    }
                }
                FindWork();
                break;
        }


    }


    public void PushProduct(Product _product)
    {
        DOTween.Kill(_product);
        Stack_Interval = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        _productStack.Push(_product);
        _product.transform.SetParent(StackPos);

        switch (_product._productType)
        {
            case Product.ProductType.Number:
                _product.transform.DOLocalJump(new Vector3(0f, 0f, 0.4f) + Vector3.up * (_productStack.Count - 1) * Stack_Interval, 1f, 1, 0.5f)
              .OnComplete(() => _product.transform.localEulerAngles = new Vector3(-180f, 90f, -180f));
                break;

            case Product.ProductType.Battery:

                _product.transform.DOLocalJump(Vector3.up * (_productStack.Count - 1) * Stack_Interval, 1f, 1, 0.5f)
            .OnComplete(() => _product.transform.localEulerAngles = Vector3.zero);

                break;
        }


    }


}
