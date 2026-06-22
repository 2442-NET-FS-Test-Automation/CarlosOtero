# MusicKata Development

## Branching Strategy

We use a feature-branch workflow. All development must happen on isolated branches before merging towards release version.

### Branch Naming Conventions
Always use lowercase and separate words with hyphens. For a basic project, and a generalized branch structure, use the name of the repository project along with a distinctive suffix,
and then provide the name of the developer assigned to that branch, all separated by a hyphen, such as: `TestBranchV2-JohnDoe`. 
For more control, start alternative with one of these prefixes:

* `feature/` - New features (e.g., `feature/user-login`)
* `bugfix/` - Fixes for existing issues (e.g., `bugfix/cart-crash`)
* `hotfix/` - Urgent fixes for production (e.g., `hotfix/payment-gateway`)
* `docs/` - Documentation changes only (e.g., `docs/update-readme`)

### Main Branches
* `main` - Production-ready code. Never commit directly to this branch, before final approval.

---

## Development Workflow

### 1. Create Your Branch
Always pull the latest changes from the `main` and add `feature/...` branch:
```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
```

### 2. Commit Guidelines
* Write clear, concise commit messages that highlight changes and implementations.
* Keep commits focused; do not mix unrelated changes.

### 3. Open a Pull Request (PR)
* Target the `feature` branch for your PR.
* Fill out the provided PR template.
* Raise the pull request and notify its status.

### 4. Code Review & Merge
* At least **one peer review** is required before merging.
* Ensure all continuous integration (CI) and conflict control tests pass.
* Use **Squash and Merge** to keep the target branch history linear.
