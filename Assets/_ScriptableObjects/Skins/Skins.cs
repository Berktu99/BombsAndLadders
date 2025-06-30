using MyBox;

public abstract class SingleSkin
{
    public string name;

    public bool isCostVideo = false;
    
    [ConditionalField(nameof(isCostVideo), false)] public int videoCost;
    [ConditionalField(nameof(isCostVideo), false)] public int videoWatched;

    [ConditionalField(nameof(isCostVideo), true)] public int cost;
}

