using UnityEngine;

public class SpawnFieldNotes : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform[] locations;

    private void Start() 
    {
        InstantiateNotes();
    }

    public void InstantiateNotes() 
    {
        foreach (Transform location in locations) 
        {
            Instantiate(notePrefab, location.position, location.rotation);
        }
    }
}
