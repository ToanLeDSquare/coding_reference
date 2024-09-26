using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class CardFight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]private CardTabGUI cardTabGUI;
    private RectTransform m_RectTrans;
    public int positionIndex; // 0 to 7

    [SerializeField] private GameObject btnEquip;

    public event Action<int> OnMouseEnter;
    public event Action<int> OnDrag;
    public event Action<int> OnEndDrag;
    public event Action<int> OnMouseExit;
    
    public Image mainCardBaseImg;
    public Image backgroundBaseImg;
    public Image cardCharacterImg;
    public Image cardCharacterElementIMG;
    public Image[] cardSkillImg;
    public Image[] cardSkillElementImg;
    public TextMeshProUGUI energyTMP;

    public GameObject baseGoldLight;
    
    public GameObject cardFreeLogo;

    private CardData _data;

    // Start is called before the first frame update
    void Awake()
    {
        m_RectTrans = GetComponent<RectTransform>();
        originPosition = m_RectTrans.anchoredPosition;
    }

    #region Set Id Card

    public void SetData(CardData _cardData)
    {
        _data = _cardData;
        mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(_data.elementId);
        //backgroundBaseIMG.sprite = ResourceManager.I.GetCardBackGroundBase(_data.charBaseId);
        cardCharacterImg.sprite = ResourceManager.I.GetCardCharacterImg(_data.characterId);
        cardCharacterElementIMG.sprite = ResourceManager.I.GetCardCharacterElementImg(_data.elementId);
        
        ShowSkillAndElement();
        
        energyTMP.text = _cardData.energy.ToString();   
        
        ShowCardFreeLogo(_cardData.nftFree);
    }

    public CardData GetCardFightData()
    {
        return _data;
    }

    void ShowSkillAndElement()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < _data.skills.Length)
            {
                cardSkillImg[i].sprite = ResourceManager.I.GetCardSkillImg(_data.skills[i].skillId);
                cardSkillElementImg[i].sprite = ResourceManager.I.GetCardSkillElementImg(_data.skills[i].elementId);

                cardSkillImg[i].gameObject.SetActive(true);
                cardSkillElementImg[i].gameObject.SetActive(true);
            }
            else
            {
                cardSkillImg[i].gameObject.SetActive(false);
                cardSkillElementImg[i].gameObject.SetActive(false);
            }
        }

        if (_data.skills.Length == 0)
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(0);
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(10);
        }
        else if (_data.skills.Length == 1)
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(1);
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(11);
        }
        else if (_data.skills.Length > 1)
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(2);         
        }

        if (_data.skills.Length == 2)
        {
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(12);
        }
        else if (_data.skills.Length > 2)
        {
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(_data.elementId);
        }

        baseGoldLight.SetActive(_data.skills.Length == 4);
    }

    /*void ShowSkillAndElementFake()
    {
        for (int i = 0; i < 4; i++)
        {
                cardSkillImg[i].sprite = ResourceManager.I.GetCardSkillImg(1);
                cardSkillElementImg[i].sprite = ResourceManager.I.GetCardSkillElementImg(1);

                cardSkillImg[i].gameObject.SetActive(true);
                cardSkillElementImg[i].gameObject.SetActive(true);
        }

        if (_data.skills.Length == 0)
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(0);
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(10);
        }
        else if (_data.skills.Length == 1)
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(1);
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(11);
        }
        else if (_data.skills.Length > 1)
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(2);
        }

        if (_data.skills.Length == 2)
        {
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(12);
        }
        else if (_data.skills.Length > 2)
        {
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(_data.elementId);
        }

        baseGoldLight.SetActive(_data.skills.Length == 4);
    }*/

    #endregion
    
    void ShowCardFreeLogo(bool isShow)
    {
        if (cardFreeLogo != null)
            cardFreeLogo.SetActive(isShow);
    }

    #region click battle card function

    public void OnClickCard()
    {
        if (cardTabGUI.isSwap && !cardTabGUI.IsFlying)
        {
            Debug.Log("Swap");
            cardTabGUI.OnSwaping(positionIndex);
        }
        else
        {
            cardTabGUI.OnChoseBattleCard(positionIndex);
            //Debug.Log(PlayerData.I.GetEquippedCardList());
        }

        AudioController.I.PlayButtonSound();
    }

    public void ShowEquipBtn(bool isShow)
    {        
        btnEquip.SetActive(isShow);
    }

    #endregion

    #region Button method

    public void OnCloseClick()
    {
        ShowEquipBtn(false);
        cardTabGUI.cardIndexChose = -1;
    }

    public void OnInfoBtnClick()
    {
        if (!CardTabGUI.IsDragging)
        {
            AudioController.I.PlayButtonSound();

        CardInfoParam param = new CardInfoParam();

        param.characterId = _data.characterId;
        param.charElementId = _data.elementId;

        if (_data.skills.Length == 1)
        {
            param.skillId1 = _data.skills[0].skillId;
            param.elementId1 = _data.skills[0].elementId;
        }
        else if (_data.skills.Length == 2)
        {
            param.skillId1 = _data.skills[0].skillId;
            param.elementId1 = _data.skills[0].elementId;
            param.skillId2 = _data.skills[1].skillId;
            param.elementId2 = _data.skills[1].elementId;
        }
        else if (_data.skills.Length == 3)
        {
            param.skillId1 = _data.skills[0].skillId;
            param.elementId1 = _data.skills[0].elementId;
            param.skillId2 = _data.skills[1].skillId;
            param.elementId2 = _data.skills[1].elementId;
            param.skillId3 = _data.skills[2].skillId;
            param.elementId3 = _data.skills[2].elementId;
        }
        else if (_data.skills.Length == 4)
        {
            param.skillId1 = _data.skills[0].skillId;
            param.elementId1 = _data.skills[0].elementId;
            param.skillId2 = _data.skills[1].skillId;
            param.elementId2 = _data.skills[1].elementId;
            param.skillId3 = _data.skills[2].skillId;
            param.elementId3 = _data.skills[2].elementId;
            param.skillId4 = _data.skills[3].skillId;
            param.elementId4 = _data.skills[3].elementId;
        }

        for (int i = 0; i < 4; i++)
        {
            if(i < _data.skills.Length)
                param.skillCount = i + 1;          
        }

        param.energy = _data.energy;
        param.deployTime = _data.deployTime;
        param.range = _data.range;
        param.atkSpeed = _data.atkSpeed;
        param.healthPoint = _data.healthPoints;
        param.damage = _data.damage;
        param.aoeDamage = _data.aoeDamage;
        param.count = _data.count;
        param.movingSpeed = _data.movementSpeed;
        param.totalBreed = _data.totalBreed;
        param.nftFree = _data.nftFree;
        
        DialogManager.I.Show(DIALOG_GUI.CardInfoDialog, param);
        }
    }

    #endregion

    #region Drag Handler
    Vector2 originPosition;
    Vector2 oldPointerPosition;
    int markSiblingIndex;
   
    public void BeginDrag(BaseEventData data)
    {
        MenuGUIController.I.EnableAllTab(false);
        if (!CardTabGUI.IsDragging && !cardTabGUI.isSwap)
        {
            Debug.Log("Drag: " + positionIndex);
            PointerEventData pointerData = (PointerEventData)data;
            oldPointerPosition = pointerData.position;
            OnDrag?.Invoke(positionIndex);
            //m_RectTrans.anchoredPosition = ((PointerEventData)data).position;
            //cardTabGUI.battleCardDraggingId = _data.id;
            cardTabGUI.ResetChoseBattleCard();
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            m_RectTrans.SetSiblingIndex(8);
        }
    }
    
    public void DragHandler(BaseEventData data)
    {
        if (CardTabGUI.DraggedCardIndex == positionIndex)
        {
            PointerEventData pointerData = (PointerEventData)data;
            Vector2 delta = pointerData.position - oldPointerPosition;
            delta /= MenuGUIController.ScaleFactor;
            m_RectTrans.anchoredPosition += delta;
            oldPointerPosition = pointerData.position;
        }
    }

    public void EndDrag(BaseEventData data)
    {
        OnEndDrag?.Invoke(positionIndex);
        Debug.Log("End drag: " + positionIndex);
        //PointerEventData pointerData = (PointerEventData)data;
        if (CardTabGUI.DraggedCardIndex == positionIndex)
        {
            //cardTabGUI.battleCardDraggingId = -1;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            cardTabGUI.SwapFightCards();
            m_RectTrans
                .DOAnchorPos(originPosition, 0.2f)
                .OnComplete(() =>
                {
                    Debug.Log("Oncomplete");
                    CardTabGUI.DraggedCardIndex = -1;
                    
                    MenuGUIController.I.EnableAllTab();
                });
                
        }
        AudioController.I.PlaySound(AudioClipDefines.I.ACTION_TAP_ON_BUTTON_SELECT_CARD);
    }

    #endregion

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!cardTabGUI.isSwap)
            OnMouseEnter?.Invoke(positionIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit?.Invoke(positionIndex);
    }
}

