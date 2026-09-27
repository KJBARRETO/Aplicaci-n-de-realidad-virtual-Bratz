using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiElementXR : MonoBehaviour
{
    public UnityEvent  OnXRPointerEnter;
    public UnityEvent OnXRPointerExit;
    private Camera xRCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CameraPointerManager.Instance != null)
            xRCamera = CameraPointerManager.Instance.GetComponent<Camera>();
        StartCoroutine(AjustarCaja());
    }

    IEnumerator AjustarCaja()
    {
        yield return null;
        RectTransform rect = GetComponent<RectTransform>();
        BoxCollider caja = GetComponent<BoxCollider>();
        if (rect == null || caja == null)
            yield break;

        Vector2 tamano = rect.rect.size;
        if (tamano.x < 1f || tamano.y < 1f)
            tamano = rect.sizeDelta;
        if (tamano.x < 1f || tamano.y < 1f)
            yield break;

        caja.size = new Vector3(Mathf.Abs(tamano.x), Mathf.Abs(tamano.y), 20f);
        caja.center = Vector3.zero;
    }

   public void OnPointerClickXR(){
        if (EventSystem.current == null)
        {
            Button boton = GetComponent<Button>();
            if (boton != null && boton.IsActive() && boton.IsInteractable())
                boton.onClick.Invoke();
            return;
        }
        PointerEventData pointerEvent = PlacePointer();
        ExecuteEvents.Execute(this.gameObject,pointerEvent,ExecuteEvents.pointerClickHandler);
   }
   public void OnPointerEnterXR(){
    if (GazeManager.Instance != null)
        GazeManager.Instance.SetUpGaze(1.5f);
    OnXRPointerEnter?.Invoke();
    PointerEventData pointerEvent = PlacePointer();
    if (pointerEvent == null)
        return;
    ExecuteEvents.Execute(this.gameObject,pointerEvent,ExecuteEvents.pointerDownHandler);
   }
   public void OnPointerExitXR(){
    if (GazeManager.Instance != null)
        GazeManager.Instance.SetUpGaze(2.5f);
    OnXRPointerExit?.Invoke();
    PointerEventData pointerEvent = PlacePointer();
    if (pointerEvent == null)
        return;
    ExecuteEvents.Execute(this.gameObject,pointerEvent,ExecuteEvents.pointerUpHandler);
   }
   private PointerEventData PlacePointer(){
    if (EventSystem.current == null || CameraPointerManager.Instance == null)
        return null;
    if (xRCamera == null)
        xRCamera = CameraPointerManager.Instance.GetComponent<Camera>();
    if (xRCamera == null)
        return null;
    Vector3 screenPos = xRCamera.WorldToScreenPoint(CameraPointerManager.Instance.hitPoint);
    var pointer = new PointerEventData(EventSystem.current);
    pointer.position = new Vector2 (screenPos.x, screenPos.y);
    return pointer;
   }
}
