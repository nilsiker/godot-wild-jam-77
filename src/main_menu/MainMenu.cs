namespace Nevergreen;

using Godot;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;

public interface IMainMenu : IControl {
  public void Open();
}

[Meta(typeof(IAutoNode))]
public partial class MainMenu : Control, IMainMenu {
  #region Exports
  #endregion

  #region Nodes
  [Node] private IButton StartGameButton { get; set; } = default!;
  [Node] private IButton OptionsButton { get; set; } = default!;
  [Node] private IButton CreditsButton { get; set; } = default!;
  [Node] private IButton QuitButton { get; set; } = default!;
  [Node] private PanelContainer Options { get; set; } = default!;
  [Node] private PanelContainer Credits { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion

  #region State
  #endregion

  public void Open() {
    Visible = true;
    StartGameButton.GrabFocus();
    Options.Visible = false;
    Credits.Visible = false;  // HACKY
  }

  #region Dependency Lifecycle
  public void Setup() { }
  public void OnResolved() {
    StartGameButton.Pressed += OnStartGameButtonPressed;
    QuitButton.Pressed += AppRepo.RequestQuitApp;
    OptionsButton.Pressed += OnOptionsButtonPressed;
    CreditsButton.Pressed += OnCreditsButtonPressed;
    StartGameButton.GrabFocus();
  }

  private void OnCreditsButtonPressed() {
    Credits.Visible = !Credits.Visible;
    Options.Visible = false;
  }

  private void OnOptionsButtonPressed() {
    Options.Visible = !Options.Visible;
    Credits.Visible = false;
  }
  private void OnStartGameButtonPressed() => AppRepo.RequestGameStart();
  #endregion


  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(true);
    SetPhysicsProcess(true);
  }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree() {
  }
  #endregion

  #region Input Callbacks
  #endregion

  #region Output Callbacks
  #endregion
}
