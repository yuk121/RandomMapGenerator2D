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

            GameObject go = PoolManager.Instance.Pop(_bomb);

            if (go == null)
            {
                go = Instantiate(_bomb, mousePos, Quaternion.identity);
              
                PoolManager.Instance.Push(go);
            }

            go.transform.position = mousePos;
            Bomb bomb = go.GetComponent<Bomb>();
            bomb.Init();
        }
        if (Input.GetMouseButtonDown(2))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject go = PoolManager.Instance.Pop(_bombEllipse.gameObject);
            if (go == null)
            {
                go = Instantiate(_bombEllipse, mousePos, Quaternion.identity);
                PoolManager.Instance.Push(go);
            }

            go.transform.position = mousePos;
            BombEllipse bomb = go.GetComponent<BombEllipse>();
            bomb.Init();
        }
    }
}
