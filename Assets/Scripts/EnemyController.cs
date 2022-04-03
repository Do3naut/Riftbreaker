using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision! Type: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Attack")
            Destroy(this.gameObject);
        else if (collision.gameObject.tag == "Player")
            GameObject.FindObjectOfType<GameManager>().DamagePlayer();
        
    }

}
