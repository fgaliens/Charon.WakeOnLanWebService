#!/usr/bin/env bash
# Builds the backend and frontend and deploys them to a linux-arm64 device running nginx.
#
# Usage:
#   ./deploy/deploy-on-linux-arm64.sh SSH_CON FE_HOST BE_HOST
#
# Example:
#   ./deploy/deploy-on-linux-arm64.sh user@192.168.2.10 wake-on-lan.home api.wake-on-lan.home
set -euo pipefail

if [ "$#" -ne 3 ]; then
  echo "Usage: $0 SSH_CON FE_HOST BE_HOST" >&2
  echo "Example: $0 user@192.168.2.10 wake-on-lan.home api.wake-on-lan.home" >&2
  exit 1
fi

SSH_CON="$1"
FE_HOST="$2"
BE_HOST="$3"

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PUBLISH_DIR="$REPO_ROOT/publish"

echo "==> Building backend (linux-arm64, self-contained)"
rm -rf "$PUBLISH_DIR"
dotnet publish "$REPO_ROOT/backend/Charon.WakeOnLanWebService" \
  -c Release -r linux-arm64 --self-contained \
  -o "$PUBLISH_DIR/api"

echo "==> Building frontend (API: http://$BE_HOST)"
(cd "$REPO_ROOT/frontend" && npm ci && VITE_API_BASE_URL="http://$BE_HOST" npm run build)

echo "==> Rendering config files"
REMOTE_USER="$(ssh "$SSH_CON" 'id -un')"
mkdir -p "$PUBLISH_DIR/config"
sed -e "s/__USER__/$REMOTE_USER/" -e "s/__FE_HOST__/$FE_HOST/" \
  "$REPO_ROOT/deploy/wake-on-lan.service" > "$PUBLISH_DIR/config/wake-on-lan.service"
sed -e "s/__FE_HOST__/$FE_HOST/" -e "s/__BE_HOST__/$BE_HOST/" \
  "$REPO_ROOT/deploy/nginx-wake-on-lan.conf" > "$PUBLISH_DIR/config/wake-on-lan.conf"

echo "==> Preparing directories on $SSH_CON (sudo password may be asked)"
ssh -t "$SSH_CON" "sudo mkdir -p /opt/wake-on-lan/api /var/www/wake-on-lan /var/lib/wake-on-lan \
  && sudo chown $REMOTE_USER /opt/wake-on-lan/api /var/www/wake-on-lan /var/lib/wake-on-lan"

echo "==> Copying files"
rsync -az --delete "$PUBLISH_DIR/api/" "$SSH_CON:/opt/wake-on-lan/api/"
rsync -az --delete "$REPO_ROOT/frontend/dist/" "$SSH_CON:/var/www/wake-on-lan/"
rsync -az "$PUBLISH_DIR/config/" "$SSH_CON:/tmp/wake-on-lan-config/"

echo "==> Installing service and nginx config, restarting"
ssh -t "$SSH_CON" "set -e
  sudo install -m 644 /tmp/wake-on-lan-config/wake-on-lan.service /etc/systemd/system/wake-on-lan.service
  sudo install -m 644 /tmp/wake-on-lan-config/wake-on-lan.conf /etc/nginx/sites-available/wake-on-lan
  sudo ln -sf /etc/nginx/sites-available/wake-on-lan /etc/nginx/sites-enabled/wake-on-lan
  rm -rf /tmp/wake-on-lan-config
  sudo systemctl daemon-reload
  sudo systemctl enable wake-on-lan
  sudo systemctl restart wake-on-lan
  sudo nginx -t
  sudo systemctl reload nginx"

echo "==> Done: http://$FE_HOST (API: http://$BE_HOST)"
