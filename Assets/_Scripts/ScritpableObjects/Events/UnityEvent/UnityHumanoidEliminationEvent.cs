using UnityEngine.Events;

[System.Serializable]
public class UnityHumanoidEliminationEvent : UnityEvent<HumanoidElimination>
{
    
}

public class HumanoidElimination
{
    public int eliminatedHumanoidKey;
    public int lastDamagerKey;
    public UnityEngine.Material lastDamagerMat;

    public HumanoidElimination()
    {

    }

    public HumanoidElimination(int eliminatedHumanoidKey)
    {
        this.eliminatedHumanoidKey = eliminatedHumanoidKey;
    }

    public HumanoidElimination(int eliminatedHumanoidKey, int lastDamagerKey, UnityEngine.Material lastDamagerMat)
    {
        this.eliminatedHumanoidKey = eliminatedHumanoidKey;
        this.lastDamagerKey = lastDamagerKey;
        this.lastDamagerMat = lastDamagerMat;
    }
}