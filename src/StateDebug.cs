namespace Nevergreen;

using System.Linq;
using Godot;

public interface IStateDebugInfo {
  public string Name { get; }
  public string State { get; }
}

public partial class StateDebug : VBoxContainer {
  public const string GROUP = "state_debug";
  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    ClearChildren();
    foreach (var info in GetTree().GetNodesInGroup("state_debug").OfType<IStateDebugInfo>()) {
      var str = $"[right]{info.Name}: [color=yellow]{info.State}";
      var node = new RichTextLabel {
        Text = str,
        FitContent = true,
        BbcodeEnabled = true,
        MouseFilter = MouseFilterEnum.Ignore
      };
      AddChild(node);
    }
  }

  private void ClearChildren() {
    foreach (var child in GetChildren()) {
      child.QueueFree();
    }
  }
}
