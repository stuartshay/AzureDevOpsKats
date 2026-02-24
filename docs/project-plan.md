# AzureDevOpsKats — Modernization Project Plan

## Overview

Rewrite the legacy AzureDevOpsKats cat-photo CRUD application from .NET 6 to
.NET 10 Blazor Web App (Interactive Server + WASM). Containerize with multi-arch
Docker, publish a Helm chart to GitHub Container Registry (OCI), and deploy to
k8s-pi5-cluster using `helm install` (no Argo CD). Follow otel-\* repository
patterns for CI/CD, linting, and developer tooling.

## Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Runtime | .NET 10 LTS | Latest LTS, released Nov 2025 |
| Frontend | Blazor Web App (Server + WASM) | Fully C#, no Node.js toolchain, self-contained |
| Database | SQLite with Kubernetes PV (NFS) | Simple, no external DB dependency |
| Helm Repository | GitHub Container Registry (OCI) | Modern, no extra infrastructure |
| Deployment | Direct `helm install` | Self-contained, no Argo CD dependency |
| Docker Image | `stuartshay/azuredevopskats` (Docker Hub) | Existing registry |
| Branch Strategy | `develop` → PR to `master` | Matches existing homelab repos |
| Application Scope | Keep cat photo CRUD functionality | Full rewrite to modern patterns |

## What Gets Deleted

All legacy CI/CD, cloud infrastructure, and obsolete tooling will be removed:

- **CI/CD**: Jenkinsfile\*, appveyor.yml, azure-pipelines.yml, Cake build system
  (build.cake, build.sh, build.ps1, build.config)
- **Infrastructure**: docker/ (old Dockerfiles), all docker-compose\*.yml,
  infrastructure/ (Azure Container Instances), terraform/ (AWS ECS/VPC),
  ecspresso/ (ECS task defs), .yair-config.yaml
- **Tooling**: e2e/ (JMeter), docfx/, docfx.json, NuGet.config, build/
- **Source**: src/ and test/ (rewritten from scratch)
- **GitHub Actions**: All existing workflows (recreated with modern patterns)

## What Gets Kept

- `README.md` (rewritten)
- `LICENSE` (MIT)
- `assets/` (cat photos — used as seed data)
- `.editorconfig` (updated)
- `.gitignore` (updated for .NET 10 + Helm)

---

## Phase 1: Repository Cleanup & Branch Setup

**Goal**: Create develop branch, remove all legacy/irrelevant files.

### Tasks

1. Create `develop` branch from current HEAD
2. Delete legacy CI/CD files
3. Delete legacy infrastructure directories
4. Delete legacy tooling directories
5. Delete old source entirely (src/, test/)
6. Update `.gitignore` for .NET 10 + Helm

### Deliverables

- Clean repository with only assets, license, docs, and config files

---

## Phase 2: Project Scaffolding

**Goal**: Create modern .NET 10 solution structure with Blazor Web App.

### New Solution Structure

```text
AzureDevOpsKats/
├── src/
│   ├── AzureDevOpsKats.Web/           # Blazor Web App (Server + WASM host)
│   │   ├── AzureDevOpsKats.Web.csproj
│   │   ├── Program.cs                  # Minimal hosting
│   │   ├── Components/                 # Blazor components
│   │   │   ├── App.razor
│   │   │   ├── Routes.razor
│   │   │   ├── Layout/
│   │   │   │   ├── MainLayout.razor
│   │   │   │   └── NavMenu.razor
│   │   │   └── Pages/
│   │   │       ├── Home.razor
│   │   │       ├── Cats.razor           # Cat list + CRUD
│   │   │       └── CatDetail.razor      # Single cat view/edit
│   │   ├── Data/
│   │   │   ├── CatDbContext.cs           # EF Core SQLite context
│   │   │   ├── Cat.cs                    # Entity model
│   │   │   └── Database.sqlite           # Seed database
│   │   ├── Services/
│   │   │   ├── ICatService.cs
│   │   │   └── CatService.cs
│   │   ├── wwwroot/                      # Static assets
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   └── AzureDevOpsKats.Client/          # WASM interactive client
│       ├── AzureDevOpsKats.Client.csproj
│       └── _Imports.razor
│
├── tests/
│   └── AzureDevOpsKats.Tests/
│       ├── AzureDevOpsKats.Tests.csproj
│       ├── Services/
│       │   └── CatServiceTests.cs
│       └── Components/
│           └── CatListTests.cs           # bUnit component tests
│
├── AzureDevOpsKats.sln
├── Directory.Build.props
└── global.json
```

### Tasks

1. Create `global.json` pinning .NET 10 SDK
2. Create `Directory.Build.props` with shared version, analyzers, nullable
3. Create solution file with Web, Client, and Tests projects
4. Use EF Core 10 with SQLite provider
5. Configure health check endpoints (`/health`, `/ready`)

---

## Phase 3: Application Code — Cat Photo CRUD

**Goal**: Rewrite the full cat photo CRUD in Blazor with EF Core + SQLite.

### Tasks

1. **Entity Model** — `Cat.cs`: Id, Name, Description, Photo (file path),
   CreatedAt, UpdatedAt
2. **EF Core DbContext** — `CatDbContext.cs`: Configure SQLite, seed data,
   code-first migrations
3. **Service Layer** — `ICatService` / `CatService`: GetAll (paginated),
   GetById, Create (with photo upload), Update, Delete
4. **Blazor Pages**:
   - `Home.razor` — Landing page with app info
   - `Cats.razor` — Paginated cat list with search, create button
   - `CatDetail.razor` — View/edit/delete single cat, photo upload via
     `InputFile`
5. **Layout** — `MainLayout.razor` + `NavMenu.razor` with navigation
6. **Minimal API Endpoints** — `/api/v1/cats` for external consumers
7. **Photo Storage** — `wwwroot/uploads/` (mapped to PV in K8s)
8. **Swagger/OpenAPI** — Built-in .NET 10 OpenAPI for API docs
9. **Configuration** — `appsettings.json` with SQLite path, logging level
10. **Logging** — Built-in `ILogger` with structured logging

### Tests

- **Unit tests**: CatService with in-memory SQLite
- **Component tests**: bUnit tests for Blazor components
- **Framework**: xUnit + bUnit + FluentAssertions

---

## Phase 4: Developer Tooling

**Goal**: Match otel-\* developer experience patterns.

### Tasks

1. **`setup.sh`** — Install .NET 10 SDK (via `dotnet-install.sh`), restore
   packages, install pre-commit, install dotnet tools
2. **`Makefile`** — Targets: `help`, `setup`, `restore`, `build`, `dev`
   (watch), `test`, `lint`, `format`, `clean`, `docker-build`, `docker-run`,
   `helm-package`, `helm-push`
3. **`VERSION`** — Start at `1.0`
4. **`.pre-commit-config.yaml`** — pre-commit-hooks, markdownlint, hadolint,
   shellcheck, cspell, dotnet-format
5. **`cspell.json`** — Project dictionary (sorted alphabetically)
6. **`.markdownlint.yaml`** — Markdown lint rules
7. **`renovate.json`** — Dependency update automation
8. **`AGENTS.md`** + **`.github/copilot-instructions.md`** — Agent
   customization

---

## Phase 5: Docker — Multi-arch Container Image

**Goal**: Single root Dockerfile, multi-arch (amd64 + arm64) for
k8s-pi5-cluster.

### Tasks

1. **`Dockerfile`** at repo root — Multi-stage build:
   - Build stage: `mcr.microsoft.com/dotnet/sdk:10.0`
   - Runtime stage: `mcr.microsoft.com/dotnet/aspnet:10.0`
   - Non-root user, HEALTHCHECK, OCI labels
   - Port 8080 (.NET 10 default)
   - No Node.js needed (Blazor, not React)
2. **`.dockerignore`** — Exclude bin/, obj/, .git, tests/, charts/, docs/
3. **Image**: `stuartshay/azuredevopskats` (existing Docker Hub repo)
4. **Platforms**: `linux/amd64,linux/arm64`

---

## Phase 6: GitHub Actions — CI/CD Workflows

**Goal**: Lint, test, Docker build, and Helm publish workflows following otel-\*
patterns.

### Workflows

1. **`lint.yml`** — Triggers on push/PR to master and develop:
   - Pre-commit checks
   - `dotnet format --verify-no-changes`
   - `dotnet build --warnaserror`
   - `dotnet test` with coverage threshold
   - Hadolint, Helm lint
2. **`docker.yml`** — Triggers on push to master + workflow\_dispatch:
   - QEMU + Docker Buildx
   - Version from `VERSION` + run number
   - Multi-platform build + push to Docker Hub
3. **`helm.yml`** — Triggers on push to master when chart changes:
   - `helm package` + `helm push` to `oci://ghcr.io/stuartshay/helm-charts`

---

## Phase 7: Helm Chart

**Goal**: Self-contained Helm chart for deployment to k8s-pi5-cluster.

### Chart Structure

```text
charts/azuredevopskats/
├── Chart.yaml
├── values.yaml
├── templates/
│   ├── _helpers.tpl
│   ├── namespace.yaml
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── ingress.yaml
│   ├── serviceaccount.yaml
│   ├── configmap.yaml
│   ├── pvc.yaml                # SQLite data + photo uploads
│   ├── tests/
│   │   └── test-connection.yaml
│   └── NOTES.txt
└── .helmignore
```

### Key Configuration (values.yaml)

| Setting | Default |
|---------|---------|
| `image.repository` | `stuartshay/azuredevopskats` |
| `image.tag` | `latest` |
| `replicaCount` | `1` (SQLite limitation) |
| `service.port` | `80` → targetPort `8080` |
| `ingress.className` | `nginx` |
| `ingress.host` | `kats.lab.informationcart.com` |
| `persistence.storageClass` | `nfs-client` |
| `persistence.size` | `1Gi` |

---

## Phase 8: Deployment to Cluster

**Goal**: Install the app on k8s-pi5-cluster using Helm from GHCR.

### Tasks

1. Create Cloudflare DNS: `kats.lab.informationcart.com` → `192.168.1.100`
2. Login to GHCR: `helm registry login ghcr.io -u stuartshay`
3. Install:

   ```bash
   helm install azuredevopskats \
     oci://ghcr.io/stuartshay/helm-charts/azuredevopskats \
     --namespace azuredevopskats --create-namespace \
     --set image.tag=1.0.1
   ```

4. Document install/upgrade/rollback in `docs/deployment.md`

---

## Verification Checklist

- [ ] `dotnet build` — zero warnings
- [ ] `dotnet test` — all tests pass, ≥80% coverage
- [ ] `pre-commit run --all-files` — all hooks pass
- [ ] `docker build .` — image builds locally
- [ ] `docker run -p 8080:8080` — `/health` returns 200
- [ ] `helm lint charts/azuredevopskats` — chart validates
- [ ] `helm template ... | kubeconform -summary` — valid K8s manifests
- [ ] `helm install` on cluster — pod running, ingress works, TLS terminates
- [ ] Browse `https://kats.lab.informationcart.com/` — Blazor app loads

---

## Target Infrastructure

| Property | Value |
|----------|-------|
| K8s Cluster | k8s-pi5-cluster |
| Architecture | ARM64 (Raspberry Pi 5) |
| K8s Version | v1.31.14 |
| Ingress | ingress-nginx (192.168.1.100) |
| TLS | cert-manager + Let's Encrypt (Cloudflare DNS-01) |
| LB | MetalLB (192.168.1.200-210) |
| DNS | \*.lab.informationcart.com |
| Storage | NFS persistent volumes |
| Container Runtime | containerd 2.2.1 |

## Related Repositories

- [k8s-gitops](https://github.com/stuartshay/k8s-gitops) — Cluster
  infrastructure reference
- [otel-data-api](https://github.com/stuartshay/otel-data-api) — CI/CD pattern
  reference
- [otel-data-gateway](https://github.com/stuartshay/otel-data-gateway) — Docker
  pattern reference
- [homelab-database-migrations](https://github.com/stuartshay/homelab-database-migrations) —
  Database pattern reference
