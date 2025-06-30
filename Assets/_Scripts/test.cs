using UnityEngine;

public class test : MonoBehaviour
{
    public VoidEvent makePool;

    private void Awake()
    {
        makePool.Raise();
    }

    public float speed = 1f;
    float a = 0;

    private void Update()
    {
        a += speed * Time.unscaledDeltaTime;
        GetComponent<Renderer>().material.SetFloat("_Speed", a);
    }

}
