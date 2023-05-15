using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Repository : MonoBehaviour
{

    public Transform[] Reposit;

    [SerializeField]
    public Stack<Transform>[] StackProducts;



    private void Start()
    {
        StackProducts = new Stack<Transform>[System.Enum.GetValues(typeof(Product.ProductType)).Length];

        int _num = transform.childCount;
        Reposit = new Transform[_num];
        for (int i = 0; i < _num; i++)
        {
            Reposit[i] = transform.GetChild(i);
        }



    }

    public void PushProduct(Transform _product)
    {
        Product.ProductType _type = _product.GetComponent<Product>()._productType;
        StackProducts[(int)_type].Push(_product);

        _product.transform.SetParent(Reposit[(int)_type]);
        _product.DOLocalJump(Vector3.up * StackProducts[(int)_type].Count, 1, 1, 0.5f);





    }

    public void PopProduct()
    {

    }

}
