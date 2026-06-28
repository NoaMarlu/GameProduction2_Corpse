using UnityEngine;

public class StageTrigger : MonoBehaviour
{
    public Stage stage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (collision.GetComponent<Player>() != null)
        {
            StageManager.Instance.isPlayerStage(stage, player);
        }
    }
}
