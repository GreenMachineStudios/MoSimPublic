using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    private Crescendo controls;

    private void Start() 
    {
        controls = new Crescendo();
        controls.Robot.Enable();
    }

    private void Update()
    {
        if (controls.Robot.Menu.IsPressed() || Input.GetKeyDown(KeyCode.Escape)) 
        {
            LoadMenu();
        }
    }

    public void LoadMenu() 
    {
        LevelManager.Instance.LoadScene("MainMenu", "CrossFade");
    }
}
