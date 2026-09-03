using Goru.Controller;
using Goru.Core;
using UnityEngine;

public class Fruit : MonoBehaviour, IInteractable
{
    [SerializeField] private float time;
    [SerializeField] private float cantidad;
    private string mensajePrompt = "Presiona [X] para comer la fruta";
    public string GetInteractPrompt() => mensajePrompt;


    public void Interact(PlayerController player)
    {
        // Validamos si el jugador tiene agua lista para usar
        if (player.estaCercaDeFruta)
        {
            player.Eat(cantidad, time); // comer y dar tiempo
            Destroy(gameObject);
        }
    }
}
