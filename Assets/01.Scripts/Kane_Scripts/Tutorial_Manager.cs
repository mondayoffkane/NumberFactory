using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class Tutorial_Manager : MonoBehaviour
{
    public Transform _arrow;
    public int _tutorial_num = 0;
    //public bool[] boolCheck;

    public GameObject[] Lock_Imgs;
    [ShowInInspector]
    public GameObject[][] Lock_Obj;


    [SerializeField] StageManager _stageManager;

    public GameObject[] _cineCams;


    // ===============================================



    public void SetTutorial(bool isfirst, StageManager _manager)
    {
        _stageManager = _manager;

        for (int i = 0; i < _cineCams.Length; i++)
        {
            _cineCams[i].SetActive(false);
        }

        if (isfirst)
        {
            _arrow.position = new Vector3(22.25f, 0f, -25.51f);
            _stageManager._counter.gameObject.SetActive(false);
            _stageManager.Machines[0].gameObject.SetActive(false);
            _stageManager._chargingMachine.gameObject.SetActive(false);
            _stageManager._generatorGroup.SetActive(false);
            _stageManager._playerHR.SetActive(false);
            _stageManager._staffHR.SetActive(false);

            foreach (ChargingTable _table in _stageManager.Tables)
            {
                _table.gameObject.SetActive(false);
            }


            for (int i = 0; i < Lock_Imgs.Length; i++)
            {
                Lock_Imgs[i].SetActive(true);
            }




        }
        else
        {
            _arrow.gameObject.SetActive(false);
            for (int i = 0; i < Lock_Imgs.Length; i++)
            {
                Lock_Imgs[i].SetActive(false);
            }
        }
    }



    public void LockOff()
    {
        switch (_tutorial_num)
        {
            case 0: // get start money => unlock machine
                Lock_Imgs[0].SetActive(false);
                Lock_Imgs[1].SetActive(false);
                _arrow.position = _stageManager.Machines[0]._interactArea.transform.position;
                _stageManager.Machines[0].gameObject.SetActive(true);

                CamOn(0, 1f);

                break;

            case 1: // Open machine => generator open
                _stageManager._generatorGroup.SetActive(true);
                _arrow.position = _stageManager.Machines[0].MachineTable.transform.position;

                _stageManager.Machines[0]._interactArea.gameObject.SetActive(false);
                _stageManager.Machines[1]._interactArea.gameObject.SetActive(false);

                CamOn(1);

                break;

            case 2: // push to generator => unlock charging machin
                _stageManager._chargingMachine.gameObject.SetActive(true);
                _arrow.position = _stageManager._chargingMachine.StackPoint.transform.position;
                Lock_Imgs[2].SetActive(false);


                CamOn(2);


                break;

            case 3: // open chargingmachine => enter customer car
                _arrow.position = _stageManager._generator.StackPoint.transform.position;
                CamOn(3, 0.5f, 3f);

                break;

            case 4: // push to chargingmacine => unlock counter
                _stageManager._counter.gameObject.SetActive(true);

                _arrow.position = _stageManager._counter._interactArea.transform.position;
                CamOn(4);


                break;

            case 5: // open counter => unlock tables1                
                Lock_Imgs[3].SetActive(false);
                _stageManager.Tables[1].gameObject.SetActive(true);

                _arrow.position = _stageManager._generator.StackPoint.transform.position;

                break;

            case 6:
                _arrow.position = _stageManager.Tables[1].transform.position;

                Lock_Imgs[4].SetActive(false);

                break;

            case 7: // open table 1 => unlock Player HR

                _arrow.position = _stageManager._staffHR.transform.position;
                _stageManager._staffHR.SetActive(true);
                Lock_Imgs[5].SetActive(false);
                CamOn(5);


                break;


            case 8:
                _arrow.gameObject.SetActive(true);
                _arrow.position = _stageManager._playerHR.transform.position;
                _stageManager._playerHR.SetActive(true);
                Lock_Imgs[6].SetActive(false);
                break;

            case 9:

                foreach (ChargingTable _table in _stageManager.Tables)
                {
                    _table.gameObject.SetActive(true);
                }
                _stageManager.Machines[0]._interactArea.gameObject.SetActive(true);
                _stageManager.Machines[1]._interactArea.gameObject.SetActive(true);

                /// add all unlock

                _stageManager._counter.SaveData();
                _stageManager._chargingMachine.SaveData();
                _stageManager.Machines[0].SaveData();
                _stageManager.Tables[1].SaveData();

                // all save data
                CamOn(6);
                _arrow.gameObject.SetActive(false);

                _stageManager._stageData.isFirst = false;
                _stageManager.SaveData();

                break;
        }
        _tutorial_num++;

    }

    public void CamOn(int _num, float _delay = 0.5f, float _interval = 2f)
    {
        StartCoroutine(Cor_Cam());

        IEnumerator Cor_Cam()
        {
            yield return new WaitForSeconds(_delay);
            _cineCams[_num].SetActive(true);

            yield return new WaitForSeconds(_interval);
            _cineCams[_num].SetActive(false);
        }
    }



}

