using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    private BoxCollider2D _coliderBox2D = null;
    private Rigidbody2D _rb2D = null;
    private float _dirX = 0;
    private bool _canMove = false;
    int _rayCheckCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _coliderBox2D = GetComponent<BoxCollider2D>();
        _canMove = true;
    }

    // Update is called once per frame
 
    private void FixedUpdate()
    {
        Vector3 ray1 = new Vector3((transform.position.x + _coliderBox2D.size.x / 2f), transform.position.y - _coliderBox2D.size.y / 2f);
        Vector2 ray2 = new Vector2(transform.position.x, transform.position.y - _coliderBox2D.size.y / 2f);
        Vector3 ray3 = new Vector3((transform.position.x - _coliderBox2D.size.x / 2f), transform.position.y - _coliderBox2D.size.y / 2f);
        Debug.DrawRay(ray1, Vector2.down * 0.3f, Color.red);
        Debug.DrawRay(ray2, Vector2.down * 0.3f, Color.red);
        Debug.DrawRay(ray3, Vector2.down * 0.3f, Color.red);

        // 움직일 수 있다면
        if (_canMove)
        {
            _dirX = Input.GetAxis("Horizontal"); // 좌우 입력

            if (_dirX != 0)
            {
                transform.right = new Vector3(_dirX, 0, 0);
                _rb2D.MovePosition(transform.position + new Vector3(_dirX * _speed * Time.fixedDeltaTime,0,0));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _canMove = IsGround() ? true : false;
            return;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _canMove = IsGround() ? true : false;
            return;
        }
    }

    
    private bool IsGround()
    {
        Vector3 ray1 = new Vector3((transform.position.x + _coliderBox2D.size.x / 2f), transform.position.y - _coliderBox2D.size.y / 2f);
        Vector2 ray2 = new Vector2(transform.position.x, transform.position.y - _coliderBox2D.size.y / 2f);
        Vector3 ray3 = new Vector3((transform.position.x - _coliderBox2D.size.x / 2f), transform.position.y - _coliderBox2D.size.y / 2f);

        _rayCheckCount = 0;

        if (Physics2D.Raycast(ray1, Vector2.down, 0.3f, 1 << LayerMask.NameToLayer("Ground")))
            _rayCheckCount++;

        if (Physics2D.Raycast(ray2, Vector2.down, 0.3f, 1 << LayerMask.NameToLayer("Ground")))
            _rayCheckCount++;

        if (Physics2D.Raycast(ray3, Vector2.down, 0.3f, 1 << LayerMask.NameToLayer("Ground")))
            _rayCheckCount++;

        return _rayCheckCount > 0;
    }
}
