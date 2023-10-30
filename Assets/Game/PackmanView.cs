using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PackmanView : MonoBehaviour
{
    public Packman model;
    private Coroutine _moveCoroutine;

    public void Move(Vector2Int targetPos, float timeToGo)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveCor(targetPos, timeToGo));
        //transform.DOMove(new Vector3(targetPos.x, targetPos.y, transform.position.z), timeToGo).SetEase(Ease.Linear).SetSpeedBased(true);
    }
    
    public IEnumerator MoveCor(Vector2Int targetPos, float timeToGo)
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        while (elapsedTime < timeToGo)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, new Vector3(targetPos.x, targetPos.y, transform.position.z), elapsedTime / timeToGo);
            yield return null;
        }

        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }

}