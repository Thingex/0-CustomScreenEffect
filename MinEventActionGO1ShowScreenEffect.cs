using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using DG.Tweening;
using UnityEngine;
public class MinEventActionGO1ShowScreenEffect : MinEventActionBase
{
    private AssetBundle ab;
    private GameObject screenEffectObj;
    private bool isActive = false;
    private string asset;
    private float? duration;
    private float? overlayAlpha;

    public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
    {
        if (duration is null || overlayAlpha is null)
        {
            global::UnityEngine.Debug.LogException(new Exception("Several props attached to this MinEventAction were not found."));
            return false;
        }
        if (isActive)
        {
            global::UnityEngine.Debug.LogWarning("The screen effect is already active and you'd wait until it's to be destroyed.");
            return false;
        }
        return true;
    }

    public override void Execute(MinEventParams _params)
    {
        EntityPlayerLocal player = _params.Self as EntityPlayerLocal;
        ThreadManager.StartCoroutine(loadExternAsset());
        GameObject instance = GameObject.Instantiate(screenEffectObj, Vector3.zero, Quaternion.identity,
            player.PlayerUI.transform);
        _ = DOTween.Sequence()
            .OnStart(() => isActive = !isActive)
            .AppendInterval((float)duration)
            .OnComplete(() =>
            {
                GameObject.Destroy(instance);
                isActive = false;
                ab.Unload(true); //卸载资源，以便下一次调用
            });
    }

    public override bool ParseXmlAttribute(XAttribute _attribute)
    {
        bool @flag = base.ParseXmlAttribute(_attribute);
        if (!@flag)
            switch (_attribute.Name.LocalName)
            {
                case "asset":
                    asset = _attribute.Value;                  
                    return true;
                case "duration":
                    duration = StringParsers.ParseFloat(_attribute.Value);
                    return true;
                case "overlay_alpha":
                    overlayAlpha = StringParsers.ParseFloat(_attribute.Value);
                    return true;
            }
        return @flag;
    }

    private IEnumerator loadExternAsset()
    {     
        if (screenEffectObj == null)
        {
            string root = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            ab = AssetBundle.LoadFromFile(root + "/" + asset.Substring(0, asset.LastIndexOf('?')));
            screenEffectObj = ab.LoadAsset<GameObject>(asset.Substring(asset.LastIndexOf('?') + 1));
        }
        yield break;
    }    
}