using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_EyeDrop_Strike : Entity
{
    private bool hit = false;
    private float hitdelay = 10.0f;

    private float spriteindex = 0;
    [SerializeField] private Sprite[] sprites;

    // Start is called before the first frame update
    protected override void Start()
    {
        yspeed = -16.0f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        UpdateMovement();

        spriterenderer.sprite = sprites[(spriteindex > 3.0f)? 0: 1];
        spriteindex = Mathf.Repeat(spriteindex + ts, 6.0f);

        if (!hit)
        {
            if ( EvaluateCollision().HasFlag(CollisionFlag.DOWN) )
            {
                hit = true;
            }
        }
        else if (hitdelay > 0.0f)
        {
            hitdelay = Mathf.Max(hitdelay-ts, 0.0f);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}

