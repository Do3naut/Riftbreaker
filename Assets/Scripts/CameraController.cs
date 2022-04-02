using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Transform pos;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pos = player.GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        Vector3 position = pos.position;
        Vector3 targetPosition = new Vector3(position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, 0.2f);
    }
}
