namespace Nevergreen;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IPauseMenu : IControl { }

[Meta(typeof(IAutoNode))]
public partial class PauseMenu : Control, IPauseMenu {
  #region Nodes
  [Node] private Button ResumeButton { get; set; }
  [Node] private Button BackToMainMenuButton { get; set; }
  #endregion


  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  [Dependency] private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    ResumeButton.Pressed += OnResumeButtonPressed;
    BackToMainMenuButton.Pressed += AppRepo.RequestMainMenu;
  }

  public void Open() {
    Visible = true;
    ResumeButton.GrabFocus();
  }

  private void OnResumeButtonPressed() {
    GameRepo.Resume();
    Visible = false;
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(Inputs.Esc)) {
      Visible = false;
      GetTree().Paused = false; // FIXME HACKY!!!
    }
  }
  #endregion
}
