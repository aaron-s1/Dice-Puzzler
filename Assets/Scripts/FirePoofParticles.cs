using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePoofParticles : MonoBehaviour
{    
    public void Fire(bool specialTileSpawned) 
    {
        Debug.Log("fired poof");
        var particlesObj = transform.GetChild(0).gameObject;
        if (particlesObj == null)
            return;

        particlesObj.SetActive(true);

        var particles = GetComponentInChildren<ParticleSystem>();
                
        if (specialTileSpawned) {
            var particlesMain = particles.main;
            particlesMain.startColor = new Color (0f, 255f, 255f);
        }

        // particlesObj.transform.parent = null;
        particlesObj.transform.parent = GameManager.Instance.storingFiredParticles.transform;
        particles.Play();
        particlesObj.SetActive(true);
    }
}
