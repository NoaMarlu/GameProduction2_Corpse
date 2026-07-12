using UnityEngine;

public class FlashSprite : MonoBehaviour
{

    public float onDuration;//‚Â‚¢‚Ä‚éŠÔ
    public float offDuration;//Á‚¦‚Ä‚éŠÔ

    //Šî–{İ’è
    private SpriteRenderer spr;
    private float timer;
    private bool isFlashing = true;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Flash();
    }

    void Flash()
    {
        if (!isFlashing) return;
        timer += Time.deltaTime;

        //•\¦‚³‚ê‚Ä‚¢‚½‚ç
        if (spr.enabled)
        {
            if (timer >= onDuration)
            {
                spr.enabled = false;
                timer = 0f;
            }
        }
        else//”ñ•\¦‚È‚ç
        {
            if (timer >= offDuration)
            {
                spr.enabled = true;
                timer = 0f;
            }
        }
    }

}
