using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject transitionsContainer;
    private SceneTransition[] transitions;

    private void Awake() 
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start() 
    {
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
    }

    public void LoadScene(string sceneName, string transitionName) 
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    public void PlayTransition(string transitionName) 
    {
        StartCoroutine(Transition(transitionName));
    }

    private IEnumerator Transition(string transitionName) 
    {
        SceneTransition transition = transitions.First(t => t.name == transitionName);

        yield return transition.AnimateTransitionIn();
        
        yield return transition.AnimateTransitionOut();
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        SceneTransition transition = transitions.First(t => t.name == transitionName);

        yield return transition.AnimateTransitionIn();

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        
        while (!scene.isDone) 
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        yield return transition.AnimateTransitionOut();
    }
}
