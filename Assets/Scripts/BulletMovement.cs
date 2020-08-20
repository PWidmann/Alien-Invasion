using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private Vector3 movement = Vector3.zero;

    public Vector3 Movement { get => movement; set => movement = value; }

    void Update()
    {
        transform.Translate(Movement * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerCharacterController.Instance.TakeDamage(2);    
        }

        Destroy(gameObject);
    }
}
