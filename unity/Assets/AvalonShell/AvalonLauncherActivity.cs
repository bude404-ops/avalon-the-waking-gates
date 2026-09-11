// AVALON launcher — Unity 6 post-processor forces the stock UnityPlayerActivity to
// android:enabled="false"; the sanctioned entry-point pattern is a custom subclass
// declared as the MAIN/LAUNCHER activity in Assets/Plugins/Android/AndroidManifest.xml.
using UnityEngine;

namespace AvalonShell
{
    public class AvalonLauncherActivity : com.unity3d.player.UnityPlayerActivity
    {
        // No overrides needed — the shell scene bootstraps via Shell.cs.
    }
}
