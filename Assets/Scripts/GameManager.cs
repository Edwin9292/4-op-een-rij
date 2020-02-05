using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject PlayerWonPanel;
    [SerializeField] Playground playground;

    public static bool canPlace = true;

    private const int Height = 6;
    private const int Width = 7;

    public static SlotColor currentPlayersTurn = SlotColor.Red;

    
    void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
        }
    }

    private void StartGame()
    {
        playground.Initialize(this, Width, Height);
    }

    private void ResetGame()
    {
        playground.Reset();
        playground.EnableCursorSlot();
        DisablePlayerWonPanel();
    }

        
    public static void UpdatePlayersTurn()
    {
        if(currentPlayersTurn == SlotColor.Red)
        {
            currentPlayersTurn = SlotColor.Green;
        }
        else
        {
            currentPlayersTurn = SlotColor.Red;
        }
    }
    
    public void GameFinished()
    {
        PlayerWonPanel.GetComponentInChildren<TMP_Text>().text = $"CONGRATULATIONS!!! \n Player {currentPlayersTurn} won";
        PlayerWonPanel.SetActive(true);
    }

    public void DisablePlayerWonPanel()
    {
        PlayerWonPanel.SetActive(false);
    }

    /*
     
    public void SpawnMovingCoin(Vector3 startPos, Vector3 targetPos, Slot targetSlot)
    {
        MovingCoin movingCoin = Instantiate(MovingCoinPrefab, GameObject.Find("GameBackground").gameObject.transform).GetComponent<MovingCoin>() ;
        movingCoin.gameObject.name = "MovingCoin";
        movingCoin.Initialize(playground, this, startPos, targetPos, targetSlot);
    }


    public void SetCoinToPlaceLocation(Slot slot)
    {
        currentSelectedSlot = slot;
        Debug.Log("Current selected slot = " + currentSelectedSlot.slotCoordinate);
        Vector3 coordinateToPlace = slot.transform.position;
        CoinToPlace.transform.position = coordinateToPlace;
        CoinToPlace.gameObject.SetActive(true); 
    }

    private bool CanSlotColorBeChanged(Slot slotToUpdate)
    {
        //if the slot isn't empty, return;
        if (slotToUpdate.GetSlotColor() != PlayerColor.None)
            return false;

        if (slotToUpdate.slotCoordinate.y == 0)
            return true;

        //if the slot below is empty, you cant place here;
        Vector2Int below = slotToUpdate.slotCoordinate + SlotBelow;
        if (IsSlotEmpty(below))
            return false;

        return true;
    }
    */

    /*
    public Slot GetFirstPlaceableSlotBelow(Slot slot)
    {
        int XcoordToCheck = slot.slotCoordinate.x;

        for (int i = slot.slotCoordinate.y; i > 0; i--)
        {
            Vector2Int slotBelow = new Vector2Int(XcoordToCheck, i) + SlotBelow;

            if(IsSlotEmpty(slotBelow))
            {
                continue;
            }
            else
            {
                return Playground[XcoordToCheck, i];
            }
        }
        return Playground[XcoordToCheck, 0];
    }
    */

}
