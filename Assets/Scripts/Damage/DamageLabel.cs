using System.Collections;
using UnityEngine;

public class DamageLabel : MonoBehaviour
{
    public enum Message { Miss }
    int curDamage = 0;
    [SerializeField]
    Vector3 originScale;
    bool isHeal;
    bool isPlayerDamge;
    
    // 아웃라인 설정
    //두께 설정
    public float pixelSize = 4;
    //설정된 Resolution보다 클 경우 pixel size 두 배로 설정
    public int doubleResolution = 1920;
    //해상도에 따라 pixel size를 조정할지 결정
    public bool resolutionDependant = false;
    TextMesh damageText;
    TextMesh[] allTextMesh;
    public Color outlineColor = Color.black;
    private MeshRenderer meshRenderer;

    void Awake()    
    {
        damageText = GetComponentInChildren<TextMesh>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>()[0];
        meshRenderer.sortingOrder = 10;
        for(int i=0;i<8; i++)
        {
            GameObject outline = new GameObject("outline", typeof(TextMesh));
            TextMesh outlineMesh = outline.GetComponent<TextMesh>();
            outlineMesh.alignment = damageText.alignment;
            outlineMesh.anchor = damageText.anchor;
            outlineMesh.characterSize = damageText.characterSize;
            outlineMesh.font = damageText.font;
            outlineMesh.fontSize = damageText.fontSize;
            outlineMesh.fontStyle = damageText.fontStyle;
            outlineMesh.richText = damageText.richText;
            outlineMesh.tabSize = damageText.tabSize;
            outlineMesh.lineSpacing = damageText.lineSpacing;
            outlineMesh.offsetZ = damageText.offsetZ;
            outline.transform.parent = damageText.transform;
            MeshRenderer otherMeshRenderer = outline.GetComponent<MeshRenderer>();
            otherMeshRenderer.material = new Material(meshRenderer.material);
            otherMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            otherMeshRenderer.receiveShadows = false;
            otherMeshRenderer.sortingLayerID = meshRenderer.sortingLayerID;
            otherMeshRenderer.sortingLayerName = meshRenderer.sortingLayerName;
            otherMeshRenderer.sortingOrder = meshRenderer.sortingOrder;
        }
        allTextMesh = damageText.GetComponentsInChildren<TextMesh>();
    }

    public void UpdateScore(int _newDamage, bool _isPlayerDamge)
    {
        isPlayerDamge = _isPlayerDamge;

        if(_newDamage<=0){
            // 힐로 분류
            isHeal = true;
            curDamage = Mathf.Abs(_newDamage);
        } else {
            // 데미지로 분류
            isHeal = false;
            curDamage = _newDamage;
        }
        
        damageText.text = curDamage.ToString();
        damageText.color = isHeal ? new Color(0,1,0) :  isPlayerDamge? new Color(1,0,0) : new Color(1,1,1);

        //현재 원본 Text의 월드 좌표를 스크린 포인트로 맵핑합니다.
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        //outlineColor.a = damageText.color.a * damageText.color.a;

        //복제된 TextMesh 옵션 설정
        for(int i=0;i<damageText.transform.childCount;i++)
        {
            //원본으로부터 복제된 자식(child)들을 불러옵니다.
            TextMesh other = damageText.transform.GetChild(i).GetComponent<TextMesh>();
            other.transform.localScale = Vector3.one;
            other.color = outlineColor;
            other.text = damageText.text;
             
            //설정된 해상도(doubleResolution)보다 큰 디바이스에서 실행될 경우
            //pixelSize를 두배로 합니다.
            bool doublePixel = resolutionDependant && (Screen.width > doubleResolution || Screen.height > doubleResolution);
            Vector3 pixelOffset = GetOffset(i) * (doublePixel ? 2.0f * pixelSize : pixelSize);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint + pixelOffset);
            other.transform.position = worldPoint;
        }
    }

    public void UpdateMessage(Message _newMessage){
        switch(_newMessage){
            case Message.Miss:
                damageText.text = _newMessage.ToString();
                damageText.color = new Color(1,0,0);

                //현재 원본 Text의 월드 좌표를 스크린 포인트로 맵핑합니다.
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
                //outlineColor.a = damageText.color.a * damageText.color.a;

                //복제된 TextMesh 옵션 설정
                for(int i=0;i<damageText.transform.childCount;i++)
                {
                    //원본으로부터 복제된 자식(child)들을 불러옵니다.
                    TextMesh other = damageText.transform.GetChild(i).GetComponent<TextMesh>();
                    other.color = outlineColor;
                    other.text = damageText.text;

                    //설정된 해상도(doubleResolution)보다 큰 디바이스에서 실행될 경우
                    //pixelSize를 두배로 합니다.
                    bool doublePixel = resolutionDependant && (Screen.width > doubleResolution || Screen.height > doubleResolution);
                    Vector3 pixelOffset = GetOffset(i) * (doublePixel ? 2.0f * pixelSize : pixelSize);
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint + pixelOffset);
                    other.transform.position = worldPoint;
                }
                break;
        }
    }

    bool CheckCurrentDegitAndUpperDegitExist(ref int[] _degits, int _currentDegit, int _maxDegit)
    {
        for (int i = _maxDegit; i >= _currentDegit; i--)
        {
            if (i == _currentDegit && _currentDegit == 0)
                return true;

            if (_degits[i] != 0)
                return true;
        }

        return false;
    }

    public void ShowDamageAnimation(int _damage, GameObject _OnObj, bool _isCritical, bool _isPlayerDamge)
    {
        UpdateScore(_damage, _isPlayerDamge);
        StartCoroutine(CoSpawnDamageLabel(_OnObj, _isCritical));
    }
    public void ShowDamageAnimation(Message _message, GameObject _OnObj)
    {
        UpdateMessage(_message);
        StartCoroutine(CoSpawnMessageLabel(_OnObj));
    }
    IEnumerator CoSpawnDamageLabel(GameObject _OnObj,bool _isCritical)
    {
        //시작 사이즈 살짝 랜덤화
        float tempOffset = Random.Range(-0.05f, 0.05f);
        transform.localScale = new Vector3(
            originScale.x + tempOffset,
            originScale.y + tempOffset,
            originScale.z);

        //살짝 위에서 시작
        Vector3 startPos = new Vector3(
            _OnObj.transform.position.x + Random.Range(-0.25f, 0.25f),
            _OnObj.transform.position.y + 0.75f + Random.Range(-0.25f, 0.25f),
            _OnObj.transform.position.z);

        Vector3 targetPos = new Vector3(startPos.x, startPos.y + 0.75f, startPos.z);

        //시작 위치 세팅
        transform.position = startPos;

        //흔들면서
        //StartCoroutine(CoShakeObj(tempScoreObj, 0.09f, 0.30f));   //안이쁨..
        
        //커진상태에서 작아지기
        StartCoroutine(MyCoroutines.CoChangeSize(gameObject, _isCritical? 10f : 3f, _isCritical? transform.localScale.x*2 :  transform.localScale.x, 0.15f));

        //빠르게 페이드인하면서 등장
        for (int i = 0; i < allTextMesh.Length; i++)
        {
            StartCoroutine(MyCoroutines.CoFadeInOutTextMesh(allTextMesh[i].gameObject, 0, 1, 0.15f));
        }

        
        //잠깐 보여줬다가
        yield return new WaitForSeconds(0.5f);

        float fadeTime = 0.3f;
        //위로 올라가면서 페이드 아웃
        for (int i = 0; i < allTextMesh.Length; i++)
        {
            StartCoroutine(MyCoroutines.CoFadeInOutTextMesh(allTextMesh[i].gameObject, 1, 0, fadeTime));
        }

        StartCoroutine(MyCoroutines.CoGlobalMove_AnimationCurve(gameObject, startPos, targetPos, fadeTime, DamageManager.Instance.labelMoveUpAnimationCurve));

        yield return new WaitForSeconds(fadeTime);

        gameObject.SetActive(false);
    }
    IEnumerator CoSpawnMessageLabel(GameObject _OnObj)
    {
        //시작 사이즈 살짝 랜덤화
        float tempOffset = Random.Range(-0.05f, 0.05f);
        transform.localScale = new Vector3(
            originScale.x + tempOffset,
            originScale.y + tempOffset,
            originScale.z);

        //살짝 위에서 시작
        Vector3 startPos = new Vector3(
            _OnObj.transform.position.x + Random.Range(-0.25f, 0.25f),
            _OnObj.transform.position.y + 0.75f + Random.Range(-0.25f, 0.25f),
            _OnObj.transform.position.z);

        Vector3 targetPos = new Vector3(startPos.x, startPos.y + 0.75f, startPos.z);

        //시작 위치 세팅
        transform.position = startPos;

        //흔들면서
        //StartCoroutine(CoShakeObj(tempScoreObj, 0.09f, 0.30f));   //안이쁨..
        
        //커진상태에서 작아지기
        StartCoroutine(MyCoroutines.CoChangeSize(gameObject, 3f, transform.localScale.x, 0.15f));

        //빠르게 페이드인하면서 등장
        for (int i = 0; i < allTextMesh.Length; i++)
        {
            StartCoroutine(MyCoroutines.CoFadeInOutTextMesh(allTextMesh[i].gameObject, 0, 1, 0.15f));
        }
        
        //잠깐 보여줬다가
        yield return new WaitForSeconds(0.5f);

        float fadeTime = 0.3f;
        //위로 올라가면서 페이드 아웃
        for (int i = 0; i < allTextMesh.Length; i++)
        {
            StartCoroutine(MyCoroutines.CoFadeInOutTextMesh(allTextMesh[i].gameObject, 1, 0, fadeTime));
        }

        StartCoroutine(MyCoroutines.CoGlobalMove_AnimationCurve(gameObject, startPos, targetPos, fadeTime, DamageManager.Instance.labelMoveUpAnimationCurve));

        yield return new WaitForSeconds(fadeTime);

        //Off시키기
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transform.localScale = originScale;
        StopAllCoroutines();
    }
    //복제된 TextMesh들의 배치정보
    Vector3 GetOffset(int i)
    {
        switch(i % 8)
        {
            case 0: return new Vector3(0, 1, 0);
            case 1: return new Vector3(1, 1, 0);
            case 2: return new Vector3(1, 0, 0);
            case 3: return new Vector3(1, -1, 0);
            case 4: return new Vector3(0, -1, 0);
            case 5: return new Vector3(-1, -1, 0);
            case 6: return new Vector3(-1, 0, 0);
            case 7: return new Vector3(-1, 1, 0);
            default: return Vector3.zero;
        }
    }
}
