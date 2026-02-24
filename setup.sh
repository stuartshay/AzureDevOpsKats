#!/usr/bin/env bash
set -euo pipefail

DOTNET_VERSION="10.0"
REQUIRED_TOOLS=("git" "curl")

echo "=== AzureDevOpsKats Development Setup ==="

# Check required tools
for tool in "${REQUIRED_TOOLS[@]}"; do
    if ! command -v "$tool" &>/dev/null; then
        echo "ERROR: $tool is required but not installed."
        exit 1
    fi
done

# Install .NET SDK if not available
if ! command -v dotnet &>/dev/null; then
    echo "Installing .NET SDK ${DOTNET_VERSION}..."
    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin \
        --channel "$DOTNET_VERSION" --install-dir "$HOME/.dotnet"
    export DOTNET_ROOT="$HOME/.dotnet"
    export PATH="$DOTNET_ROOT:$PATH"
    echo ""
    echo "Add to your shell profile:"
    echo "  export DOTNET_ROOT=\"\$HOME/.dotnet\""
    echo "  export PATH=\"\$DOTNET_ROOT:\$PATH\""
    echo ""
else
    echo "Found dotnet: $(dotnet --version)"
fi

# Restore NuGet packages
echo "Restoring NuGet packages..."
dotnet restore

# Build solution
echo "Building solution..."
dotnet build --no-restore

# Install pre-commit if available
if command -v pre-commit &>/dev/null; then
    echo "Installing pre-commit hooks..."
    pre-commit install
else
    echo "WARN: pre-commit not found. Install with: pip install pre-commit"
fi

# Docker check
if command -v docker &>/dev/null; then
    echo "Found docker: $(docker --version)"
else
    echo "WARN: Docker not found. Required for container builds."
fi

# Helm check
if command -v helm &>/dev/null; then
    echo "Found helm: $(helm version --short)"
else
    echo "WARN: Helm not found. Required for chart operations."
fi

echo ""
echo "=== Setup Complete ==="
echo "Run 'make dev' to start the development server."
