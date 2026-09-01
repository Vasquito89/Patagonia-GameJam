using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Guru.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> targetTransforms = new List<Transform>();

        [SerializeField] private GameObject playerPrefab;

        private void Start()
        {
            SpawnPlayer();
        }

        void SpawnPlayer()
        {
            Vector3 playerInstate = new Vector3(0.273926f, 3f, 185.111f);
            Quaternion playerRotate = Quaternion.Euler(0f, 125.247f, 0f);

            // Instancia el clon directamente con la posici�n y rotaci�n correctas
            Instantiate(playerPrefab, playerInstate, playerRotate);
        }

    }
}