using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MachineTable : MonoBehaviour
{

    [ShowInInspector]
    public Stack<Product> ProductStack = new Stack<Product>();

}
