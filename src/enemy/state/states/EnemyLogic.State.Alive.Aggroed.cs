namespace Nevergreen;

using Godot;

public partial class EnemyLogic {
  public partial record State {
    public partial record Alive {
      public partial record Aggroed : Alive, IGet<Input.Move> {
        public Aggroed() {
          OnAttach(() => {
            var gameRepo = Get<IGameRepo>();
            gameRepo.PlayerGlobalPosition.Sync += OnPlayerGlobalPositionSync;
          });
          OnDetach(() => {
            var gameRepo = Get<IGameRepo>();
            gameRepo.PlayerGlobalPosition.Sync -= OnPlayerGlobalPositionSync;
          });
        }

        private void OnPlayerGlobalPositionSync(Vector2 vector) =>
          Output(new Output.PlayerTrackedAt(vector));

        public Transition On(in Input.Move input) {
          Output(new Output.ForceApplied(input.Direction * Get<Data>().Speed, false));
          return ToSelf();
        }
      }
    }
  }
}
