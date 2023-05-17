using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
//using UnityEngine.AI;



public class StageManager : MonoBehaviour
{


    public List<ChargingTable> Tables;
    public List<ChargingPark> Parks;

    public GameObject Customer_Human;
    public GameObject Customer_Car;


    public Transform[] MovePos;

    public List<Customer> List_Humans = new List<Customer>();
    public List<Customer> List_Cars = new List<Customer>();

    public float Customer_Inteval = 3f;
    public float Pos_Interval = 1f;

    public Counter _counter;
    public int Min_Count = 2;
    public int Max_Count = 10;
    // =============================

    private void Start()
    {
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
        _counter = GameObject.FindGameObjectWithTag("Counter").GetComponent<Counter>();



        StartCoroutine(Cor_Update());
        StartCoroutine(Cor_Order());
    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return new WaitForSeconds(Customer_Inteval);
            AddCustomer_Human();
            //FindTable();
        }
    }
    [Button]
    public void AddCustomer_Human()
    {
        if (List_Humans.Count < 10)
        {
            Transform _human = Managers.Pool.Pop(Customer_Human, transform).transform;
            _human.position = MovePos[0].position;
            Customer _customerHuman = _human.GetComponent<Customer>();
            _customerHuman.SetInit(this, Random.Range(Min_Count, Max_Count));

            List_Humans.Add(_customerHuman);

            for (int i = 0; i < List_Humans.Count; i++)
            {
                List_Humans[i].SetDest(MovePos[1].position + new Vector3(Pos_Interval * Mathf.Sin(45), 0f, -Pos_Interval * Mathf.Sin(45)) * i);
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
                            _customer.CustomerState = Customer.State.Wait;
                        }
                        break;

                        //case Customer.State.Wait:
                        //    {
                        //    }

                        //    break;
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




    [Button]
    public void FindTable2()
    {
        if (List_Humans.Count > 0)
        {
            for (int i = 0; i < List_Humans.Count; i++)
            {
                if (List_Humans[i].CustomerState == Customer.State.Order)
                {
                    List_Humans[i].SetDest(MovePos[1].position + new Vector3(Pos_Interval * Mathf.Sin(45), 0f, -Pos_Interval * Mathf.Sin(45)) * i);
                }
            }

            for (int i = 0; i < Tables.Count; i++)
            {
                if (Tables[i].isEmpty == true)
                {
                    Customer _customer = List_Humans[0];

                    if (_customer.CustomerState == Customer.State.Order)
                    {
                        Tables[i].isEmpty = false;
                        _customer.SetDest(Tables[i].transform.position);
                        _customer._chargingTable = Tables[i];
                        _customer.CustomerState = Customer.State.Wait;
                        List_Humans.Remove(_customer);
                    }

                    break;
                }
            }
        }

    }

    public void FIndPark()
    {

    }










}
