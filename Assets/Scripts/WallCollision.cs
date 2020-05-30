using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    [SerializeField] float glowWait = 0.1f;

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Renderer>().sharedMaterial.SetFloat("_GlowStrength", 1f);
    }

    private void OnCollisionExit(Collision collision)
    {
        Invoke("RestoreGlow", glowWait);
        
    }
    private void RestoreGlow()
    {
        GetComponent<Renderer>().sharedMaterial.SetFloat("_GlowStrength", 0.8f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
