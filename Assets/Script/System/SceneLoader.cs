using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string sceneName;

    private int gridX;
    private int gridY;
    private bool isLoading = false;

    void Start()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);

        TurnManager.Instance.AddSceneLoader(this);
    }

    public bool IsAtPosition(int x, int y)
    {
        return gridX == x && gridY == y;
    }
    public void TriggerLoad()
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(LoadSceneRoutine());
    }
    IEnumerator LoadSceneRoutine()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while(!op.isDone)yield return null;
    }

}
