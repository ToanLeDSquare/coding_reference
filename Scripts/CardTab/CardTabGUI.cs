using System;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

public class CardTabGUI : TabGUI, IEnhancedScrollerDelegate
{
    [Header("UI")]
    [SerializeField] private RectTransform _contentRect;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _averageTxt;
    [SerializeField] private TextMeshProUGUI _cardsNumberTxt;
    [SerializeField] private GameObject _swapErrorDialog;
    [SerializeField] private GameObject sortEnergy;
    [SerializeField] private GameObject sortRarity;

    [Header("Card Datas")]
    [SerializeField] private CardFight[] _lsBattleCard;

    [Header("Scroller")]
    public EnhancedScroller myScroller;
    [SerializeField] private CardSubCellView _cardSubPrefab;
    [HideInInspector] public CardSubCellView cardSubToSwap;
    [HideInInspector] public EnhancedScrollerCellView cellViewSelected;
    public int indexCardSelected = -1;
    private int indexCardEnter = -1;

    public static int DraggedCardIndex { get; set; } = -1;
    public static bool IsDragging => DraggedCardIndex != -1;

    public bool isWapSubtoFight = false;
    
    //For Swaping
    private List<CardData> _cardFightList;
    private List<CardData> _cardSubList;
    private CardData fightCardTemp, subCardTemp;
    public CardFight tempCard;
    [HideInInspector] public bool isSwap;
    public bool IsFlying { get; private set; }
    //public int battleCardDraggingId;
    public TextMeshProUGUI sortTxt;

    protected override void OnAwake()
    {
        base.OnAwake();
        foreach (var card in _lsBattleCard)
        {
            card.OnDrag += (i) =>
            {
                //If no card is being chosen
                if (!IsDragging)
                    DraggedCardIndex = i;
            };
            card.OnMouseEnter += (i) =>
            {
                //If there is exist a dragging card
                if (IsDragging)
                    indexCardEnter = i;
            };
            //card.OnEndDrag += i => draggedCardIndex = -1;
            card.OnMouseExit += (i) =>
            {
                //Just set enter index to -1 if this is the card owner
                if (indexCardEnter == i)
                    indexCardEnter = -1;
            };
        }
    }

    public override IEnumerator Setup(TabGuiParam tabParams)
    {
        bool done = false;
        RestApiManager.I.GetCardsData((isSuccess) =>
        {
           if (isSuccess)
           {
               isSwap = false;
               _canvasGroup.alpha = 0;
               //battleCardDraggingId = -1;
               PlayerData.I.SetEquipedCard();
               _cardFightList = PlayerData.I.GetFightCardList();
               _cardSubList = PlayerData.I.GetSubCardList(sortIndex);
               _cardsNumberTxt.text = PlayerData.I.GetAllCardList().Count.ToString();
               SetFightCardData();
               _canvasGroup.alpha = 1;
               _contentRect.localScale = Vector3.zero;
               _contentRect.DOScale(new Vector3(1, 1, 1), 0.5f);
               myScroller.Delegate = this;
           }
           done = true;
        });
        yield return new WaitUntil(() => done);
    }

    #region WalkIn Effect

    public override void PreWalkIn()
    {
        //_contentRect.localScale = Vector3.zero;
        //_contentRect.DOScale(new Vector3(0, 0, 0), 0);
    }

    public override IEnumerator WalkIn()
    {
        yield return new WaitForSeconds(0.3f);       
    }

    public override IEnumerator WalkOut()
    {
        FinishSwap();
        ResetChoseBattleCard();
        //_contentRect.DOScale(new Vector3(0, 0, 0), 0);
        yield return null;
    }
    #endregion

    #region Button Medthod
    int sortIndex = 0;
    public void SortButton()
    {
        if (sortIndex == 0)
        {
            sortTxt.GetComponent<LocalizeStringEvent>().SetEntry("SORT_2");
            sortEnergy.SetActive(false);
            sortRarity.SetActive(true);
            sortIndex = 1;
        }
        else
        {
            sortTxt.GetComponent<LocalizeStringEvent>().SetEntry("SORT_1");
            sortEnergy.SetActive(true);
            sortRarity.SetActive(false);
            sortIndex = 0;
        }
        _cardSubList = PlayerData.I.GetSubCardList(sortIndex);

        FinishSwap();
        myScroller.ReloadData();
        AudioController.I.PlayButtonSound();
    }
    #endregion

    #region Swap Card Data
    public void SwapFightCards()
    {
        if (DraggedCardIndex != -1 && indexCardEnter != -1 && indexCardEnter != DraggedCardIndex)
        {
            CardData firstCardData = PlayerData.I.GetCardDataByID(_lsBattleCard[DraggedCardIndex].GetCardFightData().id);
            CardData secondCardData = PlayerData.I.GetCardDataByID(_lsBattleCard[indexCardEnter].GetCardFightData().id);
            PlayerData.I.SwapFightCardsIndex(DraggedCardIndex, indexCardEnter);
            if (firstCardData != null && secondCardData != null)
            {
                _lsBattleCard[DraggedCardIndex].SetData(secondCardData);
                _lsBattleCard[indexCardEnter].SetData(firstCardData);
            }
            //Call API
            RestApiManager.I.PutBattleCardData(isSuccess =>
                {
                    if (isSuccess)
                    {
                        _cardFightList = PlayerData.I.GetFightCardList();
                    }
                    else
                    {
                        ShowErrorMsgDialog(Constant.ERROR_MESS_FAILED_PUT_DATA);
                    }
                },
                PlayerData.I.GetPlayerInfo().equippedCards);
        }
        //DraggedCardIndex = -1;
        indexCardEnter = -1;
        // CardData firstCardData = PlayerData.I.GetCardDataByID(_lsBattleCard[firstID].GetCardFightData().id);
        // CardData secondCardData = PlayerData.I.GetCardDataByID(_lsBattleCard[secondID].GetCardFightData().id);
        // PlayerData.I.SwapFightCardsIndex(firstID, secondID);
        //
        // if (firstCardData != null && secondCardData != null)
        // {
        //     _lsBattleCard[firstID].SetData(secondCardData);
        //     _lsBattleCard[secondID].SetData(firstCardData);
        // }
        //
        //
    }
    // public void SwapFightCards(int firstID, int secondID)
    // {
    //     CardData firstCardData = PlayerData.I.GetCardDataByID(_lsBattleCard[firstID].GetCardFightData().id);
    //     CardData secondCardData = PlayerData.I.GetCardDataByID(_lsBattleCard[secondID].GetCardFightData().id);
    //     PlayerData.I.SwapFightCardsIndex(firstID, secondID);
    //
    //     if (firstCardData != null && secondCardData != null)
    //     {
    //         _lsBattleCard[firstID].SetData(secondCardData);
    //         _lsBattleCard[secondID].SetData(firstCardData);
    //     }
    //
    //     //Call API
    //     RestApiManager.I.PutBattleCardData(isSuccess =>
    //     {
    //         if (isSuccess)
    //         {
    //             _cardFightList = PlayerData.I.GetFightCardList();
    //         }
    //         else
    //         {
    //             ShowErrorMsgDialog(Constant.ERROR_MESS_FAILED_PUT_DATA);
    //         }
    //     },
    //     PlayerData.I.GetPlayerInfo().equippedCards);
    // }

    public void BeginSwap() //Run after click "Use" button
    {
        MenuGUIController.I.EnableAllTab(false);
        myScroller.ScrollRect.enabled = false;
        isSwap = true;
        ResetChoseBattleCard();
        ShakeFightCards();
    }

    private void ShakeFightCards()
    {
        for (int i = 0; i < 8; i++)
            _lsBattleCard[i].GetComponent<RectTransform>().DOShakePosition(1800, 3);
    }

    public void OnSwaping(int index) //Run after click fight card
    {
        IsFlying = true;
        //Checking for duplicatecard

        if (cardSubToSwap.GetCardSubData().characterId == _lsBattleCard[index].GetCardFightData().characterId)
        {
            //Set data to temp CardData 
            fightCardTemp = PlayerData.I.GetCardDataByID(_lsBattleCard[index].GetCardFightData().id);
            subCardTemp = cardSubToSwap.GetCardSubData();

            PlayerData.I.SwapSubToFightCardIndex(index, cardSubToSwap.GetCardSubData().id);

            _cardFightList[index] = cardSubToSwap.GetCardSubData();
            
            cardSubToSwap.SetData(fightCardTemp, this);
            
            StartCoroutine(SwapCardAnimation(subCardTemp, index));
        }
        else
        {
            if (!PlayerData.I.CheckDuplicateCardInBattleCard(cardSubToSwap.GetCardSubData().characterId))
            {
                fightCardTemp = PlayerData.I.GetCardDataByID(_lsBattleCard[index].GetCardFightData().id);
                subCardTemp = cardSubToSwap.GetCardSubData();

                _cardFightList[index] = cardSubToSwap.GetCardSubData();

                PlayerData.I.SwapSubToFightCardIndex(index, cardSubToSwap.GetCardSubData().id);
                
                cardSubToSwap.SetData(fightCardTemp, this);

                StartCoroutine(SwapCardAnimation(subCardTemp, index));
                
            }
            else
            {
                StartCoroutine(ShowDuplicateCardError());             
            }
        }
    }

    public IEnumerator ShowDuplicateCardError ()
    {
        FinishSwap();
        _swapErrorDialog.SetActive(true);
        yield return new WaitForSeconds(2);
        _swapErrorDialog.SetActive(false);
    }

    private void OnDisable()
    {
        _swapErrorDialog.SetActive(false);
    }

    public IEnumerator SwapCardAnimation(CardData firstData, int index)
    {
        TurnOnTempCard(true);
        tempCard.SetData(firstData);
        tempCard.transform.position = cardSubToSwap.transform.position;


        cardSubToSwap.DOKill();
        //Moving animation
        _lsBattleCard[index].GetComponent<RectTransform>().DOMove(cardSubToSwap.oldPosition, 0.3f);
        tempCard.transform.DOMove(_lsBattleCard[index].transform.position, 0.3f);

        //Wait finish animation
        yield return new WaitForSeconds(0.3f);


        _lsBattleCard[index].SetData(firstData);
        TurnOnTempCard(false);
        UpdateCardSubList();

        //Call API
        RestApiManager.I.PutBattleCardData(isSuccess =>
        {
            if (isSuccess)
            {
                _cardFightList = PlayerData.I.GetFightCardList();
                UpdateFightAverage();
            }
            else
            {
                ShowErrorMsgDialog(Constant.ERROR_MESS_FAILED_PUT_DATA);
            }
        },
        PlayerData.I.GetPlayerInfo().equippedCards);

        FinishSwap();
        
        for (int j = 0; j < 8; j++)
        {
            if (_lsBattleCard[j].cardFreeLogo != null && _lsBattleCard[j].cardFreeLogo.activeSelf)
            {
                ShowErrorMsgDialog(Constant.ERROR_CARD_FREE_HAS_EXIST);
                break;
            }
        }
        
        AudioController.I.PlaySound(AudioClipDefines.I.ACTION_TAP_ON_BUTTON_SELECT_CARD);
    }

    public void UpdateCardSubList()
    {
        _cardSubList[cardSubToSwap.cellIndex] = cardSubToSwap.GetCardSubData();
    }

    private void TurnOnTempCard(bool check)
    {
        tempCard.gameObject.SetActive(check);
        if (check)
        {
            cardSubToSwap.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
            cardSubToSwap.GetComponent<CanvasGroup>().alpha = 1;
    }

    public void FinishSwap()
    {
        StopShakingCard();
        //Turn off swap state
        myScroller.ScrollRect.enabled = true;
        isSwap = false;
        IsFlying = false;
        if (!IsDragging)
            MenuGUIController.I.EnableAllTab();
    }

    private void StopShakingCard()
    {
        if (cardSubToSwap != null)
        {
            cardSubToSwap.OnChoseCard(false);
            for (int i = 0; i < 8; i++)
            {
                _lsBattleCard[i].GetComponent<RectTransform>().DORestart();
                _lsBattleCard[i].GetComponent<RectTransform>().DOKill();
            }
        }
    }

    #endregion

    #region Fight Cards Control

    public int cardIndexChose = -1;
    
    public void SetFightCardData()
    {
        if (_lsBattleCard == null || _lsBattleCard.Length < 8 || _cardFightList.Count < 8)
        {
            Debug.LogWarning("_lsBattleCard == null || _lsBattleCard.Length < 8 || _cardFightList.Count < 8");
            return;
        }
            
        float sum = 0;
        
        for (int i = 0; i < 8; i++)
        {
            _lsBattleCard[i].SetData(_cardFightList[i]);

            sum += _cardFightList[i].energy;
        }
        
        _averageTxt.text = string.Format(" {0:0.##}", sum / 8f);

        if (sortIndex == 0)
        {
            sortTxt.GetComponent<LocalizeStringEvent>().SetEntry("SORT_1");
        }
        else
        {
            sortTxt.GetComponent<LocalizeStringEvent>().SetEntry("SORT_2");
        }
        
        for (int j = 0; j < 8; j++)
        {
            if (_lsBattleCard[j].cardFreeLogo != null && _lsBattleCard[j].cardFreeLogo.activeSelf)
            {
                ShowErrorMsgDialog(Constant.ERROR_CARD_FREE_HAS_EXIST);
                break;
            }
        }
    }

    private void UpdateFightAverage()
    {
        float sum = 0;

        for (int i = 0; i < 8; i++)
        {
            sum += _cardFightList[i].energy;
        }

        _averageTxt.text = string.Format(" {0:0.##}", sum / 8f);
    }

    public void OnChoseBattleCard(int index)
    {
        if (index != cardIndexChose)
        {
            _lsBattleCard[index].ShowEquipBtn(true);

            if (cardIndexChose != -1)
            {
                _lsBattleCard[cardIndexChose].ShowEquipBtn(false);
            }

            cardIndexChose = index;
        }
        else
        {
            _lsBattleCard[cardIndexChose].ShowEquipBtn(false);
            cardIndexChose = -1;
        }
    }

    public void ResetChoseBattleCard()
    {
        if (cardIndexChose != -1)
        {
            _lsBattleCard[cardIndexChose].ShowEquipBtn(false);
            cardIndexChose = -1;
        }
    }

    public int GetCardFightIndex(Vector2 mousePos)
    {
        int idx = -1;

        for (int i = 0; i < _lsBattleCard.Length; i++)
        {
            if (i < _lsBattleCard[i].GetComponent<RectTransform>().GetSiblingIndex())
            {
                //Debug.Log("i = " + i);
                continue;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(_lsBattleCard[i].GetComponent<RectTransform>(), mousePos))
            {
                idx = i;
                break;
            }
        }

        return idx;
    }
    #endregion

    #region EnhancedScroller

    public void OnScrolling()
    {
        /*        
                SetHideButtonSubCardSelected();
                indexCardSelected = -1;*/
    }
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _cardSubList.Count;

    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 170f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        CardSubCellView cellView = scroller.GetCellView(_cardSubPrefab) as CardSubCellView;
        cellView.SetData(_cardSubList[dataIndex], this);
        return cellView;
    }

    public void SetHideButtonSubCardSelected()
    {
        if (cellViewSelected == null) return;
        var cardSubCellView = cellViewSelected.gameObject.GetComponent<CardSubCellView>();
        cardSubCellView.OnChoseCard(false);
    }

    #endregion

    #region Error Handler

    public void ShowErrorMsgDialog(string error)
    {
        ErrorText param = new ErrorText();
        param.errorString = error;
        DialogManager.I.Show(DIALOG_GUI.ErrorDialog, param);
    }

    #endregion

}
