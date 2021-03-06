using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorExplosion : WeaponProjectile
{
    [SerializeField] GameObject[] sparkleparticle;

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] castresults = new RaycastHit2D[16];
        hitboxcollider.Cast(
            new Vector2(0.0f, 0.0f),
            new ContactFilter2D() {layerMask=LAYER_HURTBOX_BIT, useLayerMask=true},
            castresults,
            0.0f,
            true
        );

        // Attack enemies in explosion range
        foreach (var hit in castresults)
        {
            Entity e = GetEntityFromCollider(hit.collider);
            if (e)
            {
                // Entity has the shootable flag set
                if (e.isshootable)
                {
                    e.ChangeHealth(-damage, weaponprojtype);
                }
            }
        }

        // Clouds
        float r = (hitboxcollider as CircleCollider2D).radius;
        for (var i = 0; i < 5; i++)
        {
            Instantiate(obj_on_hit).transform.position = transform.position + new Vector3(
                Random.Range(-r, r),
                Random.Range(-r, r),
                0.0f
            );

            Instantiate(sparkleparticle[UnityEngine.Random.Range(0f, 1f) < 0.5f? 0: 1]).transform.position = 
                transform.position + new Vector3(
                    UnityEngine.Random.Range(-r, r),
                    UnityEngine.Random.Range(-r, r),
                    -1.0f
                );
        }

        
        
        Destroy(gameObject);
    }

    public void SetRadius(float r) 
    {
        (hitboxcollider as CircleCollider2D).radius = r;
        spriterenderer.transform.localScale = new Vector3(r*2f, r*2f, 1.0f);
    }
}
