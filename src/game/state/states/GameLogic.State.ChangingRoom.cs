namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State {
    public partial record ChangingRoom : State, IGet<Input.RoomResolved> {
      public ChangingRoom() {
        OnAttach(() => {
          Get<IGameRepo>().RoomResolved += OnRoomResolved;
          Get<IAppRepo>().FadeOutFinished += OnFadeOutFinished;
        });
        OnDetach(() => {
          Get<IGameRepo>().RoomResolved -= OnRoomResolved;
          Get<IAppRepo>().FadeOutFinished -= OnFadeOutFinished;
        });

        this.OnEnter(() => {
          Get<IAppRepo>().RequestFadeOut();
          Output(new Output.SetPauseMode(true));
        });
      }

      private void OnFadeOutFinished() => Output(new Output.RoomTransitionRequested(Get<Data>().Room));

      public Transition On(in Input.RoomResolved input) {
        Get<IAppRepo>().RequestFadeIn();
        return To<InRoom>();
      }
      private void OnRoomResolved() => Input(new Input.RoomResolved());
    }
  }
}
