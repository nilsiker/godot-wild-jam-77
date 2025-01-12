namespace Nevergreen;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;


public interface IPlayer : ICharacterBody2D, IStateDebugInfo { }

[Meta(typeof(IAutoNode))]
public partial class Player : CharacterBody2D, IPlayer {

  #region Exports
  #endregion

  #region Nodes
  [Node]
  private ISprite2D PlayerModel { get; set; } = default!;
  [Node]
  private IAnimationPlayer AnimationPlayer { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region State
  private PlayerLogic Logic { get; set; } = default!;
  private PlayerLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region IStateDebugInfo
  string IStateDebugInfo.Name => Name;
  public string State => Logic.Value.GetType().Name;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding.Handle(
      (in PlayerLogic.Output.VelocityUpdated output) => OnOutputVelocityUpdated(output.Velocity)
    ).Handle(
      (in PlayerLogic.Output.AnimationUpdated output) => OnOutputAnimationUpdated(output.Animation)
    ).Handle(
      (in PlayerLogic.Output.FlipSprite output) => OnOutputFlipSprite(output.Flip)
    );


    Logic.Set(GameRepo);
    Logic.Set(new PlayerLogic.Data() {
      Speed = 100f
    });

    AddToGroup("state_debug");
    Logic.Start();
  }

  #endregion




  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(true);
    SetPhysicsProcess(true);
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) {
    var inputDirection = Input.GetVector(Inputs.Left, Inputs.Right, Inputs.Up, Inputs.Down);
    Move(inputDirection);

    Logic.Input(new PlayerLogic.Input.UpdateGlobalPosition(GlobalPosition));
    MoveAndSlide();
  }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion


  #region Input Callbacks
  private void Move(Vector2 direction) => Logic.Input(new PlayerLogic.Input.Move(direction));
  #endregion

  #region Output Callbacks
  private void OnOutputVelocityUpdated(Vector2 velocity) => Velocity = velocity;
  private void OnOutputAnimationUpdated(StringName animation) => AnimationPlayer.Play(animation);
  private void OnOutputFlipSprite(bool flip) => PlayerModel.FlipH = flip;

  #endregion
}

public interface IPlayerLogic : ILogicBlock<PlayerLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class PlayerLogic
  : LogicBlock<PlayerLogic.State>,
    IPlayerLogic {
  protected override void HandleError(Exception e) => throw e;
  public override Transition GetInitialState() => To<State.Idle>();

  public class Data {
    public Vector2 CurrentVelocity { get; set; }
    public float Speed { get; set; }
  }

  public static class Input {
    public record struct UpdateGlobalPosition(Vector2 GlobalPosition);
    public record struct Move(Vector2 Direction);
    public record struct Attack(Vector2 Direction);
    public record struct Damage(float Amount); // TODO maybe int?
  }

  public static class Output {
    public record struct VelocityUpdated(Vector2 Velocity);
    public record struct Attacked(Vector2 Direction);
    public record struct Damaged(float Amount);
    public record struct AnimationUpdated(StringName Animation);
    public record struct FlipSprite(bool Flip);
  }

  public partial record State : StateLogic<State>, IGet<Input.UpdateGlobalPosition> {
    public State() {
      OnAttach(() => { });
      OnDetach(() => { });
    }

    public Transition On(in Input.UpdateGlobalPosition input) {
      Get<IGameRepo>().UpdatePlayerGlobalPosition(input.GlobalPosition);
      return ToSelf();
    }

    public partial record Idle : State, IGet<Input.Move> {
      public Idle() {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => Output(new Output.AnimationUpdated("idle")));
        this.OnExit(() => { });
      }

      public Transition On(in Input.Move input) =>
        input.Direction.IsZeroApprox()
          ? ToSelf()
          : To<Moving>();
    }

    public partial record Moving : State, IGet<Input.Move> {
      public Moving() {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => Output(new Output.AnimationUpdated("run")));
        this.OnExit(() => { });
      }

      public Transition On(in Input.Move input) {
        var data = Get<Data>();
        var velocity = input.Direction * data.Speed;

        if (data.CurrentVelocity != velocity) {
          data.CurrentVelocity = velocity;
          Output(new Output.VelocityUpdated(data.CurrentVelocity));

          if (data.CurrentVelocity.X < 0) {
            Output(new Output.FlipSprite(true));
          }
          else if (data.CurrentVelocity.X > 0) {
            Output(new Output.FlipSprite(false));
          }
        }

        return velocity.IsZeroApprox()
          ? To<Idle>()
          : ToSelf();
      }
    }
  }
}
