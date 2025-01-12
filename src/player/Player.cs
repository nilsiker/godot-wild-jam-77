namespace Nevergreen;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
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
