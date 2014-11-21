using System.Collections;

public class TrackedBundleVersion
{
	public static readonly string bundleIdentifier = "com.kprojekt.fingerofgod";

	public static readonly TrackedBundleVersionInfo Version_1_15 =  new TrackedBundleVersionInfo ("1.15", 0);
	public static readonly TrackedBundleVersionInfo Version_1_14 =  new TrackedBundleVersionInfo ("1.14", 1);
	
	public ArrayList history = new ArrayList ();

	public TrackedBundleVersionInfo current = new TrackedBundleVersionInfo ("1.15", 2);

	public  TrackedBundleVersion() {
		history.Add (Version_1_15);
		history.Add (Version_1_14);
		history.Add (current);
	}

}
