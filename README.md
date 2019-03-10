# RTI Penguin in the Desert

2D platformer game developed to research semantic proximity and object recognition.

## Getting started

## Deployment

To deploy to Firebase hosting, first install the Firebase CLI tools:

```
npm install -g firebase-tools
```

Next, sign into Firebase using your Google account by running:

```
firebase login
```

Then, build the project in Unity from the File -> Build and Run... menu. In the Build Settings dialog, click Build and create a directory named `build` in the `runner-game` directory if it doesn't already exist. The _Save As_ value should be `web-runner`. Then, to deploy, change to the project directory and run:

```
firebase deploy
```

This deploys the project from the `runner-game/build/web-runner` directory.
