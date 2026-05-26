# Alphy

A comprehensive, open-source Rocket League cosmetic mod manager designed for safety, simplicity, and performance. 

> ⚠️ **IMPORTANT NOTE:** Alphy is strictly an external file management utility. The application does **NOT** contain, distribute, or provide any proprietary mods or custom items. It is designed solely to safely install, organize, and remove custom cosmetic files (`.upk`, `.bnk`) provided by the user while flawlessly preserving the integrity of the original game files.

---

## 📋 Table of Contents
* [Core Features & Safety Systems](#features)
* [Supported Mod Categories](#categories)
* [Installation & First Setup](#installation)
* [How to Use Alphy](#how-to-use)
* [🧩 Plugin System & Alphy Swapper](#plugins)
* [How It Works](#how-it-works)
* [Community, Tools & Support](#community)

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

---

## <a id="installation"></a>📥 Installation & First Setup

1. Download the latest `Alphy.exe` from the [Releases](https://github.com/AC-Storm-YT/Alphy-Public/releases) page.
2. Place `Alphy.exe` in an empty folder (e.g., `Desktop/Alphy Mod Manager/`). *Do not run it directly from your Downloads folder or inside your Rocket League game files!*
3. Launch `Alphy.exe`.
4. The application will automatically attempt to locate your Rocket League `CookedPCConsole` folder. 
5. If it cannot find it automatically, a prompt will appear. Navigate to your Rocket League installation and select the `TAGame/CookedPCConsole` folder.

---

## <a id="how-to-use"></a>🎮 How to Use Alphy

1. Click the **"IMPORT NEW MOD"** button at the bottom of the screen.
2. Fill out the Mod Name, select what vanilla item the mod replaces, choose the correct Category, and select your `.upk` or `.bnk` mod files.
3. Once imported, the mod will appear in the grid. Simply click the card to toggle it ON or OFF. 
4. Click the **"SAVE CHANGES"** button. Alphy will automatically secure your original files and replace them with the selected mods.
5. Launch Rocket League!

---

## <a id="plugins"></a>🧩 Plugin System & Alphy Swapper

Alphy now supports an extensible Plugin System! Plugins allow you to add powerful new tools directly inside the Alphy interface.

### The Plugin Manager
To access the Plugin Manager, click the **"Plugins"** button located at the bottom of the right sidebar. This menu allows you to browse, download, install, and update official plugins directly from the cloud with a single click.

### Official Plugin: Alphy Swapper
The **Alphy Swapper** is an advanced, offline asset swapper that allows you to generate your own custom mods by swapping internal Rocket League `.upk` files. 

Because it is a native plugin, it is fully automated:
* **Zero Configuration:** It automatically pulls your game directory straight from Alphy's memory. You don't need to manually link any folders.
* **Smart Exporting:** When you generate a swap, the plugin automatically routes the new mod directly into your Alphy `mods/` folder and categorizes it perfectly.
* **Instant Refresh:** As soon as a swap is generated, the plugin tells Alphy to instantly refresh its interface. Your newly created mod will appear in your grid immediately—**no restarts required!**

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
* 💬 **Discord:** [Join the Alphy Community Server](https://discord.gg/UuC5j8smR7)

---

### Open Source
Alphy is 100% open-source. This ensures complete transparency regarding how your local game files are handled. We welcome code reviews, feedback, and requests from the community.
