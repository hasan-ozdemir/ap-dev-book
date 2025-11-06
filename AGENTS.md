# Repository Guidelines

## Project Structure & Module Organization
- `docs/` — primary knowledge base; grouped by Foundations, Mobile MAUI, Architecture, DevOps, Cloud, Delivery, Toolbox, and Blueprints. All instructional content lives here and must carry inline citation markers (`cite…`).
- `samples/` — runnable .NET 9 console and MAUI examples referenced from the guides. Keep projects compilable on Windows and macOS targets.
- `scripts/` — PowerShell automation (e.g., `build-android.ps1`, `build-ios.ps1`) and utility helpers used in release preparation.

## Workflow & Repeated Tasks
- Work from PowerShell (`pwsh -NoLogo`) and prefer `rg`/`rg --files-without-match` for repository scans. Avoid destructive commands (`git reset --hard`, `rm -rf`).
- Use `apply_patch` for precise edits; maintain ASCII text aside from required citation markers.
- For multi-step doc efforts, draft a short plan, gather current sources, update content, then rerun the citation scan before committing.
- Treat documentation edits as code: run commands locally, capture outputs in notes, and keep instructions reproducible.

## Build, Test, and Development Commands
- `pwsh -NoLogo -File scripts/build-android.ps1` — builds MAUI Android artifacts with the installed workloads.
- `pwsh -NoLogo -File scripts/build-ios.ps1` — produces iOS packages; execute on macOS with signing assets configured.
- `dotnet test samples/TodoApp/TodoApp.Tests.csproj` — sample test suite template; mirror this when adding new instructional tests.
- `pwsh -NoLogo -Command "rg --files-without-match \"\uE200cite\" docs"` — confirms every Markdown file carries citations.

## Coding Style & Naming Conventions
- Documentation: professional tone, concise paragraphs, sentence case headings. Use bullet lists for checklists and keep sections scoped to 200–400 words.
- Code samples: four-space indentation, PascalCase for types, camelCase for locals. Place multi-target MAUI handlers under `Handlers/` with `Platforms/<Target>/` partials.
- File names: kebab-case for Markdown (`testing-quality.md`), PascalCase for C# (`RichCameraViewHandler.cs`).

## Testing Guidelines
- Compile and run code snippets (MAUI, console, PowerShell) before publishing; note any platform prerequisites.
- Add or update README instructions whenever tests require special tooling (Simulators, Android SDK components, Azure resources).
- Document manual validation steps when automated tests are impractical, especially for UI walkthroughs.

## Commit & Pull Request Guidelines
- Follow the existing present-tense pattern: `docs: add agent contributor guide`, `scripts: improve ios build`. Group related changes into a single commit.
- PRs should summarise scope, list updated sections, link issues, and attach screenshots or log excerpts for build/test changes.
- Before requesting review: re-run the citation scan, spell-check revised docs, `git status --short`, and ensure commands were executed on the target OS.

## Citation & Research Protocol
- Source every non-trivial statement with current references (Microsoft Learn, Azure Architecture Center, official GitHub docs). Replace footnotes with `citeturnXsearchY`.
- Record search identifiers in work notes so future agents can audit. Update both prose and citations whenever upstream guidance changes.
- Large research tasks should gather multiple sources before editing to keep documentation consistent and traceable.
