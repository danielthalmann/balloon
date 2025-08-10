## Guide: Créer des étages dans la nacelle

### Principe (résumé)
- **Multi-étages**: gérés par `BasketWalker` via des repères `levelMarkers`.
- **Sans repères**: 2 niveaux auto (sol et plafond du `BoxCollider` de la nacelle).
- **Changement d’étage**: via des volumes Trigger taggés `Ladder`.
- **Déplacements**:
  - Sol: X (gauche/droite) et Z (avant/arrière).
  - Échelle: Y (monter/descendre), X verrouillé au centre de l’échelle.

### À quoi servent `levelMarkers` ?
- **Définir les étages**: chaque `Transform` de `levelMarkers` est un repère de hauteur d’étage (le “plancher”).
- **Hauteur exacte utilisée**: `markerY + demiHauteurDuPerso + paddingY`, pour poser les pieds sans clipper.
- **Tri et usage**:
  - Les hauteurs sont triées du bas vers le haut à chaque frame.
  - Servent à coller le perso au niveau courant quand il marche.
  - Servent à choisir le palier le plus proche en sortie d’échelle.
- **Fallback**: si la liste est vide, le script crée 2 paliers (sol/plafond du `BoxCollider` de la nacelle).
- **Découplage**: évite d’avoir un collider par étage; un seul `BoxCollider` enveloppe la nacelle.

### Comment placer les `levelMarkers` (en pratique)
- Crée un empty par étage dans la nacelle (ex: `Level_0`, `Level_1`, …).
- Positionne chaque repère au niveau du “plancher” (axe Y) de l’étage correspondant.
- Dans l’Inspector du perso, assigne tous ces `Transform` au champ `levelMarkers`.
- Vérifie que les repères sont à l’intérieur du volume du `BoxCollider` de la nacelle.
- Tu peux animer/déplacer les repères: le script relit leurs positions à chaque `Update`.

### Références de code utiles
```25:29:Assets/_MainProject/Scripts/BasketWalker.cs
[Header("Niveaux (multi-étages)")]
[SerializeField] Transform[] levelMarkers;
```

```182:201:Assets/_MainProject/Scripts/BasketWalker.cs
// Construit la liste des hauteurs de niveaux (du bas vers le haut)
float[] GetLevelsY(Bounds basketBounds, float halfCharY)
{
    if (levelMarkers != null && levelMarkers.Length > 0)
    {
        var ys = new float[levelMarkers.Length];
        for (int i = 0; i < levelMarkers.Length; i++)
            ys[i] = levelMarkers[i].position.y + halfCharY + paddingY;
        System.Array.Sort(ys);
        return ys;
    }
    else
    {
        float floorY = basketBounds.min.y + halfCharY + paddingY;
        float upperY = basketBounds.max.y - halfCharY - paddingY;
        return new float[] { floorY, upperY };
    }
}
```
### Progression par niveaux (0 → 10 étages)
- Un seul prefab `Player` avec `BasketWalker` suffit pour tout le jeu.
- Gère la complexité via des variantes de la nacelle:
  - `Nacelle_E0`: aucun `levelMarker` (fallback 2 niveaux) ou 1 seul marker au plancher si tu veux strictement 1 niveau.
  - `Nacelle_E3`, `Nacelle_E5`, … `Nacelle_E10`: crée 3/5/…/10 empties `Level_i` au niveau des planchers et les assigne à `levelMarkers`. Ajoute des triggers `Ladder` entre les paliers utiles.
- Dans chaque scène de niveau:
  - Instancie la variante de nacelle correspondante.
  - Place `Player` comme enfant de la nacelle (le script récupère `basket`/`basketCollider` automatiquement).
- Pas besoin de dupliquer le personnage par niveau.

Astuce: utilise des Prefab Variants pour éviter de tout reconfigurer; le base contient le `BoxCollider` et le décor, les variantes ajoutent juste les `levelMarkers` et les `Ladder`.