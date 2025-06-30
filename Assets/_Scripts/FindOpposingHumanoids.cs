using UnityEngine;

public class FindOpposingHumanoids : MonoBehaviour
{
    public Humanoid[] GetOpposingHumanoids(Humanoid h)
    {
        Humanoid[] allHumanoids = FindObjectsOfType<Humanoid>(true);
        Humanoid[] opponentHumanoids = new Humanoid[allHumanoids.Length - 1];

        int j = 0;
        for (int i = 0; i < allHumanoids.Length; i++)
        {
            if (allHumanoids[i] != h)
            {
                opponentHumanoids[j] = allHumanoids[i];
                j++;
            }
        }

        return opponentHumanoids;
    }
}
