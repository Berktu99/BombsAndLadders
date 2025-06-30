
[UnityEngine.CreateAssetMenu(fileName = "Event_Void_", menuName ="CustomUnityEvents/Void Event")]
public class VoidEvent : BaseGameEvent<Void>
{
    public void Raise() => Raise(new Void());
}
