using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObj_NonRot : MonoBehaviour
{
    public Transform Player;

    [SerializeField] Vector3 Offset;
    public bool isInit = false;
    // Start is called before the first frame update
    public void Start()
    {
        if (Player == null) Player = GameObject.FindGameObjectWithTag("Player").transform;

        if (isInit == false)
        {
            isInit = true;
            Offset = transform.position - Player.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.position + Offset;
    }
}
