using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class InteractArea : MonoBehaviour
{

    public GameObject TargetObj;
    [SerializeField] Object _target;

    public double[] Max_Money_List = new double[3] { 10d, 100d, 1000d };
    public double Max_Money;
    public double Current_Money;

    public enum TargetType
    {
        Machine,
        Table,
        Park
    }
    public TargetType _targetType;

    public Text MoneyText;

    [SerializeField] Player _player;


    private void Start()
    {
        switch (_targetType)
        {
            case TargetType.Machine:
                _target = TargetObj.GetComponent<Machine>();
                break;

            case TargetType.Table:
                _target = TargetObj.GetComponent<ChargingTable>();
                break;

            case TargetType.Park:
                _target = TargetObj.GetComponent<ChargingPark>();
                break;
        }

        MoneyText = transform.Find("MoneyText").GetComponent<Text>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void PayMoney()
    {

    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }


}
