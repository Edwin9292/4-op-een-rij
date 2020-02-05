using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Playground : MonoBehaviour
{
    [SerializeField] GameObject SlotHolder;
    [SerializeField] GameObject SlotPrefab;
    [SerializeField] GameObject CursorSlot;
    [SerializeField] GameObject MovingCoinPrefab;

    GameManager manager;

    Slot cursorSlot;

    private Slot[,] PlayingField;
    
    public void Initialize(GameManager manager, int Width, int Height)
    {
        this.manager = manager;

        this.PlayingField = new Slot[Width, Height];
        
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Slot slot = CreateSlot(x, y);

                PlayingField[x, y] = slot;
            }
        }

        this.gameObject.SetActive(true);
    }
    
    public void Reset()
    {
        foreach(Slot slot in PlayingField)
        {
            slot.SetSlotColor(SlotColor.Empty);
        }
    }

    private Slot CreateSlot(int x, int y)
    {
        Slot slot = Instantiate(SlotPrefab, SlotHolder.transform).GetComponent<Slot>();
        slot.Initialize(new Vector2Int(x, y));

        slot.name = $"{x},{y}";

        slot.slotSelected += SetCursorSlotPosition;

        slot.slotClicked += SpawnMovingCoin;
        slot.slotClicked += DisableCursorSlot;

        return slot;
    }


    public void ChangeSlotColor(Slot slotToUpdate, SlotColor color)
    {
        if (!CanSlotColorBeChanged(slotToUpdate))
            return;

        slotToUpdate.SetSlotColor(color);
    }


    #region Get/Find Slots
    public Slot GetLowestEmptySlotInCollum(int xCoord)
    {
        for (int i = 0; i < PlayingField.GetLength(1); i++)
        {
            Slot slot = PlayingField[xCoord, i];

            if (!IsSlotEmpty(slot))
                continue;
            else
                return slot;
        }
        return null;
    }

    public Slot GetSlotByCoordinate(Vector2Int coordinates)
    {
        if (AreSlotCoordinatesInPlayground(coordinates))
            return PlayingField[coordinates.x, coordinates.y];
        else
            return null;
    }

    public Slot GetNeighbourSlot(Slot slotToCheck, Direction direction)
    {
        switch (direction)
        {
            case Direction.Below:
                return GetSlotByCoordinate(slotToCheck.slotCoordinate + new Vector2Int(0, -1));

            case Direction.LeftBelow:
                return GetSlotByCoordinate(slotToCheck.slotCoordinate + new Vector2Int(-1, -1));

            case Direction.Left:
                return GetSlotByCoordinate(slotToCheck.slotCoordinate + new Vector2Int(-1, 0));

            case Direction.LeftAbove:
                return GetSlotByCoordinate(slotToCheck.slotCoordinate + new Vector2Int(-1, 1));

            case Direction.Above:
                return GetSlotByCoordinate(slotToCheck.slotCoordinate + new Vector2Int(0, 1));

            case Direction.RightAbove:
                return GetSlotByCoordinate(slotToCheck.slotCoordinate + new Vector2Int(1, 1));

            case Direction.Right:
                return GetSlotByCoordinate(slotToCheck.slotCoordinate + new Vector2Int(1, 0));

            case Direction.RightBelow:
                return GetSlotByCoordinate(slotToCheck.slotCoordinate + new Vector2Int(1, -1));

            default:
                throw new Exception("Invalid direction");
        }
    }
    #endregion

    #region Validations
    private bool CanSlotColorBeChanged(Slot slotToUpdate)
    {
        if (!IsSlotEmpty(slotToUpdate))
            return false;

        if (slotToUpdate.slotCoordinate.y == 0)
            return true;

        Slot slotBelow = GetNeighbourSlot(slotToUpdate, Direction.Below);
        if (IsSlotEmpty(slotBelow))
            return false;

        return true;
    }

    public bool IsSlotEmpty(Slot slot)
    {
        if (slot.GetSlotColor() == SlotColor.Empty)
            return true;
        else
            return false;
    }

    private bool AreSlotCoordinatesInPlayground(Vector2Int coordinate)
    {
        if (coordinate.y < 0 || coordinate.y >= PlayingField.GetLength(1))
            return false;
        if (coordinate.x < 0 || coordinate.x >= PlayingField.GetLength(0))
            return false;

        return true;
    }

    #endregion

    #region FourInARowChecks
    private bool IsFourInARow(Slot slot)
    {
        if (CheckHorizontal(slot) >= 4 ||
            CheckVertical(slot) >= 4 ||
            CheckDiagonalLeftBelowToRightAbove(slot) >= 4 ||
            CheckDiagionalLeftAboveToRightBelow(slot) >= 4)
        {
        return true;
        }
        return false;
    }


    private int CheckHorizontal(Slot slot)
    {
        int counter = 1;
        counter += CheckSlot(slot, Direction.Left);
        counter += CheckSlot(slot, Direction.Right);
        return counter;
    }

    private int CheckVertical(Slot slot)
    {
        int counter = 1;
        counter += CheckSlot(slot, Direction.Above);
        counter += CheckSlot(slot, Direction.Below);
        return counter;
    }

    private int CheckDiagionalLeftAboveToRightBelow(Slot slot)
    {
        int counter = 1;
        counter += CheckSlot(slot, Direction.LeftAbove);
        counter += CheckSlot(slot, Direction.RightBelow);
        return counter;
    }

    private int CheckDiagonalLeftBelowToRightAbove(Slot slot)
    {
        int counter = 1;
        counter += CheckSlot(slot, Direction.LeftBelow);
        counter += CheckSlot(slot, Direction.RightAbove);
        return counter;
    }

    private int CheckSlot(Slot slot, Direction direction)
    {
        Slot slotToCheck = GetNeighbourSlot(slot, direction);

        if (slotToCheck == null)
            return 0;
        
        if (IsSlotEmpty(slotToCheck))
            return 0;

        if (slotToCheck.GetSlotColor() == slot.GetSlotColor())
        {
            return 1 + CheckSlot(slotToCheck, direction);
        }

        else return 0;
    }

    #endregion


    #region CursorSlotMethods
    private void InstantiateCursorSlot(SlotColor color)
    {
        cursorSlot = Instantiate(CursorSlot, this.transform).GetComponent<Slot>();
        cursorSlot.transform.SetAsFirstSibling();
        cursorSlot.name = "CursorSlot";
        cursorSlot.SetSlotColor(color);
    }

    private void SetCursorSlotPosition(object sender, EventArgs args)
    {
        Slot slot = (Slot)sender;

        if (cursorSlot == null)
            InstantiateCursorSlot(GameManager.currentPlayersTurn);

        cursorSlot.transform.position = slot.transform.position;
    }

    private void DisableCursorSlot(object sender, EventArgs args)
    {
        cursorSlot.gameObject.SetActive(false);
        GameManager.canPlace = false;
    }

    public void EnableCursorSlot()
    {
        cursorSlot.gameObject.SetActive(true);
        GameManager.canPlace = true;
    }

    #endregion

    #region MovingCoinMethods     
    public void SpawnMovingCoin(object sender, EventArgs data)
    {
        Slot clickedSlot = (Slot)sender;
        Slot targetSlot = GetLowestEmptySlotInCollum(clickedSlot.slotCoordinate.x);

        MovingCoin movingCoin = Instantiate(MovingCoinPrefab, GameObject.Find("GameBackground").gameObject.transform).GetComponent<MovingCoin>();
        movingCoin.transform.SetSiblingIndex(0); 
        movingCoin.gameObject.name = "MovingCoin";

        movingCoin.destinationReached += OnMovingCoinDestinationReached;

        movingCoin.Initialize(this, clickedSlot.transform.position, targetSlot);

    }
    private void OnMovingCoinDestinationReached(object sender, DestinationReachedEventArgs args)
    {
        ChangeSlotColor(args.targetSlot, GameManager.currentPlayersTurn);

        if (IsFourInARow(args.targetSlot))
        {
            manager.GameFinished();
            return;
        }

        GameManager.UpdatePlayersTurn();
        cursorSlot.SetSlotColor(GameManager.currentPlayersTurn);
        EnableCursorSlot();

    }
    #endregion

}


































/*

public Vector2Int SlotBelow = new Vector2Int(0, -1);
public Vector2Int SlotLeftBelow = new Vector2Int(-1, -1);
public Vector2Int SlotLeft = new Vector2Int(-1, 0);
public Vector2Int SlotLeftAbove = new Vector2Int(-1, 1);
public Vector2Int SlotAbove = new Vector2Int(0, 1);
public Vector2Int SlotRightAbove = new Vector2Int(1, 1);
public Vector2Int SlotRight = new Vector2Int(1, 0);
public Vector2Int SlotRightBelow = new Vector2Int(1, -1);


public Slot GetFirstPlaceableSlotBelow(Slot slot)
{
    int XcoordToCheck = slot.slotCoordinate.x;

    for (int i = slot.slotCoordinate.y; i > 0; i--)
    {
        Vector2Int slotBelow = new Vector2Int(XcoordToCheck, i) + SlotBelow;

        if (IsSlotEmpty(slotBelow))
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
