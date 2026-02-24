# AzureDevOpsKats

[![Lint and Test](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/lint.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/lint.yml)
[![Docker Build and Push](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/docker.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/docker.yml)
[![Helm Package and Push](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/helm.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/helm.yml)
[![Docker Hub](https://img.shields.io/badge/Docker%20Hub-stuartshay%2Fazuredevopskats-blue?logo=docker)](https://hub.docker.com/r/stuartshay/azuredevopskats/)
[![Helm Chart](https://img.shields.io/badge/Helm%20Chart-GHCR-0F1689?logo=helm&logoColor=white)](https://github.com/stuartshay/AzureDevOpsKats/pkgs/container/helm-charts%2Fazuredevopskats)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)

Cat photo gallery built with .NET 10 Blazor Web App (Interactive Server + WASM).

## Features

- CRUD operations for cat photos
- Photo upload with file storage
- SQLite database with EF Core 10
- Health check endpoints (`/health`, `/ready`)
- Multi-arch Docker image (amd64 + arm64)
- Helm chart published to GHCR OCI registry

## Tech Stack

| Component | Technology |
|-----------|-----------|
| Runtime | .NET 10 LTS |
| Frontend | Blazor Web App (Server + WASM) |
| Database | SQLite + EF Core 10 |
| Container | Docker (multi-arch) |
| Orchestration | Kubernetes (Helm) |
| CI/CD | GitHub Actions |

## Quick Start

```bash
# Setup development environment
./setup.sh

# Start development server (hot reload)
make dev

# Run tests
make test

# Build Docker image
make docker-build
```

## Project Structure

```text
AzureDevOpsKats/
├── src/
│   ├── AzureDevOpsKats.Web/        # Blazor Web App (Server + WASM host)
│   │   ├── Components/             # Blazor components (pages, layout)
│   │   ├── Data/                   # EF Core context, entities
│   │   ├── Services/               # Business logic
│   │   └── wwwroot/                # Static assets
│   └── AzureDevOpsKats.Client/     # WASM interactive client
├── tests/
│   └── AzureDevOpsKats.Tests/      # xUnit + bUnit tests
├── charts/
│   └── azuredevopskats/            # Helm chart
├── docs/                           # Documentation
├── Dockerfile                      # Multi-stage, multi-arch
├── Makefile                        # Build automation
└── setup.sh                        # Dev environment bootstrap
```

## Development

```bash
make help          # Show all commands
make dev           # Start dev server (hot reload on port 8080)
make build         # Build solution
make test          # Run tests with coverage
make lint          # Check code formatting
make format        # Format code
```

## Docker

```bash
make docker-build  # Build multi-arch image
make docker-run    # Run container locally (port 8080)
```

## Helm Deployment

| Package | Registry |
|---------|----------|
| [stuartshay/azuredevopskats](https://hub.docker.com/r/stuartshay/azuredevopskats/) | Docker Hub |
| [helm-charts/azuredevopskats](https://github.com/stuartshay/AzureDevOpsKats/pkgs/container/helm-charts%2Fazuredevopskats) | GHCR OCI |

```bash
# Install from GHCR OCI registry
helm install azuredevopskats \
  oci://ghcr.io/stuartshay/helm-charts/azuredevopskats \
  --namespace azuredevopskats --create-namespace \
  --set image.tag=10.0.1

# Upgrade
helm upgrade azuredevopskats \
  oci://ghcr.io/stuartshay/helm-charts/azuredevopskats \
  --namespace azuredevopskats \
  --set image.tag=<new-version>
```

## Endpoints

| Path | Description |
|------|-------------|
| `/` | Home page |
| `/cats` | Cat gallery (CRUD) |
| `/health` | Liveness probe |
| `/ready` | Readiness probe |

## CI/CD

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| `lint.yml` | Push/PR | Pre-commit, build, test, hadolint, helm lint |
| `docker.yml` | Push to master | Multi-arch Docker build + push |
| `helm.yml` | Push to master (charts/) | Helm package + push to GHCR |

## License

[MIT](LICENSE) © Stuart Shay
