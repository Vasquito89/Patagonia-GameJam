using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Guru.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> targetTransforms = new List<Transform>();

        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject blueberryPrefab;
        [SerializeField] private GameObject strawberryPrefab;

        private void Start()
        {
            SpawnPlayer();
            SpawnFruit();
        }

        void SpawnPlayer()
        {
            Vector3 playerInstate = new Vector3(0.273926f, 3f, 185.111f);
            Quaternion playerRotate = Quaternion.Euler(0f, 125.247f, 0f);

            // Instancia el clon directamente con la posici�n y rotaci�n correctas
            Instantiate(playerPrefab, playerInstate, playerRotate);
        }
        void SpawnFruit()
        {
            if (targetTransforms.Count < 20)
            {
                Debug.LogWarning("Se necesitan al menos 20 puntos de aparición en la lista.");
                return;
            }

            // Crear una copia de la lista para mezclar las posiciones
            List<Transform> availablePoints = new List<Transform>(targetTransforms);

            // Algoritmo de mezcla (Fisher-Yates) para aleatorizar las posiciones
            for (int i = availablePoints.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                Transform temp = availablePoints[i];
                availablePoints[i] = availablePoints[randomIndex];
                availablePoints[randomIndex] = temp;
            }

            // Instanciar 10 frutas de la variedad A
            for (int i = 0; i < 10; i++)
            {
                Instantiate(blueberryPrefab, availablePoints[i].position, availablePoints[i].rotation);
            }

            // Instanciar 10 frutas de la variedad B en las siguientes posiciones
            for (int i = 10; i < 20; i++)
            {
                Instantiate(strawberryPrefab, availablePoints[i].position, availablePoints[i].rotation);
            }
        
        }

    }
}