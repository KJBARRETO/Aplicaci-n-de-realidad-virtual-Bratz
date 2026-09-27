using UnityEngine;

public class MostrarBotonAlColocar : MonoBehaviour
{
    public PlateBehaviour mesa;
    public GameObject boton;

    void Update()
    {
        if (mesa == null || boton == null)
            return;

        bool visible = mesa.heldObject != null;
        if (boton.activeSelf != visible)
            boton.SetActive(visible);
    }
}
