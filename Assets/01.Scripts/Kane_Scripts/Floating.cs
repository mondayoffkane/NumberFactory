using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{

    public Vector3 _angle = new Vector3(45f, 0f, 0f);

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = _angle;
    }
}
