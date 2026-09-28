using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PonerBotonSalirFinal
{
    static PonerBotonSalirFinal()
    {
        EditorApplication.delayCall += CrearSiFalta;
    }

    static void CrearSiFalta()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        Scene escena = SceneManager.GetActiveScene();
        if (!escena.isLoaded || escena.name != "Final")
            return;
        if (GameObject.Find("SALIR") != null)
            return;

        Vector3 posicion = new Vector3(0f, 1.5f, 2f);
        Quaternion rotacion = Quaternion.identity;
        SceneView vista = SceneView.lastActiveSceneView;
        if (vista != null)
        {
            posicion = vista.pivot;
            Vector3 haciaCamara = vista.camera.transform.position - posicion;
            if (haciaCamara.sqrMagnitude > 0.01f)
                rotacion = Quaternion.LookRotation(haciaCamara);
        }

        GameObject boton = BotonSalirFinal.Crear(posicion, rotacion);
        if (boton == null)
            return;

        Undo.RegisterCreatedObjectUndo(boton, "Crear SALIR");
        EditorSceneManager.MarkSceneDirty(escena);
        Selection.activeGameObject = boton;
    }
}
