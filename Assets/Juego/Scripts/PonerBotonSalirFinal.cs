using UnityEngine;
using UnityEngine.SceneManagement;

public class PonerBotonSalirFinal : MonoBehaviour
{
    [SerializeField] private GameObject botonSalir;

    void Start()
    {
        // Solo funciona en la escena Final
        if (SceneManager.GetActiveScene().name != "Final")
            return;

        // Activar el botón si está asignado
        if (botonSalir != null)
        {
            botonSalir.SetActive(true);
        }
    }
}