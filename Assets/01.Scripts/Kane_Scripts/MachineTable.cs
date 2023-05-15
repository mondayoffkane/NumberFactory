using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using Sirenix.OdinInspector;

public class MachineTable : MonoBehaviour
{

    [ShowInInspector]
    public Stack<Product> ProductStack = new Stack<Product>();
=======

public class MachineTable : MonoBehaviour
{
    public int Max_Count = 10;
    public Stack<Transform> ProductStack = new Stack<Transform>();



    public void SetChange(int _maxCount)
    {
        Max_Count = _maxCount;
    }

>>>>>>> c4d44bd848e11b411eb88f4cca107d57fada4928

}
