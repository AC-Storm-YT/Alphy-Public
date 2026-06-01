# Alphy

A comprehensive Rocket League cosmetic mod manager designed for safety, simplicity, and performance.

> **Alphy v2.0.0 Notice:** Starting with v2.0.0, the official Alphy application is now closed-source. The public repository remains available as the official README, release, download, and legacy source hub. Public source code is preserved up to **Alphy v1.7.1**.

> ⚠️ **IMPORTANT NOTE:** Alphy is strictly an external file management utility. The application does **NOT** contain, distribute, or provide any proprietary mods or custom items. It is designed solely to safely install, organize, and remove custom cosmetic files (`.upk`, `.bnk`) provided by the user while flawlessly preserving the integrity of the original game files.

---

## 📋 Table of Contents
* [Core Features & Safety Systems](#security)
* [Core Features & Safety Systems](#features)
* [Supported Mod Categories](#categories)
* [Discord Access & Privacy](#discord-access)
* [How to Use Alphy](#how-to-use)
* [🧩 Plugin System & Alphy Swapper](#plugins)
* [How It Works](#how-it-works)
* [Community, Tools & Support](#community)
* [Repository Status](#repository-status)

---

## <a id="security"></a>🛡️ Security & False Positives

You might notice that Alphy triggers some warnings on VirusTotal, but **these are false positives**. 

Because Alphy automates low-level file system operations to safely swap protected game files, some antivirus incorrectly assume the app is acting maliciously.

Alphy versions up to **v1.7.1** remain available as public source code in this repository.

Starting with **Alphy v2.0.0**, the official build is closed-source because the app now connects to cloud infrastructure and uses Discord authorization for role-based access. Keeping backend routing and security logic public would create unnecessary risk for the official service.

To maintain trust, official Alphy builds are **not obfuscated**. The app remains focused on local file management, safe backups, and cosmetic mod organization.

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

## <a id="discord-access"></a>Discord Access & Privacy

Starting with **Alphy v2.0.0**, the official build uses Discord authorization before opening the app.

This is used to verify that users are in the official Alphy Discord server and have the required role to use the app. It also allows Alphy to support future role-based permissions, such as beta access, without hardcoding private permission rules into the public release repository.

To use Alphy, you need to:

1. Join the official Alphy Discord server: [https://discord.gg/A2mrCVdpPw](https://discord.gg/A2mrCVdpPw)
2. Have the required **Member** role.
3. Click **Authorize with Discord** when Alphy opens.
4. Paste the authorization code back into Alphy.

Alphy does **not** receive your Discord password, email, private messages, friends list, or Rocket League account information. Discord handles the authorization page directly.

Alphy only uses the Discord authorization result needed to verify your Discord account, avatar, server membership, and roles in the official Alphy server. Those roles decide whether you can use the app, plugins, custom mods, or beta-only features.

Alphy's authorization service is hosted through Cloudflare. Like most web and API infrastructure providers, Cloudflare may process standard request metadata needed to route, secure, and debug requests, such as IP address, approximate location/network information, user agent, timestamps, request paths, and diagnostic logs.

Alphy does not use Cloudflare request metadata to profile users. It is used only as part of the infrastructure that runs the authorization service.

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

#### ⚠️ Prerequisites
The Alphy Swapper requires **Python 3.8.0 or newer** to execute the background asset modifications. 

The swapper plugin should automatically install it, but if you run into an error, you will have to install it.

If you get a cryptography module error please install it using CMD:
1. `python -m pip install --upgrade pip`
2. `python -m pip install cryptography`


---

## <a id="how-it-works"></a>🔧 How It Works

For developers and curious users, here is how Alphy manages your files locally:

* **Mod Storage:** Imported mods are stored within the `mods/` directory right next to your `Alphy.exe` file.
* **Backups:** Original game file backups (`.bak`) are securely saved to your local AppData folder at `%AppData%\Alphy\Backups`.
* **Configuration:** Your active mod states, digital seals, and saved file paths are stored at `%AppData%\Alphy\Config\settings.txt`.
* **Discord Session:** Your local authorization session is stored at `%AppData%\Alphy\Auth\session.json` so you do not need to authorize every time you open Alphy.
* **Plugins:** Installed plugin modules (`.dll` files) are stored and loaded from `%AppData%\Alphy\Plugins`.
* **UI Framework:** The application interface is built using `MaterialSkin` to provide a clean, accessible, dark-themed experience.

---

## <a id="community"></a>🤝 Community, Tools & Support

Have questions, want to report a bug, or looking for cosmetic files to download? Join the community!

* 🖼️ **Asset Database:** [Alphy Items Database](https://ac-storm-yt.github.io/alphy-items.github.io/) (Find exact item IDs for mod importation)
* 🌐 **Website:** [Alphy Official Site](https://ac-storm-yt.github.io/alphy.github.io/)
* 💬 **Discord:** [Join the Alphy Community Server](https://discord.gg/A2mrCVdpPw)

---

## <a id="repository-status"></a>Repository Status

Alphy is transitioning to a closed-source official build starting with **v2.0.0**.

This repository will stay online as the official public hub for:

* Downloading official releases.
* Reading the latest README and usage information.
* Preserving legacy source code up to **Alphy v1.7.1**.

The **Alphy Swapper** public source will remain available up to **v1.0.3** for history and transparency, but newer official plugin builds are closed-source.

This change was made because v2.0.0 connects to cloud infrastructure and Discord authorization. Keeping backend routing and permission logic public would make the official service easier to abuse.

Official builds remain **clean and unobfuscated**.
