//using UnityEngine;

//public class DayNightCycle : MonoBehaviour
//{
//    [SerializeField] private Light directionalLight;
//    [SerializeField] private Transform sunPivot; // Un punto alrededor del cual el sol girará
//    [SerializeField] private float dayDuration = 120f; // Duración de un día completo en segundos

//    private float rotationSpeed;

//    private void Start()
//    {
//        // Calcula la velocidad de rotación necesaria para un ciclo de día completo
//        rotationSpeed = 360f / dayDuration;
//    }

//    private void Update()
//    {
//        // Rota la luz direccional alrededor del punto pivot (sol)
//        sunPivot.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
//    }
//}