using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
internal static class DeepCopyUtils {
    public static object DeepCopy(this object target) {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        try {
            binaryFormatter.Serialize(memoryStream, target);
            memoryStream.Position = 0L;
            return binaryFormatter.Deserialize(memoryStream);
        } finally {
            memoryStream.Close();
        }
    }
}
