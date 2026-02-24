#!/usr/bin/env bash
# =============================================================================
# AzureDevOpsKats Development Environment Setup
# =============================================================================
#
# Description:
#   Installs and configures all tooling dependencies:
#   - .NET SDK (build + runtime)
#   - Docker (container builds)
#   - Helm (chart packaging and deployment)
#   - Validation tools (kubeconform, hadolint)
#   - Pre-commit hooks
#
# Usage:
#   ./setup.sh              # Full setup
#   ./setup.sh --check      # Verify all tools are installed
#
# =============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

DOTNET_VERSION="10.0"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

section() { echo -e "\n${BLUE}=== $* ===${NC}"; }
success() { echo -e "${GREEN}✓${NC} $*"; }
warn() { echo -e "${YELLOW}⚠${NC} $*"; }
error() { echo -e "${RED}✗${NC} $*"; }

check_command() {
    if command -v "$1" &>/dev/null; then
        success "$1 installed ($(command -v "$1"))"
        return 0
    else
        error "$1 not installed"
        return 1
    fi
}

# Detect architecture
ARCH=$(uname -m)
case $ARCH in
    x86_64)  ARCH="amd64" ;;
    aarch64) ARCH="arm64" ;;
    armv7l)  ARCH="arm" ;;
esac
OS=$(uname -s | tr '[:upper:]' '[:lower:]')

SUDO=""
[ "$(id -u)" -ne 0 ] && command -v sudo &>/dev/null && SUDO="sudo"

# =============================================================================
# .NET SDK
# =============================================================================
install_dotnet() {
    section "Installing .NET SDK ${DOTNET_VERSION}"

    if command -v dotnet &>/dev/null; then
        success "dotnet already installed: $(dotnet --version)"
        return 0
    fi

    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin \
        --channel "$DOTNET_VERSION" --install-dir "$HOME/.dotnet"
    export DOTNET_ROOT="$HOME/.dotnet"
    export PATH="$DOTNET_ROOT:$PATH"

    success "dotnet installed: $(dotnet --version)"
    echo ""
    echo "  Add to your shell profile:"
    echo "    export DOTNET_ROOT=\"\$HOME/.dotnet\""
    echo "    export PATH=\"\$DOTNET_ROOT:\$PATH\""
}

# =============================================================================
# Docker
# =============================================================================
check_docker() {
    section "Checking Docker"

    if command -v docker &>/dev/null; then
        if docker info &>/dev/null 2>&1; then
            success "Docker is running: $(docker --version | cut -d' ' -f3 | tr -d ',')"
        else
            warn "Docker installed but not running"
        fi
    else
        warn "Docker not installed (required for container builds)"
        echo "  Install: https://docs.docker.com/engine/install/"
    fi
}

# =============================================================================
# Helm
# =============================================================================
install_helm() {
    section "Installing Helm"

    if command -v helm &>/dev/null; then
        success "helm already installed: $(helm version --short 2>/dev/null)"
        return 0
    fi

    curl -sL https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash
    success "helm installed: $(helm version --short 2>/dev/null)"
}

configure_helm_repos() {
    section "Configuring Helm Repositories"

    declare -A HELM_REPOS=(
        ["ingress-nginx"]="https://kubernetes.github.io/ingress-nginx"
        ["jetstack"]="https://charts.jetstack.io"
        ["bitnami"]="https://charts.bitnami.com/bitnami"
    )

    for repo_name in "${!HELM_REPOS[@]}"; do
        local repo_url="${HELM_REPOS[$repo_name]}"
        if helm repo list 2>/dev/null | grep -q "^${repo_name}"; then
            success "${repo_name} repo already added"
        else
            helm repo add "$repo_name" "$repo_url" --force-update
            success "${repo_name} repo added"
        fi
    done

    helm repo update
    success "Helm repos updated"

    echo ""
    echo "  OCI Helm chart (no repo add needed):"
    echo "    oci://ghcr.io/stuartshay/helm-charts/azuredevopskats"
}

# =============================================================================
# Validation Tools
# =============================================================================
install_kubeconform() {
    section "Installing kubeconform"

    if command -v kubeconform &>/dev/null; then
        success "kubeconform already installed"
        return 0
    fi

    local version
    version=$(curl -sL https://api.github.com/repos/yannh/kubeconform/releases/latest | jq -r '.tag_name')
    curl -sLO "https://github.com/yannh/kubeconform/releases/download/${version}/kubeconform-${OS}-${ARCH}.tar.gz"
    tar -xzf "kubeconform-${OS}-${ARCH}.tar.gz" kubeconform
    $SUDO mv kubeconform /usr/local/bin/
    rm -f "kubeconform-${OS}-${ARCH}.tar.gz"
    success "kubeconform installed: $version"
}

install_hadolint() {
    section "Installing hadolint"

    if command -v hadolint &>/dev/null; then
        success "hadolint already installed"
        return 0
    fi

    local version
    version=$(curl -sL https://api.github.com/repos/hadolint/hadolint/releases/latest | jq -r '.tag_name')

    local binary="hadolint-Linux-x86_64"
    if [ "$ARCH" = "arm64" ]; then
        binary="hadolint-Linux-arm64"
    fi

    curl -sLO "https://github.com/hadolint/hadolint/releases/download/${version}/${binary}"
    chmod +x "$binary"
    $SUDO mv "$binary" /usr/local/bin/hadolint
    success "hadolint installed: $version"
}

# =============================================================================
# Pre-commit
# =============================================================================
install_pre_commit() {
    section "Installing pre-commit"

    if command -v pre-commit &>/dev/null; then
        success "pre-commit already installed"
    else
        if command -v pip3 &>/dev/null; then
            pip3 install --user pre-commit
            success "pre-commit installed"
        else
            warn "pip3 not found, skipping pre-commit"
            return 0
        fi
    fi

    if [ -f ".pre-commit-config.yaml" ]; then
        pre-commit install
        success "pre-commit hooks installed"
    fi
}

# =============================================================================
# .NET Build
# =============================================================================
build_project() {
    section "Building Project"

    echo "Restoring NuGet packages..."
    dotnet restore
    success "NuGet packages restored"

    echo "Building solution..."
    dotnet build --no-restore
    success "Solution built"
}

# =============================================================================
# Verification
# =============================================================================
verify_installation() {
    section "Verification"

    echo ""
    echo "Core Tools:"
    check_command dotnet || true
    check_command docker || true

    echo ""
    echo "Helm & Chart Tools:"
    check_command helm || true
    check_command kubeconform || true

    echo ""
    echo "Lint Tools:"
    check_command hadolint || true
    check_command pre-commit || true

    echo ""
    echo "Helm Repositories:"
    if command -v helm &>/dev/null; then
        helm repo list 2>/dev/null || warn "No Helm repos configured"
    fi

    echo ""
    echo "OCI Helm Chart:"
    echo "  oci://ghcr.io/stuartshay/helm-charts/azuredevopskats"
}

# =============================================================================
# Main
# =============================================================================
main() {
    echo ""
    echo "╔═══════════════════════════════════════════════════════════════╗"
    echo "║       AzureDevOpsKats Development Environment Setup         ║"
    echo "╚═══════════════════════════════════════════════════════════════╝"

    case "${1:-}" in
        --check)
            verify_installation
            exit 0
            ;;
        --help|-h)
            echo ""
            echo "Usage: ./setup.sh [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --check   Verify all tools are installed"
            echo "  --help    Show this help"
            echo ""
            exit 0
            ;;
    esac

    install_dotnet
    check_docker
    install_helm
    configure_helm_repos
    install_kubeconform
    install_hadolint
    install_pre_commit
    build_project
    verify_installation

    section "Setup Complete!"
    echo ""
    echo "Next steps:"
    echo "  make dev           # Start development server (hot reload)"
    echo "  make test          # Run tests"
    echo "  make docker-build  # Build Docker image"
    echo "  make helm-lint     # Lint Helm chart"
    echo ""
}

main "$@"
