namespace Nevergreen;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public partial record State : StateLogic<State>, IGet<Input.UpdateGlobalPosition> {
    public State() {
      OnAttach(() => { });
      OnDetach(() => { });
    }

    public Transition On(in Input.UpdateGlobalPosition input) {
      Get<IGameRepo>().UpdatePlayerGlobalPosition(input.GlobalPosition);
      return ToSelf();
    }
  }
}
