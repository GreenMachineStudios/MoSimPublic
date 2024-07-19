using System.Collections;
using UnityEngine;

public class RingBehaviour : MonoBehaviour
{
    private Vector3 originalScale = new Vector3(0.55f, 0.55f, 0.55f);

    void Update()
    {
        if (transform.position.y < -20)
        {
            Destroy(gameObject);
        }
    }

    public void DownSideForceLeft() 
    {
        gameObject.GetComponent<ConstantForce>().force = new Vector3(gameObject.GetComponent<ConstantForce>().force.x, -0.2f, gameObject.GetComponent<ConstantForce>().force.z);
        StartCoroutine(Timer(false));
    }

    public void DownSideForceRight() 
    {
        gameObject.GetComponent<ConstantForce>().force = new Vector3(gameObject.GetComponent<ConstantForce>().force.x, -0.2f, gameObject.GetComponent<ConstantForce>().force.z);
        StartCoroutine(Timer(true));
    }

    public IEnumerator UnSquishhhh() 
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;
        float duration = 0.15f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(originalScale, startScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private IEnumerator Timer(bool goOtherWay)
    {
        yield return new WaitForSeconds(0.8f);
        if (goOtherWay) 
        {
            gameObject.GetComponent<ConstantForce>().force = new Vector3(0.3f, 0f, gameObject.GetComponent<ConstantForce>().force.z);
        }
        else 
        {
            gameObject.GetComponent<ConstantForce>().force = new Vector3(-0.3f, 0f, gameObject.GetComponent<ConstantForce>().force.z);
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject.GetComponent<ConstantForce>());
    }

    private void OnTriggerEnter(Collider other) 
    {
        bool isBlueRobotInRedZone = ZoneControl.blueRobotInRedZoneUpdated;
        bool isRedRobotInBlueZone = ZoneControl.redRobotInBlueZoneUpdated;

        bool isOtherBlueRobotInRedZone = ZoneControl.blueOtherRobotInRedZoneUpdated;
        bool isOtherRedRobotInBlueZone = ZoneControl.redOtherRobotInBlueZoneUpdated;

        if ((other.CompareTag("Player") && !isBlueRobotInRedZone) ||
            (other.CompareTag("Player2") && !isOtherBlueRobotInRedZone) ||
            (other.CompareTag("RedPlayer") && !isRedRobotInBlueZone) ||
            (other.CompareTag("RedPlayer2") && !isOtherRedRobotInBlueZone)) 
        {
            tag = "Ring";
        }
        else if (other.CompareTag("Ring"))
        {
            tag = "Ring";
        }
    }
}
