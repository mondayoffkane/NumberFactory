using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour
{
    //public int ProductNum = 0;



    MeshFilter _meshfilter;


    public enum ProductType
    {
        Burger,
        Coke,
        Hotdog,
        Nugget

    }
    public ProductType _productType;




    public void Setproduct(Mesh _mesh, ProductType _type)
    {
        _meshfilter = transform.GetComponent<MeshFilter>();
        _meshfilter.sharedMesh = _mesh;
        _productType = _type;

    }
}
