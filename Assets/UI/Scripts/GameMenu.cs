#nullable enable

using SimpleVoxelTanks.UI;

using UnityEngine;
using UnityEngine.UI;

public class GameMenu : AbstractMenu
{
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    private GameMenuInput _inputActions;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

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

    protected void OnDisable () => _inputActions.Disable();

    protected void OnEnable () => _inputActions.Enable();

    protected void ToggleActiveStarted (UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_canvas.activeSelf)
            HideMenu();
        else
            ShowMenu();
    }
}