namespace Nevergreen;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IGameOverScreen : IPanelContainer { }

[Meta(typeof(IAutoNode))]
public partial class GameOverScreen : PanelContainer, IPanelContainer, IGameOverScreen {
  #region Exports
  #endregion

  #region Nodes
  [Node] private Button ReturnToMainMenuButton { get; set; } = default!;
  [Node] private Button TryAgainButton { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  [Dependency] private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion

  #region State
  private GameOverScreenLogic Logic { get; set; } = default!;
  private GameOverScreenLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding
      .Handle((in GameOverScreenLogic.Output.Hide _) => Visible = false)
      .Handle((in GameOverScreenLogic.Output.Show _) => Visible = true);

    Logic.Set(AppRepo);
    Logic.Set(GameRepo);

    Logic.Start();
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public override void _Ready() {
    ReturnToMainMenuButton.Pressed += OnReturnToMainMenuButtonPressed;
    TryAgainButton.Pressed += OnTryAgainButtonPressed;
    TryAgainButton.GrabFocus();
  }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
  }

  // FIXME REMOVE DEBUG
  public override void _UnhandledInput(InputEvent @event) {
    if (Input.IsKeyLabelPressed(Key.P)) {
      GameRepo.Lose();

    }
  }
  #endregion

  #region Input Callbacks
  private void OnReturnToMainMenuButtonPressed() => Logic.Input(new GameOverScreenLogic.Input.ClickToMainMenu());
  private void OnTryAgainButtonPressed() {
    Visible = false;
    GameRepo.RequestRoomTransition(ERoom.Stump);
    GameRepo.ResetPlayer();
  }

  #endregion


  #region Output Callbacks
  #endregion
}

public interface IGameOverScreenLogic : ILogicBlock<GameOverScreenLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class GameOverScreenLogic
  : LogicBlock<GameOverScreenLogic.State>,
    IGameOverScreenLogic {
  public override Transition GetInitialState() => To<State>();

  public static class Input {
    public record struct ClickToMainMenu;

  }

  public static class Output {
    public record struct Hide;
    public record struct Show;
  }

  public partial record State : StateLogic<State>, IGet<Input.ClickToMainMenu> {
    public State() {
      OnAttach(() => Get<IGameRepo>().GameOver += OnGameOver);
      OnDetach(() => Get<IGameRepo>().GameOver -= OnGameOver);

      this.OnEnter(() => Output(new Output.Hide()));
    }

    public Transition On(in Input.ClickToMainMenu input) {
      Output(new Output.Hide());
      Get<IAppRepo>().RequestMainMenu();
      return ToSelf();
    }

    private void OnGameOver(EGameOverReason reason) {
      Get<IGameRepo>().Pause();
      if (reason == EGameOverReason.Lost) {
        Output(new Output.Show());
      }
    }
  }
}
