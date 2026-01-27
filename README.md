# MyGodot-HowToUseRenderingDevice-Triangle
C# RenderingDeviceを使用して、画面に基本的な三角形を描写します
次のプロジェクトをC#に翻訳しました。https://github.com/acui/customized-pipeline/tree/master

# やること
CompositorEffectを継承したグローバルクラス(←これ重要)を定義して、RenderingDeviceを使用した3D描画処理を実行する

## 説明
CompositorEffectは、WorldEnvironment(環境)やCamera3Dに適用することができる。と思う。

Camera3Dに適用すると、そのカメラの描写時に、任意の描写タイミング(不透明描写の前後、透明描写の前後など)で呼び出される関数で、独自の図形描写を実行することができる。と思う。

WorldEnviromentに適用すると、全てのカメラの描写時に呼び出されるようになる。と思う。
# 使い方
## 0. 使いたいコードをプロジェクトの任意の場所に追加
## 1. プロジェクト → ツール → C# → Create c# solution
<img width="899" height="516" alt="image" src="https://github.com/user-attachments/assets/2620d437-639b-42f0-90d2-aa24231806c8" />

## 2. 新しく追加されたトンカチマーク🔨を押す
<img width="740" height="341" alt="image" src="https://github.com/user-attachments/assets/fa4b6707-b6a4-44a6-b19d-f085e94e7382" />

## 3. WorldEnviromentを作成する
<img width="488" height="666" alt="image" src="https://github.com/user-attachments/assets/3810c25e-634f-48ba-8d82-4751df0fdeef" />

<img width="930" height="741" alt="image" src="https://github.com/user-attachments/assets/0cd20f4c-a6f4-45a8-b665-1d2ed174ada9" />

## 4. WorldEnviroment - Compositor 
###「空」をクリック
<img width="1241" height="1079" alt="image" src="https://github.com/user-attachments/assets/48ef1c77-facc-4464-8d9e-0677913fc6ca" />

### 新しいのを作る
<img width="553" height="491" alt="image" src="https://github.com/user-attachments/assets/4a21e74c-5099-4370-a16f-38f69c01e6b8" />

### <空>だった場所をクリック
<img width="675" height="600" alt="image" src="https://github.com/user-attachments/assets/1e664b17-8719-4052-8a40-8dc2876c0147" />

### CompositorEffectの右欄をクリック
<img width="639" height="503" alt="image" src="https://github.com/user-attachments/assets/56707e88-5959-4aca-bb5d-7406ff1e4429" />

### 要素を追加を押す
<img width="516" height="405" alt="image" src="https://github.com/user-attachments/assets/8dc6c7a8-a689-44cb-b899-6bfa5c73fac3" />

### 「空」をクリック、気になるクラスを選択
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/3d4e8fe1-637f-415b-bef4-6665ce14570c" />
