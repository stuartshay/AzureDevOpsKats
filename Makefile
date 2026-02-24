.DEFAULT_GOAL := help

PROJECT_NAME := AzureDevOpsKats
VERSION := $(shell cat VERSION 2>/dev/null || echo "0.0.0")
DOCKER_IMAGE := stuartshay/azuredevopskats
SOLUTION := AzureDevOpsKats.sln
WEB_PROJECT := src/AzureDevOpsKats.Web/AzureDevOpsKats.Web.csproj
TEST_PROJECT := tests/AzureDevOpsKats.Tests/AzureDevOpsKats.Tests.csproj

.PHONY: help setup build dev start stop clean lint format test test-cov \
        docker-build docker-run docker-push helm-lint helm-package outdated verify health

## —— Setup ———————————————————————————————————————
help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

setup: ## Run setup script
	./setup.sh

## —— Development ——————————————————————————————————
build: ## Build the solution
	dotnet build $(SOLUTION)

dev: ## Start development server with hot reload
	dotnet watch run --project $(WEB_PROJECT)

start: ## Start production server
	dotnet run --project $(WEB_PROJECT) --configuration Release

stop: ## Stop running processes
	@pkill -f "dotnet.*AzureDevOpsKats" || true

clean: ## Clean build artifacts
	dotnet clean $(SOLUTION)
	rm -rf **/bin **/obj artifacts publish TestResults coverage coverage-html

## —— Quality ——————————————————————————————————————
lint: ## Check code formatting
	dotnet format $(SOLUTION) --verify-no-changes

format: ## Format code
	dotnet format $(SOLUTION)

test: ## Run tests with coverage
	dotnet test $(TEST_PROJECT) \
		--collect:"XPlat Code Coverage" \
		--results-directory TestResults \
		--logger "console;verbosity=normal"

test-cov: ## Run tests with HTML coverage report
	dotnet test $(TEST_PROJECT) \
		--collect:"XPlat Code Coverage" \
		--results-directory TestResults
	@echo "Coverage report in TestResults/"

## —— Docker ———————————————————————————————————————
docker-build: ## Build Docker image
	docker build -t $(DOCKER_IMAGE):$(VERSION) -t $(DOCKER_IMAGE):latest .

docker-run: ## Run Docker container
	docker run --rm -p 8080:8080 $(DOCKER_IMAGE):$(VERSION)

docker-push: ## Push Docker image
	docker push $(DOCKER_IMAGE):$(VERSION)
	docker push $(DOCKER_IMAGE):latest

## —— Helm —————————————————————————————————————————
helm-lint: ## Lint Helm chart
	helm lint charts/azuredevopskats

helm-package: ## Package Helm chart
	helm package charts/azuredevopskats

## —— Utility ——————————————————————————————————————
outdated: ## Show outdated NuGet dependencies
	dotnet list $(SOLUTION) package --outdated

verify: ## Verify build, lint, and tests
	$(MAKE) build
	$(MAKE) lint
	$(MAKE) test

health: ## Check health endpoint
	@curl -sf http://localhost:8080/health && echo " OK" || echo " FAIL"
