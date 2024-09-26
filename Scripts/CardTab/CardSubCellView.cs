using EnhancedUI.EnhancedScroller;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardSubCellView : EnhancedScrollerCellView, IPointerClickHandler
{
    public Image mainCardBaseImg;
    public Image backgroundBaseImg;
    public Image cardCharacterImg;
    public Image cardCharacterElementImg;
    public Image[] cardSkillImg;
    public Image[] cardSkillElementImg;
    public TextMeshProUGUI energyTxt;

    private CardTabGUI _CardTabGUI;
    private CardData _Data;
    public GameObject btnSub;

    public GameObject baseGoldLight;
    
    public GameObject cardFreeLogo;

    [HideInInspector] public Vector2 oldPosition;

    public void SetData(CardData data, CardTabGUI cardTabGUI)
    {
        _Data = data;
        _CardTabGUI = cardTabGUI;
        mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(_Data.elementId);

        //backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(_Data.charBaseId);
        cardCharacterImg.sprite = ResourceManager.I.GetCardCharacterImg(_Data.characterId);
        cardCharacterElementImg.sprite = ResourceManager.I.GetCardCharacterElementImg(_Data.elementId);

        ShowSkillAndElement();

        energyTxt.text = _Data.energy.ToString();
        ShowCardFreeLogo(data.nftFree);
        
        OnChoseCard(false);

        if (cardTabGUI.indexCardSelected == data.id)
        {
            _CardTabGUI.cellViewSelected = this;
        }
    }
    
    public CardData GetCardSubData()
    {
        return _Data;
    }

    void ShowSkillAndElement()
    {        
        for (int i = 0; i < 4; i++)
        {
            if (i < _Data.skills.Length)
            {
                cardSkillImg[i].sprite = ResourceManager.I.GetCardSkillImg(_Data.skills[i].skillId);
                cardSkillElementImg[i].sprite = ResourceManager.I.GetCardSkillElementImg(_Data.skills[i].elementId);

                cardSkillImg[i].gameObject.SetActive(true);
                cardSkillElementImg[i].gameObject.SetActive(true);
            }
            else 
            {
                cardSkillImg[i].gameObject.SetActive(false);
                cardSkillElementImg[i].gameObject.SetActive(false);
            }      
        }

        if (_Data.skills.Length == 0)
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(0);
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(10);
        }
        else if (_Data.skills.Length == 1)
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(1);
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(11);
        }
        else if (_Data.skills.Length > 1 )
        {
            mainCardBaseImg.sprite = ResourceManager.I.GetCardMainBase(2);       
        }

        if (_Data.skills.Length == 2)
        {
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(12);
        }
        else if (_Data.skills.Length > 2)
        {
            backgroundBaseImg.sprite = ResourceManager.I.GetCardBackGroundBase(_Data.elementId);
        }

        baseGoldLight.SetActive(_Data.skills.Length == 4);
    }

    void ShowCardFreeLogo(bool isShow)
    {
        if (cardFreeLogo != null)
            cardFreeLogo.SetActive(isShow);
    }

    int useIndex = 0;
    public void OnPointerClick(PointerEventData eventData)
    {       
        if (_CardTabGUI.indexCardSelected != -1 && _CardTabGUI.indexCardSelected == _Data.id)
        {
            //OnChoseCard(false);
            _CardTabGUI.indexCardSelected = -1;
            _CardTabGUI.FinishSwap();
        }
        else
        {
            useIndex = 0;
            _CardTabGUI.FinishSwap();
            _CardTabGUI.SetHideButtonSubCardSelected();
            OnChoseCard(true);
            _CardTabGUI.indexCardSelected = _Data.id;
            _CardTabGUI.cellViewSelected = this;          
        }
        AudioController.I.PlayButtonSound();
    }

    public void OnChoseCard(bool isChosen)
    {       
        btnSub.SetActive(isChosen);
    }

    #region Button Method
    
    public void OnCloseClick()
    {
        OnChoseCard(false);
        _CardTabGUI.indexCardSelected = -1;
        _CardTabGUI.FinishSwap();
    }
    
    public void OnUseClick()
    {
        if (!CardTabGUI.IsDragging)
        {
            AudioController.I.PlayButtonSound();

            if (useIndex == 0)
            {
                useIndex = 1;
                _CardTabGUI.BeginSwap();
                _CardTabGUI.cardSubToSwap = this;
                oldPosition = this.transform.position;
                this.transform.DOLocalMoveY(1, 0.3f);
            
                Debug.Log("use");
            }     
        }
    }

    public void OnInfoBtnClick()
    {
        if (!CardTabGUI.IsDragging)
        {
            AudioController.I.PlayButtonSound();

            CardInfoParam param = new CardInfoParam();

            param.characterId = _Data.characterId;
            param.charElementId = _Data.elementId;

            if (_Data.skills.Length == 1)
            {
                param.skillId1 = _Data.skills[0].skillId;
                param.elementId1 = _Data.skills[0].elementId;
            }
            else if (_Data.skills.Length == 2)
            {
                param.skillId1 = _Data.skills[0].skillId;
                param.elementId1 = _Data.skills[0].elementId;
                param.skillId2 = _Data.skills[1].skillId;
                param.elementId2 = _Data.skills[1].elementId;
            }
            else if (_Data.skills.Length == 3)
            {
                param.skillId1 = _Data.skills[0].skillId;
                param.elementId1 = _Data.skills[0].elementId;
                param.skillId2 = _Data.skills[1].skillId;
                param.elementId2 = _Data.skills[1].elementId;
                param.skillId3 = _Data.skills[2].skillId;
                param.elementId3 = _Data.skills[2].elementId;
            }
            else if (_Data.skills.Length == 4)
            {
                param.skillId1 = _Data.skills[0].skillId;
                param.elementId1 = _Data.skills[0].elementId;
                param.skillId2 = _Data.skills[1].skillId;
                param.elementId2 = _Data.skills[1].elementId;
                param.skillId3 = _Data.skills[2].skillId;
                param.elementId3 = _Data.skills[2].elementId;
                param.skillId4 = _Data.skills[3].skillId;
                param.elementId4 = _Data.skills[3].elementId;
            }

            //param.skillsParam = new SkillParam[_Data.skills.Length];


            for (int i = 0; i < 4; i++)
            {
                if (i < _Data.skills.Length)
                {
                    param.skillCount = i + 1;
                    //Debug.Log(_Data.skills[i].skillId);
                    // param.skillsParam[i].skillId = _Data.skills[i].skillId;

                    // Debug.Log(param.skillsParam[i].skillId);
                }
            }

            param.energy = _Data.energy;
            param.deployTime = _Data.deployTime;
            param.range = _Data.range;
            param.atkSpeed = _Data.atkSpeed;
            param.healthPoint = _Data.healthPoints;
            param.damage = _Data.damage;
            param.aoeDamage = _Data.aoeDamage;
            param.count = _Data.count;
            param.movingSpeed = _Data.movementSpeed;
            param.totalBreed = _Data.totalBreed;
            param.nftFree = _Data.nftFree;
            
            DialogManager.I.Show(DIALOG_GUI.CardInfoDialog, param);
        }
    }

    #endregion
}
