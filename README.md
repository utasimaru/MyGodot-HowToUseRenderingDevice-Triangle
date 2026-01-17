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
## 2つのコードをプロジェクトのルート直下にぶち込む
## WorldEnviromentを作成する
