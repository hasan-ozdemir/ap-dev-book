@echo off
setlocal enabledelayedexpansion

set "SCRIPT_NAME=%~n0"
set "SCRIPT_DIR=%~dp0\..\"
set "REPO_ROOT=%SCRIPT_DIR%"
for %%I in ("%REPO_ROOT%") do set "REPO_ROOT=%%~fI"

set "REMOTE_SLUG=%~1"
if "%REMOTE_SLUG%"=="" (
  echo Usage: %~n0 ^<owner/repo^> [remote-name]
  echo Example: %~n0 acme/repo-mirror origin
  exit /b 1
)

set "REMOTE_NAME=%~2"
if "%REMOTE_NAME%"=="" set "REMOTE_NAME=origin"

set "VISIBILITY=%GH_REPO_VISIBILITY%"
if "%VISIBILITY%"=="" set "VISIBILITY=private"

set "VIS_FLAG=--private"
if /I "%VISIBILITY%"=="public" set "VIS_FLAG=--public"
if /I "%VISIBILITY%"=="internal" set "VIS_FLAG=--internal"

pushd "%REPO_ROOT%" >NUL
if errorlevel 1 (
  echo [%SCRIPT_NAME%] ERROR Unable to enter repository root "%REPO_ROOT%".
  exit /b 1
)

where git >NUL 2>&1
if errorlevel 1 (
  echo [%SCRIPT_NAME%] ERROR git CLI not found in PATH.
  set "RC=1"
  goto :cleanup
)

where gh >NUL 2>&1
if errorlevel 1 (
  echo [%SCRIPT_NAME%] ERROR GitHub CLI (gh) not found in PATH.
  set "RC=1"
  goto :cleanup
)

git rev-parse --is-inside-work-tree >NUL 2>&1
if errorlevel 1 (
  echo [%SCRIPT_NAME%] ERROR "%REPO_ROOT%" is not a git repository.
  set "RC=1"
  goto :cleanup
)

git remote get-url "%REMOTE_NAME%" >NUL 2>&1
if not errorlevel 1 (
  echo [%SCRIPT_NAME%] ERROR Remote "%REMOTE_NAME%" already exists. Remove it or specify a different remote name.
  set "RC=1"
  goto :cleanup
)

for /f "delims=" %%B in ('git rev-parse --abbrev-ref HEAD 2^>NUL') do set "CURRENT_BRANCH=%%B"
if not defined CURRENT_BRANCH (
  echo [%SCRIPT_NAME%] ERROR Unable to determine the current branch.
  set "RC=1"
  goto :cleanup
)

git show-ref --verify --quiet refs/heads/main
if errorlevel 1 (
  if /I not "!CURRENT_BRANCH!"=="main" (
    echo [%SCRIPT_NAME%] INFO  Renaming branch "!CURRENT_BRANCH!" to "main".
    git branch -M main
    if errorlevel 1 (
      echo [%SCRIPT_NAME%] ERROR Failed to rename the current branch to main.
      set "RC=1"
      goto :cleanup
    )
  )
) else (
  if /I not "!CURRENT_BRANCH!"=="main" (
    echo [%SCRIPT_NAME%] INFO  Checking out existing main branch.
    git checkout main
    if errorlevel 1 (
      echo [%SCRIPT_NAME%] ERROR Unable to check out the main branch.
      set "RC=1"
      goto :cleanup
    )
    set "CURRENT_BRANCH=main"
  ) else (
    echo [%SCRIPT_NAME%] INFO  Using existing main branch.
  )
)

echo [%SCRIPT_NAME%] INFO  Creating GitHub repository "%REMOTE_SLUG%" with remote "%REMOTE_NAME%".
gh repo create "%REMOTE_SLUG%" %VIS_FLAG% --source . --remote "%REMOTE_NAME%" --push --branch main
if errorlevel 1 (
  echo [%SCRIPT_NAME%] ERROR gh repo create failed.
  set "RC=!ERRORLEVEL!"
  goto :cleanup
)

echo [%SCRIPT_NAME%] INFO  Pushing tags to "%REMOTE_NAME%".
git push "%REMOTE_NAME%" --tags
if errorlevel 1 (
  echo [%SCRIPT_NAME%] WARN  Unable to push tags; continue without tags.
) else (
  echo [%SCRIPT_NAME%] INFO  Tags pushed successfully.
)

echo [%SCRIPT_NAME%] INFO  Remote summary:
git remote -v
git branch -vv

echo [%SCRIPT_NAME%] SUCCESS Repository mirrored to GitHub remote "%REMOTE_SLUG%".
set "RC=0"

:cleanup
popd >NUL
pause
exit /b %RC%
