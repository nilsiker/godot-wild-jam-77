namespace Nevergreen;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;


public interface IGame : INode2D, IProvide<IGameRepo> {
  public void StartGame();
}

[Meta(typeof(IAutoNode))]
public partial class Game : Node2D, IGame {
  #region State
  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic Logic { get; set; } = default!;
  public GameLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions

  IGameRepo IProvide<IGameRepo>.Value() => GameRepo;
  #endregion

  #region Dependencies
  [Dependency]
  private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion


  #region Dependency Lifecycle
  public void Setup() => Logic = new GameLogic();

  public void OnResolved() {
    GameRepo = new GameRepo();
    Logic.Set(GameRepo);
    Logic.Set(AppRepo);

    Binding = Logic.Bind();
    Binding.Handle((in GameLogic.Output.SetPauseMode output) => SetGamePaused(output.IsPaused));

    this.Provide();
  }
  #endregion


  #region Input Callbacks
  public void StartGame() => Logic.Input(new GameLogic.Input.StartGame());
  #endregion

  #region Output Callbacks
  private void SetGamePaused(bool isPaused) => GetTree().Paused = isPaused;
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public override void _Input(InputEvent @event) {
    if (Input.IsActionJustPressed(Inputs.Esc)) {
      Logic.Input(new GameLogic.Input.PauseButtonPressed());
    }
  }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
    GameRepo.Dispose();
  }
  #endregion
}


public interface IGameLogic : ILogicBlock<GameLogic.State>;

[Meta, LogicBlock(typeof(State), Diagram = true)]
public partial class GameLogic : LogicBlock<GameLogic.State>, IGameLogic {
  public override Transition GetInitialState() => To<State>();

  public abstract partial record State : StateLogic<State>;

  public static class Input {
    public readonly record struct PauseButtonPressed;
    public readonly record struct StartGame;
  }

  public static class Output {
    public readonly record struct SetPauseMode(bool IsPaused);
  }
}
