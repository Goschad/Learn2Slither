#!/usr/bin/env bash
set -euo pipefail

# Version du SDK .NET à installer (LTS recommandée)
DOTNET_CHANNEL="${DOTNET_CHANNEL:-10.0}"
DOTNET_VERSION="${DOTNET_VERSION:-latest}"

# Dossier d'installation local (sans sudo)
DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
DOTNET_TOOLS="$HOME/.dotnet/tools"

echo "==> Installation .NET SDK (user-only, no sudo)"
echo "    Channel : $DOTNET_CHANNEL"
echo "    Version : $DOTNET_VERSION"
echo "    Target  : $DOTNET_ROOT"

mkdir -p "$DOTNET_ROOT"

# Télécharger le script officiel Microsoft
INSTALL_SCRIPT="/tmp/dotnet-install.sh"
curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$INSTALL_SCRIPT"
chmod +x "$INSTALL_SCRIPT"

# Installer le SDK .NET
"$INSTALL_SCRIPT" --channel "$DOTNET_CHANNEL" --version "$DOTNET_VERSION" --install-dir "$DOTNET_ROOT"

# Vérifier installation avec chemin absolu
"$DOTNET_ROOT/dotnet" --info

# Ajouter au PATH pour la session courante
export DOTNET_ROOT="$DOTNET_ROOT"
export PATH="$DOTNET_ROOT:$DOTNET_TOOLS:$PATH"

echo
echo "==> Ajout des variables dans ~/.bashrc (si absent)"
grep -q 'export DOTNET_ROOT="$HOME/.dotnet"' "$HOME/.bashrc" || echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> "$HOME/.bashrc"
grep -q 'export PATH="$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"' "$HOME/.bashrc" || echo 'export PATH="$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"' >> "$HOME/.bashrc"

echo
echo "✅ Installation terminée."
echo "➡️  Recharge le shell : source ~/.bashrc"
echo "➡️  Puis teste : dotnet --info"
