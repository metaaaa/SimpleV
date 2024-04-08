# SimpleV
VRM1.0モデル用の簡易的なVTuberシステムです。主にMediaPipeUnityPluginを使用して作成されています。

動作確認をするには有料AssetのFinalIKが必要です。

https://github.com/metaaaa/SimpleV/assets/56059182/ff8fb197-6a51-4d0f-b5b5-9f294e2b975a

## Env

```
Unity2022.3.6f1
MediaPipeUnityPlugin v0.14.1
```

## Setup

- 以下のURLより MediaPipeUnity.0.14.1.unitypackage をダウンロードしてください
https://github.com/homuler/MediaPipeUnityPlugin/releases/tag/v0.14.1
- UnityでProjectを開き、上記のunitypackageをインポートしてください
- FinalIKをインポートしてください
- Assets/__WorkSpace/SimpleV/Scenes/SimpleV.unity を開いて動作確認をしてください
- Assets/__WorkSpace/SimpleV/Settings 内にSetting用のファイルがあるので適宜設定してください

## VRM Setup

- VRMモデルをインポートしてVRM1.0にマイグレーションしてください
- サンプルシーンを参考にマイグレーションしたVRMにVRIKを設定してください
