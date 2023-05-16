using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/LevelItemData")]

public class LevelItemData : ScriptableObject
{
    public enum ItemType { 용사0, 용사1, 용사2, 용사3, 용사4, 용사5, 용사6, 용사7, 용사8, 용사9, 용사10, 용사11, 용사12, 용사13, 용사14, 용사15,
    궁수0 ,궁수1, 궁수2, 궁수3, 궁수4, 궁수5, 궁수6, 궁수7, 궁수8, 궁수9, 궁수10, 궁수11, 궁수12, 궁수13, 궁수14, 궁수15,
    현자0, 현자1, 현자2, 현자3, 현자4, 현자5, 현자6, 현자7, 현자8, 현자9, 현자10, 현자11, 현자12, 현자13, 현자14, 현자15,
    사제0, 사제1, 사제2, 사제3, 사제4, 사제5, 사제6, 사제7, 사제8, 사제9, 사제10, 사제11, 사제12, 사제13, 사제14, 사제15, 
    빈칸 };
    public enum ItemTypeName{ 기본스킬, 유니크스킬, 레벨스킬, 빈칸 }

    [Header("# Main Info")]
    public ItemType itemType;
    public bool isLevelUp = false;
    public int level;
    public float value1;
    public float value2;
    public string itemName;
    public ItemTypeName itemTypeName;
    [TextArea]
    public string itemDesc;
    
}
