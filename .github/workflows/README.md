# GitHub Actions Workflows

This repository uses GitHub Actions for CI/CD automation. Below is a description of each workflow and how to use them.

## 📋 Available Workflows

### 1. Build and Test (`build.yml`)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Manual trigger via workflow dispatch

**What it does:**
- Builds the project on multiple .NET versions (6.0 and 8.0)
- Runs all tests
- Creates NuGet packages
- Validates code quality
- Uploads build artifacts

**Jobs:**
- `build`: Compiles and tests on .NET 6.0 and 8.0
- `code-quality`: Checks code formatting and style
- `package-validation`: Validates the generated NuGet package

**Artifacts:**
- `nuget-package`: The main .nupkg file (retained for 7 days)
- `symbol-package`: The .snupkg symbol file (retained for 7 days)

### 2. Publish to NuGet (`publish.yml`)

**Triggers:**
- Push of version tags (e.g., `v0.1.0`, `v1.0.0`)
- Manual trigger with version input

**What it does:**
- Builds the project in Release mode
- Runs all tests to ensure quality
- Creates NuGet packages
- Publishes to NuGet.org
- Creates a GitHub Release with the packages attached

**Requirements:**
- `NUGET_API_KEY` secret must be configured in repository settings

**How to use:**

#### Option 1: Tag-based Release (Recommended)
```bash
# Create and push a version tag
git tag v0.1.0
git push origin v0.1.0
```

#### Option 2: Manual Trigger
1. Go to Actions tab in GitHub
2. Select "Publish to NuGet" workflow
3. Click "Run workflow"
4. Enter the version number (e.g., `0.1.0`)
5. Click "Run workflow"

### 3. PR Validation (`pr-validation.yml`)

**Triggers:**
- Opening a pull request to `main` or `develop`
- Pushing new commits to an open pull request
- Reopening a pull request

**What it does:**
- Validates that the code builds successfully
- Runs all tests
- Checks code formatting
- Performs a dry-run package build
- Posts build status as a comment on the PR
- Runs security scans

**Jobs:**
- `validate`: Build, test, and package validation
- `security`: Security and linting checks

## 🔧 Setup Instructions

### 1. Configure NuGet API Key

To enable publishing to NuGet.org:

1. Get your NuGet API key:
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with "Push" permission
   - Copy the key (you'll only see it once!)

2. Add it to GitHub:
   - Go to your repository Settings
   - Navigate to Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Paste your NuGet API key
   - Click "Add secret"

### 2. Branch Protection (Optional but Recommended)

Configure branch protection rules for `main`:

1. Go to Settings → Branches
2. Add rule for `main` branch
3. Enable:
   - ✅ Require pull request reviews before merging
   - ✅ Require status checks to pass before merging
   - ✅ Require branches to be up to date before merging
   - Select required checks:
     - `Build and Test / build`
     - `PR Validation / validate`

## 📦 Publishing a New Version

### Step-by-Step Release Process

1. **Update version and changelog:**
   ```bash
   # Edit the version in johnverbiest.belgian.insz.csproj
   # Update CHANGELOG.md with release notes
   ```

2. **Commit and push changes:**
   ```bash
   git add .
   git commit -m "Bump version to 0.2.0"
   git push origin develop
   ```

3. **Create and push version tag:**
   ```bash
   git tag -a v0.2.0 -m "Release version 0.2.0"
   git push origin v0.2.0
   ```

4. **Monitor the workflow:**
   - Go to Actions tab in GitHub
   - Watch the "Publish to NuGet" workflow run
   - It will automatically:
     - Build and test
     - Publish to NuGet.org
     - Create a GitHub Release

5. **Verify publication:**
   - Check NuGet.org: https://www.nuget.org/packages/JohnVerbiest.Belgium.Insz/
   - Check GitHub Releases: https://github.com/johnverbiest/NationalNumber.Be.DotNet/releases

## 🔍 Workflow Status Badges

Add these badges to your README.md:

```markdown
[![Build](https://github.com/johnverbiest/NationalNumber.Be.DotNet/actions/workflows/build.yml/badge.svg)](https://github.com/johnverbiest/NationalNumber.Be.DotNet/actions/workflows/build.yml)
[![Publish](https://github.com/johnverbiest/NationalNumber.Be.DotNet/actions/workflows/publish.yml/badge.svg)](https://github.com/johnverbiest/NationalNumber.Be.DotNet/actions/workflows/publish.yml)
```

## 🐛 Troubleshooting

### Build Fails on Push

**Problem**: Workflow fails immediately after push

**Solutions:**
- Check that all dependencies are properly restored
- Ensure the .csproj file is valid XML
- Review build logs in the Actions tab

### Tests Fail

**Problem**: Tests pass locally but fail in CI

**Solutions:**
- Check for environment-specific code (file paths, etc.)
- Ensure test data is committed to the repository
- Review test output in the Actions tab

### Publish Fails

**Problem**: Package doesn't publish to NuGet.org

**Common causes:**
- `NUGET_API_KEY` secret not configured or expired
- Package with same version already exists on NuGet.org
- API key doesn't have "Push" permission

**Solutions:**
1. Verify the secret exists: Settings → Secrets and variables → Actions
2. Check API key permissions on nuget.org
3. Ensure version number is incremented
4. Check workflow logs for detailed error message

### Package Validation Fails

**Problem**: dotnet-validate reports issues

**Solutions:**
- Review package contents using NuGet Package Explorer
- Ensure README.md is included in the package
- Verify all metadata is correct in .csproj

## 🎯 Best Practices

1. **Always test locally before pushing:**
   ```bash
   dotnet build
   dotnet test
   dotnet pack
   ```

2. **Use semantic versioning:**
   - `MAJOR.MINOR.PATCH` (e.g., `1.0.0`)
   - Increment MAJOR for breaking changes
   - Increment MINOR for new features
   - Increment PATCH for bug fixes

3. **Keep CHANGELOG.md updated:**
   - Document all changes before releasing
   - Follow [Keep a Changelog](https://keepachangelog.com/) format

4. **Test packages locally before publishing:**
   ```bash
   dotnet nuget add source ./bin/Release -n LocalTest
   dotnet add package JohnVerbiest.Belgium.Insz --source LocalTest
   ```

5. **Monitor workflow runs:**
   - Check the Actions tab after pushing
   - Review logs if something fails
   - Fix issues promptly

## 📊 Workflow Diagram

```
┌─────────────┐
│ Code Change │
└──────┬──────┘
       │
       ├─── Push to develop/main
       │    └──► Build and Test Workflow
       │         ├─ Build on .NET 6.0
       │         ├─ Build on .NET 8.0
       │         ├─ Run tests
       │         ├─ Code quality checks
       │         └─ Create artifacts
       │
       ├─── Open Pull Request
       │    └──► PR Validation Workflow
       │         ├─ Build validation
       │         ├─ Test validation
       │         ├─ Security scan
       │         └─ Post PR comment
       │
       └─── Push version tag (v*.*.*)
            └──► Publish Workflow
                 ├─ Build & Test
                 ├─ Create package
                 ├─ Publish to NuGet.org
                 └─ Create GitHub Release
```

## 🔗 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [NuGet Publishing Guide](https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [Semantic Versioning](https://semver.org/)
- [Keep a Changelog](https://keepachangelog.com/)

## 💡 Tips

- Use draft releases for testing the release process
- Enable email notifications for failed workflows
- Review security advisories in the Security tab
- Keep dependencies updated using Dependabot
- Use environments for production deployments (requires GitHub Pro/Team)

---

**Need Help?** Open an issue or check the workflow logs in the Actions tab.

