using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallsOnTouch : MonoBehaviour
{
    [SerializeField] string collision_tag;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == 18)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
			//    if (rb == null)
			//    {
			//        return;
			//    }

			//    // make object fall upon collision


			//    destroy(gameobject, 3f);
			//}

	}
}
