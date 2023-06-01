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



    // ===============================================

    private void Start()
    {


        StartCoroutine(Cor_Start());

    }


    IEnumerator Cor_Start()
    {
        yield return null;
        if (Managers.Game._stagemanager._stageData.isFirst)
        {

            //for (int i = 0; i < Lock_Obj.Length; i++)
            //{
            //    for (int j = 0; j < Lock_Obj[i].Length; j++)
            //    {
            //        Lock_Obj[i][j].SetActive(false);
            //    }
            //}


        }
        else
        {
            Debug.Log("not first");
            for (int i = 0; i < Lock_Imgs.Length; i++)
            {
                Lock_Imgs[i].SetActive(false);
            }


        }


    }



    public void LockOff()
    {
        for (int i = 0; i < Lock_Obj[_tutorial_num].Length; i++)
        {
            Lock_Obj[_tutorial_num][i].SetActive(true);

        }



        _tutorial_num++;
        _arrow.transform.position = Lock_Obj[_tutorial_num][0].transform.position;

    }



}

