namespace Nevergreen;

public partial class EnemyLogic {
  public partial record State {
    public partial record Alive {
      public partial record Wandering : Alive, IGet<Input.PlayerInRange> {
        public Wandering() {

        }

        public Transition On(in Input.PlayerInRange input) => To<Aggroed>();
      }
    }
  }
}
