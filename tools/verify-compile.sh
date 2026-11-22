#!/usr/bin/env bash
# Verifie que le code du mod compile, sans installation KSP, en le compilant
# contre les stubs de l'API (tools/stubs/KspStubs.cs).
# Necessite mono (mcs) ou le .NET SDK (dotnet).
set -euo pipefail
cd "$(dirname "$0")/.."
REPO="$(pwd)"
OUT="$(mktemp -d)"
trap 'rm -rf "$OUT"' EXIT

if command -v mcs >/dev/null 2>&1; then
  mcs -target:library -langversion:7.3 -out:"$OUT/RocketAnalyzer.dll" \
      tools/stubs/KspStubs.cs src/RocketAnalyzer/*.cs
  echo "OK (mcs) : compilation reussie contre les stubs."
elif command -v dotnet >/dev/null 2>&1; then
  cat > "$OUT/check.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>7.3</LangVersion>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <Nullable>disable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="$REPO/tools/stubs/KspStubs.cs" />
    <Compile Include="$REPO/src/RocketAnalyzer/*.cs" />
  </ItemGroup>
</Project>
EOF
  dotnet build "$OUT/check.csproj" -v q -nologo
  echo "OK (dotnet) : compilation reussie contre les stubs."
else
  echo "Aucun compilateur C# trouve (mcs ou dotnet requis)." >&2
  exit 1
fi
