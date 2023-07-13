using UnityEngine;

public class BuffEffect : MonoBehaviour
{
    public Transform target;
    private void Update() {
        if(target.gameObject.activeSelf)
            transform.position = target.position;
        else
            DestroyBuffEffect();
    }
    public void DestroyBuffEffect()
    {
        gameObject.SetActive(false);
    }
}
