#!/usr/bin/env python3
"""AVALON launcher verifier — exits 0 only if the APK binary has an ENABLED
MAIN/LAUNCHER activity named *UnityPlayerGameActivity. Never trust manifest
source; always inspect the built binary (v107-v110 ghost-install lesson)."""
import sys, re, logging
logging.disable(logging.CRITICAL)
from androguard.core.apk import APK

def check(path):
    apk = APK(path)
    acts = apk.get_activities()
    main = apk.get_main_activity()
    xml = apk.get_android_manifest_axml().get_xml().decode()
    frag = ""
    if main:
        m = re.search(r'<activity[^>]*' + re.escape(main.split('.')[-1]) + r'[^>]*>', xml)
        if m:
            frag = m.group(0)
    ok = (main is not None
          and "UnityPlayerGameActivity" in main
          and 'android:enabled="false"' not in frag)
    print("ACTS:", acts)
    print("MAIN:", main)
    print("VCODE:", apk.get_androidversion_code())
    print("LAUNCHER:", "PASS" if ok else "FAIL")
    return 0 if ok else 1

if __name__ == "__main__":
    sys.exit(check(sys.argv[1]))
