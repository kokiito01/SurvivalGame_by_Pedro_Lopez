//using UnityEngine;

//public class DayNightCycle : MonoBehaviour
//{
//    [SerializeField] private Light directionalLight;
//    [SerializeField] private Transform sunPivot; // Un punto alrededor del cual el sol girar�
//    [SerializeField] private float dayDuration = 120f; // Duraci�n de un d�a completo en segundos

//    private float rotationSpeed;

//    private void Start()
//    {
//        // Calcula la velocidad de rotaci�n necesaria para un ciclo de d�a completo
//        rotationSpeed = 360f / dayDuration;
//    }

//    private void Update()
//    {
//        // Rota la luz direccional alrededor del punto pivot (sol)
//        sunPivot.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
//    }
//}