namespace Nevergreen;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public partial record State : StateLogic<State>, IGet<Input.UpdateGlobalPosition>, IGet<Input.Attack> {
    public State() {
      OnAttach(() => { });
      OnDetach(() => { });
    }

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
