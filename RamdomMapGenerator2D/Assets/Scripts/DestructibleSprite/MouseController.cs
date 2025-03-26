using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private GameObject _bomb;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Instantiate(_bomb, mousePos, Quaternion.identity);
        }
    }
}
