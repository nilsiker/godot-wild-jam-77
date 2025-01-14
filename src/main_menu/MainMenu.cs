namespace Nevergreen;

using Godot;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;

public interface IMainMenu : IControl { }

[Meta(typeof(IAutoNode))]
public partial class MainMenu : Control, IMainMenu {
  #region Exports
  #endregion

  #region Nodes
  [Node] private ITextureRect Background { get; set; } = default!;
  [Node] private IButton StartGameButton { get; set; } = default!;
  [Node] private IButton OptionsButton { get; set; } = default!;
  [Node] private IButton CreditsButton { get; set; } = default!;
  [Node] private IButton QuitButton { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion

  #region State
  #endregion

  #region Dependency Lifecycle
  public void Setup() { }
  public void OnResolved() {
    StartGameButton.Pressed += () => Visible = false;
    QuitButton.Pressed += AppRepo.RequestQuitApp;
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(true);
    SetPhysicsProcess(true);
  }

  public void OnProcess(double delta) {
    var offset = ((FastNoiseLite)((NoiseTexture2D)Background.Texture).Noise).Offset;
    offset.Z += (float)delta;
    ((FastNoiseLite)((NoiseTexture2D)Background.Texture).Noise).Offset = offset;
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
