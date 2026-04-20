using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour
{
    public Transform player;

    public Vector3 targetPos;
    public float moveDuration = 0.8f;

    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isMoving = false;

    public void LevelSelection()
    {
        if (isMoving) return;

        StartCoroutine(MovePlayer(player.position, targetPos));
    }

    IEnumerator MovePlayer(Vector3 start, Vector3 target)
    {
        isMoving = true;

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;

            float t = timer / moveDuration;
            float curveT = moveCurve.Evaluate(t);

            player.position = Vector3.Lerp(start, target, curveT);

            yield return null;
        }

        player.position = target;

        isMoving = false;
    }

  public void StartLevel(int sceneNo)
    {
        Debug.Log("点击了 StartLevel, sceneNo = " + sceneNo);
        LevelManager.EnterLevel(sceneNo);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
