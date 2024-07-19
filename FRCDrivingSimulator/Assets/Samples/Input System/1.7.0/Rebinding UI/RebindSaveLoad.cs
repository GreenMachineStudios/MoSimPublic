using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    public InputActionAsset actions;
    public bool isOtherControlsAsset = false;
    string text = "rebinds";

    public void OnEnable()
    {
        if (isOtherControlsAsset) 
        {
            text = "rebinds2";
        }
        var rebinds = PlayerPrefs.GetString(text);
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }

    public void OnDisable()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(text, rebinds);
    }
}
