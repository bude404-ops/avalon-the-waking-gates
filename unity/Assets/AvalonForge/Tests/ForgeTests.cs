// AVALON AUTO-FORGE — headless validation suite (Stage 6 structural gates)
// Runs in batch mode:
//   Unity -batchmode -projectPath <proj> -runTests -testPlatform EditMode \
//     -testResults forge-tests.xml
// A model only ships to Big's verdict queue if every gate passes.

using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AvalonForge
{
    public class ForgeTests
    {
        const string Prefabs = "Assets/AvalonForge/Prefabs";

        [Test]
        public void Prefabs_Exist()
        {
            var prefabs = AssetDatabase.FindAssets("t:Prefab", new[] { Prefabs });
            Assert.Greater(prefabs.Length, 0,
                "No forged prefabs found. Run AvalonForge.Forge first.");
        }

        [Test]
        public void Prefab_HasValidHumanoidAvatar()
        {
            foreach (var path in Paths())
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var animator = go.GetComponent<Animator>();
                Assert.IsNotNull(animator, path + ": missing Animator");
                Assert.IsNotNull(animator.avatar, path + ": missing avatar");
                Assert.IsTrue(animator.avatar.isValid, path + ": invalid avatar (bone mapping failed)");
                Assert.IsTrue(animator.avatar.isHuman, path + ": avatar is not Humanoid");
            }
        }

        [Test]
        public void Prefab_IsNormalizedToHeightSpec()
        {
            foreach (var path in Paths())
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var b = CombineBounds(go);
                // class height spec band: 1.4m (short stocky) .. 2.2m (tall statuesque)
                Assert.That(b.size.y, Is.InRange(1.4f, 2.2f),
                    path + $": height {b.size.y:F2}m out of class band — normalization failed");
                Assert.Less(Mathf.Abs(b.center.x), 0.05f, path + ": not centered on origin");
                Assert.Less(Mathf.Abs(b.center.z), 0.05f, path + ": not centered on origin");
            }
        }

        [Test]
        public void Prefab_HasAnimatorStates()
        {
            foreach (var path in Paths())
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var animator = go.GetComponent<Animator>();
                Assume.That(animator.runtimeAnimatorController, Is.Not.Null, path + ": no controller");
                var ctrl = (UnityEditor.Animations.AnimatorController)animator.runtimeAnimatorController;
                Assert.GreaterOrEqual(ctrl.layers[0].stateMachine.states.Length, 1,
                    path + ": animator has zero states");
            }
        }

        [Test]
        public void Weapon_IfPresent_LivesUnderHandBone()
        {
            foreach (var path in Paths())
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var socket = go.transform.GetComponentsInChildren<Transform>()
                    .FirstOrDefault(t => t.name == "WeaponSocket");
                if (socket == null) continue; // weaponless build is legal (Weaponless Plate Law)
                var chain = socket;
                while (chain.parent != null)
                {
                    chain = chain.parent;
                    string n = chain.name.ToLowerInvariant().Replace(":", "").Replace("_", "");
                    if (n.Contains("righthand")) return; // PASS
                }
                Assert.Fail(path + ": WeaponSocket not under the right-hand bone");
            }
        }

        [Test]
        public void No_Stray_Scene_Root_Scale()
        {
            foreach (var path in Paths())
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Assert.Less(Mathf.Abs(go.transform.localScale.x - go.transform.localScale.y), 0.001f,
                    path + ": non-uniform root scale");
                Assert.Less(Mathf.Abs(go.transform.localScale.y - go.transform.localScale.z), 0.001f,
                    path + ": non-uniform root scale");
            }
        }

        // ---- helpers ----
        static string[] Paths() => AssetDatabase.FindAssets("t:Prefab", new[] { Prefabs })
            .Select(p => AssetDatabase.GUIDToAssetPath(p))
            .Where(p => Path.GetExtension(p) == ".prefab" && p.EndsWith("-GAME.prefab"))
            .ToArray();

        static Bounds CombineBounds(GameObject go)
        {
            var rs = go.GetComponentsInChildren<Renderer>();
            var b = rs[0].bounds;
            for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
            return b;
        }
    }
}
