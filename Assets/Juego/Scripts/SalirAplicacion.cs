using UnityEngine;
using UnityEngine.UI;

public class SalirAplicacion : MonoBehaviour
{
    void Start()
    {
        Button boton = GetComponent<Button>();
        if (boton != null)
            boton.onClick.AddListener(SalirDelJuego);
    }

    public void OnPointerClickXR()
    {
        SalirDelJuego();
    }

    public void SalirDelJuego()
    {
        // Cierra la aplicación compilada
        Application.Quit();

        // Opcional: Esto detiene el modo Play en el Editor de Unity para que puedas probarlo
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Debug.Log("El juego se ha cerrado.");
    }
}
