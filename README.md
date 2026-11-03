# Steering Behaviours in Unity

## Common scene setup

1. Create two platforms placed apart from each other: one for Seek & Flee and one for Flocking.
---

## Platform 1: Seek & Flee

### Setup

1. Create two objects (for example, two capsules) on the Seek & Flee platform.
2. Add a **NavMesh Agent** component to each one.
3. Add the **Seek and Flee** script to the chasing and fleeing agent (one of the two, doesn't mater which')
4. In the script, assign the target `Transform` in the Inspector (the other agent) and change the safety thresholds or values as desired.

---

## Platform 2: Flocking

A flocking system based on the three classic rules (cohesion, alignment and separation), plus leader following and obstacle avoidance. Movement itself is executed by each agent's `NavMeshAgent`.

### Scripts

- **`FlockManager`**: spawns the flock, stores all the parameters and keeps the list of agents.
- **`FlockAgent`**: computes each agent's direction every `updateInterval` seconds and sends it to the NavMesh with `SetDestination`.

### Step-by-step setup

#### 1. Create the layers

Go to **Edit > Project Settings > Tags and Layers** and create two layers:

- `Agent`
- `Ground`

Assign `Ground` to the flocking platform and `Agent` to the agent prefab (step 2).

> This is important: if agents and ground are on the layers that the `SphereCast` checks, agents will see each other as obstacles and get stuck.

#### 2. Create the agent prefab

1. Create a `Cube` in the scene and name it `FlockAgent`.
2. Add a **NavMesh Agent** component.
3. Add the **`FlockAgent`** script.
4. Assign the agent layer to that Gameobject.
5. Drag it into the assets folder of unity to create the prefab, then delete it from the scene.

#### 3. Create the FlockManager

1. Create an empty object named `FlockManager` in the center of the flocking platform (placed in the floor as it will be the spawn point of the flocking agents).
2. Add the **`FlockManager`** script.
3. Drag the prefab into the `Agent Prefab` field.

#### 4. Create the target

1. Create another empty object named `FlockTarget` and place it on the platform (also in the floor).
2. Drag it into the `Target` field of the `FlockManager`.
3. You can move it during Play mode to see how the flock reacts.

#### 5. Configure the obstacle layers

In the `FlockManager`, under `Obstacle Layers`, **uncheck `Agent` and `Ground`**. Leave only the real obstacle layers checked (walls, boxes, etc.).

#### 6. Press Play

When you press Play, the `FlockManager` spawns the agents inside a circle of radius `Spawn Radius` around its position.
