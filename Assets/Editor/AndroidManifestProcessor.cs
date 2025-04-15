using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class AndroidManifestProcessor : IPreprocessBuildWithReport
{
    // This determines the order in which the pre-build scripts run.
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        // Use a custom scripting define (e.g., METAQUEST) or some other build configuration
        // to determine which manifest file to use.
#if VR_PLATFORM
        string sourceManifest = "Assets/Plugins/Android/AndroidManifest_VR.xml";
#else
        string sourceManifest = "Assets/Plugins/Android/AndroidManifest_Mobile.xml";
#endif

        string destinationManifest = "Assets/Plugins/Android/AndroidManifest.xml";

        // Copy the correct manifest file to the destination.
        File.Copy(sourceManifest, destinationManifest, true);
        AssetDatabase.Refresh();
    }
}
