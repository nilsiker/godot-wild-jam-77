namespace Nevergreen;
public partial class GameLogic {
  public abstract partial record State {
    public partial record InRoom : State, IGet<Input.TransitionRoom> {
      public InRoom() {
        OnAttach(() => Get<IGameRepo>().RoomTransitionRequested += OnRoomTransitionRequested);
        OnDetach(() => Get<IGameRepo>().RoomTransitionRequested -= OnRoomTransitionRequested);
      }

      public Transition On(in Input.TransitionRoom input) {
        Get<Data>().Room = input.Room;
        return To<ChangingRoom>();
      }

      private void OnRoomTransitionRequested(ERoom room) => Input(new Input.TransitionRoom(room));
    }
  }
}
