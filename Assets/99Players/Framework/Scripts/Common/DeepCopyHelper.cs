using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class DeepCopyHelper {
    public static T DeepCopy<T>(T target) {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        try {
            binaryFormatter.Serialize(memoryStream, target);
            memoryStream.Position = 0L;
            return (T)binaryFormatter.Deserialize(memoryStream);
        } finally {
            memoryStream.Close();
        }
    }
}
