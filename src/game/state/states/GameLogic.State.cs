namespace Nevergreen;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State : StateLogic<State>, IGet<Input.TeleportPlayerTo> {
    public Transition On(in Input.TeleportPlayerTo input) {
      Get<IGameRepo>().RequestPlayerTeleportation(input.GlobalPosition);
      return ToSelf();
    }
  }
}
