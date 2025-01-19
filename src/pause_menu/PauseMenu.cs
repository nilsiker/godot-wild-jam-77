namespace Nevergreen;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IPauseMenu : IControl {
}

[Meta(typeof(IAutoNode))]
public partial class PauseMenu : Control, IPauseMenu {
  #region Nodes
  [Node] private Button ResumeButton { get; set; } = default!;
  [Node] private Button BackToMainMenuButton { get; set; } = default!;
  #endregion


  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  [Dependency] private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public bool HackyDumbOpenState { get; set; }

  public void OnReady() {
    ResumeButton.Pressed += Close;
    BackToMainMenuButton.Pressed += AppRepo.RequestMainMenu;
  }

  public void Open() {
    Visible = true;
    ResumeButton.GrabFocus();
    HackyDumbOpenState = true;
  }

  public void Close() {
    GameRepo.Resume();
    Visible = false;
    HackyDumbOpenState = false;
  }

  public override void _Input(InputEvent @event) {
    if (HackyDumbOpenState && @event.IsActionPressed(Inputs.Esc)) {
      // HACKY
      Close();
      GetViewport().SetInputAsHandled();
    }
  }
  #endregion
}
