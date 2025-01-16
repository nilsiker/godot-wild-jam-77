namespace Nevergreen;

using Chickensoft.LogicBlocks;
using Godot;

public partial class PlayerLogic {
  public partial record State : StateLogic<State>, IGet<Input.UpdateGlobalPosition>, IGet<Input.Attack> {
    public State() {
      OnAttach(() => Get<IGameRepo>().PlayerTeleportationRequested += OnPlayerTeleportationRequested);
      OnDetach(() => Get<IGameRepo>().PlayerTeleportationRequested -= OnPlayerTeleportationRequested);
    }

    private void OnPlayerTeleportationRequested(Vector2 vector) => Output(new Output.Teleport(vector));

    public Transition On(in Input.UpdateGlobalPosition input) {
      Get<IGameRepo>().UpdatePlayerGlobalPosition(input.GlobalPosition);
      return ToSelf();
    }

    public Transition On(in Input.Attack input) {
      Get<Data>().AttackDirection = input.Direction;
      return To<Attacking>();
    }
  }
}
