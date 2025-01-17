namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State {
    public partial record InRoom : State, IGet<Input.TransitionRoom>, IGet<Input.RequestOutro> {
      public InRoom() {
        OnAttach(() => Get<IGameRepo>().RoomTransitionRequested += OnRoomTransitionRequested);
        OnDetach(() => Get<IGameRepo>().RoomTransitionRequested -= OnRoomTransitionRequested);

        this.OnEnter(() => Output(new Output.SetPauseMode(false)));
      }

      public Transition On(in Input.TransitionRoom input) {
        Get<Data>().Room = input.Room;
        return To<ChangingRoom>();
      }

      public Transition On(in Input.RequestOutro input) => To<Outro>();
      private void OnRoomTransitionRequested(ERoom room) =>
        Input(new Input.TransitionRoom(room));
    }
  }
}
