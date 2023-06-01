using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.IO;

public class StartMoney : MonoBehaviour
{
    public GameObject Money_Pref;
    public double Price = 500d;
    public Stack<Transform> MoneyStack = new Stack<Transform>();
    // ===== Values
    public float Stack_Interval = 0.2f;
    public float Update_Interval = 1f;
    public float BaseUp_Interval = 0.5f;

    public int width = 5, height = 3;
    public float Left = 0.5f;
    public float back = 0.2f;
    public float horizontal_interval = 0.3f;
    public float vertical_inteval = 0.6f;
    public float height_interval = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        //if(Managers.Data.)
        GetComponent<Renderer>().enabled = false;
        if (Money_Pref == null) Money_Pref = Resources.Load<GameObject>("Money_Pref");

        if (Managers.Game.Money == 0)
        {
            SpawnMoney(200);

        }

    }

    [Button]
    public void SpawnMoney(int _popCount = 100)
    {
        for (int i = 0; i < _popCount; i++)
        {
            Transform _momey = Managers.Pool.Pop(Money_Pref).transform;
            MoneyStack.Push(_momey);
            int _count = MoneyStack.Count;
            _momey.transform.SetParent(transform);
            _momey.eulerAngles = new Vector3(0f, 45f, 0f);
            _momey.transform.localPosition = new Vector3(
                -Left + horizontal_interval * ((_count - 1) % width)
                , height_interval * (_count / (width * height))
                , -back + vertical_inteval * (int)(((_count - 1) % (width * height)) / width));
        }
        //if (BatteryStack.Count > 0)
        //{
        //    Managers.Pool.Push(BatteryStack.Pop().GetComponent<Poolable>());

        //}
    }
}
