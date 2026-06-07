# Alphy

A Rocket League cosmetic mod manager designed for safer local file swapping, custom imports, backups, mod sharing, and plugins.

> **Alphy v2.3.0 Notice:** Starting with v2.0.0, the official Alphy application is closed-source. The public repository remains available as the official README, release, download, and legacy source hub. Public source code is preserved up to **Alphy v1.7.1**.

> **Important:** Alphy is an external file management utility. It does **not** contain, distribute, or provide proprietary mods, paid items, or preloaded mod packs. Users import and manage their own cosmetic files.

---

## Table of Contents

* [Security & False Positives](#security)
* [Core Features](#features)
* [Supported Categories](#categories)
* [Discord Access & Privacy](#discord-access)
* [How to Use Alphy](#how-to-use)
* [Alphy Launcher](#launcher)
* [Plugin System & Alphy Swapper](#plugins)
* [How It Works](#how-it-works)
* [Community, Tools & Support](#community)
* [Repository Status](#repository-status)

---

## <a id="security"></a>Security & False Positives

Some antivirus tools may flag modding utilities because they edit game files, launch helper scripts, or perform automated file operations.

Alphy does **not** inject into Rocket League, does **not** hook game memory, and does **not** need to run inside the game process.

Alphy works by editing local asset files, creating backups, and restoring files when needed.

Starting with **Alphy v2.0.0**, the official build is closed-source because the app connects to cloud infrastructure and uses Discord authorization for role-based access. Keeping backend routing and security logic public would create unnecessary risk for the official service.

Official Alphy builds are **not obfuscated**.

---

## <a id="features"></a>Core Features

### Visual Mod Manager

Alphy uses a visual mod grid with categories, search, icons, active counts, and a modern dark interface.

### No Injection

Alphy operates outside of game memory. It only performs local file operations.

### Smart Backups

Before changing Rocket League files, Alphy creates vanilla backups in `%AppData%\Alphy\Backups`. Those backups are used to restore files when mods are removed or disabled.

### Digital Sealing & Mismatch Detection

If you verify game files or Rocket League updates restore modded files back to vanilla, Alphy detects the mismatch and resets affected mod states.

### Custom Texture Support

Alphy supports compatible custom decals, custom balls, and custom boost meters, including supported AlphaConsole-style ZIP packs.

### Mod Export & Mod Packs

Alphy includes a dedicated Export Mods window. You can export one mod as a shareable ZIP or export multiple mods as a `mods.zip` pack.

Exports do not include vanilla backup files or backup folders. The receiving Alphy install creates missing backups from that user's own Rocket League files when needed.

### Improved Setup Flow

If no valid game path is saved, Alphy requires the user to select the installation folder named `rocketleague` before the app unlocks. The folder can be changed later from Settings.

### Improved Remove Mods

The Remove Mods window now uses a searchable checklist similar to Export Mods while keeping the restore-active-mod-before-delete safety behavior.

### Plugin Architecture

Alphy includes a Plugin Manager for official tools like Alphy Swapper.

### Account Access Protection

Alphy uses Discord authorization and a random local install ID to help prevent account access sharing. An account can only be linked to one Alphy install at a time unless the user logs out, switches account, or contacts the development team for a reset.

### Alphy Launcher

Alphy includes a dedicated launcher that can prepare Rocket League safely before startup. When enabled, it keeps the game clean before opening Epic Games or Steam, applies selected mods only when launching, and restores files back to vanilla after Rocket League closes.

### Themes and Settings

Alphy includes a Settings page for launcher behavior, mod activation preferences, streamer mode, error logging, game folder selection, update checks, and visual themes.

---

## <a id="categories"></a>Supported Categories

* Antenna
* Banner
* Body
* Boost
* Boost Audio
* Decal
* Goal Explosion
* Hat / Toppers
* Paint / Finishes
* Trail
* Wheels
* Engine Audio
* Avatar Border
* Custom Decals
* Custom Balls
* Custom Boost Meter

---

## <a id="discord-access"></a>Discord Access & Privacy

Starting with **Alphy v2.0.0**, the official build uses Discord authorization before opening the app.

This verifies that users are in the official Alphy Discord server and have the required role to use the official build. It also supports future role-based permissions, such as beta features, plugins, or custom tools.

To use Alphy:

1. Join the official Alphy Discord server: [https://discord.gg/A2mrCVdpPw](https://discord.gg/A2mrCVdpPw)
2. Have the required **Member** role.
3. Click **Authorize with Discord** when Alphy opens.
4. Approve the Discord authorization page.
5. Paste the authorization code back into Alphy.

Alphy does **not** receive your Discord password, email, private messages, friends list, or Rocket League account information. Discord handles the authorization page directly.

Alphy only uses the Discord authorization result needed to verify your Discord account, avatar, server membership, and roles in the official Alphy server.

Alphy also creates a random local install ID after the user accepts the access requirement. This is not your hardware serial number, Discord password, Rocket League account, or personal file data. It is a random ID used to link one Discord account to one Alphy install.

The local install ID is protected on Windows and sent to Alphy Auth when access is checked. The server stores a hashed version of that install ID and a hashed authorization-code fingerprint. The real authorization code is not stored by the server.

If an authorization code is copied to another PC, Alphy Auth rejects it and resets that shared code. If an account is already linked to another device, users must either use **Logout** or **Switch account** on the original device, or contact the development team to reset and use the account on a new device.

Alphy's authorization service is hosted through Cloudflare. Like most web and API infrastructure providers, Cloudflare may process standard request metadata needed to route, secure, and debug requests, such as IP address, approximate location/network information, user agent, timestamps, request paths, and diagnostic logs.

Alphy does not use Cloudflare request metadata to profile users. It is used only as part of the infrastructure that runs the authorization service.

Alphy silently checks Discord access while the app is open. This does **not** force users to re-login every few minutes. Users normally re-authorize every **30 days**, unless access is revoked, the session expires, the account is reset, or the device link no longer matches.

---

## <a id="how-to-use"></a>How to Use Alphy

### First-Time Setup

1. Launch Alphy and authorize with Discord.
2. If Alphy asks for the game folder, select the folder named `rocketleague`.
3. Alphy validates the folder and stores the internal `TAGame\CookedPCConsole` path automatically.
4. You can change the folder later from **Settings**.

### Discord Account Access

Alphy must be authorized with a Discord account that has access to the official Alphy server.

* **Refresh access** checks your current roles and permissions.
* **Logout** releases your linked device and closes the current session.
* **Switch account** releases your linked device and sends you back to the login screen.
* If you want to use your account on a new device and cannot access the old one, contact the development team for a reset.

### Importing Standard Mods (`.upk` / `.bnk`)

1. Click **IMPORT NEW MOD**.
2. Enter the mod name, replacement item, category, and select the `.upk` or `.bnk` file.
3. The mod appears in the grid.
4. Toggle the mod ON and click **SAVE CHANGES**.

If **Use Launcher** is enabled in Settings, **SAVE CHANGES** saves your selected mod setup without applying it immediately. The launcher applies those mods when you launch Rocket League and restores the game after Rocket League closes.

If **Use Launcher** is disabled, **SAVE CHANGES** applies or removes the selected mods immediately, like older Alphy versions.

### Importing Custom Texture Mods / AlphaConsole ZIPs

Alphy can import supported AlphaConsole-style `.zip` packs for custom decals, custom balls, and custom boost meters.

1. Click **IMPORT NEW MOD** and select the `.zip` file.
2. Alphy reads supported packs and fills in the mod type, replacement item, name, and preview image when possible.
3. Toggle the imported custom mod ON and click **SAVE CHANGES**.

### Importing Alphy Mods and Mod Packs

Alphy can import mods exported from Alphy.

* A single exported mod usually looks like `Fennec (Replaces Octane).zip`.
* A multi-mod pack is named `mods.zip`.
* Mod packs preserve category folders, such as `Body\Fennec (Replaces Octane)`.

To import one, click **IMPORT NEW MOD**, select the ZIP, and Alphy restores it into the correct folder automatically.

### Exporting Mods

1. Click **EXPORT MODS**.
2. Search for the mod or mods you want to export.
3. Select one mod to create a single shareable ZIP.
4. Select multiple mods to create a `mods.zip` pack.

Exported ZIPs do not include backup files, backup folders, or temporary Alphy export metadata.

### Removing Mods

1. Click **REMOVE MODS**.
2. Search for the mods you want to delete.
3. Select one or more mods and click **Remove Selected**.
4. If a selected mod is active, Alphy restores the game files back to vanilla before deleting the mod.

---

## <a id="launcher"></a>Alphy Launcher

The Alphy Launcher is designed for users who want Alphy to handle the safest launch flow automatically.

When **Use Launcher** is enabled:

1. Choose your active mods in Alphy.
2. Click **SAVE CHANGES** to save the selection.
3. Click **LAUNCH**.
4. Alphy prepares Rocket League, applies the saved setup, launches the game, and keeps track of the session.
5. When Rocket League closes, Alphy restores launcher-applied files back to vanilla.

This helps avoid Epic Games or Steam seeing modded files before the game starts.

Launcher settings include:

* **Launch without mods:** Starts Rocket League without applying the saved mod setup.
* **Launch without Easy Anti-Cheat:** Adds the supported no-EAC launch option when available.
* **JoyToKey compatibility:** Restarts JoyToKey in the background before launch when JoyToKey is found on the system.

If Rocket League is running through the launcher, Alphy needs to stay open until the game closes so it can restore files correctly.

---

## <a id="plugins"></a>Plugin System & Alphy Swapper

Alphy supports official plugins through the Plugin Manager.

### The Plugin Manager

Click **Plugins** inside Alphy to browse, download, install, and update official plugins directly from the cloud.

### Official Plugin: Alphy Swapper

**Alphy Swapper** is an asset swapper that lets users generate custom mods by swapping internal Rocket League `.upk` files.

It integrates with Alphy:

* **Zero Configuration:** It reads the game directory from Alphy.
* **Smart Exporting:** Generated swaps are routed directly into the Alphy `mods/` folder.
* **Instant Refresh:** New generated mods appear in Alphy without restarting.
* **Multiple Engines:** Users can choose between Alphy Pro (Extreme), Alphy Pro, Alphy, and RLUPKTools.
* **Clearer Swapping Flow:** The Swapper uses "Item you equip" and "Item you see" selectors with search boxes to make duplicate item names easier to manage.

Alphy Pro (Extreme) is the default engine. Other engines remain available as fallback options for cases where a specific swap does not work correctly with the default engine.

The Pro engines are handled internally by the official plugin build instead of being left as editable backend scripts.

### Prerequisites

Alphy Swapper uses Python for some backend operations. The plugin should automatically prepare what it needs, including modules such as `cryptography`.

If you still get a cryptography error, install Python 64-bit and run:

```cmd
python -m pip install --upgrade pip
python -m pip install cryptography
```

---

## <a id="how-it-works"></a>How It Works

For developers and curious users, here is how Alphy manages files locally:

* **Mod Storage:** Imported mods are stored in the `mods/` directory next to `Alphy.exe`.
* **Backups:** Original game file backups (`.bak`) are saved to `%AppData%\Alphy\Backups`.
* **Configuration:** Active mod states, digital seals, and saved file paths are stored at `%AppData%\Alphy\Config\settings.txt`.
* **Discord Session:** The local authorization session is stored at `%AppData%\Alphy\Auth\session.json`.
* **Install ID:** A random protected install ID is stored at `%AppData%\Alphy\Auth\device.json`.
* **Plugins:** Installed plugin modules are stored in `%AppData%\Alphy\Plugins`.
* **Exports:** Shared mod ZIPs do not include vanilla backups. Backups are recreated from the receiver's own game files when needed.
* **Launcher Mode:** When launcher mode is enabled, selected mods are saved first, then applied only for the Rocket League launch session and restored after the game closes.

---

## <a id="community"></a>Community, Tools & Support

* **Asset Database:** [Alphy Items Database](https://ac-storm-yt.github.io/alphy-items.github.io/)
* **Discord:** [Join the Alphy Community Server](https://discord.gg/A2mrCVdpPw)

---

## <a id="repository-status"></a>Repository Status

Alphy is closed-source starting with **v2.0.0**.

This repository remains online as the official public hub for:

* Downloading official releases.
* Reading the latest README and usage information.
* Preserving legacy source code up to **Alphy v1.7.1**.

The **Alphy Swapper** public source remains available up to **v1.0.3** for history and transparency, but newer official plugin builds are closed-source.

This change was made because v2.0.0 connects to cloud infrastructure and Discord authorization. Keeping backend routing and permission logic public would make the official service easier to abuse.

Official builds remain **clean and unobfuscated**.

---

## Credits

Shadxw also provided his work for the swapping system used in [Oryx](https://discord.gg/sWhS6F8v9a), which allowed me to create a fallback engine for Alphy Swapper **v1.1.0**. (Discontinued)

Additional thanks to Crunchy and [RLUPKTools](https://github.com/CrunchyRL/RLUPKTools) for the foundational technical research.
