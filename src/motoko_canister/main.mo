import Map "mo:base/HashMap";
import Text "mo:base/Text";

actor {

  type Key = Text;
  type Data = Text;

  type Entry = {
    desc: Text;
    model: Data;
  };

  let db = Map.HashMap<Key, Entry>(0, Text.equal, Text.hash);

  public func insert(key : Key, entry : Entry): async () {
    db.put(key, entry);
  };

  public query func lookup(key : Key) : async ?Entry {
    db.get(key)
  };
};
