using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MenuInfo : IEnumerable<MenuInfoEntry>
{
    [SerializeField]
    private List<MenuInfoEntry> entries;

    public MenuInfo(List<MenuInfoEntry> e) {
        entries = e;
    }

    public MenuInfo() {
        entries = new();
    }

    public bool Contains(string key) {
        return entries.Where(e => e.key == key).Any();
    }

    public int Count => entries.Count;

    public bool AddEntry(string key, string value, UnityAction callback) {
        string[] keys = entries.Select(e => e.key).ToArray();
        if (!keys.Contains(key)) {
            entries.Add(new MenuInfoEntry(key, value, callback));
            return true;
        }
        return false;
    }

    public bool AddEntry(string key, string value) {
        return AddEntry(key, value, null);
    }

    public bool AddEntries(MenuInfo otherMenuInfo) {
        bool conflicts = true;
        foreach (MenuInfoEntry entry in otherMenuInfo) {
            if (!AddEntry(entry.key, entry.stringValue, entry.buttonCallback)) {
                conflicts = false;
            }
        }
        return conflicts;
    }

    public (string text, UnityAction callback) this[string key] {
        get {
            (string, UnityAction) info = entries
                                         .Where(e => e.key == key)
                                         .Select(e => (e.stringValue, e.buttonCallback))
                                         .First();
            return info;
        }
    }

    public Dictionary<string, string> AsDictionary() {
        Dictionary<string, string> dict = new();
        foreach (MenuInfoEntry menuInfoEntry in entries) {
            dict.TryAdd(menuInfoEntry.key, menuInfoEntry.stringValue);
        }
        return dict;
    }
    /// <inheritdoc />
    public IEnumerator<MenuInfoEntry> GetEnumerator() {
        return entries.GetEnumerator();
    }
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

[Serializable]
public struct MenuInfoEntry
{
    public string key;
    public string stringValue;
    public UnityAction buttonCallback;

    public MenuInfoEntry(string k, string v) : this(k, v, null) { }
    public MenuInfoEntry(string k, string v, UnityAction callback) {
        key = k;
        stringValue = v;
        buttonCallback = callback;
    }
}