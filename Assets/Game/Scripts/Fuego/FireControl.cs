using UnityEngine;

public class FireControl : MonoBehaviour
{
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject fire;

    private void Start()
    {
        fire.SetActive (false);
    }

    private void OnTriggerEnter(Collider colission)
    {
        if(colission.CompareTag("Player"))
        {
            fire.SetActive(true);
            Destroy(gameObject);
        }
    }
}
