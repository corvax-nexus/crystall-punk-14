using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Corvax.CrystallPunk.DayCycle;
public sealed partial class DayCycleSystem : EntitySystem
{

    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DayCycleComponent, MapInitEvent>(OnMapInitDayCycle);
    }

    private void OnMapInitDayCycle(Entity<DayCycleComponent> dayCycle, ref MapInitEvent args)
    {
        EnsureComp<MapLightComponent>(dayCycle);
        var currentEntry = dayCycle.Comp.TimeEntries[dayCycle.Comp.CurrentTimeEntry];

        dayCycle.Comp.EntryStartTime = _timing.CurTime;
        dayCycle.Comp.EntryEndTime = _timing.CurTime + currentEntry.Duration;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var dayCycleQuery = EntityQueryEnumerator<DayCycleComponent, MapLightComponent>();
        while (dayCycleQuery.MoveNext(out var uid, out var dayCycle, out var mapLight))
        {
            if (dayCycle.TimeEntries.Count <= 1) continue;

            var start = dayCycle.EntryStartTime;
            var end = dayCycle.EntryEndTime;

            var lerpValue = GetLerpValue((float) start.TotalSeconds, (float) end.TotalSeconds, (float) _timing.CurTime.TotalSeconds);

            var nextEntry = (dayCycle.CurrentTimeEntry + 1 == dayCycle.TimeEntries.Count) ? 0 : dayCycle.CurrentTimeEntry + 1;

            var startColor = dayCycle.TimeEntries[dayCycle.CurrentTimeEntry].StartColor;
            var endColor = dayCycle.TimeEntries[nextEntry].StartColor;

            Color curColor = ColorLerp(startColor, endColor, lerpValue);

            mapLight.AmbientLightColor = curColor;
            Dirty(uid, mapLight);

            if (_timing.CurTime > dayCycle.EntryEndTime)
            {
                dayCycle.CurrentTimeEntry = nextEntry;
                dayCycle.EntryStartTime = dayCycle.EntryEndTime;
                dayCycle.EntryEndTime = dayCycle.EntryEndTime + dayCycle.TimeEntries[nextEntry].Duration;
            }
        }
    }

    public static float GetLerpValue(float start, float end, float current)
    {
        if (start == end)
            return 0f;
        else
        {
            float distanceFromStart = current - start;
            float totalDistance = end - start;

            return MathHelper.Clamp01(distanceFromStart / totalDistance);
        }
    }

    // TODO: RobustToolbox PR
    public static Color ColorLerp(Color startColor, Color endColor, float t)
    {
        t = MathHelper.Clamp01(t);

        float lerpedR = MathHelper.Lerp(startColor.R, endColor.R, t);
        float lerpedG = MathHelper.Lerp(startColor.G, endColor.G, t);
        float lerpedB = MathHelper.Lerp(startColor.B, endColor.B, t);
        float lerpedA = MathHelper.Lerp(startColor.A, endColor.A, t);

        return new Color(lerpedR, lerpedG, lerpedB, lerpedA);
    }
}
