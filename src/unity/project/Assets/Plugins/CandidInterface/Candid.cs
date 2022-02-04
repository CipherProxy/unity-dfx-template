using AOT;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// Class with a JS Plugin functions for WebGL.
public static class Candid
{
    [DllImport("__Internal")]
    private static extern bool init();

    [DllImport("__Internal")]
    private static extern void lookup(int callbackID, string keyPtr, RequestCallback callbackPtr);

    [DllImport("__Internal")]
    private static extern void insert(int callbackID, string keyPtr, string descPtr, string modelPtr, RequestCallback callbackPtr);

    // This is the callback, whose pointer we'll send to javascript and is called by emscripten's Runtime.dynCall.
    public delegate void RequestCallback(int callbackID, string response, string error);


    /// Everytime a request is issued, give it the current id and increment this for next request.
    static int callbackIDIncrementer = 0;

    /// Keeps track of pending callbacks by their id, once callback is received it is executed and removed from the book.
    static Dictionary<int, Action<string, string>> callbacksBook = new Dictionary<int, Action<string, string>>();

    /// Called from the javascript side, this is the function whose pointer we passed to lookup
    /// This must match the return type and arguments of RequestCallback
    [MonoPInvokeCallback(typeof(Action))]
    private static void GlobalCallback(int callbackID, string response, string error)
    {
        if (callbacksBook.TryGetValue(callbackID, out Action<string, string> callback))
        {
            callback?.Invoke(response, error);

        }
        // Remove this request from the tracker as it is done.
        callbacksBook.Remove(callbackID);
    }

    /// Setup the callback. Returns the callbackID
    private static int SetupCallback(Action<string, string> callback) 
    {
        int callbackID = callbackIDIncrementer;
        callbackIDIncrementer++;
        callbacksBook.Add(callbackID, callback);
        return callbackID;
    }

    /// Lookup a hashtable entry with the motoko canister
    public static void Lookup(string key, Action<string, string> callback)
    {
        // Now call the javascript function and when it is done it'll callback the C# GlobalCallback function.
        lookup(SetupCallback(callback), key, GlobalCallback);
    }

    /// Insert a hashtable entry with the motoko canister
    public static void Insert(string key, string desc, string model, Action<string, string> callback)
    {
        // Now call the javascript function and when it is done it'll callback the C# GlobalCallback function.
        insert(SetupCallback(callback), key, desc, model, GlobalCallback);
    }
}