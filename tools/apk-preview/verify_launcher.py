#!/usr/bin/env python3
"""AVALON launcher verifier — exits 0 only if the APK binary has an ENABLED
MAIN/LAUNCHER activity named *UnityPlayerGameActivity. Never trust manifest
source; always inspect the built binary (v107-v110 ghost-install lesson).
Uses androguard when available, else the stdlib axml_lite parser — the gate
must never die on a missing pip in the CI image (v112 lesson)."""
import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

try:
    from androguard.core.apk import APK
    def check(path):
        apk = APK(path)
        main = apk.get_main_activity()
        vcode = apk.get_androidversion_code()
        ok = main is not None and "UnityPlayerGameActivity" in main
        print("MAIN:", main)
        print("VCODE:", vcode)
        print("LAUNCHER:", "PASS" if ok else "FAIL")
        return 0 if ok else 1
except ImportError:
    import axml_lite
    def check(path):
        root = axml_lite.get_manifest_tree(path)
        acts = axml_lite.findall(root, 'activity')
        main = None
        for a in acts:
            for filt in a.children:
                if filt.name != 'intent-filter':
                    continue
                has_main = any(c.name == 'action' and c.attrs.get('android:name') == 'android.intent.action.MAIN' for c in filt.children)
                has_launch = any(c.name == 'category' and c.attrs.get('android:name') == 'android.intent.category.LAUNCHER' for c in filt.children)
                if has_main and has_launch:
                    main = a.attrs.get('android:name')
                    if a.attrs.get('android:enabled') == 'false':
                        main = main + " [DISABLED]"
        m = [n for n in root.children if n.name == 'manifest'][0]
        vcode = m.attrs.get('android:versionCode')
        ok = main is not None and "UnityPlayerGameActivity" in main and "DISABLED" not in main
        print("MAIN:", main)
        print("VCODE:", vcode)
        print("LAUNCHER:", "PASS" if ok else "FAIL")
        return 0 if ok else 1

if __name__ == "__main__":
    sys.exit(check(sys.argv[1]))
