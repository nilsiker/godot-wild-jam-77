namespace Woodblight;

using Chickensoft.LogicBlocks;

public partial class EnemyLogic {
  public partial record State {
    public partial record Alive {
      public partial record Metamorphosis : Alive, IGet<Input.AnimationFinished> {
        public Metamorphosis() {
          OnAttach(() => { });
          OnDetach(() => { });

          this.OnEnter(() => Output(new Output.AnimationUpdated("metamorphize")));
        }

        public Transition On(in Input.AnimationFinished input) {
          Output(new Output.ReplaceWithBroodmother());
          return ToSelf();
        }
      }
    }
  }
}
