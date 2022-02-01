var candid_interface = {
	$contextObject: {
		// Notifies C# that the link is received for the function that request it.
		SendResult: function (requestID, link, error, callback) {
			var linkBytes = this.AllocateString(link);
			var errorBytes = this.AllocateString(error);

			// Calls a C# function using its pointer 'callback',
			// 'v' means it is a void function, the count of 'i's if the count of arguments this function accepts in our case 3 arguments, request id, the link and an error.
			Module["dynCall_viii"](callback, requestID, linkBytes, errorBytes);

			// Free up the allocated bytes by the strings
			if (linkBytes) _free(linkBytes);
			if (errorBytes) _free(errorBytes);
		},

		// Utility function to convert javascript string to C# string.
		// Handles if the string is null or empty.
		AllocateString: function (str) {
			if (str) {
				var length = lengthBytesUTF8(str) + 1;
				var buff = _malloc(length);

				stringToUTF8Array(str, HEAPU8, buff, length);

				return buff;
			}
			return 0;
		},
	},

	// Initialize
	init: function () {
		return;
	},

	// Lookup a key in motoko canister
	lookup: function (requestID, keyPtr, callback) {
		Candid.motoko_canister
			.lookup(UTF8ToString(keyPtr))
			.then(function (responseObj) {
				// Send the link to the communicator to send it to a C# callback.
				contextObject.SendResult(requestID, JSON.stringify(responseObj), null, callback);
			})
			.catch(function (error) {
				// Send the error to the communicator to send to a C# callback.
				contextObject.SendResult(
					requestID,
					null,
					JSON.stringify(error),
					callback
				);
			});
	},

	insert: function (requestID, keyPtr, descPtr, modelPtr, callback) {
		var entry = {
			desc: UTF8ToString(descPtr),
			model: UTF8ToString(modelPtr),
		};
		Candid.motoko_canister
			.insert(UTF8ToString(keyPtr), entry)
			.then(function (responseObj) {
				// Send the link to the communicator to send it to a C# callback.
				contextObject.SendResult(requestID, JSON.stringify(responseObj), null, callback);
			})
			.catch(function (error) {
				// Send the error to the communicator to send to a C# callback.
				contextObject.SendResult(
					requestID,
					null,
					JSON.stringify(error),
					callback
				);
			});
	},
};

autoAddDeps(candid_interface, "$contextObject"); // tell emscripten about this dependency
mergeInto(LibraryManager.library, candid_interface);
