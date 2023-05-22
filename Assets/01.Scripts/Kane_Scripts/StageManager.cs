using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
//using UnityEngine.AI;



public class StageManager : MonoBehaviour
{
    public int Stage_Num = 0;

    public MachineTable[] _numberTables;

    public List<ChargingTable> Tables;
    public List<ChargingPark> Parks;

    public Generator _generator;
    public GameObject Customer_Human;
    public GameObject Customer_Car;
    public GameObject Staff_Pref;


    public Transform[] HumanMovePos;
    public Transform[] CarMovePos;
    public Transform StaffPos;

    public List<Customer> List_Humans = new List<Customer>();
    public List<CustomerCar> List_Cars = new List<CustomerCar>();

    public float Customer_Inteval = 3f;
    public float HumanPos_Interval = 1f;
    public float CarPos_Interval = 2f;

    public Counter _counter;
    public ChargingMachine _chargingMachine;
    public int Min_Count = 2;
    public int Max_Count = 10;

    public Player _player;


    //public List<MachineTable> _machineTables = new List<MachineTable>();


    // =============================
    //[ShowInInspector]
    [SerializeField] StageData _stageData;

    private void Start()
    {
        _stageData = Managers.Data.LoadData(Stage_Num); // Load Data


        GameObject[] _list1 = GameObject.FindGameObjectsWithTag("ChargingTable");
        for (int i = 0; i < _list1.Length; i++)
        {
            Tables.Add(_list1[i].GetComponent<ChargingTable>());
        }

        GameObject[] _list2 = GameObject.FindGameObjectsWithTag("ChargingPark");
        for (int i = 0; i < _list2.Length; i++)
        {
            Parks.Add(_list2[i].GetComponent<ChargingPark>());
        }

        Customer_Human = Resources.Load<GameObject>("Customer_Human");
        Customer_Car = Resources.Load<GameObject>("Customer_Car");
        Staff_Pref = Resources.Load<GameObject>("Staff");
        _counter = GameObject.FindGameObjectWithTag("Counter").GetComponent<Counter>();
        _chargingMachine = GameObject.FindGameObjectWithTag("ChargingMachine").GetComponent<ChargingMachine>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _generator = GameObject.FindGameObjectWithTag("Generator").GetComponent<Generator>();

        StageSetting();
        StartCoroutine(Cor_Update());
        StartCoroutine(Cor_Order());
    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return new WaitForSeconds(Customer_Inteval);
            AddCustomer_Human();
            AddCustomer_Car();
            //FindTable();
        }
    }
    [Button]
    public void AddCustomer_Human()
    {
        if (List_Humans.Count < 10)
        {
            Transform _human = Managers.Pool.Pop(Customer_Human, transform).transform;
            _human.position = HumanMovePos[0].position;
            Customer _customerHuman = _human.GetComponent<Customer>();
            _customerHuman.SetInit(this, Random.Range(Min_Count, Max_Count));

            List_Humans.Add(_customerHuman);

            for (int i = 0; i < List_Humans.Count; i++)
            {
                List_Humans[i].SetDest(HumanMovePos[1].position + new Vector3(HumanPos_Interval * Mathf.Sin(45), 0f, -HumanPos_Interval * Mathf.Sin(45)) * i);
            }
        }
    }
    public void AddCustomer_Car()
    {
        if (List_Cars.Count < 10)
        {
            Transform _car = Managers.Pool.Pop(Customer_Car, transform).transform;
            _car.position = CarMovePos[0].position;
            CustomerCar _customerCar = _car.GetComponent<CustomerCar>();
            _customerCar.SetInit(this, _chargingMachine, Random.Range(Min_Count, Max_Count));

            List_Cars.Add(_customerCar);

            for (int i = 0; i < List_Cars.Count; i++)
            {

                int num = i > 7 ? 7 : i;

                List_Cars[i].SetDest(CarMovePos[1].position
                    + new Vector3((CarPos_Interval * Mathf.Sin(45)) * num, 0f, (CarPos_Interval * Mathf.Sin(45)) * num));

            }
        }
    }


    IEnumerator Cor_Order()
    {
        WaitForSeconds _interval = new WaitForSeconds(0.5f);
        while (true)
        {
            yield return _interval;

            if (List_Humans.Count > 0)
            {
                Customer _customer = List_Humans[0];
                switch (_customer.CustomerState)
                {
                    case Customer.State.Order:
                        if (_customer.OrderCount > 0 && _counter.BatteryStack.Count > 0)
                        {
                            _customer.PushBattery(_counter.BatteryStack.Pop());

                        }
                        if (_customer.OrderCount <= 0)
                        {
                            ChargingTable _table = FindTable();
                            if (_table != null)
                            {
                                _table.isEmpty = false;
                                _customer.SetDest(_table.transform.position);
                                _customer._chargingTable = _table;
                                List_Humans.Remove(_customer);
                            }
                            _customer._animator.SetBool("Walk", true);
                            _customer.CustomerState = Customer.State.Wait;
                        }
                        break;
                }
            }

            if (List_Cars.Count > 0)
            {
                CustomerCar _customercar = List_Cars[0];
                switch (_customercar.CustomerCarState)
                {
                    case CustomerCar.State.Wait:
                        if (_customercar.OrderCount > 0 && _chargingMachine.BatteryStack.Count > 0)
                        {
                            _chargingMachine.BatteryStack.Pop().gameObject.SetActive(false);
                            _customercar.OrderCount--;
                            _customercar.CurrentCount++;
                        }
                        if (_customercar.OrderCount <= 0)
                        {
                            ChargingPark _park = FindPark();
                            if (_park != null)
                            {
                                _park.isEmpty = false;
                                _customercar.SetDest(_park.ParkPos.position);
                                _customercar._chargingPark = _park;
                                List_Cars.Remove(_customercar);
                            }
                            _customercar.CustomerCarState = CustomerCar.State.Move;
                        }
                        break;
                }
            }


        }
    }

    public ChargingTable FindTable()
    {
        for (int i = 0; i < Tables.Count; i++)
        {
            if (Tables[i].isEmpty == true)
            {
                return Tables[i];
            }
        }
        return null;
    }

    public ChargingPark FindPark()
    {
        for (int i = 0; i < Parks.Count; i++)
        {
            if (Parks[i].isEmpty == true)
            {
                return Parks[i];
            }
        }
        return null;
    }

    public void StageSetting()
    {
        // User Setting
        _player.InitPlayer(_stageData.PlayerSpeed_Level, _stageData.PlayerCapacity_Level, _stageData.PlayerIncome_Level);


        // Staff Setting
        for (int i = 0; i < _stageData.StaffCount_Level; i++)
        {
            AddStaff();
        }
    }

    [Button]
    public void AddStaff()
    {
        Staff _staff = Managers.Pool.Pop(Staff_Pref, transform).GetComponent<Staff>();
        _staff._stageManager = this;
        _staff.SetStaffLevel(_stageData.StaffSpeed_Level, _stageData.StaffCapacity_Level);
        _staff.transform.position = StaffPos.position;

    }


    [Button]
    public void SaveData()
    {
        Managers.Data.SaveData(_stageData, Stage_Num); // SaveData;
    }




}
