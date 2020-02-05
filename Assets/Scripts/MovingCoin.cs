using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingCoin : MonoBehaviour
{
    Vector3 spawnPosition;
    Vector3 targetPosition;
    Slot targetSlot;
    Playground playground;

    private float starttime;
    private float journeyLength;

    public event EventHandler<DestinationReachedEventArgs> destinationReached;

    public void Initialize(Playground playground, Vector3 spawnPos, Slot targetSlot)
    {
        this.playground = playground;

        this.transform.position = spawnPos;
        this.spawnPosition = spawnPos;
        this.targetPosition = targetSlot.transform.position;

        this.targetSlot = targetSlot;

        this.starttime = Time.time;
        this.journeyLength = Vector3.Distance(spawnPos, targetPosition);

        SetCoinColor();
    }

    private void Update()
    {
        float distCovered = (Time.time - starttime) * 750;
        float fracJourney = distCovered / journeyLength;

        transform.position = Vector3.Lerp(spawnPosition, targetPosition, fracJourney);

        //destination reached
        if (transform.position.y == targetPosition.y)
        {
            Debug.Log("SameLocation");
            Destroy(this.gameObject);

            destinationReached?.Invoke(this, new DestinationReachedEventArgs(targetSlot));
        }
    }

    private void SetCoinColor()
    {
        Image coinImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (GameManager.currentPlayersTurn == SlotColor.Green)
        {
            coinImage.color = new Color32(49, 156, 90, 255);
        }
        else if (GameManager.currentPlayersTurn == SlotColor.Red)
        {
            coinImage.color = new Color32(126, 35, 27, 255);
        }
    }

}

public class DestinationReachedEventArgs
{
    public Slot targetSlot;

    public DestinationReachedEventArgs(Slot targetSlot)
    {
        this.targetSlot = targetSlot;
    }
}