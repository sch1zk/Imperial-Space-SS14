using static Content.Shared.Store.VendorMachine.SharedVendorMachineComponent;
using Robust.Client.Animations;
using Robust.Client.GameObjects;

namespace Content.Client.Store.VendorMachine;

public sealed class VendorMachineSystem : EntitySystem
{
    [Dependency] private readonly AnimationPlayerSystem _animationPlayer = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VendorMachineComponent, AppearanceChangeEvent>(OnAppearanceChange);
        SubscribeLocalEvent<VendorMachineComponent, AnimationCompletedEvent>(OnAnimationCompleted);
    }

    private void OnAnimationCompleted(EntityUid uid, VendorMachineComponent component, AnimationCompletedEvent args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;
        if (!TryComp<AppearanceComponent>(uid, out var appearance) ||
            !_appearanceSystem.TryGetData<VendorMachineVisualState>(uid, VendorMachineVisuals.VisualState, out var visualState, appearance))
        {
            visualState = VendorMachineVisualState.Normal;
        }

        UpdateAppearance(uid, visualState, component, sprite);
    }

    private void OnAppearanceChange(EntityUid uid, VendorMachineComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!args.AppearanceData.TryGetValue(VendorMachineVisuals.VisualState, out var visualStateObject) ||
            visualStateObject is not VendorMachineVisualState visualState)
        {
            visualState = VendorMachineVisualState.Normal;
        }

        UpdateAppearance(uid, visualState, component, args.Sprite);
    }

    private void UpdateAppearance(EntityUid uid, VendorMachineVisualState visualState, VendorMachineComponent component, SpriteComponent sprite)
    {
        SetLayerState(VendorMachineVisualLayers.Base, component.OffState, sprite);

        switch (visualState)
        {
            case VendorMachineVisualState.Normal:
                SetLayerState(VendorMachineVisualLayers.BaseUnshaded, component.NormalState, sprite);
                SetLayerState(VendorMachineVisualLayers.Screen, component.ScreenState, sprite);
                break;

            case VendorMachineVisualState.Deny:
                if (component.LoopDenyAnimation)
                    SetLayerState(VendorMachineVisualLayers.BaseUnshaded, component.DenyState, sprite);
                else
                    PlayAnimation(uid, VendorMachineVisualLayers.BaseUnshaded, component.DenyState, component.DenyDelay, sprite);

                SetLayerState(VendorMachineVisualLayers.Screen, component.ScreenState, sprite);
                break;

            case VendorMachineVisualState.Eject:
                PlayAnimation(uid, VendorMachineVisualLayers.BaseUnshaded, component.EjectState, component.EjectDelay, sprite);
                SetLayerState(VendorMachineVisualLayers.Screen, component.ScreenState, sprite);
                break;

            case VendorMachineVisualState.Broken:
                HideLayers(sprite);
                SetLayerState(VendorMachineVisualLayers.Base, component.BrokenState, sprite);
                break;

            case VendorMachineVisualState.Off:
                HideLayers(sprite);
                break;
        }
    }

    private static void SetLayerState(VendorMachineVisualLayers layer, string? state, SpriteComponent sprite)
    {
        if (string.IsNullOrEmpty(state))
            return;

        sprite.LayerSetVisible(layer, true);
        sprite.LayerSetAutoAnimated(layer, true);
        sprite.LayerSetState(layer, state);
    }

    private void PlayAnimation(EntityUid uid, VendorMachineVisualLayers layer, string? state, float animationTime, SpriteComponent sprite)
    {
        if (string.IsNullOrEmpty(state))
            return;

        if (!_animationPlayer.HasRunningAnimation(uid, state))
        {
            var animation = GetAnimation(layer, state, animationTime);
            sprite.LayerSetVisible(layer, true);
            _animationPlayer.Play(uid, animation, state);
        }
    }

    private static Animation GetAnimation(VendorMachineVisualLayers layer, string state, float animationTime)
    {
        return new Animation
        {
            Length = TimeSpan.FromSeconds(animationTime),
            AnimationTracks =
                {
                    new AnimationTrackSpriteFlick
                    {
                        LayerKey = layer,
                        KeyFrames =
                        {
                            new AnimationTrackSpriteFlick.KeyFrame(state, 0f)
                        }
                    }
                }
        };
    }

    private static void HideLayers(SpriteComponent sprite)
    {
        HideLayer(VendorMachineVisualLayers.BaseUnshaded, sprite);
        HideLayer(VendorMachineVisualLayers.Screen, sprite);
    }

    private static void HideLayer(VendorMachineVisualLayers layer, SpriteComponent sprite)
    {
        if (!sprite.LayerMapTryGet(layer, out var actualLayer))
            return;

        sprite.LayerSetVisible(actualLayer, false);
    }
}
public enum VendorMachineVisualLayers : byte
{
    /// <summary>
    /// Off / Broken. The other layers will overlay this if the machine is on.
    /// </summary>
    Base,
    /// <summary>
    /// Normal / Deny / Eject
    /// </summary>    
    BaseUnshaded,
    /// <summary>
    /// Screens that are persistent (where the machine is not off or broken)
    /// </summary>
    Screen
}
