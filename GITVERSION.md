# GitVersion Configuration Guide

This project uses [GitVersion](https://gitversion.net/) for automatic semantic versioning based on Git history and branching strategy.

## 🎯 Overview

GitVersion automatically calculates version numbers based on:
- Git branch names
- Git tags
- Commit messages
- Merge history

No more manual version updates in project files! 🎉

## 📋 Configuration File

The configuration is in `GitVersion.yml` at the repository root.

## 🌿 Branch Strategies

### Main Branch (`main` or `master`)
- **Mode**: Continuous Delivery
- **Tag**: None (production releases)
- **Increment**: Patch
- **Version Example**: `1.0.0`, `1.0.1`, `1.0.2`

**Usage**: Stable production releases

### Develop Branch (`develop` or `development`)
- **Mode**: Continuous Deployment
- **Tag**: `alpha`
- **Increment**: Minor
- **Version Example**: `0.2.0-alpha.1`, `0.2.0-alpha.2`

**Usage**: Active development, pre-release versions

### Release Branch (`release/` or `releases/`)
- **Mode**: Continuous Delivery
- **Tag**: `beta`
- **Increment**: None (version set when branch created)
- **Version Example**: `1.0.0-beta.1`, `1.0.0-beta.2`

**Usage**: Release candidates and stabilization

### Feature Branch (`feature/` or `features/`)
- **Mode**: Continuous Deployment
- **Tag**: Branch name
- **Increment**: Inherit from target branch
- **Version Example**: `0.2.0-my-feature.1`

**Usage**: Feature development

### Hotfix Branch (`hotfix/` or `hotfixes/`)
- **Mode**: Continuous Deployment
- **Tag**: `beta`
- **Increment**: Patch
- **Version Example**: `1.0.1-beta.1`

**Usage**: Critical production fixes

### Pull Request Branch (`pull/`, `pull-requests/`, `pr/`)
- **Mode**: Continuous Deployment
- **Tag**: `PullRequest`
- **Increment**: Inherit
- **Version Example**: `0.2.0-PullRequest0123.1`

**Usage**: Automatically handled by GitHub PRs

## 🏷️ Version Bumping with Commit Messages

You can control version increments using special commit messages:

### Major Version Bump (Breaking Changes)
```bash
git commit -m "feat: new API +semver: major"
# or
git commit -m "BREAKING CHANGE: removed old API +semver: breaking"
```
**Result**: `1.0.0` → `2.0.0`

### Minor Version Bump (New Features)
```bash
git commit -m "feat: add new validation feature +semver: minor"
# or
git commit -m "Add feature +semver: feature"
```
**Result**: `1.0.0` → `1.1.0`

### Patch Version Bump (Bug Fixes)
```bash
git commit -m "fix: correct validation bug +semver: patch"
# or
git commit -m "Bug fix +semver: fix"
```
**Result**: `1.0.0` → `1.0.1`

### Skip Version Bump
```bash
git commit -m "docs: update README +semver: none"
# or
git commit -m "chore: update dependencies +semver: skip"
```
**Result**: Version stays the same

## 📦 Typical Workflows

### Workflow 1: Feature Development

```bash
# Start from develop
git checkout develop
git pull

# Create feature branch
git checkout -b feature/new-validator

# Make changes and commit
git add .
git commit -m "feat: add new validator +semver: minor"

# Push and create PR
git push origin feature/new-validator
# Version will be something like: 0.2.0-new-validator.1
```

### Workflow 2: Release to Production

```bash
# Create release branch from develop
git checkout develop
git checkout -b release/1.0.0

# Stabilize, fix bugs
git commit -m "fix: minor bug +semver: patch"
# Version: 1.0.0-beta.1, 1.0.0-beta.2, etc.

# Merge to main when ready
git checkout main
git merge release/1.0.0

# Tag the release
git tag v1.0.0
git push origin main --tags
# Version: 1.0.0 (no pre-release tag)
```

### Workflow 3: Hotfix

```bash
# Create hotfix from main
git checkout main
git checkout -b hotfix/critical-bug

# Fix the bug
git commit -m "fix: critical security issue +semver: patch"
# Version: 1.0.1-beta.1

# Merge back to main and develop
git checkout main
git merge hotfix/critical-bug
git tag v1.0.1
git push origin main --tags

git checkout develop
git merge hotfix/critical-bug
git push origin develop
```

## 🔍 Local Testing

### Install GitVersion CLI

```bash
dotnet tool install --global GitVersion.Tool
```

### Check Current Version

```bash
# From repository root
dotnet-gitversion

# Output will show:
# - Major, Minor, Patch
# - SemVer, NuGetVersion
# - Branch name
# - Commit info
```

### See Version for Specific Branch

```bash
git checkout feature/my-feature
dotnet-gitversion
```

### Show Version Variables

```bash
dotnet-gitversion /showvariable SemVer
dotnet-gitversion /showvariable NuGetVersion
dotnet-gitversion /showvariable FullSemVer
```

## 🚀 CI/CD Integration

GitVersion is integrated into all GitHub Actions workflows:

### Build Workflow
- Calculates version on every build
- Uses version for assembly and package metadata
- Includes version in artifact names

### Publish Workflow
- Determines version from git tag
- Uses for NuGet package version
- Creates GitHub Release with version

### PR Validation
- Shows what version would be built
- Includes version in PR comment

## 📊 Version Examples by Branch

| Branch | Example Version | Notes |
|--------|----------------|-------|
| `main` | `1.0.0` | Clean production version |
| `develop` | `0.2.0-alpha.5` | Pre-release, auto-incremented |
| `release/1.0.0` | `1.0.0-beta.1` | Release candidate |
| `feature/login` | `0.2.0-login.3` | Feature branch version |
| `hotfix/bug` | `1.0.1-beta.1` | Hotfix version |
| PR #123 | `0.2.0-PullRequest0123.1` | PR-specific version |

## 🎯 Initial Version Setup

If you're starting fresh or want to set an initial version:

```bash
# Tag the initial version
git tag v0.1.0
git push origin v0.1.0

# GitVersion will use this as the base
# Next versions will increment from here
```

## ⚙️ Configuration Options

### Tag Prefix
Default: `v` (tags like `v1.0.0`)

To change: Edit `tag-prefix` in `GitVersion.yml`

### Increment Strategy
Current settings:
- `main`: Patch increment (1.0.0 → 1.0.1)
- `develop`: Minor increment (0.1.0 → 0.2.0)
- `release/*`: No increment (set when branch created)
- `feature/*`: Inherit from parent branch

### Pre-release Tags
- `develop`: Uses `alpha`
- `release/*`: Uses `beta`
- `feature/*`: Uses branch name
- `hotfix/*`: Uses `beta`

## 🐛 Troubleshooting

### Issue: "No version could be determined"

**Solution**: Ensure you have at least one commit and optionally a tag:
```bash
git tag v0.1.0
git push origin v0.1.0
```

### Issue: Version doesn't increment

**Cause**: Default increment is set to `None` or no commits since last version

**Solution**: Use commit message semver tags:
```bash
git commit -m "fix: issue +semver: patch"
```

### Issue: Wrong version calculated

**Cause**: Wrong branch detected or configuration mismatch

**Solution**: 
1. Check current branch: `git branch --show-current`
2. Verify it matches a pattern in `GitVersion.yml`
3. Run `dotnet-gitversion` to see what GitVersion detects

### Issue: CI/CD fails with GitVersion error

**Cause**: Shallow clone (not enough git history)

**Solution**: Workflows are already configured with `fetch-depth: 0`

## 📚 Additional Resources

- [GitVersion Documentation](https://gitversion.net/docs/)
- [Semantic Versioning](https://semver.org/)
- [Branch Strategies](https://gitversion.net/docs/learn/branching-strategies/)
- [Configuration Reference](https://gitversion.net/docs/reference/configuration)

## 💡 Best Practices

1. **Always use semantic commit messages**
   - Makes it clear what changed
   - Helps GitVersion choose correct increment

2. **Tag releases on main branch**
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

3. **Let GitVersion manage versions**
   - Don't manually edit version in `.csproj`
   - Trust the calculated version

4. **Use branches as intended**
   - `main`: Production-ready code only
   - `develop`: Integration branch
   - `feature/*`: Individual features
   - `release/*`: Release preparation

5. **Check version locally before pushing**
   ```bash
   dotnet-gitversion
   ```

## 🎉 Benefits

✅ **No manual version management**
✅ **Consistent versioning across team**
✅ **Semantic versioning by default**
✅ **Pre-release versions automatic**
✅ **Works with any CI/CD system**
✅ **Based on git history (single source of truth)**

---

**Questions?** Check the [GitVersion docs](https://gitversion.net/) or open an issue!

