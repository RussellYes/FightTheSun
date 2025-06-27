📦 FLEXIBLE UPDATE MANAGER
By Jen's Awesome Assets

🛠️ Easily integrate Android in-app updates (flexible type) into your Unity project using Google Play Core.

---
📁 INCLUDED FILES

Assets/
├── FlexibleUpdater/
│   ├── FlexibleUpdateManager.cs        ← Main script
│   ├── FlexibleUpdateManager.prefab    ← Drop-in prefab
│   ├── Editor/
│   │   └── FlexibleUpdateValidator.cs  ← Simple editor check
├── Plugins/
│   └── Android/
│       └── play-core-1.10.3.aar        ← Required Play Core dependency

---
⚙️ SETUP INSTRUCTIONS

1. Drag the `FlexibleUpdateManager.prefab` into your first loaded scene.
2. Requires TextMeshPro (Window > TextMeshPro > Import TMP Essentials)
3. Assign your UI elements in the inspector:
   - `progressBar`: UnityEngine.UI.Slider
   - `statusText`: TextMeshPro text for update status
   - `restartPopup`: GameObject with Yes/No buttons
   - `readyText`: TextMeshPro text inside the popup
   - `yesButton` and `noButton`: UnityEngine.UI.Buttons
4. Set your desired post-update scene name in `mainSceneName`.

> ✅ A valid Google Play Store build is required for updates to function correctly.
> ✅ You must use a consistent keystore to update builds on Play Store.

---
💡 DEV NOTES

- Uses Google Play Core’s Flexible App Update flow.
- Debug logs included to trace execution during testing.
- Safe to include alongside your game content.
- Works with Unity 2022+ and Unity 6.
- Requires Android SDK API level 33 or higher.

---
📞 SUPPORT

Questions? Feedback?  
Contact: jensawesomeshow@gmail.com 

---
💸 TIPS
E-transfer (Canada only): jbarrett@sasktel.net
WAXP: l4lay.wam
ETH: 0xcE60022E9D7Cf10A8491F3B6Bb68daf746Ff4d3f

---
🎮 Make your updates seamless — so your players stay in the game.
