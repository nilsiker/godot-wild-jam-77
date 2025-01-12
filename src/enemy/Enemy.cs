namespace Nevergreen;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IEnemy : IArea2D { }

[Meta(typeof(IAutoNode))]
public partial class Enemy : Area2D, IEnemy {
  #region Exports
  #endregion

  #region Nodes
  [Node]
  private AnimationPlayer AnimationPlayer { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region State
  private EnemyLogic Logic { get; set; } = default!;
  private EnemyLogic.IBinding Binding { get; set; } = default!;
  private Vector2 PlayerPosition { get; set; } = Vector2.Zero;  // TODO move this into EnemyData or something
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding.Handle(
      (in EnemyLogic.Output.PlayerTrackedAt output) =>
        OnOutputPlayerTrackedAt(output.GlobalPosition)
    ).Handle(
      (in EnemyLogic.Output.Damaged output) =>
        OnOutputDamaged(output.Amount)
    );

    Logic.Set(GameRepo);

    Logic.Start();
  }
  #endregion



  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(true);
    SetPhysicsProcess(true);

    BodyEntered += OnBodyEntered;
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) {
    var velocity = GlobalPosition.DirectionTo(PlayerPosition) * 10.0f * (float)GetPhysicsProcessDeltaTime();
    GlobalPosition += velocity;
  }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  private void OnBodyEntered(Node2D body) {
    if (body is IPlayer) {
      Logic.Input(new EnemyLogic.Input.OnPlayerTouched());
    }
  }
  #endregion

  #region Output Callbacks
  private void OnOutputPlayerTrackedAt(Vector2 globalPosition) =>
    PlayerPosition = globalPosition;

  private void OnOutputDamaged(float amount)
    => AnimationPlayer.Play("hit");

  #endregion
}

public interface IEnemyLogic : ILogicBlock<EnemyLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class EnemyLogic
  : LogicBlock<EnemyLogic.State>,
    IEnemyLogic {
  public override Transition GetInitialState() => To<State>();

  public static class Input {
    public record struct OnPlayerTouched;
  }

  public static class Output {
    public record struct PlayerTrackedAt(Vector2 GlobalPosition);
    public record struct Damaged(float Amount);
  }

  public partial record State : StateLogic<State>, IGet<Input.OnPlayerTouched> {
    public State() {
      OnAttach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.PlayerGlobalPosition.Sync += OnPlayerGlobalPositionSync;
      });
      OnDetach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.PlayerGlobalPosition.Sync -= OnPlayerGlobalPositionSync;
      });
    }

    public Transition On(in Input.OnPlayerTouched input) {
      Output(new Output.Damaged(1));
      return ToSelf();
    }

    private void OnPlayerGlobalPositionSync(Vector2 vector) =>
      Output(new Output.PlayerTrackedAt(vector));
  }
}
