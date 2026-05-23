# Alphy

A comprehensive, open-source Rocket League cosmetic mod manager designed for safety, simplicity, and performance. 

> ⚠️ **IMPORTANT NOTE:** Alphy is strictly an external file management utility. The application does **NOT** contain, distribute, or provide any proprietary mods or custom items. It is designed solely to safely install, organize, and remove custom cosmetic files (`.upk`, `.bnk`) provided by the user while flawlessly preserving the integrity of the original game files.

---

## 📋 Table of Contents
* [Core Features & Safety Systems](#features)
* [Supported Mod Categories](#categories)
* [Installation & First Setup](#installation)
* [How to Use Alphy](#how-to-use)
* [How It Works](#how-it-works)
* [Community, Tools & Support](#community)

---

## <a id="features"></a>🛡️ Core Features & Safety Systems

Alphy is built from the ground up to ensure your game files are never corrupted. It utilizes several automated background systems to protect your installation.

### 1. Cloud Safety Lockdown & Live Build Tracking
Alphy continuously cross-references your local Rocket League `.exe` version and the live server build via the cloud. If an unexpected game patch is released, Alphy detects the build mismatch instantly and automatically locks the application's modding capabilities. This guarantees that incompatible mods are never injected into a freshly updated game, completely preventing crashes and file corruption.

### 2. Smart Hash-Sync Backups
Before Alphy applies any custom mod, it calculates an MD5 hash of your original, vanilla game files. It then creates a secure `.bak` backup in your local AppData folder. When the game updates, the "Smart Hash-Sync" system automatically updates your backups to the latest vanilla versions, ensuring you always have a pristine point of return.

### 3. Active Process Guard
You can't accidentally break the game by applying mods while playing. Alphy monitors your system processes and will alert you if `RocketLeague.exe` is currently running. It even offers a built-in prompt to safely close the game for you before proceeding with modifications.

### 4. Built-in Mod Importer & Eraser
Forget about manually digging through Windows File Explorer.
* **The Importer:** A streamlined wizard that lets you select your downloaded `.upk` or `.bnk` files, assign them to a category, and specify what item they replace. Alphy automatically builds the correct folder structure in the background.
* **The Eraser:** A dedicated removal tool that lets you safely and permanently delete custom items with a few clicks, including a mass-deletion option.

### 5. Single-Selection Architecture
To prevent file collisions and visual bugs (such as trying to apply two different wheel mods that replace the same base file), Alphy uses a strict radio-button selection system. Only one mod can be active per category at any given time, ensuring total stability.

### 6. Automatic Self-Updater
Alphy checks a remote GitHub repository for updates on launch. If a new version is detected, the built-in updater handles downloading the new `.zip`, extracting the files, and silently patching the application via a background batch script.

---

## <a id="categories"></a>🏎️ Supported Mod Categories

Alphy organizes your custom files into 11 dedicated, easy-to-navigate categories via a modern side-drawer menu:
* Antenna
* Banner
* Body
* Boost
* Boost Audio
* Decal
* Goal Explosion
* Hat
* Paint
* Trail
* Wheels

---

## <a id="installation"></a>🚀 Installation & First Setup

1. Navigate to the **[Releases](../../releases/latest)** tab on this repository.
2. Download the latest `alphy.zip` file.
3. Extract the folder to your preferred location on your PC (e.g., your Desktop or Documents folder).
4. Run `Alphy.exe`.
5. On your first launch, Alphy will ask you to select your Rocket League folder. 
   * Navigate to your Steam or Epic Games installation path.
   * Select the root `rocketleague` folder. Alphy will automatically locate the `TAGame\CookedPCConsole` directory.

---

## <a id="how-to-use"></a>⚙️ How to Use Alphy

**Importing a Mod:**
1. Click the **"IMPORT NEW MOD"** button at the bottom left of the dashboard.
2. Enter the name of your mod (e.g., *Titanium White Fennec*).
   > 💡 **Pro Tip:** Use the official [Alphy Items Database](https://ac-storm-yt.github.io/alphy-items.github.io/) to search for the item you are using. Copy and paste the exact ID / Name from the site into Alphy, and the app will automatically attach the correct item image to your mod!
3. Specify the item it replaces. 
4. Select the appropriate category from the dropdown menu.
5. Click **"SELECT MOD FILES"** and choose your `.upk` or `.bnk` files.
6. Click **"ADD MOD"**. Alphy will refresh and display your new item in the corresponding category tab.

**Applying Mods:**
1. Navigate through the category tabs on the left menu.
2. Select the radio button next to the mod you wish to apply.
3. Click the **"SAVE CHANGES"** button. Alphy will automatically secure your original files and replace them with the selected mods.
4. Launch Rocket League!

---

## <a id="how-it-works"></a>🔧 How It Works

For developers and curious users, here is how Alphy manages your files locally:

* **Mod Storage:** Imported mods are stored within the `mods/` directory right next to your `Alphy.exe` file.
* **Backups:** Original game file backups (`.bak`) are securely saved to your local AppData folder at `%AppData%\Alphy\Backups`.
* **Configuration:** Your active mod states, digital seals, and saved file paths are stored at `%AppData%\Alphy\Config\settings.txt`.
* **UI Framework:** The application interface is built using `MaterialSkin` to provide a clean, accessible, dark-themed experience.

---

## <a id="community"></a>🤝 Community, Tools & Support

Have questions, want to report a bug, or looking for cosmetic files to download? Join the community!

* 🖼️ **Asset Database:** [Alphy Items Database](https://ac-storm-yt.github.io/alphy-items.github.io/) (Find exact item IDs for mod importation)
* 🌐 **Website:** [Alphy Official Site](https://ac-storm-yt.github.io/alphy.github.io/)
* 💬 **Discord:** [Join the Alphy Community Server](https://discord.gg/UuC5j8smR7)

---

### Open Source
Alphy is 100% open-source. This ensures complete transparency regarding how your local game files are handled. We welcome code reviews, feedback, and requests from the community to help make Alphy even better!