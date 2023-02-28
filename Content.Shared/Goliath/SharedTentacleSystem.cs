using Content.Shared.Goliath;

namespace Content.Shared.Goliath;
public abstract class SharedTentacleSystem : EntitySystem
{
//    [Dependency] private readonly IPrototypeManager _proto = default!;


//    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

//        SubscribeLocalEvent<SharedTentacleComponent, ComponentStartup>(OnTentacleStartup);



    }

    }
