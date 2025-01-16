namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class EnemyLogic {
  public partial record State {
    public partial record Dead : State, IGet<Input.AnimationFinished> {
      public Dead() {
        this.OnEnter(() => Output(new Output.AnimationUpdated("die")));
      }

      public Transition On(in Input.AnimationFinished input) {
        Get<IRoomRepo>().OnEnemyKilled();
        Output(new Output.Died());
        return ToSelf();
      }
    }
  }
}
