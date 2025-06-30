using UnityEngine.Events;

[System.Serializable]
public class UnityPickUpPickedUpEvent : UnityEvent<PickUpPickedUp>
{
    
}

public class PickUpPickedUp
{
    public int dePooledFloor;
    public int dePooledIndex;

    public PickUpPickedUp(int dePooledFloor, int dePooledIndex)
    {
        this.dePooledFloor = dePooledFloor;
        this.dePooledIndex = dePooledIndex;
    }
}