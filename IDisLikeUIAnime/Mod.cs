using MelonLoader;
using System.Reflection;
using VRC.UI.Elements;
using System.Linq;
using HarmonyLib;
using UnityEngine;


namespace IDisLikeUIAnime
{
    public class Mod : MelonMod
    {
        public static MelonPreferences_Category Category;
        public static MelonPreferences_Entry<QmAnimationType> QmAnimations;
        public override void OnApplicationStart() {
            Category = MelonPreferences.CreateCategory("IDisLikeUIAnime");
            QmAnimations = Category.CreateEntry("QmAnimations", QmAnimationType.None);
            //xref is based on one from vrcuk https://github.com/SleepyVRC/Mods/blob/6660930110353c467ae638b9ba43bd160ebd4963/VRChatUtilityKit/Ui/UiManager.cs#L176
            //credits to loukylor and the team now mantaining vrcuk
            foreach (MethodInfo me in typeof(UIPage).GetMethods()
                 .Where(method => !method.Name.Contains("_PDM_") && method.GetParameters().Length > 1 && method.GetParameters()[1]?.ParameterType == typeof(UIPage.TransitionType)))
                HarmonyInstance.Patch(me, new HarmonyMethod(typeof(Mod), nameof(PushPagePrefix)));
        }

        static void PushPagePrefix(ref UIPage.TransitionType __1, UIPage __instance) {
            switch (QmAnimations.Value) {
                case QmAnimationType.None:
                    __1 = UIPage.TransitionType.None;
                    __instance.GetComponent<CanvasGroup>().alpha = 1;
                    RectTransform rect = __instance.gameObject.transform.parent.GetComponent<RectTransform>();
                    Vector3 center = new Vector3(rect.rect.center.x, rect.rect.center.y);
                    __instance.gameObject.transform.set_localPosition_Injected(ref center);
                    break;
                case QmAnimationType.InPlace:
                    __1 = UIPage.TransitionType.InPlace;
                    break;
                default:
                    break;
            }
        }

        public enum QmAnimationType {
            Default,
            InPlace,
            None,
        }
    }
}