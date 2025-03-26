using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _explosionRadius.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Player"))
            return;

        _explosionRadius.SetActive(true);
        Destroy(gameObject,0.02f);
    }

}
