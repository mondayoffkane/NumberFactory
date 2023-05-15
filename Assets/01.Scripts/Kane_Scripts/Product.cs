using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour
{
    //public int ProductNum = 0;



    MeshFilter _meshfilter;


    public enum ProductType
    {
<<<<<<< HEAD
        Stone,
        Number,
        Battery

    }
    public ProductType _productType;
    public int Number = 0;

    public Mesh[] NumberMeshes;

    //public void SetMeshes()
    //{
    //    NumberMeshes = new Mesh[10];
    //}

    public void Setproduct(Mesh _mesh, ProductType _type, int _num = 1)
=======
        Burger,
        Coke,
        Hotdog,
        Nugget

    }
    public ProductType _productType;




    public void Setproduct(Mesh _mesh, ProductType _type)
>>>>>>> c4d44bd848e11b411eb88f4cca107d57fada4928
    {
        _meshfilter = transform.GetComponent<MeshFilter>();
        _meshfilter.sharedMesh = _mesh;
        _productType = _type;
<<<<<<< HEAD
        Number = _num;

    }

    public void SetNum(int _num)
    {
        Number = _num;
        _meshfilter.sharedMesh = NumberMeshes[Number];
        _productType = ProductType.Number;
    }

=======

    }
>>>>>>> c4d44bd848e11b411eb88f4cca107d57fada4928
}
