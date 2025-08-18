# Guide complet - Unity Input System

Ce guide détaille comment utiliser le nouveau Input System de Unity pour notre projet de montgolfière.

## Table des matières

1. [Installation et configuration](#installation-et-configuration)
2. [Création d'un fichier Input Actions](#création-dun-fichier-input-actions)
3. [Configuration des actions](#configuration-des-actions)
4. [Binding des touches](#binding-des-touches)
5. [Intégration dans les scripts](#intégration-dans-les-scripts)
6. [Exemples pratiques](#exemples-pratiques)
7. [Troubleshooting](#troubleshooting)

---

## Installation et configuration

### Étape 1 : Installation du package

1. **Ouvrir le Package Manager**
   - Menu : `Window → Package Manager`
   - Sélectionner `Unity Registry` dans le dropdown

2. **Installer Input System**
   - Chercher "Input System"
   - Cliquer sur `Install`
   - Unity redémarrera automatiquement

### Étape 2 : Configuration des Player Settings

1. **Ouvrir les Project Settings**
   - Menu : `Edit → Project Settings`

2. **Configurer l'Active Input Handling**
   - Aller dans `Player → Configuration`
   - Dans `Active Input Handling`, sélectionner :
     - `Input System Package (New)` - pour utiliser uniquement le nouveau système
     - `Both` - pour garder les deux systèmes (transition)

3. **Redémarrer Unity** si demandé

---

## Création d'un fichier Input Actions

### Méthode 1 : Création manuelle

1. **Dans la fenêtre Project**
   - Clic droit dans le dossier `Assets`
   - `Create → Input Actions`
   - Nommer le fichier (ex: `InputSystem_Actions`)

### Méthode 2 : Depuis un GameObject

1. **Sélectionner un GameObject**
2. **Ajouter le composant PlayerInput**
   - `Add Component → Input → Player Input`
3. **Créer les actions**
   - Cliquer sur `Create Actions...`
   - Choisir l'emplacement et le nom

---

## Configuration des actions

### Ouvrir l'éditeur Input Actions

1. **Double-cliquer sur le fichier `.inputactions`**
2. **L'éditeur Input Actions s'ouvre**

### Structure de l'éditeur

L'éditeur est divisé en 3 colonnes :

1. **Action Maps** (gauche) - Groupes d'actions
2. **Actions** (centre) - Actions individuelles
3. **Properties** (droite) - Propriétés et bindings

### Créer un Action Map

1. **Cliquer sur le `+` à côté de "Action Maps"**
2. **Nommer le map** (ex: "Player", "UI", "Vehicle")
3. **Sélectionner le map créé**

### Créer des Actions

Pour chaque action nécessaire :

1. **Sélectionner l'Action Map**
2. **Cliquer sur le `+` à côté de "Actions"**
3. **Configurer l'action :**

#### Types d'actions courantes :

**Button** - Pour les actions on/off
```
Nom: Jump
Type: Button
Control Type: Button
```

**Value** - Pour les valeurs continues
```
Nom: Move
Type: Value
Control Type: Vector2
```

**Pass Through** - Pour les données brutes
```
Nom: Look
Type: PassThrough
Control Type: Vector2
```

### Actions recommandées pour notre projet montgolfière :

```
Action Map: Player
├── Move (Value, Vector2) - Déplacement WASD
├── Jump (Button) - Montée simple
├── Attack (Button) - Boost rapide
├── Interact (Button + Hold) - Montée continue
└── Look (Value, Vector2) - Rotation caméra
```

---

## Binding des touches

### Ajouter un binding simple

1. **Sélectionner une action**
2. **Cliquer sur le `+` à côté de "Bindings"**
3. **Choisir "Add Binding"**
4. **Cliquer sur "Path"**
5. **Sélectionner la touche** dans la liste ou utiliser "Listen"

### Types de bindings

#### Binding simple
```
Action: Jump
Binding: <Keyboard>/space
```

#### Composite 2D Vector (WASD)
```
Action: Move
Binding: 2D Vector Composite
├── Up: <Keyboard>/w
├── Down: <Keyboard>/s
├── Left: <Keyboard>/a
└── Right: <Keyboard>/d
```

#### Binding multiple (plusieurs touches pour la même action)
```
Action: Jump
Bindings:
├── <Keyboard>/space
├── <Gamepad>/buttonSouth
└── <Mouse>/leftButton
```

### Interactions spéciales

#### Hold Interaction
```
Action: Interact
Binding: <Keyboard>/e
Interactions: Hold
Hold Time: 0.4 (secondes)
```

#### Multi-Tap
```
Action: DoubleJump
Binding: <Keyboard>/space
Interactions: MultiTap
Tap Count: 2
Max Tap Spacing: 0.5
```

### Processors (modificateurs)

#### Invert (inverser)
```
Action: Look
Binding: <Mouse>/delta
Processors: Invert Vector2
```

#### Scale (multiplier)
```
Action: Move
Binding: <Gamepad>/leftStick
Processors: Scale Vector2
Scale: 2.0
```

---

## Intégration dans les scripts

### Méthode 1 : PlayerInput avec Send Messages

#### Configuration du PlayerInput
```
Behavior: Send Messages
Default Map: Player
```

#### Script exemple
```csharp
public class PlayerController : MonoBehaviour
{
    // Méthodes appelées automatiquement par PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        // Logique de mouvement
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Logique de saut
        }
    }
}
```

### Méthode 2 : Références directes aux actions

#### Script exemple
```csharp
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }
    
    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        jumpAction.performed += OnJump;
    }
    
    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        jumpAction.performed -= OnJump;
    }
    
    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        // Utiliser moveInput...
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        // Logique de saut
    }
}
```

### Méthode 3 : Classe générée automatiquement

#### Activer la génération
1. **Dans l'éditeur Input Actions**
2. **Cocher "Generate C# Class"**
3. **Cliquer "Apply"**

#### Utilisation
```csharp
public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    
    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }
    
    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }
    
    void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Jump.performed -= OnJump;
    }
    
    void Update()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
    }
}
```

---

## Exemples pratiques

### Configuration complète pour notre montgolfière

#### Actions définies :
```
Player Action Map:
├── Move (Vector2) - Déplacement du joueur dans la nacelle
├── BalloonLift (Button) - Montée simple de la montgolfière
├── BalloonBoost (Button) - Boost rapide
├── BalloonHold (Button + Hold) - Montée continue
└── ClimbLadder (Button) - Interaction avec les échelles
```

#### Bindings recommandés :
```
Move:
├── WASD Composite
└── Arrow Keys Composite

BalloonLift:
├── <Keyboard>/space
└── <Gamepad>/buttonSouth

BalloonBoost:
├── <Mouse>/leftButton
└── <Gamepad>/buttonWest

BalloonHold:
├── <Keyboard>/e (Hold, 0.2s)
└── <Gamepad>/rightTrigger (Hold, 0.2s)

ClimbLadder:
├── <Keyboard>/f
└── <Gamepad>/buttonNorth
```

### Script PlayerMovement adapté

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform basketCenter;
    [SerializeField] private float maxMoveDistance = 3f;
    
    private Vector2 moveInput;
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    void Update()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
            Vector3 newPosition = transform.position + movement * moveSpeed * Time.deltaTime;
            
            if (IsPositionValid(newPosition))
            {
                transform.position = newPosition;
            }
        }
    }
    
    private bool IsPositionValid(Vector3 position)
    {
        if (basketCenter == null) return true;
        
        float distance = Vector3.Distance(
            new Vector3(position.x, basketCenter.position.y, position.z),
            basketCenter.position
        );
        
        return distance <= maxMoveDistance;
    }
}
```

### Script BalloonActions adapté

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class BalloonActions : MonoBehaviour
{
    [SerializeField] private BalloonController balloonController;
    [SerializeField] private float liftForce = 3f;
    [SerializeField] private float liftDuration = 2f;
    
    public void OnBalloonLift(InputAction.CallbackContext context)
    {
        if (context.performed && balloonController != null)
        {
            balloonController.ApplyLift(liftForce, liftDuration);
        }
    }
    
    public void OnBalloonBoost(InputAction.CallbackContext context)
    {
        if (context.performed && balloonController != null)
        {
            balloonController.ApplyLift(liftForce * 0.7f, liftDuration * 0.5f);
        }
    }
    
    public void OnBalloonHold(InputAction.CallbackContext context)
    {
        if (balloonController == null) return;
        
        if (context.started)
        {
            // Début de la montée continue
            balloonController.FallModifier = -1f;
        }
        else if (context.canceled)
        {
            // Fin de la montée continue
            balloonController.FallModifier = 1f;
        }
    }
}
```

---

## Troubleshooting

### Problèmes courants

#### Le fichier .inputactions n'apparaît pas
**Solutions :**
- Rafraîchir Unity (Ctrl+R / Cmd+R)
- Vérifier les filtres de la fenêtre Project
- Chercher "t:InputActionAsset" dans la barre de recherche

#### Les actions ne fonctionnent pas
**Vérifications :**
- Le composant PlayerInput est-il présent ?
- Le fichier .inputactions est-il assigné ?
- Le Default Map est-il correct ?
- Les méthodes OnActionName correspondent-elles aux noms des actions ?

#### Erreur "InputSystem_Actions does not exist"
**Solution :**
- Cocher "Generate C# Class" dans l'éditeur Input Actions
- Cliquer "Apply"
- Attendre la recompilation

#### Les bindings ne répondent pas
**Vérifications :**
- L'action est-elle activée (Enable()) ?
- Le GameObject avec PlayerInput est-il actif ?
- Y a-t-il des conflits avec d'autres systèmes d'input ?

### Debug et test

#### Activer les logs d'input
```csharp
void Start()
{
    InputSystem.onActionChange += (obj, change) =>
    {
        Debug.Log($"Action {obj} changed: {change}");
    };
}
```

#### Tester les bindings
1. **Window → Analysis → Input Debugger**
2. **Sélectionner le device (Keyboard, Mouse, etc.)**
3. **Observer les inputs en temps réel**

#### Vérifier les actions actives
```csharp
void Update()
{
    foreach(var action in playerInput.actions)
    {
        if (action.WasPressedThisFrame())
        {
            Debug.Log($"Action pressed: {action.name}");
        }
    }
}
```

---

## Bonnes pratiques

### Organisation des Action Maps
- **Player** : Actions du joueur (mouvement, actions)
- **UI** : Navigation dans les menus
- **Vehicle** : Contrôles spécifiques aux véhicules
- **Debug** : Actions de debug (désactivées en release)

### Nommage des actions
- Utiliser PascalCase : `BalloonLift`, `ClimbLadder`
- Être descriptif : `Move` plutôt que `M`
- Grouper logiquement : `Balloon*` pour toutes les actions de montgolfière

### Gestion des contextes
```csharp
public void OnAction(InputAction.CallbackContext context)
{
    if (context.started)
    {
        // Action commence (première frame)
    }
    else if (context.performed)
    {
        // Action effectuée (peut se répéter)
    }
    else if (context.canceled)
    {
        // Action annulée/relâchée
    }
}
```

### Performance
- Désactiver les actions non utilisées
- Utiliser les Action Maps pour grouper les actions par contexte
- Éviter les ReadValue() dans Update() si possible

---

## Ressources utiles

- [Documentation officielle Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Input System Samples](https://github.com/Unity-Technologies/InputSystem)
- [Video Tutorial Unity](https://learn.unity.com/tutorial/input-system)

---

*Documentation créée pour le projet Montgolfière - Version 1.0*
