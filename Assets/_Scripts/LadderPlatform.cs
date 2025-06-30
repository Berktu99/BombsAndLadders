using System.Collections.Generic;
using UnityEngine;

public class LadderPlatform : MonoBehaviour, IamTarget
{
    [SerializeField] private Material greyMaterial;
    [SerializeField] private Renderer rend;
    [SerializeField] private Transform ladderStackParent;

    public int key = -1;

    public Humanoid climbingHumanoid = null;

    public bool isAssignedToHumanoid = false;

    private void Awake()
    {
        key = -1;
    }    

    public bool compareKey(int key)
    {
        if (this.key == key)
        {
            return true;
        }
        return false;
    }

    public void assignHumanoid(HumanoidPickUpVariables variables)
    {
        this.key = variables.keyValue;
        rend.material.color = variables.color;
        isAssignedToHumanoid = true;
    }

    private void resetLadderPlatform()
    {
        rend.sharedMaterial.color = Color.gray;
        key = -1;
    }

    public void onTheLadder(Humanoid h)
    {
        climbingHumanoid = h;
    }

    public void offTheLadder()
    {
        climbingHumanoid = null;
    }

    public void tryExplodeLadders()
    {
        if (ladderStackParent.childCount > 0)
        {
            // i need also to inform humanoid script if anyone climnig this,
            // human will fall.

            if (climbingHumanoid != null)
            {
                float height = climbingHumanoid.transform.position.y - this.transform.position.y;
                climbingHumanoid.fallDownLadder(height);
            }


            for (int i = ladderStackParent.childCount - 1; i >= 0; i--)
            {
                ladderStackParent.GetChild(i).GetComponent<LadderGFX>().simulatePhysics(Vector3.one, Vector3.one, 3f);
            }

            if (ladderStackParent.childCount == 0)
            {
                resetLadderPlatform();
            }

            //for (int i = 0; i < ladderStackParent.childCount; i++)
            //{
            //    ladderStackParent.GetChild(i).GetComponent<LadderGFX>().simulatePhysics(Vector3.one, Vector3.one, 3f);
            //}
        }
        
    }
}
