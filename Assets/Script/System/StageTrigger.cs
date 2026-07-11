using UnityEngine;

public class StageTrigger : MonoBehaviour
{
    public Stage stage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            StageManager.Instance.isPlayerStage(stage, player);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        Player player = collider.GetComponent<Player>();
        if (player != null) StageManager.Instance.IsPlayerExitStage(stage);
    }

}
