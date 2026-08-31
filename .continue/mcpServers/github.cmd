@echo off
for /f "usebackq delims=" %%T in (`gh auth token`) do set "GITHUB_PERSONAL_ACCESS_TOKEN=%%T"
if not defined GITHUB_PERSONAL_ACCESS_TOKEN (
  echo gh auth token returned no token 1>&2
  exit /b 1
)
docker run -i --rm --env GITHUB_PERSONAL_ACCESS_TOKEN ghcr.io/github/github-mcp-server