# Unity/jslib canister template

This is a template for a unity canister on the Internet Computer Blockchain.

The example project includes a motoko canister with a hashtable, and a jslib/c# library for using the candid interface.

The candid interface is included in the `src/unity/project` folder. Import it as a plugin in unity and add the demoscript to a gameobject.

## Building the Unity project

Unity projects must be built with compression: off in the webgl build settings. Output the build to `src/unity/build`. 

## Running the project locally

```bash
# Starts the replica, running in the background
dfx start --background

# Deploys your canisters to the replica and generates your candid interface
dfx deploy
```

The application will be available at `http://localhost:8000?canisterId={asset_canister_id}`.

You can start a development server with

```bash
npm start
```

Which will start a server at `http://localhost:8080`, proxying API requests to the replica at port 8000.
