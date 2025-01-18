namespace Nevergreen;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IOptions : IPanelContainer { }

[Meta(typeof(IAutoNode))]
public partial class Options : PanelContainer, IOptions {
  #region Nodes
  [Node] HSlider MusicSlider { get; set; } = default!;
  [Node] HSlider SFXSlider { get; set; } = default!;
  #endregion




  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    MusicSlider.ValueChanged += OnMusicSliderValueChanged;
    SFXSlider.ValueChanged += OnSFXSliderValueChanged;
  }

  private void OnMusicSliderValueChanged(double value) =>
    AudioServer.SetBusVolumeDb(1, (float)Mathf.LinearToDb(value));


  private void OnSFXSliderValueChanged(double value) =>
    AudioServer.SetBusVolumeDb(2, (float)Mathf.LinearToDb(value));


  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree() {

  }
  #endregion

  #region Input Callbacks
  #endregion

  #region Output Callbacks
  #endregion
}
