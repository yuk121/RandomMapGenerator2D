using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private GameObject _bomb;
    [SerializeField] private GameObject _bombEllipse;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Instantiate(_bomb, mousePos, Quaternion.identity);
        }
        if(Input.GetMouseButtonDown(2))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Instantiate(_bombEllipse, mousePos, Quaternion.identity);
        }
    }
}
