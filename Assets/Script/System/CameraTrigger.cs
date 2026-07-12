using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public GameObject cameraMarker;
    //アタッチしているオブジェクトの範囲内にいる場合、cameraMarkerの位置にカメラを遷移
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null) return;
        if (cameraMarker == null) return;
        CameraManager.Instance.MoveTo(cameraMarker.transform.position);
    }

}
