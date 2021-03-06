using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (SpriteRenderer))]
public class GoblinController : MonoBehaviour
{

    enum Mood
    {
        Casual,
        Hungry,
        Angry,
        Stunned
    }

    Dictionary<string, Mood> spriteMoodDict = new Dictionary<string, Mood>
    {
        { "Fruit", Mood.Hungry },
        { "Farmer", Mood.Angry },
        { "Waypoint", Mood.Casual },
        { "Stunned", Mood.Stunned }
    };

    Dictionary<Mood, float> moodSpeedDict = new Dictionary<Mood, float>
    {
        { Mood.Hungry, 3f },
        { Mood.Angry, 3f },
        { Mood.Casual, 2f },
        { Mood.Stunned, 0f }
    };

    // Level specific waypoint list.
    // Don't forget to for every level in the editor!
    public List<GameObject> defaultWaypoints;
    public List<Sprite> goblinSprites = new List<Sprite>();

    private float speed;
    LinkedList<GameObject> curPath;
    public float sightRadius = 5f;

    private float stunDuration = 3000f;
    private float lastStunned;

    SpriteRenderer spriteRenderer;

    GameObject currentTarget;

    Mood goblinMood = Mood.Casual;


    void Start()
    {
        curPath = new LinkedList<GameObject>();
        loadDefaultPath();
        currentTarget = curPath.First.Value;
        curPath.RemoveFirst();
        spriteRenderer = GetComponent<SpriteRenderer>();
        speed = moodSpeedDict[goblinMood];

        lastStunned = Time.time * 1000;
    }

    private void loadDefaultPath()
    {
        for (int i = defaultWaypoints.Count - 1; i > 0; i--)
        {
            curPath.AddLast(defaultWaypoints[i]);
        }
    }

    void Update()
    {
        if (goblinMood == Mood.Stunned && ((Time.time * 1000) - lastStunned < stunDuration))
        {
            
        }

        else
        {
            WalkAround();
        }
    }

    void WalkAround()
    {
        while (currentTarget == null)
        {
            currentTarget = curPath.First.Value;
            curPath.RemoveFirst();
        }

        speed = moodSpeedDict[goblinMood];
        goblinMood = spriteMoodDict[currentTarget.gameObject.tag];
        spriteRenderer.sprite = goblinSprites[(int)goblinMood];

        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, step);

        spriteRenderer.flipX = (currentTarget.transform.position.x > transform.position.x) ? true : false;

        if (transform.position == currentTarget.transform.position)
        {
            if (curPath.Count == 0)
            {
                loadDefaultPath();
            }

            currentTarget = curPath.First.Value;
            curPath.RemoveFirst();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fruit"))
        {
            curPath.AddFirst(currentTarget);
            currentTarget = collision.gameObject;
        }

        if (collision.gameObject.CompareTag("Farmer"))
        {
            curPath.AddFirst(currentTarget);
            currentTarget = collision.gameObject;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fruit"))
        {
            currentTarget = curPath.First.Value;
            curPath.RemoveFirst();
        }

        if (collision.gameObject.CompareTag("Rake"))
        {
            goblinMood = Mood.Stunned;
            spriteRenderer.sprite = goblinSprites[(int)goblinMood];
            lastStunned = Time.time * 1000;
            curPath.Clear();
            loadDefaultPath();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
