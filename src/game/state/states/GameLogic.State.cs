namespace Woodblight;

using System;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State : StateLogic<State>, IGet<Input.TeleportPlayerTo> {

    public State() {
      OnAttach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.Paused += OnGamePaused;
        gameRepo.Resumed += OnGameResumed;
        gameRepo.GameOver += OnGameOver;
      });

      OnDetach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.Paused -= OnGamePaused;
        gameRepo.Resumed -= OnGameResumed;
        gameRepo.GameOver -= OnGameOver;

      });
    }

    private void OnGameOver(EGameOverReason reason) {
      if (reason == EGameOverReason.Won) {
        Input(new Input.RequestOutro());
      }
    }

    private void OnGameResumed() => Output(new Output.SetPauseMode(false));
    private void OnGamePaused() => Output(new Output.SetPauseMode(true));

    public Transition On(in Input.TeleportPlayerTo input) {
      Get<IGameRepo>().RequestPlayerTeleportation(input.GlobalPosition);
      return ToSelf();
    }
  }
}
