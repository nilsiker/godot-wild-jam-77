namespace Nevergreen;

using System;
using Chickensoft.LogicBlocks;
using Godot;

public partial class PlayerLogic {
  public partial record State : StateLogic<State>, IGet<Input.UpdateGlobalPosition>, IGet<Input.Revive> {
    public State() {
      OnAttach(() => {
        Get<IGameRepo>().PlayerTeleportationRequested += OnPlayerTeleportationRequested;
        Get<IGameRepo>().PlayerReset += OnPlayerReset;
      });
      OnDetach(() => {
        Get<IGameRepo>().PlayerTeleportationRequested -= OnPlayerTeleportationRequested;
        Get<IGameRepo>().PlayerReset -= OnPlayerReset;
      });
    }

    private void OnPlayerReset() {
      Output(new Output.Teleport(new Vector2(-19, -4)));
      Get<IPlayerRepo>().SetHealth(3);
      Input(new Input.Revive());
    }

    private void OnPlayerTeleportationRequested(Vector2 vector) => Output(new Output.Teleport(vector));

    public Transition On(in Input.UpdateGlobalPosition input) {
      Get<IGameRepo>().UpdatePlayerGlobalPosition(input.GlobalPosition);
      return ToSelf();
    }

    public Transition On(in Input.Revive input) => To<Alive.Idle>();
  }
}
