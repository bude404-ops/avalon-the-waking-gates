#!/usr/bin/env python3
"""Injects the Unity 6 launcher activity into a decoded (apktool) AndroidManifest.xml.
Usage: inject_launcher.py /tmp/dec/AndroidManifest.xml"""
import re, sys

ACT = """    <activity android:name="com.unity3d.player.UnityPlayerGameActivity" android:label="AVALON" android:exported="true" android:launchMode="singleTask" android:configChanges="mcc|mnc|locale|touchscreen|keyboard|keyboardHidden|navigation|orientation|screenLayout|uiMode|screenSize|smallestScreenSize|fontScale|layoutDirection|density" android:resizeableActivity="false">
        <intent-filter>
            <action android:name="android.intent.action.MAIN" />
            <category android:name="android.intent.category.LAUNCHER" />
        </intent-filter>
        <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
    </activity>
"""

p = sys.argv[1]
s = open(p, encoding="utf-8").read()
if "UnityPlayerGameActivity" in s:
    print("activity already present — no injection")
    sys.exit(0)
app = re.search(r"<application[^>]*>", s)
if not app:
    print("ERROR: no <application> node"); sys.exit(1)
head = app.group(0)
if head.endswith("/>"):
    s = s.replace(head, head[:-2] + ">\n" + ACT + "</application>", 1)
else:
    s = s.replace(head, head + "\n" + ACT, 1)
open(p, "w", encoding="utf-8").write(s)
print("manifest patched")
