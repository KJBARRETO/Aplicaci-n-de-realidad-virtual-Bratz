using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    // solo necesitamos un metodo propio para cambiar la escena
    // el nombre del metodo debe ser diferente a la clase, sino, no aparece la variable int en el Inspector del btn de cambio escena
    public void CambioEscene (int numeroEscena)
    {
        SceneManager.LoadScene(numeroEscena); // lo principal para cambiar la escena
        // en onClick del boton, se asigna la escena por numeros --> revisar las escenas en Build Profiles
    }
}
// ESTE SCRIPT SE PUEDE REUTILIZAR
