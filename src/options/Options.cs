namespace Woodblight;

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
  [Node] CheckBox MysteryBox { get; set; } = default!;
  #endregion

  #region Dependencies
  [Dependency] private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion


  public void OnResolved() {
    MusicSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(1));
    SFXSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(2));
    AppRepo.UseDice.Sync += (useDice) => MysteryBox.ButtonPressed = useDice;

  }

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    MusicSlider.ValueChanged += OnMusicSliderValueChanged;
    SFXSlider.ValueChanged += OnSFXSliderValueChanged;
    MysteryBox.Toggled += OnMysteryBoxToggled;
  }

  private void OnMysteryBoxToggled(bool toggledOn) => AppRepo.SetUseDice(toggledOn);
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
