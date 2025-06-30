using UnityEngine;
using System.Collections;

public class LadderGFX : MonoBehaviour, IMustInitialize
{
    [SerializeField] private Renderer selfRenderer;
    [SerializeField] private Transform poolParentTr;

    public void Initialize()
    {
        poolParentTr = transform.parent;
    }

    public void matchColors(Color32 color)
    {
        selfRenderer.material.color = color; 
    }

    public void simulatePhysics(Vector3 force, Vector3 torque, float simulateTime = 3f)
    {
        repool();
        //GetComponent<Collider>().enabled = true;

        //gameObject.AddComponent<Rigidbody>();
        //GetComponent<Rigidbody>().isKinematic = false;
        //GetComponent<Rigidbody>().AddExplosionForce(5f, this.transform.position, 4f);
        //GetComponent<Rigidbody>().AddTorque(torque);

        //StartCoroutine(repoolRoutine());

        //IEnumerator repoolRoutine()
        //{
        //    yield return new WaitForSeconds(simulateTime);
        //    repool();
        //}
    }

    public void repool()
    {
        selfRenderer.material.color = Color.gray;

        GetComponent<Collider>().enabled = false;

        Destroy(this.GetComponent<Rigidbody>());

        this.transform.parent = poolParentTr;
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;

        this.gameObject.SetActive(false);
    }

}
