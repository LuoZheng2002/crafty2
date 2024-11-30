using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOnTouch : MonoBehaviour
{
    [SerializeField] string layer_name = "ContentCrate";
    [SerializeField] private Transform broken_wall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layer_index = collision.gameObject.layer;

        if (LayerMask.LayerToName(layer_index) == layer_name)
        {
            Instantiate(broken_wall, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
