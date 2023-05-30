using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

public class Generator : MonoBehaviour
{
    public string _objectName = "Generator";
    public Double Num;


    [Range(0.5f, 5f)]
    public float Size;

    public float Interval = 0.1f;


    public Mesh[] NumberMeshes;
    public MeshFilter[] Digit_MeshFilter;
    public Mesh[] Unit_Meshs;


    public Transform Dot_Obj;
    public MeshFilter Unit_Obj;


    [SerializeField] int[] digits = new int[3];

    [SerializeField] int Dot_num = 0;
    [SerializeField] int digit_count = 0;
    public int suffixIndex = 0;
    [SerializeField] GameObject Battery;
    //public Stack<Product> BatteryStack = new Stack<Product>();
    public MachineTable StackPoint;
    // ========= values
    public float Double_Pos_interval = 0.75f;
    public float Third_Pos_interval = 1.5f;
    public float Spawn_Interval = 1f;
    public float Stack_Interval = 0.5f;
    public float Stack_MoveInterval = 0.5f;
    public int Max_Count = 40;
    public float BaseUp_Interval = 0.5f;
    public float BaseSide_Interval = 0.09f;

    float _initY;

    // ====================================
    public bool isSpawn = true;

    private void Start()
    {

        Num = Managers.Data.GetDouble(_objectName + Managers.Game._stagemanager.Stage_Num.ToString());

        transform.DOScaleY(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        Battery = Resources.Load<GameObject>("Battery");
        StartCoroutine(Cor_Update());
        Stack_Interval = Battery.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        BaseSide_Interval = Battery.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * 0.5f;
        _initY = Digit_MeshFilter[0].transform.localPosition.y;

        AbbreviateNumber(Num);
        SetMesh();

        MoneyUi_Update();


    }



    IEnumerator Cor_Update()
    {
        //WaitForSeconds _interval = new WaitForSeconds(Spawn_Interval);
        while (true)
        {
            yield return new WaitForSeconds(Spawn_Interval); //_interval;
            if (isSpawn && Num > 0 && StackPoint.ProductStack.Count < Max_Count)
            {
                Transform _battery = Managers.Pool.Pop(Battery, StackPoint.transform).transform;

                _battery.localScale = Vector3.one;
                _battery.eulerAngles = new Vector3(0f, 45f, 0f);
                _battery.transform.position = transform.position;
                DOTween.Kill(_battery);
                int _count = StackPoint.ProductStack.Count - 1;
                if (_count % 2 == 0)
                {
                    StackPoint.ProductStack.Push(_battery.GetComponent<Product>());
                    _battery.DOJump(StackPoint.transform.position + new Vector3(-BaseSide_Interval * Mathf.Sin(45), BaseUp_Interval + Stack_Interval * (_count / 2), -BaseSide_Interval * Mathf.Sin(45)), 1, 1, Stack_MoveInterval).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            _battery.position = StackPoint.transform.position + new Vector3(-BaseSide_Interval * Mathf.Sin(45), BaseUp_Interval + Stack_Interval * (_count / 2), -BaseSide_Interval * Mathf.Sin(45));
                        });
                }
                else
                {
                    StackPoint.ProductStack.Push(_battery.GetComponent<Product>());
                    _battery.DOJump(StackPoint.transform.position + new Vector3(BaseSide_Interval * Mathf.Sin(45), BaseUp_Interval + Stack_Interval * (_count / 2), BaseSide_Interval * Mathf.Sin(45)), 1, 1, Stack_MoveInterval).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _battery.position = StackPoint.transform.position + new Vector3(BaseSide_Interval * Mathf.Sin(45), BaseUp_Interval + Stack_Interval * (_count / 2), BaseSide_Interval * Mathf.Sin(45));
                    });
                }
            }
        }
    }



    [Button]
    public void SetObj()
    {
        if (NumberMeshes == null)
            NumberMeshes = Resources.LoadAll<Mesh>("NumberMeshes");

        if (Digit_MeshFilter == null)
        {
            Digit_MeshFilter = new MeshFilter[3];
        }

    }



    [Button]
    public void AddNum(int _num)
    {
        Num += _num;
        Managers.Data.SetDouble(_objectName + Managers.Game._stagemanager.Stage_Num.ToString(), Num);
        AbbreviateNumber(Num);
        SetMesh();
        // Change Size Value;
        //  transform.DOScale(Size, Interval).SetEase(Ease.Linear);

    }


    public void SetMesh()
    {
        // 자릿수 별로 메쉬변경 및 위치 정
        for (int i = 0; i < 3; i++)
        {
            if (digits[i] == -1)
            {
                Digit_MeshFilter[i].gameObject.SetActive(false);
            }
            else
            {
                Digit_MeshFilter[i].gameObject.SetActive(true);
                Digit_MeshFilter[i].sharedMesh = NumberMeshes[digits[i]];
            }
        }
        Unit_Obj.sharedMesh = Unit_Meshs[suffixIndex];

        if (Num < 1000)
        {
            switch (digit_count)
            {
                case 0:
                    Digit_MeshFilter[0].transform.localPosition = Vector3.up * _initY;
                    //Unit_Obj.transform.position = Digit_MeshFilter[0].transform.position + Vector3.right * 1.5;
                    break;

                case 1:
                    Digit_MeshFilter[0].transform.localPosition = Vector3.left * Double_Pos_interval + Vector3.up * _initY;
                    Digit_MeshFilter[1].transform.localPosition = Vector3.right * Double_Pos_interval + Vector3.up * _initY;
                    break;

                case 2:
                    Digit_MeshFilter[0].transform.localPosition = Vector3.left * Third_Pos_interval + Vector3.up * _initY;
                    Digit_MeshFilter[1].transform.localPosition = Vector3.zero + Vector3.up * _initY;
                    Digit_MeshFilter[2].transform.localPosition = Vector3.right * Third_Pos_interval + Vector3.up * _initY;
                    //Unit_Obj.transform.position = Digit_MeshFilter[2].transform.position + Vector3.right * (Third_Pos_interval + 0.25f);
                    break;
            }

            Dot_Obj.gameObject.SetActive(false);

        }
        else if (Num > 1000)
        {
            switch (digit_count)
            {
                case 0:
                    Digit_MeshFilter[0].transform.localPosition = Vector3.left * Double_Pos_interval
                       + Vector3.up * _initY;
                    Unit_Obj.transform.localPosition = Vector3.right * Double_Pos_interval + Vector3.up * _initY;
                    break;

                case 1:
                    Digit_MeshFilter[0].transform.localPosition = Vector3.left * Third_Pos_interval + Vector3.up * _initY;
                    Digit_MeshFilter[1].transform.localPosition = Vector3.zero + Vector3.up * _initY;
                    Unit_Obj.transform.localPosition = Vector3.right * Third_Pos_interval + Vector3.up * _initY;
                    break;

                case 2:
                    Digit_MeshFilter[0].transform.localPosition = Vector3.left * Double_Pos_interval * 3 + Vector3.up * _initY;
                    Digit_MeshFilter[1].transform.localPosition = Vector3.left * Double_Pos_interval + Vector3.up * _initY;
                    Digit_MeshFilter[2].transform.localPosition = Vector3.right * Double_Pos_interval + Vector3.up * _initY;
                    Unit_Obj.transform.localPosition = Vector3.right * Double_Pos_interval * 3 + Vector3.up * _initY;
                    break;
            }

            switch (Dot_num)
            {
                case 0:
                    Dot_Obj.gameObject.SetActive(true);
                    Dot_Obj.transform.position = Digit_MeshFilter[0].transform.position + Vector3.right * 0.85f;
                    break;

                case 1:
                    Dot_Obj.gameObject.SetActive(true);
                    Dot_Obj.transform.position = Digit_MeshFilter[1].transform.position + Vector3.right * 0.85f;
                    break;

                case 2:
                    Dot_Obj.gameObject.SetActive(false);
                    break;
            }

        }

    }


    public void AbbreviateNumber(double number)
    {
        // string[] suffixes = new string[] { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc" };
        suffixIndex = 0;

        string numString = number.ToString("F2"); // 소수점 둘째 자리까지 표시
        numString = numString.Replace(".", ""); // 소수점 제거

        digits = new int[3];

        for (int i = 0; i < Math.Min(numString.Length, 3); i++)
        {
            digits[i] = int.Parse(numString[i].ToString());
        }
        digit_count = 2;

        if (number < 100)
        {
            digits[2] = -1;
            digit_count = 1;
        }
        if (number < 10)
        {
            digits[1] = -1;
            digit_count = 0;
        }

        Dot_num = 2;
        if (number >= 1000)
        {
            double a = Math.Log10(number);
            Dot_num = (int)(a % 3);

            while (number >= 1000 /*&& suffixIndex < suffixes.Length - 1*/)
            {
                number /= 1000;
                suffixIndex++;
            }
            // return string.Format("{0:0.00}{1}", number, suffixes[suffixIndex]);
        }
        else
        {
            // return number.ToString();
        }
    }

    public void PushNum(Product _product)
    {
        DOTween.Kill(_product);
        _product.transform.SetParent(transform);
        _product.transform.DOLocalJump(Vector3.zero, 1f, 1, 0.5f)
            .OnComplete(() =>
            {
                AddNum(_product.Number);
                Managers.Pool.Push(_product.GetComponent<Poolable>());
            });

        MoneyUi_Update();

    }

    public void MoneyUi_Update()
    {
        Managers.GameUI.StageGuage_Text.text = $"{Managers.ToCurrencyString(Num)} / {Managers.ToCurrencyString(Managers.Game._stagemanager.Clear_Money)}";

        Managers.GameUI.Stage_Slider.DOFillAmount((float)(Num / Managers.Game._stagemanager.Clear_Money), 0.2f).SetEase(Ease.Linear);

    }
}
