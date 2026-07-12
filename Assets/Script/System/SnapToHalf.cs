using UnityEngine;

public class SnapToHalf : MonoBehaviour
{

    private void Update()
    {
        Vector3 currentPos = transform.position;
        float snappedX = RoundToHalf(currentPos.x);
        float snappedY = RoundToHalf(currentPos.y);
        transform.position = new Vector3(snappedX, snappedY, currentPos.z);
    }

    private float RoundToHalf(float value)
    {
        return Mathf.Round(value - 0.5f) +0.5f;
    }

}
