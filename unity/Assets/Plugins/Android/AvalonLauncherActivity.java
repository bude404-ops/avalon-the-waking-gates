// AVALON launcher entry point — Unity 6 disables the stock UnityPlayerActivity when a
// custom main manifest is present, so the launcher must be a custom subclass.
// Documented Unity pattern: Java plugin in Assets/Plugins/Android + MAIN/LAUNCHER in manifest.
package com.bigfoot404.avalon;

public class AvalonLauncherActivity extends com.unity3d.player.UnityPlayerActivity
{
    // Shell scene bootstraps via Shell.cs — no overrides needed.
}
