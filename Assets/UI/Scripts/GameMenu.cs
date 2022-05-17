using SimpleVoxelTanks.UI;

using UnityEngine;
using UnityEngine.UI;

public class GameMenu : AbstractMenu
{
    private GameMenuInput _inputActions;

    #region Buttons

    [SerializeField]
    protected Button _mainMenuButton;

    #endregion Buttons

    protected override void Awake ()
    {
        base.Awake();
        _inputActions = new();
        _inputActions.Menu.ToggleActive.started += ToggleActiveStarted;
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
        HideMenu();
    }

    protected void LoadMainMenu () => LevelLoader.GoToMainMenu();

    protected void OnDisable ()
    {
        _inputActions.Disable();
    }

    protected void OnEnable ()
    {
        _inputActions.Enable();
    }

    protected void ToggleActiveStarted (UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_canvas.activeSelf)
            HideMenu();
        else
            ShowMenu();
    }
}