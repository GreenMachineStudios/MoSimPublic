using UnityEngine;

public class ChainDetector : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    [SerializeField] private BoxCollider[] chainColliders;
    [SerializeField] private BoxCollider[] hookColliders;

    public static bool isRedTouchingChain;
    public static bool isBlueTouchingChain;

    private void Update()
    {
        //Detect whether an alliances hooks are touching their chain
        foreach (var hookCollider in hookColliders)
        {
            foreach (var chainCollider in chainColliders)
            {
                if (hookCollider.bounds.Intersects(chainCollider.bounds))
                {
                    if (alliance == Alliance.Blue)
                    {
                        isBlueTouchingChain = true;
                    }
                    else if (alliance == Alliance.Red)
                    {
                        isRedTouchingChain = true;
                    }
                    break;
                }
                else 
                {
                    if (alliance == Alliance.Blue)
                    {
                        isBlueTouchingChain = false;
                    }
                    else if (alliance == Alliance.Red)
                    {
                        isRedTouchingChain = false;
                    }
                }
            }

            if ((alliance == Alliance.Blue && isBlueTouchingChain) || (alliance == Alliance.Red && isRedTouchingChain))
            {
                break;
            }
        }
    }
}
