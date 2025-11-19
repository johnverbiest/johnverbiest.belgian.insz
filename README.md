# johnverbiest.belgian.insz

A representation of a Belgian INSZ identifier. This module provides validation and formatting
functions for the INSZ number, which is used as a national identification number in Belgium.

An INSZ number is also known as:
- RN (Rijksregisternummer) for persons registered in the National Register.
- BIS number for persons not registered in the National Register.
- NISS (Numéro d'Identification de la Sécurité Sociale) in the context of social security and eHealth.
- NISS INSZ as referred to by the eHealth platform.
- Rijksregisternummer (RRN) in Dutch.
- Numéro de registre national (NRN) in French.
- Numéro di registro nazionale (NRN) in Italian.
- Nummer des Nationalregisters (NRN) in German.

## Rules for the INSZ identifier

See [BelgiumRegistryRules.md](docs/BelgiumRegistryRules.md) for a summary of the authoritative rules.

## CI/CD and Deployment

This project uses GitHub Actions for continuous integration and deployment:

- **Pull Requests**: Automated validation including build, test, code formatting, and security scans
- **Master Branch**: Automatic deployment to NuGet.org and GitHub releases
- **Build Status**: Test results are reported directly in GitHub for easy tracking

### Deployment Process

When code is pushed to the `master` branch:
1. Build and test validation
2. NuGet package creation and validation
3. Automatic publishing to [NuGet.org](https://www.nuget.org/packages/JohnVerbiest.Belgium.Insz/)
4. GitHub release creation with version tags and artifacts

For deployment setup instructions, see [DEPLOYMENT_SETUP.md](docs/DEPLOYMENT_SETUP.md).

