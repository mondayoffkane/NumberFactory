using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;


public class Product : MonoBehaviour
{
    //public int ProductNum = 0;



    MeshFilter _meshfilter;


    public enum ProductType
    {
        Stone,
        Number,
        Battery

    }
    public ProductType _productType;
    public int Number = 0;

    public Mesh[] NumberMeshes;

    [SerializeField] int[] digits = new int[3];
    [SerializeField] int digit_count = 0;
    public MeshFilter[] Digit_MeshFilter;

    public float Double_Pos_interval = 0.3f;
    public float Third_Pos_interval = 0.55f;

    //public void SetMeshes()
    //{
    //    NumberMeshes = new Mesh[10];
    //}

    [Button]
    public void Setproduct(Mesh _mesh, ProductType _type, int _num = 1)
    {
        _meshfilter = transform.GetComponent<MeshFilter>();
        _meshfilter.sharedMesh = _mesh;
        _productType = _type;
        Number = _num;
        if (_type == ProductType.Number)
        {
            GetComponent<Renderer>().enabled = false;
        }
        else
        {
            GetComponent<Renderer>().enabled = true;
            Digit_MeshFilter[0].gameObject.SetActive(false);
        }
        SetNum(Number);
    }

    public void SetNum(int _num)
    {
        Number = _num;

        if (Number > 999)
        {
            Number = 999;
        }
        //_meshfilter.sharedMesh = NumberMeshes[Number];
        _productType = ProductType.Number;

        Digit_MeshFilter[0].gameObject.SetActive(true);
        string numString = Number.ToString("F0");



        for (int i = 0; i < Math.Min(numString.Length, 3); i++)
        {
            digits[i] = int.Parse(numString[i].ToString());
        }

        digit_count = 2;

        if (Number < 100)
        {
            digits[2] = -1;
            digit_count = 1;
        }
        if (Number < 10)
        {
            digits[1] = -1;
            digit_count = 0;
        }


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

        switch (digit_count)
        {
            case 0:
                Digit_MeshFilter[0].transform.localPosition = Vector3.zero;
                break;

            case 1:
                Digit_MeshFilter[0].transform.localPosition = Vector3.back * Double_Pos_interval;
                Digit_MeshFilter[1].transform.localPosition = Vector3.forward * Double_Pos_interval;
                break;

            case 2:
                Digit_MeshFilter[0].transform.localPosition = Vector3.back * Third_Pos_interval;
                Digit_MeshFilter[1].transform.localPosition = Vector3.zero;
                Digit_MeshFilter[2].transform.localPosition = Vector3.forward * Third_Pos_interval;

                break;
        }

    }

}
