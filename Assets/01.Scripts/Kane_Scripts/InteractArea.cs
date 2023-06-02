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


    public enum TargetType
    {
        Machine,
        Table,
        Park,
        Counter,
        ChargingMachine
    }
    public TargetType _targetType;

    //public Text MoneyText;

    [SerializeField] Machine _machine;
    [SerializeField] ChargingTable _table;
    [SerializeField] ChargingPark _park;
    [SerializeField] Counter _counter;
    [SerializeField] ChargingMachine _chargingmachine;

    public double Max_Price;
    public double Current_Price;

    public Image _fillImg;
    public Text _priceText;

    public void SetPrice(double _value)
    {
        Max_Price = _value;
        Current_Price = Max_Price;
        _priceText.text = $"{Managers.ToCurrencyString(Current_Price)}";
        _fillImg.fillAmount = (float)((Max_Price - Current_Price) / Max_Price);

    }
    public void UpdatePrice(double _value)
    {
        Current_Price = _value;
        _priceText.text = $"{Managers.ToCurrencyString(Current_Price)}";
        _fillImg.fillAmount = (float)((Max_Price - Current_Price) / Max_Price);

    }


    public void SetTarget(Object _obj, TargetType _type)
    {
        _target = _obj;
        _targetType = _type;

        switch (_targetType)
        {
            case TargetType.Machine:
                _machine = (Machine)_target;
                break;

            case TargetType.Table:
                _table = (ChargingTable)_target;
                break;

            case TargetType.Park:
                _park = (ChargingPark)_target;
                break;

            case TargetType.Counter:
                _counter = (Counter)_target;
                break;

            case TargetType.ChargingMachine:
                _chargingmachine = (ChargingMachine)_target;
                break;
        }


        //MoneyText = transform.Find("MoneyText").GetComponent<Text>();
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (_targetType)
            {
                case TargetType.Machine:
                    _machine.isPlayerIn = true;
                    break;

                case TargetType.Table:
                    _table.isPlayerIn = true;
                    break;

                case TargetType.Park:

                    break;

                case TargetType.Counter:
                    _counter.isPlayerIn = true;
                    break;

                case TargetType.ChargingMachine:
                    _chargingmachine.isPlayerIn = true;

                    break;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (_targetType)
            {
                case TargetType.Machine:
                    _machine.isPlayerIn = false;
                    break;

                case TargetType.Table:
                    _table.isPlayerIn = false;
                    break;

                case TargetType.Park:

                    break;

                case TargetType.Counter:
                    _counter.isPlayerIn = false;
                    break;

                case TargetType.ChargingMachine:
                    _chargingmachine.isPlayerIn = false;
                    break;
            }
        }
    }


}
