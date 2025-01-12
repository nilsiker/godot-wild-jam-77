namespace Nevergreen;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

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
}
