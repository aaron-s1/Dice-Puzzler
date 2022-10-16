




// Sticking to firing via GameManager for now.




// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class FireParticlesOnDisable : MonoBehaviour
// {
//     ParticleSystem particles;
//     GameObject particlesObj;
    

//     void Awake()
//     {
//         if (GetComponentInChildren<ParticleSystem>() != null)
//         {
//             particles = GetComponentInChildren<ParticleSystem>();
//             particlesObj = particles.gameObject;
//             // var particles
//             // particles = particlesObj
//             // particles = 
//         }
//     }

//     void OnDisable() 
//     {
//         var particlesMain = particles.main;
//         particlesObj.transform.parent = null;
//         particlesObj.SetActive(true);        
//     }


//         // if (particleObj != null) {


//             // var particles = particleObj.GetComponent<ParticleSystem>().main;

//         //     if (isSpecialTile) {
//         //         // particles.startColor = Color.red;
//         //         Color testColor = new Color (0f, 255f, 255f);
//         //         particles.startColor = testColor;
//         //         // particleObj.GetComponent<ParticleSystem>().main.startColor = Color.blue;
//         //     }

//         //     particleObj.GetComponent<ParticleSystem>().Play(); 
//         // }

// }
