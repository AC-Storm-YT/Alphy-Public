# Alphy

A comprehensive, Rocket League cosmetic mod manager designed for safety, simplicity, and performance. 

> ⚠️ **IMPORTANT NOTE:** Alphy is strictly an external file management utility. The application does **NOT** contain, distribute, or provide any proprietary mods or custom items. It is designed solely to safely install, organize, and remove custom cosmetic files (`.upk`, `.bnk`) provided by the user while flawlessly preserving the integrity of the original game files.

---

## 📋 Table of Contents
* [Core Features & Safety Systems](#security)
* [Core Features & Safety Systems](#features)
* [Supported Mod Categories](#categories)
* [Installation & First Setup](#installation)
* [Alphy Discord Access](#discord-access)
* [How to Use Alphy](#how-to-use)
* [🧩 Plugin System & Alphy Swapper](#plugins)
* [Swapping Engines](#swapping-engines)
* [How It Works](#how-it-works)
* [Community, Tools & Support](#community)

---

## <a id="security"></a>🛡️ Security & False Positives

You might notice that Alphy triggers some warnings on VirusTotal, but **these are false positives**. 

Because Alphy automates low-level file system operations to safely swap protected game files, some antivirus incorrectly assume the app is acting maliciously.

**Alphy is 100% safe as well as any plugins.**
---

## <a id="features"></a>🛡️ Core Features & Safety Systems

Alphy is built from the ground up to ensure your game files are never corrupted. It utilizes several automated background systems to protect your installation.

### 1. Cloud Safety Lockdown & Live Build Tracking
Alphy continuously cross-references your local Rocket League `.exe` version and the live server build via the cloud. If an unexpected game patch is released, Alphy detects the build mismatch instantly and automatically locks the application's modding capabilities. This guarantees that incompatible mods are never injected into a freshly updated game, completely preventing crashes and file corruption.

### 2. Smart Hash-Sync Backups
Before Alphy applies any mods, it performs a cryptographic hash check on your local game files. If a file is original (vanilla), Alphy automatically secures a backup. When a mod is removed, Alphy intelligently restores that specific backup, ensuring your game is always left in a clean, pristine state.

### 3. Digital Sealing
Active mods are "sealed" with digital fingerprints. If you verify your game files through Epic Games or Steam (which overwrites mods with vanilla files), Alphy will automatically detect the overwrite on its next launch, disable the affected mods in its interface, and prevent ghost-file conflicts.

### 4. Advanced UI & Rendering
Alphy features a Master-Detail UI with a persistent sidebar, real-time search filtering, and custom scroll-rendering technology to ensure the interface remains smooth and tear-free, regardless of how many mods you install.

### 5. Extensible Plugin Architecture
Alphy features a built-in Plugin Manager that allows the community to expand its capabilities. Official plugins (like the Alphy Swapper) integrate directly into the app, automatically syncing with your game paths and refreshing the UI instantly without ever needing to restart the manager.

---

## <a id="categories"></a>🏷️ Supported Mod Categories

Alphy strictly organizes mods to ensure single-file integrity (only one mod can be active per category to prevent overlap crashes).

* **Antenna**
* **Banner**
* **Body**
* **Boost**
* **Boost Audio**
* **Decal**
* **Goal Explosion**
* **Hat (Toppers)**
* **Paint (Finishes)**
* **Trail**
* **Wheels**
* **Custom Decals**
* **Custom Balls**
* **Custom Boost Meter**

---

## <a id="installation"></a>📥 Installation & First Setup

1. Download the latest `Alphy.exe` from the [Releases](https://github.com/AC-Storm-YT/Alphy-Public/releases) page.
2. Place `Alphy.exe` in an empty folder (e.g., `Desktop/Alphy Mod Manager/`). *Do not run it directly from your Downloads folder or inside your Rocket League game files!*
3. Launch `Alphy.exe`.
4. Authorize your Discord account when prompted. You must be in the official Alphy Discord server with the required **Member** role to use the official build.
5. The application will automatically attempt to locate your Rocket League `CookedPCConsole` folder. 
6. If it cannot find it automatically, a prompt will appear. Navigate to your Rocket League installation and select the `TAGame/CookedPCConsole` folder.

---

## <a id="discord-access"></a>Alphy Discord Access

Starting with **Alphy 2.0.0**, the official build uses Discord authorization before opening the app.

This system exists to protect the official Alphy release, manage access and beta features, and make future role-based permissions possible.

To use Alphy, you need to:

1. Join the official Alphy Discord server: [https://discord.gg/A2mrCVdpPw](https://discord.gg/A2mrCVdpPw)
2. Have the required **Member** role.
3. Click **Authorize with Discord** when Alphy opens.
4. Paste the authorization code back into Alphy.

### 🔒 Privacy & Network Security
To handle this validation safely without exposing private API keys, Alphy uses a secure, stateless backend hosted on Cloudflare Workers. 

When you authorize the application:
* **What passes through:** The temporary Discord authentication code is securely sent to our Cloudflare Worker, which asks Discord to verify your server membership and roles.
* **Network-level data:** Like almost any web service, Cloudflare automatically processes standard network data (such as your IP address, general region, and timestamps) to route the request and protect our backend from DDoS attacks.
* **No data retention:** Our backend is completely stateless. **We do not run a database, and we do not store your IP, personal information, or Discord data.** Once the role check is complete, the data is entirely dropped.

Alphy only uses the Discord authorization result needed to verify your username, avatar, server membership, and roles in the official Alphy server. Those roles decide whether you can use the app, plugins, custom mods, or beta-only features.

If you do not have access yet, join the Discord server and make sure your account has the required role.

---

## <a id="how-to-use"></a>🎮 How to Use Alphy

### Importing Standard Mods (`.upk` / `.bnk`)

1. Click the **"IMPORT NEW MOD"** button at the bottom of the screen.
2. Fill out the Mod Name, select what vanilla item the mod replaces, choose the correct Category, and select your `.upk` or `.bnk` mod files.
3. Once imported, the mod will appear in the grid. Simply click the card to toggle it ON or OFF.
4. Click the **"SAVE CHANGES"** button. Alphy will automatically secure your original files and replace them with the selected mods.
5. Launch Rocket League!

### Importing Custom Texture Mods / AlphaConsole ZIPs

Alphy can also import supported AlphaConsole-style `.zip` packs directly. These are used for custom decals, custom balls, and custom boost meters.

1. Click **"IMPORT NEW MOD"** and select the downloaded `.zip` file.
2. If Alphy detects an AlphaConsole custom texture pack, it will automatically read the pack and fill in the correct mod type, replacement item, name, and preview image when possible.
3. For custom decals, Alphy can detect supported Octane, Fennec, Dominus, and Universal decal packs. If a pack contains multiple supported car variants, Alphy can import it as one mod that replaces all supported bodies at the same time.
4. For custom balls and boost meters, Alphy imports the texture pack into the correct custom category and uses the included preview image on the mod card.
5. After importing, toggle the custom mod ON in the grid and click **"SAVE CHANGES"**. The first apply may take a little longer while Alphy creates backups and safely patches the required game textures.
6. To switch to a different custom texture later, select the new mod and save again. Alphy will skip unchanged active mods so only the necessary files are updated.

---

## <a id="plugins"></a>🧩 Plugin System & Alphy Swapper

Alphy now supports an extensible Plugin System! Plugins allow you to add powerful new tools directly inside the Alphy interface.

### The Plugin Manager

To access the Plugin Manager, click the **"Plugins"** button located at the bottom of the right sidebar. This menu allows you to browse, download, install, and update official plugins directly from the cloud with a single click.

### Official Plugin: Alphy Swapper

The **Alphy Swapper** is an advanced, asset swapper that allows you to generate your own custom mods by swapping internal Rocket League `.upk` files.

Because it is a native plugin, it is fully automated:

* **Zero Configuration:** It automatically pulls your game directory straight from Alphy's memory. You don't need to manually link any folders.
* **Smart Exporting:** When you generate a swap, the plugin automatically routes the new mod directly into your Alphy `mods/` folder and categorizes it perfectly.
* **Instant Refresh:** As soon as a swap is generated, the plugin tells Alphy to instantly refresh its interface. Your newly created mod will appear in your grid immediately—**no restarts required!**

### <a id="swapping-engines"></a>Swapping Engines

Alphy Swapper now supports selectable swapping engines. A swapping engine is the backend system that reads Rocket League item data, processes the selected donor and target items, and generates the final mod files that Alphy can install.

The default engine is **RLUPKTools**. This is the recommended engine for normal use because it is the most complete and is designed to safely rebuild supported `.upk` swaps.

An additional **Alphy** engine is available as a fallback option. It is based on a simpler file replacement style swap system and is intended for cases where a specific swap does not work correctly with the default RLUPKTools engine.

When switching away from RLUPKTools, Alphy Swapper will show a warning because alternate engines may be less stable depending on the item, category, and game files involved. Most users should leave the engine set to **RLUPKTools (Default)** unless they are troubleshooting a specific swap.

The selected engine is remembered between launches, and backend engine files are automatically refreshed from the plugin when an update includes newer versions.

#### Background Dependencies
Alphy Swapper uses Python in the background to run its asset tools.

The plugin automatically checks for a working Python environment, verifies required packages such as `cryptography`, and installs missing dependencies when needed. If Python is not available, Alphy Swapper can prepare its own portable Python backend inside `%AppData%\AlphySwapper\Backend`.

If you still get a cryptography module error, try restarting Alphy first so the plugin can re-check the backend. As a manual fallback, you can run:
1. `python -m pip install --upgrade pip`
2. `python -m pip install cryptography`

---

## <a id="how-it-works"></a>🔧 How It Works

For developers and curious users, here is how Alphy manages your files locally:

* **Mod Storage:** Imported mods are stored within the `mods/` directory right next to your `Alphy.exe` file.
* **Backups:** Original game file backups (`.bak`) are securely saved to your local AppData folder at `%AppData%\Alphy\Backups`.
* **Configuration:** Your active mod states, digital seals, and saved file paths are stored at `%AppData%\Alphy\Config\settings.txt`.
* **Plugins:** Installed plugin modules (`.dll` files) are stored and loaded from `%AppData%\Alphy\Plugins`.
* **UI Framework:** The application interface is built using `MaterialSkin` to provide a clean, accessible, dark-themed experience.

---

## <a id="community"></a>🤝 Community, Tools & Support

Have questions, want to report a bug, or looking for cosmetic files to download? Join the community!

* 🖼️ **Asset Database:** [Alphy Items Database](https://ac-storm-yt.github.io/alphy-items.github.io/) (Find exact item IDs for mod importation)
* 🌐 **Website:** [Alphy Official Site](https://ac-storm-yt.github.io/alphy.github.io/)
* 💬 **Discord:** [Join the Alphy Community Server](https://discord.gg/A2mrCVdpPw)

---

### Open Source
Alphy is open-source till update v1.7.1. The open-source project was discontinnued for security measure after update v2.0.0
