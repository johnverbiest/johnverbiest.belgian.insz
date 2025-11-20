# Trusted Publishing Setup Guide

This project uses **Trusted Publishing** (keyless authentication) for both NuGet and npm deployments. This eliminates the need to store long-lived API tokens as secrets! 🔐

## 🎯 Overview

Trusted Publishing uses OpenID Connect (OIDC) to securely authenticate GitHub Actions with package registries. Instead of using API keys/tokens, the workflow receives a short-lived token directly from the registry.

### Benefits

✅ **No API keys to manage** - No need to create, rotate, or revoke tokens
✅ **Better security** - Short-lived tokens that expire automatically  
✅ **Audit trail** - Package registries know exactly which workflow published each version
✅ **Provenance** - Verifiable build attestations showing package origin
✅ **Zero secrets** - Nothing sensitive stored in GitHub repository

## 📦 NuGet Trusted Publishing

### Prerequisites

- NuGet.org account with package ownership
- Access to package settings on NuGet.org

### Setup Steps

1. **Go to NuGet.org**
   - Sign in to your account
   - Navigate to your package: https://www.nuget.org/packages/johnverbiest.belgian.insz

2. **Enable Trusted Publishers**
   - Go to package **Manage** page
   - Click on **Trusted Publishers** tab
   - Click **Add Trusted Publisher**

3. **Configure GitHub Actions Publisher**
   Fill in the following details:
   
   | Field | Value |
   |-------|-------|
   | **Owner** | `johnverbiest` (your GitHub username/org) |
   | **Repository** | `johnverbiest.belgian.insz` |
   | **Workflow** | `.github/workflows/build.yml` |
   | **Environment** | `production` (optional, recommended) |
   | **Ref** | `refs/heads/master` (optional, for master branch only) |

4. **Save Configuration**
   - Click **Add Trusted Publisher**
   - The publisher will appear in your trusted publishers list

### How It Works

```yaml
# In build.yml
permissions:
  id-token: write  # Enables OIDC token generation

steps:
  - name: Publish to NuGet (Trusted Publishing)
    run: dotnet nuget push "./packages/*.nupkg" --source https://api.nuget.org/v3/index.json
    # No --api-key needed! NuGet validates the OIDC token automatically
```

When the workflow runs:
1. GitHub generates a short-lived OIDC token
2. .NET CLI sends this token to NuGet.org
3. NuGet verifies the token matches your trusted publisher configuration
4. Package is published if validation succeeds

### Troubleshooting

**Error: "Forbidden" or "401 Unauthorized"**
- Verify trusted publisher is configured correctly on NuGet.org
- Check that repository name, workflow path, and environment match exactly
- Ensure `id-token: write` permission is set in the workflow

**Error: "Package already exists"**
- Use `--skip-duplicate` flag (already included in workflow)
- Check if version already exists on NuGet.org

## 📦 npm Trusted Publishing

### Prerequisites

- npm account with package publish permissions
- Package must be scoped (e.g., `@username/package`) OR unscoped with provenance enabled

### Setup Steps

1. **Go to npmjs.com**
   - Sign in to your account
   - Navigate to your package: https://www.npmjs.com/package/johnverbiest.belgian.insz

2. **Enable Trusted Publishing**
   
   For **Granular Access Tokens** (Recommended):
   - Go to https://www.npmjs.com/settings/YOUR_USERNAME/tokens
   - Click **Generate New Token** → **Granular Access Token**
   - Configure automation token (optional, but good for initial setup)
   
   For **Provenance** (Automatic with GitHub Actions):
   - npm automatically verifies provenance when `--provenance` flag is used
   - No additional configuration needed on npm side!

3. **Configure GitHub OIDC (Already Done)**
   
   The workflow is already configured:
   ```yaml
   permissions:
     id-token: write  # Enables OIDC
   
   steps:
     - name: Setup Node.js
       uses: actions/setup-node@v4
       with:
         registry-url: 'https://registry.npmjs.org'
     
     - name: Publish to npm (Trusted Publishing)
       run: npm publish --provenance --access public ./packages/*.tgz
   ```

### How It Works

1. **Provenance Generation**
   - GitHub Actions generates attestations about the build
   - OIDC token proves the workflow identity
   - `--provenance` flag includes this in the package

2. **npm Verification**
   - npm receives the package with provenance data
   - Verifies the attestation signature
   - Links package to GitHub repository and workflow
   - Publishes with verified provenance badge

3. **No Manual Token Needed** (with automation tokens)
   - For initial setup, you may need an npm token
   - After provenance is established, OIDC can be used
   - Token removal is optional - provenance works with or without it

### npm Automation Tokens (Alternative)

If you want fully keyless npm publishing:

1. **Create npm Automation Token**
   - Go to https://www.npmjs.com/settings/YOUR_USERNAME/tokens
   - Click **Generate New Token** → **Automation**
   - Select **Publish** permission
   - Restrict to specific packages (recommended)
   - Copy the token

2. **Add to GitHub Secrets**
   - Go to your GitHub repository
   - Settings → Secrets and variables → Actions
   - Add secret: `NPM_TOKEN` with the automation token
   - Update workflow to use: `NODE_AUTH_TOKEN: ${{ secrets.NPM_TOKEN }}`

### Provenance Benefits

✅ **Transparency** - Users can verify package origin
✅ **Security** - Tamper-proof build attestations  
✅ **Trust** - npm displays verified badge on package page
✅ **Audit** - Complete build history linked to GitHub

## 🔍 Verifying Trusted Publishing

### For NuGet

1. **Check Package Metadata**
   - Go to package page on NuGet.org
   - Look for repository link
   - Verify it points to your GitHub repo

2. **View Audit Log**
   - In NuGet.org package manage page
   - Check audit log for publish events
   - Verify "Published via trusted publisher" entries

### For npm

1. **Check Provenance Badge**
   - Go to package page on npmjs.com
   - Look for "Provenance: GitHub Actions" badge
   - Click to view attestation details

2. **Verify with CLI**
   ```bash
   npm view johnverbiest.belgian.insz --json | grep "provenance"
   ```

3. **Inspect Provenance**
   ```bash
   npm audit signatures
   ```

## 🚀 Migration from API Keys

### NuGet Migration

**Before:**
```yaml
- name: Publish to NuGet
  run: dotnet nuget push "./packages/*.nupkg" --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

**After:**
```yaml
permissions:
  id-token: write  # Add this

- name: Publish to NuGet (Trusted Publishing)
  run: dotnet nuget push "./packages/*.nupkg" --source https://api.nuget.org/v3/index.json
  # No --api-key flag needed!
```

**Steps:**
1. Configure trusted publisher on NuGet.org
2. Update workflow (already done)
3. Test deployment
4. Delete `NUGET_API_KEY` secret from GitHub (optional)

### npm Migration

**Before:**
```yaml
- name: Publish to npm
  run: npm publish ./packages/*.tgz
  env:
    NODE_AUTH_TOKEN: ${{ secrets.NPM_TOKEN }}
```

**After:**
```yaml
permissions:
  id-token: write  # Add this

- name: Publish to npm (Trusted Publishing)
  run: npm publish --provenance --access public ./packages/*.tgz
  # No NODE_AUTH_TOKEN needed!
```

**Steps:**
1. Add `--provenance` flag (already done)
2. Ensure `id-token: write` permission (already done)
3. Test deployment
4. Keep or remove `NPM_TOKEN` secret (both work)

## 🛡️ Security Best Practices

### 1. Use Environment Protection Rules

Configure the `production` environment in GitHub:
- Settings → Environments → production
- Add required reviewers (optional)
- Add environment secrets (if needed)
- Restrict to master branch only

### 2. Restrict Trusted Publishers

On NuGet.org, specify:
- **Specific repository** (not wildcard)
- **Specific workflow** (not `*`)
- **Specific environment** (`production`)
- **Specific branch** (`refs/heads/master`)

### 3. Monitor Deployments

- Review GitHub Actions logs regularly
- Check package registry audit logs
- Set up notifications for new package versions

### 4. Enable 2FA

- Enable 2FA on GitHub account
- Enable 2FA on NuGet.org account
- Enable 2FA on npm account

## 📊 Workflow Permissions

Current configuration:

```yaml
# deploy-nuget job
permissions:
  contents: write      # For tagging and releases
  packages: write      # For GitHub Packages (optional)
  id-token: write      # For NuGet trusted publishing ⭐

# deploy-npm job  
permissions:
  contents: read       # Read repository
  packages: write      # For GitHub Packages (optional)
  id-token: write      # For npm provenance ⭐
```

## 🔗 Additional Resources

### NuGet Trusted Publishing
- [NuGet Trusted Publishers Documentation](https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package#trusted-publishers)
- [GitHub OIDC with NuGet](https://devblogs.microsoft.com/nuget/introducing-trusted-publishers/)

### npm Provenance
- [npm Provenance Documentation](https://docs.npmjs.com/generating-provenance-statements)
- [GitHub Actions OIDC](https://docs.github.com/en/actions/deployment/security-hardening-your-deployments/about-security-hardening-with-openid-connect)
- [npm Provenance Blog Post](https://github.blog/2023-04-19-introducing-npm-package-provenance/)

### Security
- [GitHub Actions Security Best Practices](https://docs.github.com/en/actions/security-guides/security-hardening-for-github-actions)
- [SLSA Framework](https://slsa.dev/)

## ✅ Verification Checklist

After setup, verify:

- [ ] NuGet package page shows repository link
- [ ] npm package page shows "Provenance: GitHub Actions" badge
- [ ] Deployment succeeds without API key errors
- [ ] Package versions are published correctly
- [ ] GitHub Actions logs show successful publish
- [ ] Audit logs on registries show trusted publisher usage
- [ ] Provenance can be verified with `npm audit signatures`

## 🎉 Benefits Summary

| Feature | Before | After |
|---------|--------|-------|
| **Security** | Long-lived API keys | Short-lived OIDC tokens |
| **Key Management** | Manual rotation needed | Automatic |
| **Audit Trail** | Limited | Full provenance |
| **Secrets** | 2 secrets to manage | 0 secrets needed |
| **Trust** | Basic | Verifiable attestations |
| **User Confidence** | Package source unclear | Verified GitHub origin |

---

**Questions?** Check the [GitHub OIDC docs](https://docs.github.com/en/actions/deployment/security-hardening-your-deployments/about-security-hardening-with-openid-connect) or open an issue!

