using System.Reflection;
using HarmonyLib;
public class ModStarter : IModApi
{
    public void InitMod(Mod _modInstance)
    {
        var harmony = new Harmony(base.GetType().Name);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}