using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    GrabManager grabManager;
    BoxCollider boxCollider;
    Vector3 spawnerPosition;
    Quaternion spawnerRotation;

    public AudioClip soundGrab;
    public AudioClip soundPlace;

    [SerializeField] public string type = "Objeto";
    [SerializeField] public GameObject spawner;

    private AudioSource player;

    void Start()
    {
        player = GetComponent<AudioSource>();
        spawnerPosition = spawner.transform.position;
        spawnerRotation = spawner.transform.rotation;
        boxCollider = GetComponent<BoxCollider>();
        grabManager = GameObject.Find("GrabManager").GetComponent<GrabManager>();
    }

    public void Grab()
    //-----------------------------------------------------------------------//
    //NOTA: La capsula para el GrabObject, la que se interactua, debe tener un BoxCollider y un Audio Source con sonidos
    // La capsula con el script GrabObject, el Spawn Cylinder en un Empy, son hijos de un Empy llamado Objetos Agarrables
    //El script GrabManager esta en un Empy
    //-----------------------------------------------------------------------//
    {
        if (soundGrab != null)
            player.PlayOneShot(soundGrab);
        if (grabManager.heldItem != null)
        {
            grabManager.heldItem.GetComponent<GrabObject>().Drop();
        }
        grabManager.heldItem = transform.gameObject;
        boxCollider.enabled = false;
    }

    public void Drop()
    {
        transform.position = spawnerPosition;
        transform.rotation = spawnerRotation;
        grabManager.heldItem = null;
        boxCollider.enabled = true;
    }

    public void Delete()
    {
        transform.position = spawnerPosition;
        transform.rotation = spawnerRotation;
        grabManager.heldItem = null;
        boxCollider.enabled = true;
        transform.gameObject.SetActive(false);
    }

    public void Respawn()
    {
        transform.position = spawnerPosition;
        transform.rotation = spawnerRotation;
        boxCollider.enabled = true;
        transform.gameObject.SetActive(true);
    }

    public void Place(Vector3 position)
    {
        if (soundPlace != null)
            player.PlayOneShot(soundPlace);
        transform.position = position;
        grabManager.heldItem = null;
        boxCollider.enabled = true;
    }

    public void OnPointerClickXR()
    {
        Grab();
    }
}
