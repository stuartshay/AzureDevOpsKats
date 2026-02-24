# Agent Operating Guide

All automation, assistants, and developers must follow
`.github/copilot-instructions.md` for workflow, safety, and environment rules.

## How to Use

- Read `.github/copilot-instructions.md` before making changes
- Apply every rule in that file as-is; do not redefine or override them here
- If updates are needed, edit `.github/copilot-instructions.md` and keep this
  file pointing to it

## Quick Reference

- **Language**: C# / .NET 10 LTS / Blazor Web App
- **Database**: SQLite with Kubernetes PersistentVolume
- **Development branch**: `develop` (default working branch)
- **Production branch**: `main` (releases only, PR-only)
- **Lint before commit**: `pre-commit run -a`
- **Build**: `make build`
- **Run dev**: `make dev`
- **Test**: `make test`
- **Docker**: `make docker-build`
- **Helm lint**: `make helm-lint`

## Development Workflow

1. Switch to develop: `git checkout develop && git pull origin develop`
2. **Rebase from main**: `git fetch origin main && git rebase origin/main`
3. Make changes to source in `src/` or `tests/`
4. Run `pre-commit run -a`
5. Run `make test`
6. Commit and push to `develop` or `feature/*` branch
7. Create PR to `main` when ready for production

## Project Structure

```text
AzureDevOpsKats/
├── src/
│   ├── AzureDevOpsKats.Web/        # Blazor Web App (Server + WASM host)
│   │   ├── Components/             # Blazor components (pages, layout)
│   │   ├── Data/                   # EF Core context, entities, SQLite
│   │   ├── Services/               # Business logic
│   │   └── Program.cs              # Entry point
│   └── AzureDevOpsKats.Client/     # WASM interactive client
├── tests/
│   └── AzureDevOpsKats.Tests/      # xUnit + bUnit tests
├── charts/
│   └── azuredevopskats/            # Helm chart (published to GHCR OCI)
├── docs/                           # Documentation
├── assets/                         # Cat photo seed data
├── Dockerfile                      # Multi-stage, multi-arch (amd64 + arm64)
├── Makefile                        # Build automation
├── setup.sh                        # Dev environment bootstrap
└── VERSION                         # Release version
```

## Deployment (Helm — No Argo CD)

This project uses a self-contained Helm-based deployment pattern, deliberately
different from the Argo CD GitOps pattern used by otel-\* projects.

```bash
# Install from GHCR OCI registry
helm install azuredevopskats \
  oci://ghcr.io/stuartshay/helm-charts/azuredevopskats \
  --namespace azuredevopskats --create-namespace \
  --set image.tag=<version>

# Upgrade
helm upgrade azuredevopskats \
  oci://ghcr.io/stuartshay/helm-charts/azuredevopskats \
  --namespace azuredevopskats \
  --set image.tag=<new-version>

# Rollback
helm rollback azuredevopskats -n azuredevopskats
```

## Target Cluster

| Property | Value |
|----------|-------|
| Cluster | k8s-pi5-cluster (ARM64) |
| Namespace | azuredevopskats |
| DNS | kats.lab.informationcart.com |
| Ingress | ingress-nginx (192.168.1.100) |
| TLS | cert-manager (Let's Encrypt) |
| Storage | NFS PersistentVolume |
