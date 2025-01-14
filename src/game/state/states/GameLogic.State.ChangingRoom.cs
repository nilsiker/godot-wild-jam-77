namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State {
    public partial record ChangingRoom : State, IGet<Input.RoomResolved> {
      public ChangingRoom() {
        OnAttach(() => Get<IGameRepo>().RoomResolved += OnRoomResolved);
        OnDetach(() => Get<IGameRepo>().RoomResolved -= OnRoomResolved);

        this.OnEnter(() => Output(new Output.RoomTransitionRequested(Get<Data>().Room)));
      }

      public Transition On(in Input.RoomResolved input) => To<InRoom>();
      private void OnRoomResolved() => Input(new Input.RoomResolved());
    }
  }
}
