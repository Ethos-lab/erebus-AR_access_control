# Notes for the policy generation from natural language

## Types of AR applications based on use case

1. Marker-based AR:
   + Developers use QR codes/basic shapes as markers
   + Probably need raw camera feed and detect certain shapes

2. Markerless AR (most common type of AR app):
   + Recognizes flat surfaces and borders of areas
   + Typically paired with machine learning models for functionality
   + Examples include furniture visualization apps, object placement apps, 
        Pokemon Go type, silly games, etc.
   + Their functionality is often restricted to handful of plane detection
        APIs.
   + What kind of ARCore APIs would these kinds of apps need?

3. Location-based (geo-based) AR:
   + Provides digital content based on user's location
   + Apps centered around tourism, travel, and sport
   + Museum guide AR apps — only should trigger when inside a museum

## ARCore APIs each type of app could use

Most of the ARCore APIs could be categorized based on their use cases.

"Allow face detection at night" --> should create policy for all face detection
APIs.

"This app may detect user faces" -> "Allow only at night"

## NL policies tested with Erebus

+ Allow image detection if I am Home and it is after 9:00pm
+ Allow this app to detect objects only for QR codes and only during evening
+ Allow this app to work only if I am at Work
+ Allow only when I am Home and during the permitted hours on weekends
+ Only allow Keyboards to be detected by the camera
+ Deny image tracking if I am at Home and it is after 10:00pm
+ If this app uses plane detection only allow if I am at Home
+ Deny location access if Batman is Home
+ If this app tracks location deny access if Superman is using the app at Work
+ If Batman is playing this game at Home allow plane detection


