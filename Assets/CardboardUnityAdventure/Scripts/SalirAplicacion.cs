using UnityEngine;

public class SalirAplicacion : MonoBehaviour
{
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
