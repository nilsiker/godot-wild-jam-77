namespace Woodblight;

using System.Collections.Generic;
using System.Linq;
using Godot;

[InputMap]
public partial class Inputs : Node {
  #region Exports
  [Export]
  private float _bufferTime;
  #endregion

  #region State
  private static readonly HashSet<StringName> _bufferedActions = [];
  #endregion

  public override void _UnhandledInput(InputEvent @event) {
    var foundAction = InputMap
      .GetActions()
      .FirstOrDefault(action => @event.IsAction(action));

    if (foundAction == null) {
      return;
    }

    _bufferedActions.Add(foundAction);

    var timer = GetTree().CreateTimer(_bufferTime);
    timer.Timeout += () => _bufferedActions.Remove(foundAction);
  }

  public bool IsBuffered(StringName action) => _bufferedActions.Contains(action);
}
