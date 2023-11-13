using LitJson;
using System;
using UnityEngine;
public class JsonMapper {
    static JsonMapper() {
        LitJson.JsonMapper.RegisterExporter(delegate (float obj, JsonWriter writer) {
            writer.Write(Convert.ToDouble(obj));
        });
        LitJson.JsonMapper.RegisterExporter(delegate (decimal obj, JsonWriter writer) {
            writer.Write(Convert.ToString(obj));
        });
        LitJson.JsonMapper.RegisterImporter((double input) => Convert.ToSingle(input));
        LitJson.JsonMapper.RegisterImporter((int input) => Convert.ToInt64(input));
        LitJson.JsonMapper.RegisterImporter((string input) => Convert.ToDecimal(input));
    }
    public static T ToObject<T>(string json) {
        return LitJson.JsonMapper.ToObject<T>(json);
    }
    public static string ToJson(object obj) {
        UnityEngine.Debug.Log("LitJson.JsonMapper.ToJson(obj) " + LitJson.JsonMapper.ToJson(obj));
        return LitJson.JsonMapper.ToJson(obj);
    }
}
