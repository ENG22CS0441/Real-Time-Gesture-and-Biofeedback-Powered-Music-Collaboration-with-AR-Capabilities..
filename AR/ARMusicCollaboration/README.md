# AR Module — Gesture & Biofeedback Music Collaboration (Unity + AR Foundation)

This folder contains the **Augmented Reality module only** (no backend, no wearable hardware). Use **Unity 2022 LTS**, **AR Foundation**, and **ARCore XR Plugin** for Android.

---

## 1. Prerequisites

- [Unity Hub](https://unity.com/download) + **Unity 2022.3 LTS** (or newer 2022 LTS patch) with **Android Build Support** (SDK & NDK, OpenJDK).
- A physical **Android phone** that supports **Google ARCore** ([supported devices](https://developers.google.com/ar/devices)).
- USB cable or Wi‑Fi debugging for deployment.

---

## 2. Create the Unity project

1. Open **Unity Hub** → **New project** → **3D (Built-in)** or **3D (URP)** — both work with AR Foundation.
2. Name it (e.g. `ARMusicCollaboration`) and create it in a path **outside** OneDrive if you hit file-locking issues (recommended for Unity projects).
3. Copy the **`Assets/Scripts`** folder from this repo into your project’s **`Assets`** folder (merge if asked).

---

## 3. Install packages (AR Foundation + ARCore)

1. **Window → Package Manager**
2. Set dropdown to **Unity Registry**
3. Install:
   - **AR Foundation** (e.g. 5.x matching your Editor)
   - **ARCore XR Plugin** (same major version as AR Foundation, e.g. 5.x)
4. Install **XR Plug-in Management** if prompted (often pulled in automatically).

**XR Plug-in Management**

1. **Edit → Project Settings → XR Plug-in Management**
2. **Android** tab → enable **ARCore**

**Player settings (Android)**

1. **Edit → Project Settings → Player**
2. **Android** → **Other Settings**:
   - **Minimum API Level**: **API 24** or higher (ARCore requirement)
   - **Scripting Backend**: **IL2CPP** (required for 64-bit ARM)
   - **Target Architectures**: enable **ARM64**
3. **Publishing Settings**: use a **keystore** for release builds (debug builds can use default debug keystore).

---

## 4. Scene hierarchy (step-by-step)

### A. AR session root

1. Remove default **Main Camera** if you will use AR (optional; AR template adds its own).
2. **GameObject → XR → AR Session** (or add empty + components per AR Foundation docs).
3. Ensure hierarchy contains:
   - **AR Session** (component: `AR Session`)
   - **AR Session Origin** (child of nothing, or structured as in AR Foundation sample)
     - **AR Camera** (tag **MainCamera**, `Tracked Pose Driver` / `ARCameraManager` as per setup)
     - **AR Plane Manager** on **AR Session Origin**
     - **AR Raycast Manager** on **AR Session Origin** (optional for future plane reticles; pads use **Physics** raycasts)

If your Unity version offers **AR Foundation template** or **XR** sample, you can start from that scene and add the scripts below.

### B. Managers (empty GameObjects)

Create an empty **GameObject** named **`Managers`** and add:

| Component | Purpose |
|-----------|---------|
| `InstrumentModeController` | Piano vs drum clip banks, `AudioSource` |
| `ARInstrumentInteraction` | Touch: tap → pad sound, horizontal swipe → switch mode |
| `HeartRateSimulator` | Simulated BPM + optional UI slider |
| `ARInstrumentSpawner` | Spawns four pads when planes are detected |

Assign references in the Inspector:

- **ARInstrumentInteraction**: `arCamera` → your AR **Main Camera**; `instrumentMode` → same object’s `InstrumentModeController`.
- **HeartRateSimulator**: drag **Slider** and **Text** if you use the UI (see section F).
- **ARInstrumentSpawner**: `planeManager` → **AR Plane Manager**; `arCamera` → AR camera; `padPrefab` → prefab from section D.
- **InstrumentModeController**: assign **Audio Source** (add component if empty) and **8 AudioClips** (4 piano + 4 drum) in section E.

### C. Biofeedback object

1. Create **Particle System** (e.g. child of **AR Session Origin** or floating in front of camera for visibility): name **`BiofeedbackParticles`**.
2. Position it slightly in front of the camera or above the pads (world space).
3. Add **`BiofeedbackVisualizer`** to the same GameObject as the **Particle System**.
4. Assign:
   - **Heart Rate Simulator** → object with `HeartRateSimulator`
   - **Target Renderer** (optional) → a **Quad** or **Sphere** child with a simple material for color tint

Tune **Low Max** / **Medium Max** BPM in `BiofeedbackVisualizer` to match your demo narrative.

### D. Instrument pad prefab

1. **GameObject → 3D Object → Cube** — scale e.g. **0.08** for hand-sized pads.
2. Add **Box Collider** (default on Cube).
3. Add **`InstrumentPad`** — set **Pad Index** 0 (spawner overwrites 0–3 at runtime).
4. Optional: create a **Material** per pad color for a clearer demo.
5. Drag the cube from **Hierarchy** into **Project** to create **`PadPrefab`**.

### E. Audio clips

You need **8 short clips** (WAV recommended):

- **Piano**: 4 notes (e.g. C–E in one octave)
- **Drums**: 4 hits (kick, snare, hi-hat, tom)

Import them under `Assets/Audio/`. Assign in **`InstrumentModeController`**:

- **Piano Clips** → size 4, assign clips
- **Drum Clips** → size 4, assign clips

Without clips, taps will run but stay silent.

### F. UI (Canvas — optional)

1. **GameObject → UI → Canvas** (Screen Space — Overlay).
2. Add **Slider** + **Text (Legacy)** for BPM.
3. On **`HeartRateSimulator`**, assign **Heart Rate Slider** and **Heart Rate Label**.
4. Add **Text** for mode: create empty child, add **`ModeLabelUI`**, assign **`InstrumentModeController`** and the **Text** component.

---

## 5. Script reference (what each file does)

| Script | Role |
|--------|------|
| `HeartRateSimulator.cs` | `float` BPM (40–180), optional **Slider** + label; raises `OnHeartRateChanged`. |
| `BiofeedbackVisualizer.cs` | Reads BPM → **particle** color/rate + optional **renderer** color (blue / green / red zones). |
| `InstrumentModeController.cs` | Two banks of **4 AudioClips**; `PlayPad(index)`; `CycleInstrumentMode()` on swipe. |
| `InstrumentPad.cs` | Per-cube index; small scale “punch” on tap. |
| `ARInstrumentInteraction.cs` | **Tap** → raycast → pad → sound; **Horizontal swipe** → cycle mode. |
| `ARInstrumentSpawner.cs` | When **AR planes** exist, spawns **4 pads** in a row on the first tracked plane. |
| `ModeLabelUI.cs` | Shows **Mode: Piano / Drums** when mode changes. |

---

## 6. Build and run on Android

1. **File → Build Settings** → select **Android** → **Switch Platform** (first time may take a while).
2. Connect phone: enable **Developer options** + **USB debugging**.
3. **Run Device** → select your phone, or **Build And Run**.

**First launch**

- Move the device slowly so **plane detection** runs; pads spawn when the first horizontal plane is tracked.
- **Tap** pads to play; **swipe left/right** to switch Piano/Drums.
- Move the **heart rate slider** to show biofeedback color and particle speed.

**Troubleshooting**

- **Black camera / no AR**: ARCore installed? Lighting permission? **AR Session** active?
- **No pads**: Wait for floor/table scan; ensure **`ARInstrumentSpawner`** has **`AR Plane Manager`** and **`padPrefab`** set.
- **No sound**: Assign all **8 clips**; check **Audio Source** not muted; device volume up.
- **Tap does nothing**: Pads need **Colliders**; **AR camera** assigned; pad in front of camera when tapping.

---

## 7. Demo tips (impressive but still student-scope)

- Add subtle **light estimation** or a simple **directional light** following AR samples so materials read well.
- Record screen with **Unity Recorder** or phone built-in screen record for your report.
- Narrate **biofeedback zones** (calm = blue, focus = green, excitement = red) tied to the slider.

---

## License

Use freely for your academic project; attribute third-party assets if you add any from the Asset Store.
