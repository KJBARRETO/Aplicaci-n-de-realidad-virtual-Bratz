using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    public GameObject[] interactables;
    public List<GameObject> objetosA; // se cambio el nombre por objetosA
    public List<GameObject> products;

    public GameObject heldItem;
    void Start()
    //-----------------------------------------------------------------------//
    //NOTA: La capsula para el GrabObject, la que se interactua, debe tener un BoxCollider y un Audio Source con sonidos
    // La capsula con el script GrabObject, el Spawn Cylinder en un Empy, son hijos de un Empy llamado Objetos Agarrables
    //El script GrabManager esta en un Empy
    //-----------------------------------------------------------------------//
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject item in interactables)
        {
            if (item.GetComponent<GrabObject>())
            {
                objetosA.Add(item);
                if (!item.GetComponent<GrabObject>().type.Equals("Objeto"))
                {
                    products.Add(item);
                    item.SetActive(false);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (heldItem != null)
        {
            GameObject pointer = GameObject.Find("GazePointer");
            Vector3 pointerPos = pointer.transform.position;
             // si en el caso se genera un error al agarrar un objeto, tenemos que sumar la posPointer en Y
            heldItem.transform.position = new Vector3(pointerPos.x, pointerPos.y + 1f, pointerPos.z);
        }
    }
}
