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
        Park
    }
    public TargetType _targetType;

    //public Text MoneyText;

    [SerializeField] Machine _machine;
    [SerializeField] ChargingTable _table;
    [SerializeField] ChargingPark _park;



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

                    break;

                case TargetType.Park:

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

                    break;

                case TargetType.Park:

                    break;
            }
        }
    }


}
