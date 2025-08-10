## Personnage (BasketWalker) — Guide de configuration

Ce document décrit chaque paramètre visible dans l’Inspector du personnage tel que sur la capture fournie, et explique leur impact en jeu.

---

## Composants du GameObject

### Mesh Renderer
- **Materials**: matériaux utilisés par le mesh du personnage. Purement visuel.

### Box Collider (du personnage)
- **Is Trigger**: doit rester décoché pour des collisions solides classiques.
- **Material**: `Physic Material` optionnel (friction/rebond).
- **Center / Size**: volume de collision. Ajuste pour épouser la silhouette du perso.

Astuce: évite un collider trop large qui touche les bords de la nacelle; utilise les paddings du script pour peaufiner.

---

## Script BasketWalker

### Déplacement
- **Speed X**: vitesse latérale sur un étage (axe X). Valeur typique: 2–6.
- **Speed Z**: avance/recul dans la nacelle (axe Z). Valeur typique: 2–6.
- **Climb Speed**: vitesse de montée/descente sur échelle (axe Y). Valeur typique: 1–4.

### Marges anti‑clip
- **Padding X / Y / Z**: petites marges ajoutées pour éviter de passer au travers des bords, sols/planchers et parois.
  - Plus la géométrie est serrée, plus ces valeurs doivent légèrement augmenter (0.01–0.05 m).

### Références nacelle
- **Basket**: `Transform` de la nacelle. Sert de repère central et de limites X/Z.
- **Basket Collider**: `BoxCollider` de la nacelle. Sert à déduire:
  - Les limites X/Z jouables de la nacelle.
  - Les niveaux par défaut si aucun repère n’est fourni (sol + plafond internes).

Bonnes pratiques:
- Le personnage doit être enfant logique de la nacelle, sinon renseigne manuellement ces champs.

### Entrées (Nouveau Input System)
- **Move Action** (`InputActionReference`):
  - Action `Value` de type `Vector2` (ex: `Player/Move`).
  - X = gauche/droite; Y = haut/bas (sur échelle) ou avant/arrière (Z) au sol.
  - Si non renseigné, le script lit `Horizontal`/`Vertical` (Input Manager).

### Niveaux (multi‑étages)
- **Level Markers** (liste de `Transform`):
  - Chaque élément correspond à un plancher. Le personnage “marche” à la hauteur:
    - `markerY + demi_hauteur_perso + paddingY`
  - Les hauteurs sont triées du plus bas au plus haut à chaque frame.
  - Si la liste est vide: 2 niveaux automatiques (sol et plafond internes du `BoxCollider` de la nacelle).

Comment les placer (rappel):
- Crée un empty par étage (`Level_00`, `Level_01`, …) à la hauteur du plancher.
- Assigne‑les dans `Level Markers` (l’ordre est libre, ils sont triés).
- Garde les repères à l’intérieur du volume de la nacelle.

Interaction avec les échelles:
- Pour changer d’étage, ajoute des colliders `Is Trigger` taggés `Ladder` dont la hauteur englobe les paliers visés.
- Sur échelle: X est “collé” au `center.x` du trigger; Y contrôle la montée/descente; pousser en X près d’un palier décroche.

---

## Rigidbody (du personnage)

Note: `BasketWalker` force certains réglages au démarrage:
- `useGravity = false`
- `isKinematic = true`
- `constraints` ≥ Freeze Rotation

Paramètres Inspector (ce que tu vois) et recommandations:
- **Mass**: sans impact majeur en mode cinématique; laisse par défaut (1).
- **Linear Damping / Angular Damping**: marges d’amortissement; peu d’effet en cinématique.
- **Use Gravity**: le script la désactive à l’exécution; l’état Inspector est ignoré.
- **Is Kinematic**: activé par le script; le mouvement est géré manuellement.
- **Interpolate**: peut rester `None` (positions fixées par script chaque frame).
- **Collision Detection**: `Discrete` suffit (pas de vitesses élevées).
- **Constraints**:
  - Recommande: cocher les 3 axes de **Freeze Rotation** (le script le fait).
  - Optionnel: geler certaines positions si nécessaire pour ton jeu, mais le script s’occupe déjà de contraindre X/Z dans la nacelle.

---

## Check‑list d’intégration

- `Basket` et `Basket Collider` pointent bien vers la nacelle.
- `Move Action` est assignée à `Player/Move (Vector2)` ou fallback Input Manager actif.
- `Level Markers`:
  - Vide → 2 niveaux auto (sol/plafond).
  - Sinon un `Transform` par plancher jouable (0, 1, … N).
- Échelles: colliders `Is Trigger` avec Tag `Ladder`, couvrant la hauteur des paliers.
- Paddings ajustés pour éviter les clips sur ta géométrie.

---

## Dépannage rapide

- Le perso reste bloqué à 2 niveaux:
  - `Level Markers` non renseigné; ajoute des repères ou accepte le fallback.
- Impossible de grimper:
  - Tag `Ladder` + `Is Trigger` ? L’échelle couvre bien la hauteur des paliers ?
  - `Move.y` atteint bien le script (Input System ok) ?
- Sorties d’échelle imprécises:
  - Approche un palier puis pousse en X (snap ≈ 0.05 m); replace les repères si nécessaire.
- Le perso dépasse les bords:
  - Augmente légèrement `Padding X/Z`. Vérifie la taille du `BoxCollider` nacelle.

---

## Réglages conseillés (base)
- Speed X / Z: 3
- Climb Speed: 2
- Padding X / Y / Z: 0.02
- Freeze Rotation: X, Y, Z cochés