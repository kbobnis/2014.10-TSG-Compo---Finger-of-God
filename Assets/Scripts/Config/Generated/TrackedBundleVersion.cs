using System.Collections;

public class TrackedBundleVersion
{
	public static readonly string bundleIdentifier = "com.kprojekt.fingerofgod";

	public static readonly TrackedBundleVersionInfo Version_1_15 =  new TrackedBundleVersionInfo ("1.15", 0);
	public static readonly TrackedBundleVersionInfo Version_1_14 =  new TrackedBundleVersionInfo ("1.14", 1);
	public static readonly TrackedBundleVersionInfo Version_1_16 =  new TrackedBundleVersionInfo ("1.16", 3);
	
	public ArrayList history = new ArrayList ();

	public TrackedBundleVersionInfo current = new TrackedBundleVersionInfo ("1.16", 3);

	public  TrackedBundleVersion() {
		history.Add (Version_1_14);
		history.Add (Version_1_15);
		history.Add (current);
	}

}
