using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookObj : MonoBehaviour
{

    public Transform _target;
    // Start is called before the first frame update
    void Start()
    {
        if (_target == null) _target = Managers.Game._stagemanager._tutorial._arrow;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_target);
    }
}
