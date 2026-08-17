using UnityEngine;

public class DecayAnime : MonoBehaviour
{

    private Animator animator;
    private float minInterval = 2f;
    private float maxInterval = 5.0f;
    private float timer;
    private float nextInterval;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        SetNextInterval();
    }
    void Update()
    {
        if (animator == null) return;

        timer += Time.deltaTime;

        if(timer >= nextInterval)
        {
            TriggerRandomBubble();
            timer = 0f;
            SetNextInterval();
        }
    }

    private void TriggerRandomBubble()
    {
        if (Random.value > 0.5f)
        {
            animator.SetTrigger("Up");
        }
        else animator.SetTrigger("Down");
    }

    private void SetNextInterval()
    {
        nextInterval = Random.Range(minInterval, maxInterval);
    }

}
