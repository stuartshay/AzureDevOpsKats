# Copilot Rules for AzureDevOpsKats Repo

These rules ensure Copilot/assistants follow best practices for .NET 10 Blazor
Web App development with Helm-based Kubernetes deployment.

## Always Read First

- **README**: Read `README.md` for project overview and architecture
- **Plan**: Check `docs/project-plan.md` for modernization phases and decisions
- **env**: Load `.env` for local configuration (gitignored)
- **bootstrap**: Run `make setup` (or `./setup.sh`) once on a fresh environment to
  install .NET SDK 10.0, Helm, pre-commit hooks, and other tooling
- **pre-commit**: ALWAYS run `pre-commit run -a` before commit/PR

## Project Overview

.NET 10 Blazor Web App (Interactive Server + WASM) providing a cat photo CRUD
application. Uses EF Core with SQLite, multi-arch Docker image, Helm chart
published to GitHub Container Registry (OCI), deployed to k8s-pi5-cluster via
`helm install` (no Argo CD).

## Target Infrastructure

| Property     | Value                                   |
| ------------ | --------------------------------------- |
| Language     | C# / .NET 10 LTS / Blazor               |
| Database     | SQLite (PersistentVolume in K8s)        |
| K8s Cluster  | k8s-pi5-cluster (ARM64)                 |
| Namespace    | azuredevopskats                         |
| Docker Image | stuartshay/azuredevopskats (Docker Hub) |
| Helm Chart   | oci://ghcr.io/stuartshay/helm-charts    |
| DNS          | kats.lab.informationcart.com            |
| Ingress      | ingress-nginx (192.168.1.100)           |
| Deployment   | Helm (self-contained, no Argo CD)       |

## Repository Structure

```text
AzureDevOpsKats/
├── src/
│   ├── AzureDevOpsKats.Web/        # Blazor Web App (Server + WASM host)
│   │   ├── Components/             # Blazor components (pages, layout)
│   │   ├── Data/                   # EF Core context, entities, SQLite
│   │   ├── Services/               # Business logic
│   │   ├── wwwroot/                # Static assets (uploads)
│   │   ├── Program.cs              # Minimal hosting entry point
│   │   └── appsettings.json        # Configuration
│   └── AzureDevOpsKats.Client/     # WASM interactive client
├── tests/
│   └── AzureDevOpsKats.Tests/      # xUnit + bUnit tests
├── charts/
│   └── azuredevopskats/            # Helm chart
├── docs/                           # Documentation
├── assets/                         # Cat photo seed data
├── AzureDevOpsKats.sln             # Solution file
├── Directory.Build.props           # Shared build properties
├── global.json                     # .NET SDK version pin
├── Dockerfile                      # Multi-stage, multi-arch
├── Makefile                        # Build automation
├── setup.sh                        # Dev environment bootstrap
└── VERSION                         # Release version
```

## Development Workflow

### Branch Strategy

⚠️ **CRITICAL RULE**: NEVER commit directly to `main` branch. All changes
MUST go through `develop` or `feature/*` branches.

- **main**: Protected branch, production-only (PR required, direct commits
  FORBIDDEN)
- **develop**: Primary development branch (work here by default)
- **feature/\***: Feature branches (use for isolated features, PR to `main`)

### Before Starting Any Work

**ALWAYS sync your working branch with the remote before making changes:**

```bash
# If working on develop:
git checkout develop && git fetch origin && git pull origin develop

# If creating a new feature branch:
git checkout main && git fetch origin && git pull origin main
git checkout -b feature/my-feature
```

**ALWAYS rebase onto the latest protected branch before creating a PR:**

```bash
git fetch origin main && git rebase origin/main
```

### Before Creating a PR

⚠️ **ALWAYS check for and resolve conflicts before creating a PR:**

1. Rebase onto the latest protected branch:
   `git fetch origin main && git rebase origin/main`
2. Resolve any conflicts during rebase
3. Force-push the rebased branch: `git push origin <branch> --force-with-lease`
4. Verify the PR shows no conflicts on GitHub before requesting review

This is especially important after squash merges, which cause develop to
diverge from main.

### Daily Workflow

1. **ALWAYS** start from `develop` or create a feature branch
2. **Sync with remote** before making any changes (see above)
3. Run `make setup` if on a fresh environment (installs .NET SDK 10, Helm, pre-commit)
4. Run `dotnet restore` to install dependencies
5. Run `make dev` for development server (hot reload)
6. Test endpoints: `curl http://localhost:8080/health`
7. Run `pre-commit run -a` before commit
8. Run `make test` to validate
9. Commit and push to `develop` or `feature/*` branch
10. **For feature branches**: rebase onto latest `main` before PR:
    `git fetch origin main && git rebase origin/main`
11. Create PR to `main` when ready for deployment
12. **NEVER**: `git push origin main` or commit directly to main

## Writing Code

### .NET Best Practices

- Use .NET 10 minimal hosting model (`WebApplication.CreateBuilder`)
- Use top-level statements in `Program.cs`
- Enable nullable reference types (`<Nullable>enable</Nullable>`)
- Enable implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`)
- Use file-scoped namespaces
- Use primary constructors where appropriate
- Use `ILogger<T>` for structured logging (no third-party logging libraries)
- Always specify `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`

### Blazor Components

- Page components in `Components/Pages/`
- Shared layout in `Components/Layout/`
- Use `@rendermode InteractiveServer` or `InteractiveWebAssembly` as appropriate
- Use `EditForm` with `DataAnnotationsValidator` for form validation
- Use `InputFile` for photo uploads
- Keep components focused — extract reusable components into `Components/Shared/`

### Database (EF Core + SQLite)

- Use EF Core 10 with SQLite provider
- Code-first migrations (`dotnet ef migrations add`)
- Use `DbContext` with dependency injection
- Configure entity relationships with Fluent API
- SQLite data file stored at configurable path (PV mount in K8s)
- Use async methods: `ToListAsync()`, `FindAsync()`, `SaveChangesAsync()`

### Health Checks

- `/health` — Liveness probe (application responding)
- `/ready` — Readiness probe (database accessible)
- Use `Microsoft.Extensions.Diagnostics.HealthChecks`

### API Endpoints (Optional)

- Use minimal API pattern: `app.MapGet("/api/v1/cats", ...)`
- Use `TypedResults` for response types
- Add OpenAPI documentation with `.WithOpenApi()`
- Validate input with `[FromQuery]` and `[FromBody]` attributes

### Spell Checking (cspell)

- The `cspell.json` `words` list **MUST always be sorted in strict alphabetical
  order** (case-insensitive)
- When adding a new word, insert it in its correct alphabetical position — do
  not append it to the end of the list

## Local Development Services

⚠️ **ALWAYS start local services in hot-reload mode.** Never use `make start`
or production mode for local development.

- **Start command**: `make dev` (runs `dotnet watch run` with hot reload)
- **Port**: 8080
- **Health check**: `curl http://localhost:8080/health`
- **Hot reload**: Automatically restarts on C# and Razor file changes
- Do NOT use `make start` for development — it runs without hot reload

## Docker

### Building Locally

```bash
# Build image
make docker-build

# Run container
make docker-run

# Verify
curl http://localhost:8080/health
```

### Multi-arch

- Docker image targets `linux/amd64,linux/arm64`
- k8s-pi5-cluster runs ARM64 (Raspberry Pi 5)
- CI builds use QEMU + Docker Buildx for cross-compilation

## Helm Chart

### Linting

```bash
# Lint chart
helm lint charts/azuredevopskats

# Template and validate
helm template azuredevopskats charts/azuredevopskats | kubeconform -summary
```

### Local Install

```bash
# Install from local chart
helm install azuredevopskats charts/azuredevopskats \
  --namespace azuredevopskats --create-namespace \
  --set image.tag=latest

# Upgrade
helm upgrade azuredevopskats charts/azuredevopskats \
  --namespace azuredevopskats \
  --set image.tag=<version>

# Uninstall
helm uninstall azuredevopskats -n azuredevopskats
```

### From OCI Registry

```bash
# Install from GHCR
helm install azuredevopskats \
  oci://ghcr.io/stuartshay/helm-charts/azuredevopskats \
  --namespace azuredevopskats --create-namespace \
  --set image.tag=1.0.1
```

## CI/CD Pipelines

Three workflows run on push/PR:

| Workflow      | File         | Checks                                                                                             |
| ------------- | ------------ | -------------------------------------------------------------------------------------------------- |
| Lint and Test | `lint.yml`   | pre-commit, dotnet format, dotnet build (warnaserror), dotnet test (coverage), hadolint, helm lint |
| Docker        | `docker.yml` | Multi-arch build, push to Docker Hub on main                                                       |
| Helm          | `helm.yml`   | Package + push chart to GHCR on main                                                               |

**Replicate CI locally before pushing:**

```bash
pre-commit run --all-files   # All lint checks
make test                    # Tests with coverage
helm lint charts/azuredevopskats  # Chart validation
```

## Testing Conventions

- Tests live in `tests/AzureDevOpsKats.Tests/` using xUnit
- Use bUnit for Blazor component testing
- Use FluentAssertions for readable assertions
- Use in-memory SQLite for database tests
- Coverage must be ≥ 80%

```bash
make test          # Run tests with coverage
make test-cov      # Run tests with HTML coverage report
```

## Safety Rules (Do Not)

- ⛔ **NEVER commit directly to main branch** - ALWAYS use develop or feature
  branches
- Do not commit secrets or `.env` files
- Do not use `latest` Docker tags in deployments
- Do not skip `pre-commit run -a` before commits
- Do not hardcode connection strings or file paths
- Do not skip `helm lint` before pushing chart changes
- Do not use `helm install` on the cluster without reviewing `helm template`
  output first

## Quick Commands

```bash
make help          # Show all commands
make setup         # Bootstrap environment (first-time: installs .NET SDK, Helm, pre-commit)
make dev           # Start dev server (hot reload)
make build         # Build solution
make test          # Run tests with coverage
make lint          # Check code formatting
make format        # Format code
make clean         # Clean build artifacts
make docker-build  # Build Docker image
make docker-run    # Run container locally
make helm-lint     # Lint Helm chart
make helm-package  # Package Helm chart
pre-commit run -a  # Pre-commit checks
```

> **Note**: The Makefile automatically sets `DOTNET_ROOT=$HOME/.dotnet` and prepends it
> to `PATH`. When running `dotnet` commands directly (not via `make`), export these
> first: `export DOTNET_ROOT="$HOME/.dotnet" && export PATH="$DOTNET_ROOT:$PATH"`

## Related Repositories

- [k8s-gitops](https://github.com/stuartshay/k8s-gitops) — Cluster
  infrastructure (ingress, cert-manager, MetalLB reference)
- [otel-data-api](https://github.com/stuartshay/otel-data-api) — CI/CD pattern
  reference (Docker, GitHub Actions, Makefile)
- [otel-data-gateway](https://github.com/stuartshay/otel-data-gateway) — Docker
  build pattern reference

## When Unsure

- Check existing code for patterns
- Reference .NET 10 and Blazor documentation
- Run `dotnet build --warnaserror` to catch issues early
- Validate with `pre-commit run -a` before asking
- Use `make dev` for local testing before pushing
- Use `helm template` to preview K8s manifests before installing
