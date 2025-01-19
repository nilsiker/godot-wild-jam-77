namespace Woodblight;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IHeartsIndicator : IAnimatedSprite2D { }

[Meta(typeof(IAutoNode))]
public partial class HeartsIndicator : AnimatedSprite2D, IHeartsIndicator {
  #region Exports
  #endregion

  #region Nodes
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IPlayerRepo PlayerRepo => this.DependOn<IPlayerRepo>();
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region State
  private HeartsIndicatorLogic Logic { get; set; } = default!;
  private HeartsIndicatorLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here

    PlayerRepo.Health.Sync += OnPlayerHealthSync;
    GameRepo.GameOver += (reason) => {
      if (reason == EGameOverReason.Won) {
        Visible = false;
      }
    };
    GameRepo.RoomTransitionRequested += (room) => {
      if (room == ERoom.Glade) {
        Visible = true;
      }
      else if (room == ERoom.Stump) {
        Visible = false;
      }

    };
    Logic.Start();
  }

  private void OnPlayerHealthSync(int health) => Frame = health;
  #endregion


  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(true);
    SetPhysicsProcess(true);
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  #endregion

  #region Output Callbacks
  #endregion
}

public interface IHeartsIndicatorLogic : ILogicBlock<HeartsIndicatorLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class HeartsIndicatorLogic
  : LogicBlock<HeartsIndicatorLogic.State>,
    IHeartsIndicatorLogic {
  public override Transition GetInitialState() => To<State>();

  public static class Input { }

  public static class Output { }

  public partial record State : StateLogic<State> {
    public State() {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}
