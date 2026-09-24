using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportPoint : MonoBehaviour
{
    
    public UnityEvent OnTeleportEnter;
    public UnityEvent OnTeleport;
    public UnityEvent OnTeleportExit;
    public Transform destination;

    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    } 

    public void OnPointerEnterXR()
    {
        
        OnTeleportEnter?.Invoke();
        
    }

    public void OnPointerClickXR()
    {
        ExecuteTeleportation();
        OnTeleport?.Invoke();
        TeleportManager.Instance.DisableTeleportPoint(gameObject);
    }

    public void OnPointerExitXR()
    {
        OnTeleportExit?.Invoke();
    }

    private void ExecuteTeleportation()
    {
        GameObject player = TeleportManager.Instance.Player;
        Transform target = destination != null ? destination : transform;
        player.transform.position = target.position;
        Camera camera = player.GetComponentInChildren<Camera>();
        float rotY = target.rotation.eulerAngles.y - camera.transform.localEulerAngles.y;
        player.transform.rotation = Quaternion.Euler(0, rotY, 0);
    }



}

