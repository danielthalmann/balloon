## Guide: Échelles de la nacelle

### Principe (résumé)
- Les échelles sont des volumes `BoxCollider` en `Is Trigger`, taggés `Ladder`.
- Entrer dans une échelle permet l’escalade via l’axe vertical d’entrée.
- En escalade:
  - Le personnage est “collé” en X sur `bounds.center.x` du collider d’échelle.
  - Le mouvement Y est contraint entre `bounds.min.y` et `bounds.max.y` (avec marges).
  - Pousser en X près d’un palier décroche sur l’étage le plus proche.
- À la sortie de l’échelle (si on était en escalade), on est recollé au palier le plus proche.
- Si la nacelle/échelle bouge, la position/hauteur de l’échelle est relue en continu.

### Références de code
```108:139:Assets/_MainProject/Scripts/BasketWalker.cs
if (climbing)
{
    pos.x = ladderX; // X collé à l’échelle
    float minY = ladderBounds.min.y + halfCharY + paddingY;
    float maxY = ladderBounds.max.y - halfCharY - paddingY;
    float deltaY = inputY * climbSpeed * Time.deltaTime;
    pos.y = Mathf.Clamp(pos.y + deltaY, minY, maxY);

    // Sortie volontaire en poussant en X près d’un palier
    if (Mathf.Abs(inputX) > 0.1f)
    {
        int closest = 0;
        float best = Mathf.Infinity;
        for (int i = 0; i < levels.Length; i++)
        {
            float d = Mathf.Abs(pos.y - levels[i]);
            if (d < best) { best = d; closest = i; }
        }
        const float detachSnap = 0.05f;
        if (best <= detachSnap)
        {
            currentLevel = closest;
            climbing = false;
            targetLevel = -1;
            pos.y = levels[currentLevel];
        }
    }
}
```

```149:156:Assets/_MainProject/Scripts/BasketWalker.cs
// Démarrer l’escalade si dans une échelle + input vertical
if (atLadder && Mathf.Abs(inputY) > 0.1f)
{
    climbing = true;
    targetLevel = -1;
    pos.x = ladderX; // snap X immédiat
}