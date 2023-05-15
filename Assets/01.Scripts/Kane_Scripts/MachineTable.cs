using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineTable : MonoBehaviour
{
    public int Max_Count = 10;
    public Stack<Transform> ProductStack = new Stack<Transform>();



    public void SetChange(int _maxCount)
    {
        Max_Count = _maxCount;
    }


}
