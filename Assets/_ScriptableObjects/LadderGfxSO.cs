using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LadderGfxSO : ScriptableObject, IMustInitialize
{
    public Renderer[] rends;
    public Material greyMat;
    public Transform poolParentTr;

    public void Initialize()
    {
        
    }

    public void setMaterial(Material mat)
    {
        foreach (Renderer rend in rends)
        {
            rend.material = mat;
        }
    }

    public void setLadder(Material mat)
    {
        foreach (Renderer rend in rends)
        {
            rend.material = mat;
        }
    }

    public void repool()
    {
        //foreach (Renderer rend in rends)
        //{
        //    rend.sharedMaterial = greyMat;
        //}

        //GetComponent<Collider>().enabled = false;

        //Destroy(this.GetComponent<Rigidbody>());

        //this.transform.parent = poolParentTr;
        //this.transform.localPosition = Vector3.zero;
        //this.transform.localRotation = Quaternion.identity;

        //this.gameObject.SetActive(false);
    }
}
