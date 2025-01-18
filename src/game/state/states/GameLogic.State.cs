namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State : StateLogic<State>, IGet<Input.TeleportPlayerTo> {

    public State() {
      OnAttach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.Paused += OnGamePaused;
        gameRepo.Resumed += OnGameResumed;
      });

      OnDetach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.Paused -= OnGamePaused;
        gameRepo.Resumed -= OnGameResumed;
      });
    }

    private void OnGameResumed() => Output(new Output.SetPauseMode(false));
    private void OnGamePaused() => Output(new Output.SetPauseMode(true));

    public Transition On(in Input.TeleportPlayerTo input) {
      Get<IGameRepo>().RequestPlayerTeleportation(input.GlobalPosition);
      return ToSelf();
    }
  }
}
