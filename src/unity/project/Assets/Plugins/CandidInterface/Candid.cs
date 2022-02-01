using AOT;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Class with a JS Plugin functions for WebGL.
/// </summary>
public static class Candid
{
    [DllImport("__Internal")]
    private static extern bool init();

    [DllImport("__Internal")]
    private static extern void lookup(int requestID, string keyPtr, RequestCallback callbackPtr);

    [DllImport("__Internal")]
    private static extern void insert(int requestID, string keyPtr, string descPtr, string modelPtr, RequestCallback callbackPtr);


    // This is the callback, whose pointer we'll send to javascript and is called by emscripten's Runtime.dynCall.
    public delegate void RequestCallback(int requestID, string response, string error);

    /// <summary>
    /// Everytime a request is issued, give it the current id and increment this for next request.
    /// </summary>
    static int requestIDIncrementer = 0;

    /// <summary>
    /// Keeps track of pending callbacks by their id, once callback is received it is executed and removed from the book.
    /// </summary>
    static Dictionary<int, Action<string, string>> callbacksBook = new Dictionary<int, Action<string, string>>();


    /// <summary>
    /// Called from the javascript side, this is the function whose pointer we passed to <see cref="lookup"/>
    /// This must match the return type and arguments of <see cref="RequestCallback"/>
    /// </summary>
    /// <param key="requestID"></param>
    /// <param key="response"></param>
    /// <param key="error"></param>
    [MonoPInvokeCallback(typeof(Action))]
    private static void GlobalCallback(int requestID, string response, string error)
    {
        if (callbacksBook.TryGetValue(requestID, out Action<string, string> callback))
        {
            callback?.Invoke(response, error);

        }
        // Remove this request from the tracker as it is done.
        callbacksBook.Remove(requestID);
    }

    /// <summary>
    /// Initializes firebase, on the javascript side.
    /// Call this once, or enough until it returns true.
    /// </summary>
    /// <returns>True if success, false if error occured.</returns>
    public static bool Init()
    {
        return init();
    }


    /// <summary>
    /// Gets a storage response using the javascript firebase sdk.
    /// </summary>
    /// <param key="key"></param>
    /// <param key="callback">callback when the link is received or an error.</param>
    public static void Lookup(string key, Action<string, string> callback)
    {
        int requestID = requestIDIncrementer;
        requestIDIncrementer++;
        callbacksBook.Add(requestID, callback);

        // Now call the javascript function and when it is done it'll callback the C# GlobalCallback function.
        lookup(requestID, key, GlobalCallback);
    }

    public static void Insert(string key, string desc, string model, Action<string, string> callback)
    {
        int requestID = requestIDIncrementer;
        requestIDIncrementer++;
        callbacksBook.Add(requestID, callback);

        // Now call the javascript function and when it is done it'll callback the C# GlobalCallback function.
        insert(requestID, key, desc, model, GlobalCallback);
    }
}