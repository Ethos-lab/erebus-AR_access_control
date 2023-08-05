## 1.0.12
+ Added default NatML icon and splash screen textures.
+ Fixed `BindingError` exception when running in WebGL.

## 1.0.11
+ Fixed `HubSettings.OnUpdateSettings` event being triggered with stale access key value.

## 1.0.10
+ Updated `HubSettings.OnUpdateSettings` event to also be invoked when the Editor opens.

## 1.0.9
+ Deprecated `Predictor.type` field as it is no longer supported by the NatML Hub API.
+ Deprecated `NatMLHub.RequestPrediction` method as it is no longer supported by the NatML Hub API.
+ Deprecated `NatMLHub.WaitForPrediction` method as it is no longer supported by the NatML Hub API.
+ Deprecated `UploadType.Feature` constant as it is no longer supported by the NatML Hub API.
+ Deprecated `DataType` class as it is no longer supported by the NatML Hub API.
+ Deprecated `Feature` class as it is no longer supported by the NatML Hub API.
+ Deprecated `Prediction` class as it is no longer supported by the NatML Hub API.
+ Deprecated `PredictorType` class as it is no longer supported by the NatML Hub API.
+ Deprecated `PredictionStatus` class as it is no longer supported by the NatML Hub API.
+ Deprecated `RequestPredictionRequest` class as it is no longer supported by the NatML Hub API.
+ Deprecated `RequestPredictionResponse` class as it is no longer supported by the NatML Hub API.
+ Removed `NatMLHub.ReportPrediction` method as it is no longer supported by the NatML Hub API.
+ Removed `NatMLHub.Subscribe` method as it is no longer supported by the NatML Hub API.
+ Removed `PredictionUpdatedRequest` class as it is no longer supported by the NatML Hub API.
+ Removed `PredictionUpdatedResponse` class as it is no longer supported by the NatML Hub API.
+ Removed `ReportPredictionRequest` class as it is no longer supported by the NatML Hub API.
+ Removed `ReportPredictionResponse` class as it is no longer supported by the NatML Hub API.

## 1.0.8
+ Added support for the WebGL platform.
+ Refactored `NatMLBundleIdentifier` attribute to `NatMLBundle`.
+ Refactored `BuildEmbedHelper.BundleIdentifierOverride` property to `BundleOverride`.

## 1.0.7
+ Fixed bug where `BuildEmbedHelper` fails to embed data on supported target.

## 1.0.6
+ Fixed serialization failures due to `HubSettings.OnUpdateSettings` being invoked on a background thread.

## 1.0.5
+ Added `BuildEmbedHelper` class to simplify embedding data and settings in builds.
+ Added `HubSettings.EditorBundle` for retrieving Editor bundle identifier when generating app tokens.
+ Updated `HubSettings.OnUpdateSettings` to not be invoked when scripts are recompiled.

## 1.0.4
+ Fixed NatML Hub library causing hard crash on Android.

## 1.0.3
+ Added `HubSettings.OnUpdateSettings` event raised when settings are updated by the user.
+ Added `NatMLHub.CurrentPlatform` property for getting the current NatML platform identifier.
+ Added `NatMLHub.ToPlatform` extension method to convert a Unity `RuntimePlatform` to a NatML platform identifier.
+ Added `NatMLHub.FormatForPlatform` method to get the default graph format for a given NatML platform.

## 1.0.2
+ Added `HubSettings.User` property for fetching NatML user info with current access key.
+ Refactored `NatMLHub.CreateApplicationToken` function to `CreateAppToken`.

## 1.0.1
+ Added `NatMLHub.CreateApplicationToken` function for creating application tokens for NatML media API's.
+ Added `NatMLBundleIdentifier` attribute for specifying runtime application bundle ID.

## 1.0.0
+ First release.