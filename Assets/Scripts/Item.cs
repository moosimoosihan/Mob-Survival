using UnityEngine;
using UnityEngine.Pool;

public class Item : MonoBehaviour
{
    public string itemName;
    public int count; // 아이템이 가진 수량
    public bool isMag = false; // 플레이어의 일정 반경에 닿았을 경우
    bool touch = false;
    SpriteRenderer spriteRenderer;
    IObjectPool<Item> _ManagedPool;
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if(isMag){
            Vector3 nextPos = GameManager.instance.playerControl.mainCharacter.transform.transform.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, GameManager.instance.playerControl.mainCharacter.transform.position, Time.deltaTime * 10f);
        }
    }
    public void Init(ItemData data){
        spriteRenderer.sprite = GameManager.instance.itemManager.itemSprite[data.spriteNum];
        itemName = data.itemName;
        count = data.count;
    }
    void OnDisable() {
        isMag = false;
        touch = false;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject == GameManager.instance.playerControl.mainCharacter && other.CompareTag("Player") && !touch){
            touch = true;
            switch(itemName){
                case "Exp":
                    GameManager.instance.GetExp(count);
                    AudioManager.Instance.ItemSfxPlay(AudioManager.ItemSfx.Exp);
                    break;
                case "Gold":
                    GameManager.instance.GetGold(count);
                    AudioManager.Instance.ItemSfxPlay(AudioManager.ItemSfx.Coin);
                    break;
                    case "Item":
                    // 아이템 별 구현
                    break;
            }
            DestroyItem();
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject == GameManager.instance.playerControl.mainCharacter && other.CompareTag("Player") && !touch){
            touch = true;
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
            DestroyItem();
        }
    }
    public void SetManagedPool(IObjectPool<Item> pool)
    {
        _ManagedPool = pool;
    }
    private void DestroyItem()
    {
        _ManagedPool.Release(this);
    }
}
