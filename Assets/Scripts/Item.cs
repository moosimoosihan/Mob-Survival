using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public int count; // 아이템이 가진 수량
    public bool isMag = false; // 플레이어의 일정 반경에 닿았을 경우

    void Update()
    {
        if(isMag){
            Vector3 nextPos = GameManager.instance.playerControl.mainCharacter.transform.transform.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, GameManager.instance.playerControl.mainCharacter.transform.position, Time.deltaTime * 5f);
        }
    }
    public void Init(ItemData data){
        itemName = data.itemName;
        count = data.count;
    }
    void OnDisable() {
        isMag = false;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject == GameManager.instance.playerControl.mainCharacter && other.CompareTag("Player")){
            switch(itemName){
                case "Exp":
                    GameManager.instance.GetExp(count);
                    break;
                case "Gold":
                    GameManager.instance.GetGold(count);
                    break;
                    case "Item":
                    // 아이템 별 구현
                    break;
            }
            gameObject.SetActive(false);
        }
    }
}
