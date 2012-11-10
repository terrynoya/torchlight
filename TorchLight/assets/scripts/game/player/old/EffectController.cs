using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour {

    public float MaxLife            = 50.0f;
    public float MoveSpeed          = 15.0f;
    public bool EnableRayCast       = true;
    public float CollisionRadius    = 0.5f;

    private Vector3 Direction;

    private float CurLife = 0.0f;

    public void SetDiretion(Vector3 InDirection)
    {
        Direction = InDirection.normalized;
        Direction.y = 0.0f;
    }
	
	// Update is called once per frame
	void Update () 
    {
        transform.position += Direction * Time.deltaTime * MoveSpeed;

        CurLife += Time.deltaTime;

        if (CheckCollision())
        {
            CheckLifeTime();
        }
	}

    bool CheckCollision()
    {
        if (Physics.Raycast(transform.position, Direction, CollisionRadius))
        {
            Destroy(gameObject);
            return false;
        }

        return true;
    }

    void CheckLifeTime()
    {
        if (CurLife > MaxLife)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
    }
}
