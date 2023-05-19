using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class CustomerCar : MonoBehaviour
{
    public Mesh[] CarMeshes;

    public StageManager StageManager;
    public bool isArrive = false;

    public NavMeshAgent _agent;

    public int OrderCount = 5;
    public int CurrentCount = 0;
    public float Current_ChargingTime = 0f;

    MeshFilter _meshfilter;
    public ChargingPark _chargingPark;
    public ChargingMachine _chargingMachine;

    public enum State
    {
        Init,
        Wait,
        Move,
        Charging,
        Exit
    }
    public State CustomerCarState;


    public void SetInit(StageManager _stagemanager, ChargingMachine _chargingmachine, int _ordercount = 5, int _Carnum = 0)
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        if (_meshfilter == null) _meshfilter = GetComponent<MeshFilter>();
        if (_Carnum == 0)
        {
            _meshfilter.sharedMesh = CarMeshes[Random.Range(0, CarMeshes.Length)];
        }
        else
        {
            _meshfilter.sharedMesh = CarMeshes[_Carnum];
        }

        StageManager = _stagemanager;
        _chargingMachine = _chargingmachine;
        CustomerCarState = State.Init;
        OrderCount = _ordercount;
    }

    public void SetDest(Vector3 _destiny)
    {
        if (_agent.enabled == false) _agent.enabled = true;
        _agent.destination = _destiny;
        isArrive = false;
    }


    private void Update()
    {
        if ((Vector3.Distance(transform.position, _agent.destination)) < 1.5f && isArrive == false)
        {
            switch (CustomerCarState)
            {
                case State.Init:
                    isArrive = true;
                    CustomerCarState = State.Wait;
                    break;

                case State.Wait:

                    break;

                case State.Move:
                    CustomerCarState = State.Charging;

                    break;

                case State.Charging:
                    if (CurrentCount > 0)
                    {
                        Current_ChargingTime += Time.deltaTime;
                        if (Current_ChargingTime >= 1f)
                        {
                            _chargingMachine.SpawnMoney();
                            Current_ChargingTime = 0f;
                            CurrentCount--;
                        }
                    }
                    else
                    {
                        SetDest(StageManager.CarMovePos[2].position);
                        _chargingPark.isEmpty = true;
                        CustomerCarState = State.Exit;

                    }
                    break;

                case State.Exit:
                    StageManager.List_Cars.Remove(this);
                    _agent.destination = StageManager.CarMovePos[0].position;
                    transform.position = StageManager.CarMovePos[0].position;
                    Managers.Pool.Push(this.GetComponent<Poolable>());
                    break;
            }
        }

    }







}
