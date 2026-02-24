# AzureDevOpsKats — Kubernetes Deployment Guide

Helm-based deployment of the AzureDevOpsKats Blazor Web App to the
k8s-pi5-cluster.

## Prerequisites

- `kubectl` configured with `k8s-pi5-cluster` context
- `helm` v3+ installed
- Docker Hub image `stuartshay/azuredevopskats` built and pushed
- DNS record for `kats.lab.informationcart.com` pointing to `192.168.1.100`

## Cluster Details

| Property          | Value                              |
|-------------------|------------------------------------|
| Cluster           | k8s-pi5-cluster                    |
| Namespace         | azuredevopskats                    |
| Nodes             | 4 (1 master + 3 workers, ARM64)    |
| Kubernetes        | v1.31.x                            |
| Ingress           | ingress-nginx (`192.168.1.100`)    |
| TLS               | cert-manager (Let's Encrypt)       |
| Storage           | NFS CSI (`nfs-general`)            |
| DNS               | `kats.lab.informationcart.com`     |

## Deployment

### Install from Local Chart

```bash
helm install azuredevopskats charts/azuredevopskats \
  --namespace azuredevopskats --create-namespace \
  --set image.tag=10.0.5
```

### Install from GHCR OCI Registry

```bash
helm install azuredevopskats \
  oci://ghcr.io/stuartshay/helm-charts/azuredevopskats \
  --namespace azuredevopskats --create-namespace \
  --set image.tag=10.0.5
```

### Upgrade

```bash
helm upgrade azuredevopskats charts/azuredevopskats \
  --namespace azuredevopskats \
  --set image.tag=<new-version>
```

### Rollback

```bash
helm rollback azuredevopskats -n azuredevopskats
```

### Uninstall

```bash
helm uninstall azuredevopskats -n azuredevopskats
kubectl delete namespace azuredevopskats
```

## Configuration

### Key Values (`values.yaml`)

| Value                            | Default                          | Description                        |
|----------------------------------|----------------------------------|------------------------------------|
| `image.repository`               | `stuartshay/azuredevopskats`     | Docker Hub image                   |
| `image.tag`                      | `latest`                         | Image tag (override at install)    |
| `ingress.enabled`                | `true`                           | Enable nginx ingress               |
| `ingress.hosts[0].host`          | `kats.lab.informationcart.com`   | External hostname                  |
| `ingress.annotations`            | `letsencrypt-dns01-production`   | Cert-manager cluster issuer        |
| `persistence.enabled`            | `true`                           | Enable PVC for SQLite data         |
| `persistence.storageClass`       | `nfs-general`                    | NFS CSI storage class              |
| `persistence.size`               | `1Gi`                            | PVC size                           |
| `securityContext.fsGroup`        | `1654`                           | Group ID for volume ownership      |

### SQLite on NFS

The application uses SQLite stored at `/app/data/cats.sqlite` on an NFS-backed
PersistentVolume. Two mechanisms ensure the non-root container user (`app`, UID
1654) can write to the NFS mount:

1. **`fsGroup: 1654`** — Kubernetes sets group ownership on the mounted volume
2. **`init-permissions` init container** — Runs `chown 1654:1654 /app/data`
   before the app starts

## Verification

### Check Pod Status

```bash
kubectl get pods -n azuredevopskats
```

Expected: `1/1 Running`, `0` restarts.

### Check All Resources

```bash
kubectl get all,pvc,ingress,certificate -n azuredevopskats
```

### Health Endpoints

```bash
# Health (liveness)
curl -s https://kats.lab.informationcart.com/health
# Expected: Healthy

# Ready (readiness)
curl -s https://kats.lab.informationcart.com/ready
# Expected: Healthy
```

### TLS Certificate

```bash
kubectl get certificate -n azuredevopskats
```

Expected: `READY=True`, issued by Let's Encrypt, valid 90 days.

### Helm Release

```bash
helm list -n azuredevopskats
helm get values azuredevopskats -n azuredevopskats
```

## Troubleshooting

### Pod CrashLoopBackOff

```bash
kubectl logs -n azuredevopskats -l app.kubernetes.io/name=azuredevopskats
kubectl describe pod -n azuredevopskats -l app.kubernetes.io/name=azuredevopskats
```

Common causes:

- **SQLite permission denied**: Check that `securityContext.fsGroup` is set to
  `1654` and the init container is running. Verify with:
  `kubectl exec -n azuredevopskats <pod> -- ls -la /app/data/`
- **PVC not bound**: Check `kubectl get pvc -n azuredevopskats` — status should
  be `Bound`
- **Image pull failure**: Verify the image tag exists on Docker Hub

### Ingress Not Working

```bash
kubectl describe ingress -n azuredevopskats
kubectl get certificate -n azuredevopskats
```

Verify DNS resolves: `dig kats.lab.informationcart.com`

### View Logs

```bash
# Current pod
kubectl logs -n azuredevopskats -l app.kubernetes.io/name=azuredevopskats

# Previous crashed pod
kubectl logs -n azuredevopskats -l app.kubernetes.io/name=azuredevopskats --previous
```

## Architecture

```text
Internet / LAN
       │
       ▼
┌──────────────────┐
│  Cloudflare DNS  │  kats.lab.informationcart.com → 192.168.1.100
└──────────────────┘
       │
       ▼
┌──────────────────┐
│  ingress-nginx   │  MetalLB LoadBalancer (192.168.1.100)
│  + cert-manager  │  TLS termination (Let's Encrypt)
└──────────────────┘
       │
       ▼
┌──────────────────┐
│  ClusterIP Svc   │  azuredevopskats:80 → pod:8080
└──────────────────┘
       │
       ▼
┌──────────────────┐
│  Blazor Web App  │  .NET 10, Kestrel on port 8080
│  + SQLite DB     │  /app/data/cats.sqlite (NFS PVC)
└──────────────────┘
```
