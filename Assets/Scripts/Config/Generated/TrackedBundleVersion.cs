using System.Collections;

public class TrackedBundleVersion
{
	public static readonly string bundleIdentifier = "com.kprojekt.fingerofgod";

	public static readonly TrackedBundleVersionInfo Version_1_14 =  new TrackedBundleVersionInfo ("1.14", 0);
	
	public ArrayList history = new ArrayList ();

	public TrackedBundleVersionInfo current = new TrackedBundleVersionInfo ("1.15", 2);

	public  TrackedBundleVersion() {
		history.Add (current);
	}

}
