using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager Instance { get; private set; }

    public float lerpSpeed = 5f;//遷移速度
    private Vector3 targetPos;
    private bool isMoving = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        targetPos = transform.position;
    }

    void Update()
    {
        //移動処理
        if (!isMoving) return;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);
        //ターゲットに到達で移動をやめる
        if(Vector3.Distance(transform.position,targetPos) < 0.01f)
        {
            transform.position = targetPos;
            isMoving = false;
        }
    }
    //外部から呼び出す移動開始処理
    public void MoveTo(Vector3 pos)
    {
        targetPos = pos;
        isMoving = true;
    }
    //カメラシェイク
    public void Shake(float duration = 0.3f, float shakePower = 0.5f, int vibrato = 20) { transform.DOShakePosition(duration, shakePower, vibrato).SetLink(gameObject); }

}
