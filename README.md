# RocketAnalyzer — analyseur de performances de fusée pour KSP 1

Mod pour **Kerbal Space Program 1** (cible l'API 1.12, .NET Framework 4.7.2 / Unity 2019.4) qui affiche
les performances de ta fusée en temps réel :

- **Dans le VAB / SPH** : tableau détaillé par étage — masse au début de l'étage, ergol, poussée,
  Isp (vide et niveau de la mer), TWR, delta-v et temps de combustion — plus en résumé la masse
  totale, le delta-v total et le **TWR au lancement**.
- **En vol** : TWR actuel, poussée actuelle, g local, delta-v restant (vide / ASL), apoapsis,
  périapsis, excentricité, temps avant apoapsis, et le détail des étages restants (l'étage en cours
  est marqué ▶).

## Installation

1. Compiler le mod (voir ci-dessous).
2. Copier le dossier `GameData/RocketAnalyzer/` dans le dossier `GameData/` de ton installation KSP.
3. Dans KSP, cliquer sur l'icône du mod dans la barre d'applications (VAB, SPH ou vol) pour
   afficher/masquer la fenêtre. La position des fenêtres est mémorisée.

## Compilation

Prérequis : le .NET SDK (ou Mono) et une installation de KSP 1.

```bash
# Windows, installation Steam par défaut :
dotnet build src/RocketAnalyzer/RocketAnalyzer.csproj -c Release

# Chemin personnalisé (variable d'environnement KSP_DIR ou propriété MSBuild) :
$env:KSP_DIR = "D:\Steam\steamapps\common\Kerbal Space Program"   # PowerShell
dotnet build src/RocketAnalyzer/RocketAnalyzer.csproj -c Release

# Linux / macOS :
dotnet build src/RocketAnalyzer/RocketAnalyzer.csproj -c Release -p:KSPPath=/chemin/vers/KSP
```

Le `RocketAnalyzer.dll` compilé est copié automatiquement dans `GameData/RocketAnalyzer/`.

Vérification de la compilation **sans** KSP installé (contre des stubs de l'API, nécessite `mcs` ou `dotnet`) :

```bash
tools/verify-compile.sh
```

## Colonnes affichées

| Colonne | Signification |
| --- | --- |
| Étage | Numéro d'étage (le plus haut se déclenche en premier) |
| Masse départ | Masse totale du vaisseau au début de l'étage, en tonnes |
| Ergol | Masse d'ergol consommable dans l'étage, en tonnes |
| Poussée | Poussée max des moteurs de l'étage (limiteur à 100 %), en kN |
| Isp vide / Isp ASL | Impulsion spécifique moyenne pondérée par la poussée, en secondes |
| TWR | Poussée / (masse × g) au début de l'étage |
| Δv | Delta-v de l'étage (Tsiolkovski, Isp vide), en m/s |
| Durée | Temps de combustion à pleine poussée, Isp vide |

## Limites connues

- La poussée affichée est la poussée maximale ; sa variation avec la pression atmosphérique
  n'est pas modélisée (seule l'Isp est différenciée vide / ASL).
- Le delta-v utilise la formule de Tsiolkovski avec l'Isp moyenne pondérée par la poussée.
- Les transferts d'ergol entre étages (lignes de carburant) ne sont pas pris en compte :
  l'ergol est compté dans l'étage où se trouve la pièce.

## Structure du dépôt

```
src/RocketAnalyzer/        code source du mod (C#)
GameData/RocketAnalyzer/   dossier à copier dans KSP (icône + DLL compilé)
tools/stubs/               stubs de l'API KSP pour la vérification de compilation
tools/verify-compile.sh    script de vérification sans installation KSP
```
