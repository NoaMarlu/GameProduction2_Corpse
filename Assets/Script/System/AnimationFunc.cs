using UnityEngine;

public class AnimationFunc : MonoBehaviour
{

    private void IsDestroy()
    {
        Destroy(gameObject);
    }
    private void TurnToWait()
    {
        TurnManager.Instance.turnState = TurnManager.TurnState.Wait;
    }

}
