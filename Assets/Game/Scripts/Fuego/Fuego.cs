using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Video;

public class Fuego : MonoBehaviour
{
    [SerializeField] private GameObject fire;
    [SerializeField] private GameObject smoke;

    public bool baldasoDeAgua = false;

    public bool extingido;

    private int vidaMax = 100;

    public int vida;
 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        extingido = false;

        vida = vidaMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (extingido) return;
        fire.SetActive(true);
        smoke.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (extingido) return;

        if (collision.CompareTag("Water"))
        {
            if (extingido) return;

            vida -= 20;

            fire.SetActive(false);
            smoke.SetActive(true);

            if (vida <= 0)
            {
                Extingido();
                return;
            }

        }

    }

    void Extingido()
    {
        if (extingido) return;

        extingido = true;

        //animator.SetTrigger("Morir");
        smoke.SetActive(false);

        Destroy(gameObject, 3f);

    }
}