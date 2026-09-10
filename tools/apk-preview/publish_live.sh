#!/bin/bash
# PUBLISH LIVE CONTENT — ships hot-updatable content to the app's self-update host.
# Usage: ./publish_live.sh [apk_path]
#   No apk_path  -> content-only update (HTML/GLB/art changes; app pulls silently on next launch)
#   With apk     -> also bumps the shell pointer so installed apps one-tap-install the new shell
set -e
PAGES=/tmp/avalon-3d-viewer
WWW="$(cd "$(dirname "$0")" && pwd)/app/assets/www"
STAMP="$(date +%Y.%m.%d)-$(( $(date +%H) * 60 + $(date +%M) ))"

[ -d "$PAGES/.git" ] || git clone "https://github.com/mcontwitter-glitch/avalon-3d-viewer.git" "$PAGES"
cd "$PAGES" && git pull -q origin main

mkdir -p live/ui live/models live/art
for f in index.html model3d.html gallery.html overview.html teaser.html; do cp "$WWW/$f" live/; done
cp "$WWW/ui/ui-index.html" live/ui/
cp "$WWW/models/"*.glb live/models/ 2>/dev/null || true
cp "$WWW/art/"* live/art/

APK_ARG=""
if [ -n "$1" ]; then
  cp "$1" live/AVALON-WAKING-GATES-TEST-LATEST.apk
  APK_ARG="$1"
fi

python3 - "$STAMP" "$APK_ARG" << 'EOF'
import os, json, subprocess, sys
stamp, apk_path = sys.argv[1], sys.argv[2]
files = []
for root, dirs, fs in os.walk('live'):
    for f in fs:
        if f == 'content.json' or f.endswith('.apk'): continue
        files.append(os.path.relpath(os.path.join(root, f), 'live'))
files.sort()
cfg = json.load(open('live/content.json')) if os.path.exists('live/content.json') else {}
apk = cfg.get('apk', {"versionCode": 14, "versionName": "0.10.0-live",
    "url": "https://mcontwitter-glitch.github.io/avalon-3d-viewer/live/AVALON-WAKING-GATES-TEST-LATEST.apk"})
if apk_path:
    code = subprocess.run(['aapt2','dump','badging',apk_path], capture_output=True, text=True).stdout
    apk['versionCode'] = int(code.split("versionCode='")[1].split("'")[0])
    apk['versionName'] = code.split("versionName='")[1].split("'")[0]
cfg = {"content_version": stamp, "files": files, "apk": apk}
open('live/content.json','w').write(json.dumps(cfg, indent=2))
print('content_version:', stamp, '| files:', len(files), '| apk pointer:', apk['versionName'])
EOF

git add -A
git commit -qm "LIVE content update ${STAMP}"
git push -q origin main
echo "LIVE PUBLISHED: ${STAMP}"
echo "Apps pull this silently on next launch (content-only) / prompt one-tap install (new shell)."
