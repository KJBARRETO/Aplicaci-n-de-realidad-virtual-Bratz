using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Benjathemaker
{
    public class SimpleGemsAnim : MonoBehaviour
    {
        public bool isRotating = false;
        public bool rotateX = false;
        public bool rotateY = false;
        public bool rotateZ = false;
        public float rotationSpeed = 90f; // Degrees per second

        public bool isFloating = false;
        public bool useEasingForFloating = false; // Separate toggle for floating ease
        public float floatHeight = 1f; // Max height displacement
        public float floatSpeed = 1f;
        private Vector3 initialPosition;
        private float floatTimer;

        private Vector3 initialScale;
        public Vector3 startScale;
        public Vector3 endScale;

        public bool isScaling = false;
        public bool useEasingForScaling = false; // Separate toggle for scaling ease
        public float scaleLerpSpeed = 1f; // Speed of scaling transition
        private float scaleTimer;
        GrabManager grabManager;
        Vector3 spawnPosition;
        Vector3 escalaGuardada;
        int capaGuardada;
        bool estabaAgarrado;
        bool flotaEnSitio;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void PrepararEscena()
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            SceneManager.sceneLoaded += AlCargarEscena;
        }

        static void AlCargarEscena(Scene escena, LoadSceneMode modo)
        {
            GameObject final = GameObject.Find("Final");
            if (final != null && final.GetComponent<SpriteRenderer>() != null && final.GetComponent<SimpleGemsAnim>() == null)
                final.AddComponent<SimpleGemsAnim>();
        }

        void Start()
        {
            flotaEnSitio = gameObject.name == "Baile" || gameObject.name == "Final";
            if (flotaEnSitio)
            {
                isFloating = true;
                useEasingForFloating = true;
                floatHeight = 0.35f;
                floatSpeed = 1.4f;
                isRotating = false;
            }

            initialScale = transform.localScale;
            initialPosition = transform.position;
            spawnPosition = initialPosition;

            // Adjust start and end scale based on initial scale
            startScale = initialScale;
            endScale = initialScale * (endScale.magnitude / startScale.magnitude);

            GameObject manager = GameObject.Find("GrabManager");
            if (manager != null)
                grabManager = manager.GetComponent<GrabManager>();
        }

        void Update()
        {
            bool agarrado = grabManager != null && grabManager.heldItem == gameObject;
            if (agarrado)
            {
                if (!estabaAgarrado)
                {
                    escalaGuardada = transform.localScale;
                    AchicarParaLaMano();
                }
                estabaAgarrado = true;
                return;
            }

            if (estabaAgarrado)
            {
                estabaAgarrado = false;
                transform.localScale = escalaGuardada;
                initialPosition = transform.position;
                CambiarCapa(capaGuardada);
                CambiarColliders(true);
            }

            if (!flotaEnSitio && Vector3.Distance(transform.position, spawnPosition) > 0.75f)
                return;

            if (isRotating)
            {
                Vector3 rotationVector = new Vector3(
                    rotateX ? 1 : 0,
                    rotateY ? 1 : 0,
                    rotateZ ? 1 : 0
                );
                transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
            }

            if (isFloating)
            {
                floatTimer += Time.deltaTime * floatSpeed;
                float t = Mathf.PingPong(floatTimer, 1f);
                if (useEasingForFloating) t = EaseInOutQuad(t);

                transform.position = initialPosition + new Vector3(0, t * floatHeight, 0);
            }

            if (isScaling)
            {
                scaleTimer += Time.deltaTime * scaleLerpSpeed;
                float t = Mathf.PingPong(scaleTimer, 1f); // Oscillates between 0 and 1

                if (useEasingForScaling)
                {
                    t = EaseInOutQuad(t);
                }

                transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }
        }

        void LateUpdate()
        {
            if (grabManager == null || grabManager.heldItem != gameObject)
                return;

            Camera camara = Camera.main;
            if (camara == null)
                return;

            transform.position = camara.transform.position
                + camara.transform.forward * 0.85f
                + camara.transform.right * 0.42f
                - camara.transform.up * 0.32f;
        }

        void AchicarParaLaMano()
        {
            capaGuardada = gameObject.layer;
            CambiarCapa(2);
            CambiarColliders(false);

            Renderer malla = GetComponentInChildren<Renderer>();
            if (malla == null)
                return;

            float tamano = malla.bounds.extents.magnitude;
            if (tamano <= 0.25f)
                return;

            transform.localScale = escalaGuardada * (0.25f / tamano);
        }

        void CambiarCapa(int capa)
        {
            Transform[] partes = GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < partes.Length; i++)
                partes[i].gameObject.layer = capa;
        }

        void CambiarColliders(bool activos)
        {
            Collider[] cajas = GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < cajas.Length; i++)
                cajas[i].enabled = activos;
        }

        float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }
    }
}

